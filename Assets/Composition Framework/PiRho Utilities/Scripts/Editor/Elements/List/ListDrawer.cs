using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(ListAttribute))]
	class ListDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PULDIT) invalid type for ListAttribute on field '{0}': List can only be applied to SerializedList or SerializedArray fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var items = property.FindPropertyRelative("_items");

			if (items != null && items.isArray)
			{
				var listAttribute = attribute as ListAttribute;
				var itemDrawer = this.GetNextDrawer();
				var tooltip = this.GetTooltip();

				return new ListField(items, itemDrawer)
				{
					Label = property.displayName,
					Tooltip = tooltip,
					EmptyLabel = listAttribute.EmptyLabel,
					AllowAdd = listAttribute.AllowAdd,
					AllowRemove = listAttribute.AllowRemove,
					AllowReorder = listAttribute.AllowReorder
				};
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
				return new FieldContainer(property.displayName, "");
			}
		}
	}
}
