using PiRhoSoft.Utilities.Editor;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableSchemaEntryControl : VisualElement
	{
		private static readonly Icon _simpleIcon = Icon.BuiltIn("d_CustomSorting");
		private static readonly Icon _advancedIcon = Icon.BuiltIn("CustomSorting");

		public VariableSchema.Entry Value;
		public List<string> Tags;

		private VariableDefinitionControl _definitionControl;
		private VisualElement _initializer;
		private VisualElement _tags;
		private IconButton _modeToggle;

		private bool _advancedMode = false;

		public VariableSchemaEntryControl(List<string> tags, VariableSchema.Entry value)
		{
			Value = value;
			Tags = tags;

			_definitionControl = new VariableDefinitionControl(Value.Definition);
			_initializer = new VisualElement();
			_tags = new VisualElement();

			Refresh();
		}

		private void Refresh()
		{
			_definitionControl.SetValueWithoutNotify(Value.Definition);
			_initializer.Clear();
			_tags.Clear();

			if (Tags.Count > 0)
				CreateTag();

			if (HasInitializer())
				CreateInitializer();

			RefreshMode();
		}

		private void RefreshMode()
		{
			if (_modeToggle != null)
			{
				_modeToggle.image = _advancedMode ? _advancedIcon.Texture : _simpleIcon.Texture;
				_modeToggle.tintColor = _advancedMode ? Color.white : Color.black;
			}
		}

		private void CreateTag()
		{
			var popup = new PopupField<string>("Tag", Tags, Value.Tag);
			_tags.Add(popup);
		}

		private void CreateInitializer()
		{
			_modeToggle = new IconButton(_simpleIcon.Texture, "Toggle simple (Default Value) vs. Advanced (Expression) mode", () =>
			{
				_advancedMode = !_advancedMode;
				RefreshMode();
			});

			_initializer.Add(_modeToggle);

			if (_advancedMode)
			{
				var expression = new ExpressionField("Expression", Value.Initializer) { tooltip = "The Expression to execute and assign when initializing, resetting, or updating the variable" };
				_initializer.Add(expression);
			}
			else
			{
				var defaultValue = Value.Initializer.Execute(null, null); // context isn't necessary since the object that would be the context is currently drawing
				if (defaultValue.IsEmpty) // If the initializer hasn't been set, use the default value.
					defaultValue = Value.Definition.Generate();

				var field = new VariableField("Default", defaultValue, Value.Definition);
				field.RegisterValueChangedCallback(evt =>
				{
					switch (defaultValue.Type)
					{
						case VariableType.Bool: Value.Initializer.SetStatement(evt.newValue.AsBool ? "true" : "false"); break;
						case VariableType.Float: Value.Initializer.SetStatement(evt.newValue.AsFloat.ToString()); break;
						case VariableType.Int: Value.Initializer.SetStatement(evt.newValue.AsInt.ToString()); break;
						case VariableType.Vector2Int: Value.Initializer.SetStatement(string.Format("Vector2Int({0}, {1})", evt.newValue.AsVector2Int.x, evt.newValue.AsVector2Int.y)); break;
						case VariableType.Vector3Int: Value.Initializer.SetStatement(string.Format("Vector3Int({0}, {1}, {2})", evt.newValue.AsVector3Int.x, evt.newValue.AsVector3Int.y, evt.newValue.AsVector3Int.z)); break;
						case VariableType.RectInt: Value.Initializer.SetStatement(string.Format("RectInt({0}, {1}, {2}, {3})", evt.newValue.AsRectInt.x, evt.newValue.AsRectInt.y, evt.newValue.AsRectInt.width, evt.newValue.AsRectInt.height)); break;
						case VariableType.BoundsInt: Value.Initializer.SetStatement(string.Format("BoundsInt({0}, {1}, {2}, {3}, {4}, {5})", evt.newValue.AsBoundsInt.x, evt.newValue.AsBoundsInt.y, evt.newValue.AsBoundsInt.z, evt.newValue.AsBoundsInt.size.x, evt.newValue.AsBoundsInt.size.y, evt.newValue.AsBoundsInt.size.z)); break;
						case VariableType.Vector2: Value.Initializer.SetStatement(string.Format("Vector2({0}, {1})", evt.newValue.AsVector2.x, evt.newValue.AsVector2.y)); break;
						case VariableType.Vector3: Value.Initializer.SetStatement(string.Format("Vector3({0}, {1}, {2})", evt.newValue.AsVector3.x, evt.newValue.AsVector3.y, evt.newValue.AsVector3.z)); break;
						case VariableType.Vector4: Value.Initializer.SetStatement(string.Format("Vector4({0}, {1}, {2}, {3})", evt.newValue.AsVector4.x, evt.newValue.AsVector4.y, evt.newValue.AsVector4.z, evt.newValue.AsVector4.w)); break;
						case VariableType.Quaternion: var euler = evt.newValue.AsQuaternion.eulerAngles; Value.Initializer.SetStatement(string.Format("Quaternion({0}, {1}, {2})", euler.x, euler.y, euler.z)); break;
						case VariableType.Rect: Value.Initializer.SetStatement(string.Format("Rect({0}, {1}, {2}, {3})", evt.newValue.AsRect.x, evt.newValue.AsRect.y, evt.newValue.AsRect.width, evt.newValue.AsRect.height)); break;
						case VariableType.Bounds: Value.Initializer.SetStatement(string.Format("Bounds({0}, {1})", evt.newValue.AsBounds.center, evt.newValue.AsBounds.extents)); break;
						case VariableType.Color: Value.Initializer.SetStatement(string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", Mathf.RoundToInt(evt.newValue.AsColor.r * 255), Mathf.RoundToInt(evt.newValue.AsColor.g * 255), Mathf.RoundToInt(evt.newValue.AsColor.b * 255), Mathf.RoundToInt(evt.newValue.AsColor.a * 255))); break;
						case VariableType.String: Value.Initializer.SetStatement("\"" + evt.newValue.AsString + "\""); break;
					}
				});

				_initializer.Add(field);
			}
		}

		private bool HasInitializer()
		{
			switch (Value.Definition.Type)
			{
				case VariableType.Bool:
				case VariableType.Float:
				case VariableType.Int:
				case VariableType.Vector2Int:
				case VariableType.Vector3Int:
				case VariableType.RectInt:
				case VariableType.BoundsInt:
				case VariableType.Vector2:
				case VariableType.Vector3:
				case VariableType.Vector4:
				case VariableType.Quaternion:
				case VariableType.Rect:
				case VariableType.Bounds:
				case VariableType.Color:
				case VariableType.String: return true;
				default: return false;
			}
		}
	}
}
