using PiRhoSoft.Utilities.Editor;
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition.Editor
{
	public class GraphViewNode : Node
	{
		public const string UssClassName = GraphViewEditor.UssClassName + "__node";
		public const string NodeDeleteButtonUssClassName = UssClassName + "__delete-button";
		public const string NodeEditableLabelUssClassName = UssClassName + "__editable-label";

		private static readonly CustomStyleProperty<Color> _nodeColorProperty = new CustomStyleProperty<Color>("--node-color");

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
			AddToClassList(UssClassName);

			IsStartNode = isStart;
			Data = new GraphNode.NodeData(node);

			title = node.name;
			titleContainer.style.backgroundColor = node.NodeColor;
			titleContainer.style.unityFontStyleAndWeight = FontStyle.Bold;

			m_CollapseButton.SetDisplayed(false);

			SetPosition(new Rect(node.GraphPosition, Vector2.zero));
		}

		public override void OnSelected()
		{
			base.OnSelected();

			Selection.activeObject = IsStartNode ? (Object)Data.Node.Graph : Data.Node;
		}

		protected TextField CreateEditableLabel(TextElement container, Func<string> getValue, Action<string> setValue, bool multiline = false)
		{
			var edit = new TextField() { value = getValue() };
			edit.multiline = multiline;
			edit.AddToClassList(NodeEditableLabelUssClassName);
			edit.RegisterValueChangedCallback(evt => setValue(evt.newValue));
			edit.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(evt => HideEditableText(edit));

			container.RegisterCallback<MouseDownEvent>(evt => OnEditEvent(evt, edit, getValue()));
			container.Add(edit);

			HideEditableText(edit);

			return edit;
		}

		protected void ShowEditableText(TextField edit, string value)
		{
			edit.SetValueWithoutNotify(value);
			edit.style.visibility = Visibility.Visible;
			edit.Focus();
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
			var deleteButton = new IconButton(Icon.Close.Texture, "Delete this node", DeleteNode);
			deleteButton.AddToClassList(NodeDeleteButtonUssClassName);
			titleButtonContainer.Add(deleteButton);
		}

		protected void DeleteNode()
		{
			var graph = GetFirstAncestorOfType<GraphView>();
			graph.RemoveNode(this, Enumerable.Empty<GraphViewNode>());
		}

		protected void ViewDocumentation()
		{
			var help = TypeHelper.GetAttribute<HelpURLAttribute>(IsStartNode ? typeof(Graph) : Data.Node.GetType());
			if (help != null)
				Application.OpenURL(help.URL);
		}

		protected override void OnCustomStyleResolved(ICustomStyle style)
		{
			base.OnCustomStyleResolved(style);

			titleContainer.style.backgroundColor = style.TryGetValue(_nodeColorProperty, out var nodeColor) ? nodeColor : Data.Node.NodeColor;
		}
	}
}
