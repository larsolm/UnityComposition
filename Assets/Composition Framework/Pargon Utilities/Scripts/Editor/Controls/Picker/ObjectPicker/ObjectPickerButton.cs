using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class ObjectPickerButton : BasePickerButton<Object>
	{
		private const string _invalidTypeWarning = "(PIOPIT) Invalid type for ObjectPicker: the type '{0}' could not be found";

		private class Factory : UxmlFactory<ObjectPickerButton, Traits> { }

		private class Traits : UxmlTraits
		{
			private UxmlStringAttributeDescription _type = new UxmlStringAttributeDescription { name = "type", use = UxmlAttributeDescription.Use.Required };
			private UxmlStringAttributeDescription _path = new UxmlStringAttributeDescription { name = "asset-path" };

			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get { yield break; }
			}

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);

				var objectPicker = ve as ObjectPickerButton;
				var typeName = _type.GetValueFromBag(bag, cc);
				var path = _path.GetValueFromBag(bag, cc);
				var type = Type.GetType(typeName);
				var obj = AssetDatabase.LoadAssetAtPath<Object>(path);

				if (objectPicker.Type != null)
					objectPicker.Setup(type, obj);
				else
					Debug.LogWarningFormat(_invalidTypeWarning, typeName);
			}
		}

		public Type Type { get; private set; }

		private Image _inspect;

		public void Setup(Type type, SerializedProperty property)
		{
			if (property.propertyType == SerializedPropertyType.ObjectReference)
				Setup(type, property.objectReferenceValue);
			else if (property.propertyType == SerializedPropertyType.String)
				Setup(type, AssetDatabase.LoadAssetAtPath<Object>(property.stringValue));

			BindToProperty(property);
		}

		public void Setup(Type type, Object initialValue)
		{
			Type = type;

			var picker = new ObjectPicker();
			picker.Setup(Type, initialValue);
			picker.OnSelected += selectedObject => value = selectedObject;

			_inspect = new Image();
			_inspect.image = Icon.Inspect.Content;
			_inspect.AddManipulator(new Clickable(Inspect));

			Setup(picker, initialValue);

			Add(_inspect);
		}

		private void Inspect()
		{
			if (value)
				Selection.activeObject = value;
		}

		#region BindableValueElement Implementation

		protected override void SetValueToProperty(SerializedProperty property, Object value)
		{
			if (property.propertyType == SerializedPropertyType.ObjectReference)
				property.objectReferenceValue = value;
			else if (property.propertyType == SerializedPropertyType.String)
				property.stringValue = AssetDatabase.GetAssetPath(value);
		}

		protected override Object GetValueFromProperty(SerializedProperty property)
		{
			if (property.propertyType == SerializedPropertyType.ObjectReference)
				return property.objectReferenceValue;
			else if (property.propertyType == SerializedPropertyType.String)
				return AssetDatabase.LoadAssetAtPath<Object>(property.stringValue);
			else
				return null;
		}

		protected override void Refresh()
		{
			var text = value == null ? $"None ({Type.Name})" : value.name;
			var icon = AssetPreview.GetMiniThumbnail(value);

			if (icon == null && value)
				icon = AssetPreview.GetMiniTypeThumbnail(value.GetType());

			SetLabel(icon, text);

			_inspect.SetEnabled(value);
		}

		#endregion
	}
}
