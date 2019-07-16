using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities.Editor
{
	public class TypePicker : BasePickerButton<string>, IDragReceiver
	{
		private class TypeProvider : PickerProvider<Type> { }

		private const string _invalidTypeWarning = "(PUCTPIT) Invalid type for TypePicker: the type '{0}' could not be found";

		private readonly Action<Type> _onSelected;

		private Type _type;

		public TypePicker(Action<Type> onSelected) { _onSelected = onSelected; }
		public TypePicker(SerializedProperty property) : base(property) { }
		public TypePicker(Object owner, Func<string> getValue, Action<string> setValue) : base(owner, getValue, setValue) { }

		public void Setup(Type type, bool showAbstract, Type value)
		{
			if (type == null)
			{
				Debug.LogWarningFormat(_invalidTypeWarning, type);
				return;
			}

			_type = type;

			var valueName = value?.AssemblyQualifiedName;
			var initialType = Type.GetType(valueName ?? string.Empty);

			var types = TypeHelper.GetTypeList(_type, showAbstract);
			var provider = ScriptableObject.CreateInstance<TypeProvider>();
			provider.Setup(types.BaseType.Name, types.Paths.Prepend("None").ToList(), types.Types.Prepend(null).ToList(), iconType => AssetPreview.GetMiniTypeThumbnail(iconType), selectedValue =>
			{
				_onSelected?.Invoke(selectedValue);
				ElementHelper.SendChangeEvent(this, valueName, selectedValue?.AssemblyQualifiedName);
			});

			Setup(provider, valueName);

			DragHelper.MakeDragReceiver(this);
		}

		public override string GetValueFromProperty(SerializedProperty property)
		{
			return property.stringValue;
		}

		public override void UpdateProperty(string value, VisualElement element, SerializedProperty property)
		{
			property.stringValue = value;
		}

		protected override void Refresh()
		{
			var type = Type.GetType(Value ?? string.Empty);
			var text = type == null ? $"None ({_type.Name})" : type.Name;
			var icon = AssetPreview.GetMiniTypeThumbnail(type);

			SetLabel(icon, text);
		}

		#region Drag and Drop

		public bool IsDragValid(Object[] objects, object data)
		{
			if (objects.Length > 0)
			{
				var obj = objects[0];
				if (obj != null)
				{
					var drag = obj.GetType();
					return _type.IsAssignableFrom(drag);
				}
			}

			return true;
		}

		public void AcceptDrag(Object[] objects, object data)
		{
			ElementHelper.SendChangeEvent(this, Value, objects[0].GetType().AssemblyQualifiedName);
		}

		#endregion
	}
}
