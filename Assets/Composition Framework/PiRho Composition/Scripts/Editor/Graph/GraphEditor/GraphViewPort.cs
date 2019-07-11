using PiRhoSoft.Composition.Engine;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class GraphViewPort : Port
	{
		public GraphViewNode Node { get; private set; }

		public GraphViewPort(GraphViewNode node, GraphViewConnector edgeListener, bool isInput) : base(Orientation.Horizontal, isInput ? Direction.Input : Direction.Output, isInput ? Capacity.Multi : Capacity.Single, null)
		{
			Node = node;
			m_EdgeConnector = new EdgeConnector<Edge>(edgeListener);
			this.AddManipulator(m_EdgeConnector);
		}
	}

	public class GraphViewInputPort : GraphViewPort
	{
		public GraphViewInputPort(GraphViewNode node, GraphViewConnector edgeListener) : base(node, edgeListener, true)
		{
			m_ConnectorText.style.marginLeft = 0;
			m_ConnectorText.style.marginRight = 0;

			style.alignSelf = Align.Center;
		}
	}

	public class GraphViewOutputPort : GraphViewPort
	{
		public GraphNode.ConnectionData Connection { get; private set; }

		public GraphViewOutputPort(GraphViewNode node, GraphNode.ConnectionData connection, GraphViewConnector edgeListener) : base(node, edgeListener, false)
		{
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
	}
}
