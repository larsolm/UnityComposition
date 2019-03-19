using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionExample
{
	[Serializable]
	public class VariableStoreAssetList : SerializedList<Object> { }

	[HelpURL(Composition.DocumentationUrl + "indexed-variable-store-asset")]
	[CreateAssetMenu(menuName = "PiRho Soft/Indexed Variable Store", fileName = nameof(IndexedVariableStoreAsset), order = 131)]
	public class IndexedVariableStoreAsset : ScriptableObject, IIndexedVariableStore
	{
		[Tooltip("The assetsaccessable in this list")]
		[ListDisplay]
		public VariableStoreAssetList Assets = new VariableStoreAssetList();

		public int Count => Assets.Count;
		public object GetItem(int index) => index >= 0 && index < Assets.Count ? Assets[index] : null;
		public bool SetItem(int index, object item) => false;
		public bool AddItem(object item) => false;
		public bool RemoveItem(int index) => false;
		public VariableValue GetVariable(string name) => IndexedVariableStore.GetVariable(this, name);
		public SetVariableResult SetVariable(string name, VariableValue value) => IndexedVariableStore.SetVariable(this, name, value);
		public IEnumerable<string> GetVariableNames() => IndexedVariableStore.GetVariableNames(this);
	}
}
