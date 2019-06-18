using PiRhoSoft.PargonUtilities.Engine;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	[CustomPropertyDrawer(typeof(ListAttribute))]
	class ListDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var list = new ListElement();

			// rebuild when property changes externally

			var items = property.FindPropertyRelative("Items");
			var itemDrawer = PropertyHelper.GetNextDrawer(fieldInfo, attribute);
			var proxy = itemDrawer != null ? new PropertyDrawerListProxy(items, itemDrawer) : new PropertyListProxy(items);

			list.Setup(property.displayName, proxy);

			return list;
		}

		private class PropertyDrawerListProxy : PropertyListProxy
		{
			private PropertyDrawer _drawer;

			public PropertyDrawerListProxy(SerializedProperty property, PropertyDrawer drawer) : base(property)
			{
				_drawer = drawer;
			}

			public override VisualElement CreateElement(int index)
			{
				return _drawer.CreatePropertyGUI(Property.GetArrayElementAtIndex(index));
			}
		}
	}
}
