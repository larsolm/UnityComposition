using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Control Flow/Wait", 100)]
	[HelpURL(Composition.DocumentationUrl + "wait-node")]
	public class WaitNode : InstructionGraphNode
	{
		private const string _timeNotFoundWarning = "(CCFTNF) Unable to wait for time on {0}: the time could not be found";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The amount of time (in seconds) to wait")]
		public NumberVariableSource Time = new NumberVariableSource(1.0f);

		public override Color NodeColor => Colors.Sequencing;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			Time.GetInputs(inputs);
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (Time.TryGetValue(variables, this, out var time))
				yield return new WaitForSeconds(time);
			else
				Debug.LogFormat(this, _timeNotFoundWarning, Name);

			graph.GoTo(Next, variables.This, nameof(Next));
		}
	}
}
