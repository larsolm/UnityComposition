using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities.Editor
{
	public class ObjectPickerControl : BasePickerControl<Object>, IDragReceiver
	{
		private class ObjectProvider : PickerProvider<Object> { }

		private const string _invalidTypeWarning = "(PUOPCIT) Invalid type for ObjectPickerControl: the type '{0}' must be derived from UnityEngine.Object";

		public new const string Stylesheet = "Picker/ObjectPicker/ObjectPickerStyle.uss";
		public new const string UssClassName = "pirho-object-picker";
		public const string InspectUssClassName = UssClassName + "__inspect";

		public Type Type { get; private set; }

		private IconButton _inspect;

		public ObjectPickerControl(Object value, Type type)
		{
			if (type == null || !(typeof(Object).IsAssignableFrom(type)))
			{
				Debug.LogWarningFormat(_invalidTypeWarning, type);
				return;
			}

			Type = type;

			_inspect = new IconButton(Icon.Inspect.Texture, "View this object in the inspector", Inspect);
			_inspect.AddToClassList(InspectUssClassName);

			//if (typeof(Component).IsAssignableFrom(type) || typeof(GameObject) == type)
			//{
			//	TODO: build hierarchy of game objects
			//}
			//else
			//{
			var assets = AssetHelper.GetAssetList(type);
			var provider = ScriptableObject.CreateInstance<ObjectProvider>();
			provider.Setup(assets.Type.Name, assets.Paths.Prepend("None").ToList(), assets.Assets.Prepend(null).ToList(), GetIcon, OnSelected);
			//}

			Setup(provider, value);
			Add(_inspect);

			this.AddStyleSheet(Configuration.ElementsPath, Stylesheet);
			AddToClassList(UssClassName);
		}

		private Texture GetIcon(Object asset)
		{
			var icon = AssetPreview.GetMiniThumbnail(asset);
			return icon == null && asset ? AssetPreview.GetMiniTypeThumbnail(asset.GetType()) : icon;
		}

		private void OnSelected(Object selected)
		{
			var previous = Value;
			SetValueWithoutNotify(selected);
			this.SendChangeEvent(previous, Value);
		}

		private void Inspect()
		{
			if (Value)
				Selection.activeObject = Value;
		}

		protected override void Refresh()
		{
			var text = Value == null ? $"None ({Type.Name})" : Value.name;
			var icon = GetIcon(Value);

			SetLabel(icon, text);

			_inspect.SetEnabled(Value);
		}

		#region IDragReceiver Implementation

		public bool IsDragValid(Object[] objects, object data)
		{
			if (objects.Length > 0)
			{
				var obj = objects[0];
				if (obj != null)
				{
					var drag = obj.GetType();
					return Type.IsAssignableFrom(drag);
				}
			}

			return true;
		}

		public void AcceptDrag(Object[] objects, object data)
		{
			OnSelected(objects[0]);
		}

		#endregion
	}
}
