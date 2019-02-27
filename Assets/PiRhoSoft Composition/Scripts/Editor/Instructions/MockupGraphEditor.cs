using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	[CustomEditor(typeof(MockupGraph), true)]
	public class MockupGraphEditor : InstructionGraphEditor
	{
		private static IconButton _addButton = new IconButton(IconButton.Add, "Add a mockup node to the graph");
		private PropertyListControl _nodesList = new PropertyListControl();
		private static GUIContent _nodesLabel = new GUIContent("Nodes");
		private float _addOffset = 0.0f;

		protected override void SetupNodes(SerializedProperty nodes)
		{
			var property = nodes.FindPropertyRelative("_items");

			_nodesList.Setup(property)
				.MakeAddable(_addButton, AddNode);
		}

		protected override void DrawNodes(SerializedProperty nodes)
		{
			_nodesList.Draw(_nodesLabel);
		}

		private void AddNode(SerializedProperty nodes)
		{
			var window = InstructionGraphWindow.FindWindowForGraph(_graph);
			var position = _graph.StartPosition + new Vector2(InstructionGraphNode.NodeData.Width + 20.0f, _addOffset);

			_addOffset += 40.0f;

			var node = CreateNode(_graph, typeof(MockupNode), "Mockup", position);

			if (window != null)
				window.AddNode(node);
		}
	}
}
