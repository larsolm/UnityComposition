using PiRhoSoft.Utilities.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class SchemaVariableCollectionControl : VariableDictionaryControl
	{
		private ObjectPickerControl _picker;

		public SchemaVariableCollectionControl(SchemaVariableCollectionProxy proxy) : base(proxy)
		{
			_picker = new ObjectPickerControl(proxy.Schema, null, typeof(VariableSchema));
			_picker.RegisterCallback<ChangeEvent<Object>>(evt => proxy.Collection.SetSchema(evt.newValue as VariableSchema));

			Header.Add(_picker);
		}
	}
}
