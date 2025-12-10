#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;

namespace VSNL.Editor.Graph
{
    // Base Node
    public class VSNLNode : Node
    {
        public string GUID;
        public string NodeType;
        public bool EntryPoint = false;
    }

    public class StartNode : VSNLNode
    {
        public StartNode()
        {
            title = "Start";
            GUID = Guid.NewGuid().ToString();
            NodeType = "Start";
            EntryPoint = true;

            var generatedPort = GeneratePort(this, Direction.Output, Port.Capacity.Single);
            generatedPort.portName = "Next";
            outputContainer.Add(generatedPort);

            RefreshExpandedState();
            RefreshPorts();
        }

        public static Port GeneratePort(VSNLNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float)); // Type doesn't matter for logic flow
        }
    }

    public class DialogueNode : VSNLNode
    {
        public string SpeakerName;
        public string DialogueText;

        public DialogueNode()
        {
            title = "Dialogue";
            GUID = Guid.NewGuid().ToString();
            NodeType = "Dialogue";

            var inputPort = StartNode.GeneratePort(this, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "In";
            inputContainer.Add(inputPort);

            var outputPort = StartNode.GeneratePort(this, Direction.Output, Port.Capacity.Single);
            outputPort.portName = "Out";
            outputContainer.Add(outputPort);

            var textFieldSpeaker = new TextField("Speaker");
            textFieldSpeaker.RegisterValueChangedCallback(evt => SpeakerName = evt.newValue);
            mainContainer.Add(textFieldSpeaker);

            var textFieldDialogue = new TextField("Text"); // Multiline ideally
            textFieldDialogue.multiline = true;
            textFieldDialogue.RegisterValueChangedCallback(evt => DialogueText = evt.newValue);
            mainContainer.Add(textFieldDialogue);

            RefreshExpandedState();
            RefreshPorts();
        }
    }

    public class CommandNode : VSNLNode
    {
        public string CommandText;

        public CommandNode()
        {
            title = "Command";
            GUID = Guid.NewGuid().ToString();
            NodeType = "Command";

            var inputPort = StartNode.GeneratePort(this, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "In";
            inputContainer.Add(inputPort);

            var outputPort = StartNode.GeneratePort(this, Direction.Output, Port.Capacity.Single);
            outputPort.portName = "Out";
            outputContainer.Add(outputPort);

            var textField = new TextField("Command (@... )");
            textField.RegisterValueChangedCallback(evt => CommandText = evt.newValue);
            mainContainer.Add(textField);

            RefreshExpandedState();
            RefreshPorts();
        }
    }

    public class ChoiceNode : VSNLNode
    {
        public string ChoiceQuestion;
        public List<string> Options = new List<string>();

        public ChoiceNode()
        {
            title = "Choice";
            GUID = Guid.NewGuid().ToString();
            NodeType = "Choice";

            var inputPort = StartNode.GeneratePort(this, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "In";
            inputContainer.Add(inputPort);

            var questionField = new TextField("Question (Optional)");
            questionField.RegisterValueChangedCallback(evt => ChoiceQuestion = evt.newValue);
            mainContainer.Add(questionField);

            var addChoiceButton = new Button(() => { AddChoicePort(); })
            {
                text = "Add Option"
            };
            titleButtonContainer.Add(addChoiceButton);

            RefreshExpandedState();
            RefreshPorts();
        }

        private void AddChoicePort(string initialText = "Option")
        {
            var outputPort = StartNode.GeneratePort(this, Direction.Output, Port.Capacity.Single);
            outputPort.portName = ""; // We label via textfield

            var textField = new TextField();
            textField.value = initialText;
            textField.RegisterValueChangedCallback(evt => outputPort.portName = evt.newValue); // Store text in port name for simplicity
            outputPort.contentContainer.Add(textField);
            
            var deleteButton = new Button(() => RemoveChoicePort(outputPort)) { text = "X" };
            outputPort.contentContainer.Add(deleteButton);

            outputContainer.Add(outputPort);
            RefreshExpandedState();
            RefreshPorts();
        }

        private void RemoveChoicePort(Port port)
        {
             outputContainer.Remove(port);
             RefreshPorts();
             RefreshExpandedState();
        }
    }
}
#endif
