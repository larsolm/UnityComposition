﻿using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Enable Game Object", 10)]
	[HelpURL(Composition.DocumentationUrl + "enable-game-object-node")]
	public class EnableGameObjectNode : InstructionGraphNode
	{
		private const string _missingObjectWarning = "(COMEGONMO) Unable to enable object for {0}: the given variables must be a GameObject";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		public override Color NodeColor => Colors.SequencingLight;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.Root is GameObject target)
				target.SetActive(true);
			else
				Debug.LogWarningFormat(this, _missingObjectWarning, Name);

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
