﻿using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition.Extensions
{
	[CreateGraphNodeMenu("Interface/Set Binding", 300)]
	public class SetBindingNode : GraphNode
	{
		[Tooltip("The node to go to once the binding is set")]
		public GraphNode Next = null;

		[Tooltip("The Binding Root to set bindings on")]
		[VariableReference(typeof(BindingRoot))]
		public VariableLookupReference Object = new VariableLookupReference();

		[Tooltip("The Variable store to set the bindings to")]
		public VariableLookupReference Binding = new VariableLookupReference();

		public override Color NodeColor => Colors.Interface;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (variables.ResolveObject(this, Object, out BindingRoot root))
			{
				root.Value = Binding.GetValue(variables);
				VariableBinding.UpdateBinding(root.gameObject, null, null);
			}

			graph.GoTo(Next, nameof(Next));
			yield break;
		}
	}
}
