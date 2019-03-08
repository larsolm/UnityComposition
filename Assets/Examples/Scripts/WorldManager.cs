using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionExample
{
	[AddComponentMenu("PiRho Soft/Examples/World Manager")]
	public class WorldManager : MonoBehaviour, IReloadable, IVariableStore
	{
		[ReloadOnChange] [AssetPopup] public VariableSchema Schema;

		public VariableList Variables = new VariableList();
		public MappedVariableStore Store = new MappedVariableStore();

		[MappedVariable] public int IntField = 7;
		[MappedVariable] public int IntProperty { get; set; }

		public virtual void OnEnable()
		{
			Store.Setup(this, Schema, Variables);
		}

		public void OnDisable()
		{
		}

		#region IVariableStore Implementation

		public VariableValue GetVariable(string name) => Store.GetVariable(name);
		public SetVariableResult SetVariable(string name, VariableValue value) => Store.SetVariable(name, value);
		public IEnumerable<string> GetVariableNames() => Store.GetVariableNames();

		#endregion
	}
}
