﻿using PiRhoSoft.UtilityEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Sequencing/TimeScale", 250)]
	[HelpURL(Composition.DocumentationUrl + "time-scale-node")]
	public class TimeScaleNode : InstructionGraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The value to set the TimeScale to")]
		[InlineDisplay(PropagateLabel = true)]
		[VariableConstraint(0.0f, 100.0f)]
		public NumberVariableSource TimeScale = new NumberVariableSource(1.0f);

		public override Color NodeColor => Colors.Sequencing;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (Resolve(variables, TimeScale, out var time))
				Time.timeScale = time;

			graph.GoTo(Next, nameof(Next));
			yield break;
		}
	}
}