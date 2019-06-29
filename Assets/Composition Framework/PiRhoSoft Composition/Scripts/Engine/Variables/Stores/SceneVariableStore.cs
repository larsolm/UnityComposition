using PiRhoSoft.PargonUtilities.Engine;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class SceneVariableStore : IVariableStore
	{
		private static readonly string[] _emptyNames = new string[0];

		public VariableValue GetVariable(string name)
		{
			var gameObject = GameObject.Find(name); // this won't find inactive objects but is fast (comparatively)

			if (gameObject == null)
				gameObject = ComponentHelper.FindObject(name); // this will find inactive objects

			if (gameObject == null)
				return VariableValue.Empty;

			return VariableValue.Create(gameObject);
		}

		public SetVariableResult SetVariable(string name, VariableValue value)
		{
			return SetVariableResult.ReadOnly;
		}

		public IList<string> GetVariableNames()
		{
			return _emptyNames;
		}
	}
}
