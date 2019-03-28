using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "selection-binding")]
	[AddComponentMenu("PiRho Soft/Interface/Selection Binding")]
	public class SelectionBinding : BindingRoot
	{
		public static string FocusName = "focus";

		[Tooltip("The Selection Control whose focused item to use for binding variables")]
		public SelectionControl SelectionControl;

		public override IEnumerable<string> GetVariableNames() => Enumerable.Repeat(FocusName, 1).Concat(base.GetVariableNames());
		public override VariableValue GetVariable(string name) => name == FocusName ? VariableValue.Create(SelectionControl.FocusedVariables) : base.GetVariable(name);
		public override SetVariableResult SetVariable(string name, VariableValue value) => name == FocusName ? SetVariableResult.ReadOnly : base.SetVariable(name, value);
	}
}
