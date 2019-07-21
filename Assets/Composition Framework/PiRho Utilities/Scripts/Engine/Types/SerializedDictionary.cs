using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Utilities
{
	public interface IEditableDictionary<KeyType, ValueType>
	{
		void PrepareForEdit();
		void ApplyEdits();

		int Count { get; }

		KeyType GetKey(int index);
		ValueType GetValue(int index);

		void Add(KeyType key, ValueType value);
		bool Remove(KeyType key);
	}

	[Serializable]
	public class SerializedDictionary<KeyType, ValueType> : Dictionary<KeyType, ValueType>, ISerializationCallbackReceiver, IEditableDictionary<KeyType, ValueType>
	{
		// these are protected so they can be found by the editor.
		[SerializeField] protected List<KeyType> _keys = new List<KeyType>();
		[SerializeField] protected List<ValueType> _values = new List<ValueType>();

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			ConvertToLists();
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			ConvertFromLists();

			_keys.Clear();
			_values.Clear();
		}

		public void PrepareForEdit()
		{
			ConvertToLists();
		}

		public void ApplyEdits()
		{
			ConvertFromLists();
		}

		private void ConvertToLists()
		{
			_keys.Clear();
			_values.Clear();

			foreach (var entry in this)
			{
				_keys.Add(entry.Key);
				_values.Add(entry.Value);
			}
		}

		private void ConvertFromLists()
		{
			Clear();

			var count = Math.Min(_keys.Count, _values.Count);

			for (var i = 0; i < count; i++)
				Add(_keys[i], _values[i]);
		}

		public KeyType GetKey(int index)
		{
			return _keys[index];
		}

		public ValueType GetValue(int index)
		{
			return _values[index];
		}
	}
}
