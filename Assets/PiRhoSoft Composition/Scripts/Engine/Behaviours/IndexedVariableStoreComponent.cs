using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionExample
{
	[Serializable]
	public class VariableStoreComponentList : SerializedList<Object> { }

	[HelpURL(Composition.DocumentationUrl + "indexed-variable-store-asset")]
	[CreateAssetMenu(menuName = "PiRho Soft/Indexed Variable Store", fileName = nameof(IndexedVariableStoreAsset), order = 131)]
	public class IndexedVariableStoreComponent : ScriptableObject, IIndexedVariableStore
	{
		[Tooltip("The objects accessable in this list")]
		[ListDisplay]
		public VariableStoreComponentList Objects = new VariableStoreComponentList();

		public int Count => Objects.Count;

		public object GetItem(int index)
		{
			if (index >= 0 && index < Objects.Count)
				return Objects[index];
			else
				return null;
		}

		public bool SetItem(int index, object item)
		{
			if (index >= 0 && index < Objects.Count && item is Object obj)
			{
				Objects[index] = obj;
				return true;
			}

			return false;
		}

		public bool AddItem(object item)
		{
			if (item is Object obj)
			{
				Objects.Add(obj);
				return true;
			}

			return false;
		}

		public bool RemoveItem(int index)
		{
			if (index >= 0 && index < Objects.Count)
			{
				Objects.RemoveAt(index);
				return true;
			}

			return false;
		}

		public VariableValue GetVariable(string name) => IndexedVariableStore.GetVariable(this, name);
		public SetVariableResult SetVariable(string name, VariableValue value) => IndexedVariableStore.SetVariable(this, name, value);
		public IEnumerable<string> GetVariableNames() => IndexedVariableStore.GetVariableNames(this);
	}
}
