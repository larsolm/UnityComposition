﻿using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateAssetMenu(fileName = nameof(ScopedGraph), menuName = "Composition/Instruction Graph - Scoped", order = 126)]
	public class ScopedGraph : InstructionGraph
	{
		public InstructionGraphNode Enter = null;
		public InstructionGraphNode Process = null;
		public InstructionGraphNode Exit = null;

		protected override IEnumerator Run(InstructionStore variables)
		{
			yield return Run(variables, Enter);
			yield return Run(variables, Process);
			yield return Run(variables, Exit);
		}
	}
}
