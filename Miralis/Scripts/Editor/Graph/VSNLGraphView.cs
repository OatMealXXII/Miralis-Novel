#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
// using VSNL.Editor.Graph.Nodes; // Namespace is just VSNL.Editor.Graph

namespace VSNL.Editor.Graph
{
    public class VSNLGraphView : GraphView
    {
        public VSNLGraphView()
        {
            // Grid
            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            var zoom = new ContentZoomer();
            this.AddManipulator(zoom);
            
            // CSS (Optional, skipping for now)

            AddElement(GenerateStartNode());
        }

        private StartNode GenerateStartNode()
        {
            var node = new StartNode();
            node.SetPosition(new Rect(100, 200, 100, 150));
            return node;
        }

        public void CreateNode(string type, Vector2 pos)
        {
            VSNLNode node = null;
            switch(type)
            {
                case "Dialogue": node = new DialogueNode(); break;
                case "Command": node = new CommandNode(); break;
                case "Choice": node = new ChoiceNode(); break;
            }

            if (node != null)
            {
                node.SetPosition(new Rect(pos, new Vector2(200, 150)));
                AddElement(node);
            }
        }

        // Context Menu
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            var pos = viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
            
            evt.menu.AppendAction("Add Dialogue", (a) => CreateNode("Dialogue", pos));
            evt.menu.AppendAction("Add Command", (a) => CreateNode("Command", pos));
            evt.menu.AppendAction("Add Choice", (a) => CreateNode("Choice", pos));
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            foreach (var port in ports)
            {
                if (startPort != port && startPort.node != port.node && startPort.direction != port.direction)
                {
                    compatiblePorts.Add(port);
                }
            }
            return compatiblePorts;
        }
    }
}
#endif
