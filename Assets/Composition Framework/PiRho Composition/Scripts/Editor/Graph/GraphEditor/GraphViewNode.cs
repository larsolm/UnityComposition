using PiRhoSoft.Composition.Engine;
using PiRhoSoft.Utilities.Editor;
using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class GraphViewNode : Node
	{
		private const string _ussDeleteClass = "delete-button";
		private const string _ussEditableClass = "editable";

		private static readonly Icon _deleteIcon = Icon.BuiltIn("d_LookDevClose");

		public GraphNode.NodeData Data { get; private set; }
		public bool IsStartNode { get; private set; }

		public override bool IsAscendable() => true;
		public override bool IsDroppable() => false;
		public override bool IsMovable() => !IsStartNode;
		public override bool IsResizable() => false;
		public override bool IsSelectable() => true;

		protected override sealed void ToggleCollapse() { }

		public GraphViewNode(GraphNode node, bool isStart)
		{
			IsStartNode = isStart;
			Data = new GraphNode.NodeData(node);

			title = node.name;
			titleContainer.style.backgroundColor = node.NodeColor;
			titleContainer.style.unityFontStyleAndWeight = FontStyle.Bold;

			ElementHelper.SetVisible(m_CollapseButton, false);

			SetPosition(new Rect(node.GraphPosition, Vector2.zero));
		}

		public override void OnSelected()
		{
			base.OnSelected();

			Selection.activeObject = Data.Node;
		}

		protected TextField CreateEditableLabel(Label container, Func<string> getValue, Action<string> setValue, bool multiline = false)
		{
			var edit = new TextField() { value = getValue(), multiline = true };
			edit.AddToClassList(_ussEditableClass);
			edit.RegisterValueChangedCallback(evt => setValue(evt.newValue));
			edit.Q(TextInputBaseField<string>.textInputUssName).RegisterCallback<FocusOutEvent>(evt => HideEditableText(edit));

			container.RegisterCallback<MouseDownEvent>(evt => OnEditEvent(evt, edit, getValue()));
			container.Add(edit);

			HideEditableText(edit);

			return edit;
		}

		protected void ShowEditableText(TextField edit, string value)
		{
			edit.SetValueWithoutNotify(value);
			edit.style.visibility = Visibility.Visible;
			edit.Q(TextField.textInputUssName).Focus();
			edit.SelectAll(); // this doesn't work for some reason
		}

		protected void HideEditableText(TextField edit)
		{
			if (edit != null) // this needs to be here for when it is called after node is destroyed
				edit.style.visibility = Visibility.Hidden;
		}

		private void OnEditEvent(MouseDownEvent evt, TextField edit, string value)
		{
			if (evt.clickCount == 2 && evt.button == (int)MouseButton.LeftMouse)
			{
				ShowEditableText(edit, value);
				evt.PreventDefault();
			}
		}

		protected void CreateDeleteButton()
		{
			var deleteButton = new Image { image = _deleteIcon.Content, tooltip = "Delete this node" };
			deleteButton.AddToClassList(_ussDeleteClass);
			deleteButton.AddManipulator(new Clickable(DeleteNode));
			titleButtonContainer.Add(deleteButton);
		}

		protected void DeleteNode()
		{
			var graph = GetFirstAncestorOfType<GraphView>();
			graph.RemoveNode(this);
		}

		protected void ViewDocumentation()
		{
			var help = TypeHelper.GetAttribute<HelpURLAttribute>(IsStartNode ? typeof(Graph) : Data.Node.GetType());
			if (help != null)
				Application.OpenURL(help.URL);
		}
	}
}
