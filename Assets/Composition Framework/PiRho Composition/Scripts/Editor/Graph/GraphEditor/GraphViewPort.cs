using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class GraphViewPort : Port
	{
		public const string UssClassName = GraphViewNode.UssClassName + "__port";
		public const string InCallstackUssClassName = UssClassName + "--in-callstack";

		public GraphViewNode Node { get; private set; }

		public GraphViewPort(GraphViewNode node, GraphViewConnector edgeListener, bool isInput) : base(Orientation.Horizontal, isInput ? Direction.Input : Direction.Output, isInput ? Capacity.Multi : Capacity.Single, null)
		{
			AddToClassList(UssClassName);
			Node = node;
			m_EdgeConnector = new EdgeConnector<Edge>(edgeListener);
			this.AddManipulator(m_EdgeConnector);
		}

		protected void ShowEditableText(TextField edit)
		{
			edit.style.visibility = Visibility.Visible;
			edit.Focus();
		}

		protected void HideEditableText(TextField edit)
		{
			if (edit != null) // this needs to be here for when it is called after node is destroyed
				edit.style.visibility = Visibility.Hidden;
		}

		private void OnEditEvent(MouseDownEvent evt, TextField edit)
		{
			if (evt.clickCount == 2 && evt.button == (int)MouseButton.LeftMouse)
			{
				ShowEditableText(edit);
				evt.PreventDefault();
			}
		}
	}

	public class GraphViewInputPort : GraphViewPort
	{
		public const string UssInputClassName = UssClassName + "--input";

		public GraphViewInputPort(GraphViewNode node, GraphViewConnector edgeListener) : base(node, edgeListener, true)
		{
			AddToClassList(UssInputClassName);

			m_ConnectorText.style.marginLeft = 0;
			m_ConnectorText.style.marginRight = 0;

			style.alignSelf = Align.Center;
		}

		public void UpdateColor(bool inCallstack)
		{
			EnableInClassList(InCallstackUssClassName, inCallstack);
		}
	}

	public class GraphViewOutputPort : GraphViewPort
	{
		public const string UssOutputClassName = UssClassName + "--output";

		public GraphNode.ConnectionData Connection { get; private set; }

		public GraphViewOutputPort(GraphViewNode node, GraphNode.ConnectionData connection, GraphViewConnector edgeListener, SerializedProperty nameProperty) : base(node, edgeListener, false)
		{
			AddToClassList(UssOutputClassName);

			Connection = connection;

			style.flexDirection = FlexDirection.RowReverse;
			style.justifyContent = Justify.SpaceBetween;
			style.alignSelf = Align.Stretch;

			m_ConnectorText.style.flexGrow = 1;
			m_ConnectorText.style.unityTextAlign = TextAnchor.MiddleLeft;

			if (nameProperty != null)
				m_ConnectorText.bindingPath = nameProperty.propertyPath;
		}

		public override void OnStartEdgeDragging()
		{
			base.OnStartEdgeDragging();

			var output = m_EdgeConnector.edgeDragHelper.edgeCandidate?.output;
			if (output == this)
			{
				var graph = GetFirstAncestorOfType<GraphView>();
				graph.DeleteElements(connections);
			}
		}

		public void UpdateColor()
		{
			var inCallstack = Connection.From.Graph.IsInCallStack(Connection);
			EnableInClassList(InCallstackUssClassName, inCallstack);
		}


		//protected TextField CreateEditableLabel(TextElement container, string bindingPath, bool multiline = false)
		//{
		//	var edit = new TextField { bindingPath = bindingPath, multiline = multiline };
		//	edit.AddToClassList(NodeEditableLabelUssClassName);
		//	edit.Q(TextField.textInputUssName).RegisterCallback<FocusOutEvent>(evt => HideEditableText(edit));
		//
		//	container.bindingPath = bindingPath;
		//	container.RegisterCallback<MouseDownEvent>(evt => OnEditEvent(evt, edit));
		//	container.Add(edit);
		//
		//	HideEditableText(edit);
		//
		//	return edit;
		//}
		//
		//protected void ShowEditableText(TextField edit)
		//{
		//	edit.style.visibility = Visibility.Visible;
		//	edit.Focus();
		//}
		//
		//protected void HideEditableText(TextField edit)
		//{
		//	if (edit != null) // this needs to be here for when it is called after node is destroyed
		//		edit.style.visibility = Visibility.Hidden;
		//}
		//
		//private void OnEditEvent(MouseDownEvent evt, TextField edit)
		//{
		//	if (evt.clickCount == 2 && evt.button == (int)MouseButton.LeftMouse)
		//	{
		//		ShowEditableText(edit);
		//		evt.PreventDefault();
		//	}
		//}
	}
}
