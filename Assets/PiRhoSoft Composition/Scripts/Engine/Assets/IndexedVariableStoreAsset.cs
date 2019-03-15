using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionExample
{
	[Serializable]
	public class VariableStoreAssetList : SerializedList<VariableStoreAsset> { }

	[HelpURL(Composition.DocumentationUrl + "indexed-variable-store-asset")]
	[CreateAssetMenu(menuName = "PiRho Soft/Indexed Variable Store", fileName = nameof(IndexedVariableStoreAsset), order = 131)]
	public class IndexedVariableStoreAsset : ScriptableObject, IIndexedVariableStore
	{
		[Tooltip("The VariableAssets accessable in this list")]
		[ListDisplay]
		public VariableStoreAssetList VariableStoreAssets = new VariableStoreAssetList();

		public int Count => VariableStoreAssets.Count;
		public IVariableStore GetItem(int index) => index >= 0 && index < VariableStoreAssets.Count ? VariableStoreAssets[index] : null;
		public VariableValue GetVariable(string name) => IndexedVariableStore.GetVariable(this, name);
		public SetVariableResult SetVariable(string name, VariableValue value) => IndexedVariableStore.SetVariable(this, name, value);
		public IEnumerable<string> GetVariableNames() => IndexedVariableStore.GetVariableNames(this);
	}
}
