using PiRhoSoft.UtilityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Sequencing/TimeScale", 250)]
	[HelpURL(Composition.DocumentationUrl + "time-scale-node")]
	public class TimeScaleNode : InstructionGraphNode
	{
		private const string _timeNotFoundWarning = "(CCFTNF) Unable to set time scale on {0}: the time could not be found";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The value to set the TimeScale to")]
		[InlineDisplay(PropagateLabel = true)]
		public NumberVariableSource TimeScale = new NumberVariableSource(1.0f);

		public override Color NodeColor => Colors.Sequencing;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			TimeScale.GetInputs(inputs);
		}

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (Resolve(variables, TimeScale, out var time))
				Time.timeScale = time;
			else
				Debug.LogFormat(this, _timeNotFoundWarning, Name);

			graph.GoTo(Next, nameof(Next));
			yield break;
		}
	}
}
