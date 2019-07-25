using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(DictionaryAttribute))]
	class DictionaryDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUDDIT) invalid type for DictionaryAttribute on field '{0}': Dictionary can only be applied to SerializedDictionary fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var keys = property.FindPropertyRelative("_keys");
			var values = property.FindPropertyRelative("_values");

			if (keys != null && keys.isArray && values != null && values.isArray && keys.arrayElementType == "string")
			{
				var dictionaryAttribute = attribute as DictionaryAttribute;
				var itemDrawer = this.GetNextDrawer();
				var tooltip = this.GetTooltip();

				return new DictionaryField(keys, values, itemDrawer)
				{
					Label = property.displayName,
					Tooltip = tooltip,
					EmptyLabel = dictionaryAttribute.EmptyLabel,
					AllowAdd = dictionaryAttribute.AllowAdd,
					AllowRemove = dictionaryAttribute.AllowRemove,
				};
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
				return new FieldContainer(property.displayName);
			}
		}
	}
}
