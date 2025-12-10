#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
// using VSNL.Editor.Graph.Nodes; // Namespace is just VSNL.Editor.Graph

namespace VSNL.Editor.Graph
{
    public static class VSNLGraphExporter
    {
        public static void Export(VSNLGraphView graphView, string path)
        {
            var builder = new StringBuilder();
            var visited = new HashSet<VSNLNode>();
            
            // Find Start Node
            StartNode startNode = null;
            graphView.nodes.ForEach(n => 
            {
                if (n is StartNode s) startNode = s;
            });

            if (startNode == null)
            {
                 Debug.LogError("No Start Node found!");
                 return;
            }

            // Traversal Loop
            var nodesToProcess = new Queue<VSNLNode>();
            // We actually need a linear sequence generation.
            // Branching requires Labels and Gotos.
            // Strategy:
            // 1. Traverse "Main Line" from Start.
            // 2. If Choice:
            //    - Write "choice" command.
            //    - Create Labels for targets.
            //    - Write options with "goto Label".
            //    - Write "stop".
            //    - Enqueue targets to be generated as new blocks (label X ...).
            
            // Better: Linearize blocks.
            // A Block starts with a Label (or Start) and ends with a Stop, Jump, or flows into next?
            // VSNL is sequential.
            
            Dictionary<VSNLNode, string> nodeLabels = new Dictionary<VSNLNode, string>();
            
            // Assign labels to all nodes that are targets of Choices or multiple inputs?
            // Simplification: Assign labels to ALL nodes for safety? No, too messy.
            // Optimization: Only assign labels to nodes that are targets of Gotos (Choice Outputs).
            
            // 1. Scan for jump targets
            foreach (var node in graphView.nodes.ToList())
            {
                if (node is ChoiceNode choice)
                {
                    foreach (var port in choice.outputContainer.Children().OfType<Port>())
                    {
                        var targetEdge = port.connections.FirstOrDefault();
                        if (targetEdge != null)
                        {
                            var targetNode = targetEdge.input.node as VSNLNode;
                             if (targetNode != null && !nodeLabels.ContainsKey(targetNode))
                             {
                                 nodeLabels[targetNode] = $"L_{targetNode.GUID.Substring(0,8)}";
                             }
                        }
                    }
                }
            }

            // 2. Linear traversal with block queue
            HashSet<VSNLNode> processed = new HashSet<VSNLNode>();
            Queue<VSNLNode> queue = new Queue<VSNLNode>();
            
            // Get first node after Start
            var startPort = startNode.outputContainer.Q<Port>();
            var initialEdge = startPort.connections.FirstOrDefault();
            if (initialEdge != null)
            {
                queue.Enqueue(initialEdge.input.node as VSNLNode);
            }

            while (queue.Count > 0)
            {
                var root = queue.Dequeue();
                if (processed.Contains(root)) continue;

                // Start writing a block
                // If this node has a pre-assigned label (from a choice jump), write it.
                if (nodeLabels.ContainsKey(root))
                {
                    builder.AppendLine($"label {nodeLabels[root]}");
                }

                // Traverse linear chain
                var current = root;
                while (current != null)
                {
                    if (processed.Contains(current)) 
                    {
                        // We hit a node already processed -> Loop or Merge
                        // If it has a label, goto it. If not, we have a problem (linear merge without label).
                        // In valid VSNL graph, merges should be labeled.
                        // For auto-export, we should have labeled it if it had >1 input?
                        // Hack: If processed, assume we need a goto. If no label, it's a bug in our Step 1 scanning (we only scanned choice targets).
                        // Fix: Step 1 should scan ALL inputs. If input count > 1, it needs a label.
                        
                        // For now, assume jumps happen only from choices.
                        if (nodeLabels.ContainsKey(current))
                        {
                             builder.AppendLine($"goto {nodeLabels[current]}");
                             break;
                        }
                        break; 
                    }

                    processed.Add(current);

                    // Write Node Content
                    if (current is DialogueNode d)
                    {
                         builder.AppendLine($"char \"{d.SpeakerName}\"");
                         builder.AppendLine($"\"{d.DialogueText}\"");
                    }
                    else if (current is CommandNode c)
                    {
                         // Remove @ if user added it
                         string cmd = c.CommandText.StartsWith("@") ? c.CommandText.Substring(1) : c.CommandText;
                         builder.AppendLine($"{cmd}");
                    }
                    else if (current is ChoiceNode ch)
                    {
                        if (!string.IsNullOrEmpty(ch.ChoiceQuestion))
                        {
                            builder.AppendLine($"char \"System\""); // Helper
                            builder.AppendLine($"\"{ch.ChoiceQuestion}\"");
                        }
                        
                        builder.AppendLine("choice");
                        
                        foreach (var port in ch.outputContainer.Children().OfType<Port>())
                        {
                            var optionText = port.portName; // We stored text here
                            // Get TextField inside contentContainer if portName logic fails (it shouldn't)
                            var tf = port.contentContainer.Q<TextField>();
                            if (tf != null) optionText = tf.value;

                            var edge = port.connections.FirstOrDefault();
                            if (edge != null)
                            {
                                var target = edge.input.node as VSNLNode;
                                if (!nodeLabels.ContainsKey(target))
                                    nodeLabels[target] = $"L_{target.GUID.Substring(0,8)}";
                                
                                builder.AppendLine($"\"{optionText}\" goto {nodeLabels[target]}");
                                queue.Enqueue(target); // Process this branch
                            }
                        }
                        
                        builder.AppendLine("stop");
                        break; // Choice ends the linear block
                    }

                    // Move to next linear node
                    // Only Dialogue and Command have "Out" single port
                    var outPort = current.outputContainer.Q<Port>("Out");
                    if (outPort != null && outPort.connections.Any())
                    {
                        var nextEdge = outPort.connections.First();
                        var nextNode = nextEdge.input.node as VSNLNode;
                        
                        // If next node is a target of a jump (has Label), we must Stop our linear flow and Goto it (or letting it flow if it's the very next block?)
                        // To be safe: If next node needs labeling (is a merge point or jump target), we stop linear here and Goto. 
                        // Unless we are the ONLY parent?
                        // Simple approach: Always Goto labeled nodes.
                        if (nodeLabels.ContainsKey(nextNode))
                        {
                            builder.AppendLine($"goto {nodeLabels[nextNode]}");
                            queue.Enqueue(nextNode); // Ensure it's generated
                            break;
                        }

                        current = nextNode;
                    }
                    else
                    {
                        current = null;
                        builder.AppendLine("stop"); // End of chain
                    }
                }
            }

            File.WriteAllText(path, builder.ToString());
            Debug.Log($"[VSNL] Exported graph to {path}");
        }
    }
}
#endif
