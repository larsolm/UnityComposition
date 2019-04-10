﻿using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "variable-pool-component")]
	[AddComponentMenu("PiRho Soft/Composition/Variable Pool Component")]
	public class VariablePoolComponent : MonoBehaviour, IVariableStore
	{
		public VariableStore Variables = new VariableStore();

		public IEnumerable<string> GetVariableNames() => Variables.GetVariableNames();
		public VariableValue GetVariable(string name) => Variables.GetVariable(name);
		public SetVariableResult SetVariable(string name, VariableValue value) => Variables.SetVariable(name, value);
	}
}
