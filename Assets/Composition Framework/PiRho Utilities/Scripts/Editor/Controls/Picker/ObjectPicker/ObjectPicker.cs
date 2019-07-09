using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities.Editor
{
	public class ObjectPicker : BasePickerButton<Object>
	{
		private class ObjectProvider : PickerProvider<Object> { }

		private const string _invalidTypeWarning = "(PUCOPIT) Invalid type for ObjectPicker: the type '{0}' must be derived from UnityEngine.Object";

		public Type Type { get; private set; }

		private Image _inspect;

		public ObjectPicker(SerializedProperty property) : base(property) { }
		public ObjectPicker(Object owner, Func<Object> getValue, Action<Object> setValue) : base(owner, getValue, setValue) { }

		public void Setup(Type type, string assetPath)
		{
			Setup(type, AssetDatabase.LoadAssetAtPath<Object>(assetPath));
		}

		public void Setup(Type type, Object value)
		{
			if (type == null || !(typeof(Object).IsAssignableFrom(type)))
			{
				Debug.LogWarningFormat(_invalidTypeWarning, type);
				return;
			}

			Type = type;

			_inspect = ElementHelper.CreateIconButton(Icon.Inspect.Content, "View this object in the inspector", Inspect);

			//if (typeof(Component).IsAssignableFrom(type) || typeof(GameObject) == type)
			//{
			//	TODO: build hierarchy of game objects
			//}
			//else
			//{
			var assets = AssetHelper.GetAssetList(type);
			var provider = ScriptableObject.CreateInstance<ObjectProvider>();
			provider.Setup(assets.Type.Name, assets.Paths.Prepend("None").ToList(), assets.Assets.Prepend(null).ToList(), asset =>
			{
				var icon = AssetPreview.GetMiniThumbnail(asset);
				return icon == null && asset ? AssetPreview.GetMiniTypeThumbnail(asset.GetType()) : icon;
			},
			selectedObject => ElementHelper.SendChangeEvent(this, value, selectedObject));
			//}

			Setup(provider, value);

			Add(_inspect);
		}

		private void Inspect()
		{
			if (Value)
				Selection.activeObject = Value;
		}

		#region BindableValueElement Implementation

		public override Object GetValueFromProperty(SerializedProperty property)
		{
			if (property.propertyType == SerializedPropertyType.ObjectReference)
				return property.objectReferenceValue;
			else if (property.propertyType == SerializedPropertyType.String)
				return AssetDatabase.LoadAssetAtPath<Object>(property.stringValue);
			else
				return null;
		}

		public override void UpdateProperty(Object value, VisualElement element, SerializedProperty property)
		{
			if (property.propertyType == SerializedPropertyType.ObjectReference)
				property.objectReferenceValue = value;
			else if (property.propertyType == SerializedPropertyType.String)
				property.stringValue = AssetDatabase.GetAssetPath(value);
		}

		protected override void Refresh()
		{
			var text = Value == null ? $"None ({Type.Name})" : Value.name;
			var icon = AssetPreview.GetMiniThumbnail(Value);

			if (icon == null && Value)
				icon = AssetPreview.GetMiniTypeThumbnail(Value.GetType());

			SetLabel(icon, text);

			_inspect.SetEnabled(Value);
		}

		#endregion
	}
}
