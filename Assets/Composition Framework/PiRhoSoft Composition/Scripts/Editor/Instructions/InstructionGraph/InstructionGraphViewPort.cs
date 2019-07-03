using PiRhoSoft.CompositionEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.CompositionEditor
{
	public class InstructionGraphViewPort : Port
	{
		public InstructionGraphViewNode Node { get; private set; }

		public InstructionGraphViewPort(InstructionGraphViewNode node, InstructionGraphViewConnector edgeListener, bool isInput) : base(Orientation.Horizontal, isInput ? Direction.Input : Direction.Output, isInput ? Capacity.Multi : Capacity.Single, null)
		{
			Node = node;
			m_EdgeConnector = new EdgeConnector<Edge>(edgeListener);
			this.AddManipulator(m_EdgeConnector);
		}
	}

	public class InstructionGraphViewInputPort : InstructionGraphViewPort
	{
		public InstructionGraphViewInputPort(InstructionGraphViewNode node, InstructionGraphViewConnector edgeListener) : base(node, edgeListener, true)
		{
			m_ConnectorText.style.marginLeft = 0;
			m_ConnectorText.style.marginRight = 0;

			style.alignSelf = Align.Center;
		}
	}

	public class InstructionGraphViewOutputPort : InstructionGraphViewPort
	{
		public InstructionGraphNode.ConnectionData Connection { get; private set; }

		public InstructionGraphViewOutputPort(InstructionGraphViewNode node, InstructionGraphNode.ConnectionData connection, InstructionGraphViewConnector edgeListener) : base(node, edgeListener, false)
		{
			Connection = connection;

			m_ConnectorText.style.flexGrow = 1;
			m_ConnectorText.style.unityTextAlign = TextAnchor.MiddleLeft;

			style.flexDirection = FlexDirection.RowReverse;
			style.justifyContent = Justify.SpaceBetween;
			style.alignSelf = Align.Stretch;
		}
	}
}
