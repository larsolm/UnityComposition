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
			//m_ConnectorText.bindingPath = "";
			this.AddManipulator(m_EdgeConnector);
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

		public GraphViewOutputPort(GraphViewNode node, GraphNode.ConnectionData connection, GraphViewConnector edgeListener) : base(node, edgeListener, false)
		{
			AddToClassList(UssOutputClassName);

			Connection = connection;

			m_ConnectorText.style.flexGrow = 1;
			m_ConnectorText.style.unityTextAlign = TextAnchor.MiddleLeft;

			style.flexDirection = FlexDirection.RowReverse;
			style.justifyContent = Justify.SpaceBetween;
			style.alignSelf = Align.Stretch;
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
	}
}
