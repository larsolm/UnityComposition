using PiRhoSoft.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	public class SceneVariableStore : IVariableCollection
	{
		private static readonly string[] _emptyNames = new string[0];

		public Variable GetVariable(string name)
		{
			var gameObject = GameObject.Find(name); // this won't find inactive objects but is fast (comparatively)

			if (gameObject == null)
				gameObject = ComponentHelper.FindObject(name); // this will find inactive objects

			if (gameObject == null)
				return Variable.Empty;

			return Variable.Object(gameObject);
		}

		public SetVariableResult SetVariable(string name, Variable value)
		{
			return SetVariableResult.ReadOnly;
		}

		public IReadOnlyList<string> VariableNames
		{
			get => _emptyNames;
		}
	}
}
