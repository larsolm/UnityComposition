﻿using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "update-binding-node")]
	[CreateInstructionGraphNodeMenu("Interface/Update Binding", 201)]
	public class UpdateBindingNode : InstructionGraphNode
	{
		[Tooltip("The node to go to once the control is shown")]
		public InstructionGraphNode Next = null;

		[Tooltip("The control to update")]
		public InterfaceReference Control = new InterfaceReference();

		[Tooltip("The binding group to update (updates all if empty)")]
		public string Group;

		public override Color NodeColor => Colors.Interface;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			var control = Control.GetControl<InterfaceControl>(this);

			if (control)
				control.UpdateBindings(variables, Group);

			graph.GoTo(Next, variables.This, nameof(Next));

			yield break;
		}
	}
}