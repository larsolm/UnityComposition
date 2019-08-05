using PiRhoSoft.Utilities.Editor;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableSchemaEntryControl : VisualElement
	{
		public const string Stylesheet = "Variables/VariableSchema/VariableSchemaEntryStyle.uss";
		public const string UssClassName = "pirho-variable-schema-entry";
		public const string InitializerUssClassName = UssClassName + "__initializer";
		public const string InitializerModeAdvancedUssClassName = InitializerUssClassName + "--advanced";
		public const string InitializerLabelUssClassName = InitializerUssClassName + "__label";
		public const string InitializerModeUssClassName = InitializerUssClassName + "__mode";
		public const string InitializerSimpleUssClassName = InitializerUssClassName + "__simple";
		public const string InitializerAdvancedUssClassName = InitializerUssClassName + "__advanced";
		public const string TagUssClassName = UssClassName + "__tag";

		private static readonly Icon _modeIcon = Icon.BuiltIn("d_CustomSorting");

		public VariableSchemaEntry Value;
		public List<string> Tags;

		private VariableDefinitionControl _definitionControl;
		private VisualElement _tag;
		private VisualElement _initializer;

		private IconButton _modeToggle;
		private VariableControl _simpleControl;
		private ExpressionControl _advancedControl;

		private bool _advancedMode = false;

		public VariableSchemaEntryControl(List<string> tags, VariableSchemaEntry value)
		{
			this.AddStyleSheet(CompositionEditor.EditorPath, Stylesheet);
			AddToClassList(UssClassName);

			Value = value;
			Tags = tags;

			_definitionControl = new VariableDefinitionControl(Value.Definition);
			_definitionControl.RegisterCallback<ChangeEvent<VariableDefinition>>(evt => Refresh());

			_tag = new VisualElement();
			_tag.AddToClassList(TagUssClassName);

			_initializer = new VisualElement();
			_initializer.AddToClassList(InitializerUssClassName);

			Add(_definitionControl);
			Add(_tag);
			Add(_initializer);

			Refresh();
		}

		private void Refresh()
		{
			_definitionControl.SetValueWithoutNotify(Value.Definition);
			_tag.Clear();
			_initializer.Clear();

			if (Tags.Count > 0)
				CreateTag();

			if (HasInitializer())
			{
				CreateInitializer();
				RefreshInitializer();
			}
		}

		private void RefreshInitializer()
		{
			_simpleControl.SetDefinition(Value.Definition);

			EnableInClassList(InitializerModeAdvancedUssClassName, _advancedMode);
		}

		private void CreateTag()
		{
			var popup = new PopupField<string>("Tag:", Tags, Value.Tag);

			_tag.Add(popup);
		}

		private void CreateInitializer()
		{
			_modeToggle = new IconButton(_modeIcon.Texture, "Toggle editing by default value (simple) vs expression (advanced)", () =>
			{
				_advancedMode = !_advancedMode;
				RefreshInitializer();
			});

			_modeToggle.AddToClassList(InitializerModeUssClassName);

			var label = new Label("Initializer:");
			label.AddToClassList(InitializerLabelUssClassName);

			// TODO: Are these the correct things to pass in the editor? null doesn't work with new isolate scopes
			var defaultValue = Value.Initializer.Execute(CompositionManager.Instance, CompositionManager.Instance.DefaultStore);
			if (!Value.Definition.IsValid(defaultValue))
			{
				defaultValue = Value.Definition.Generate();
				SetInitializer(defaultValue);
			}

			_simpleControl = new VariableControl(defaultValue, Value.Definition);
			_simpleControl.AddToClassList(InitializerSimpleUssClassName);
			_simpleControl.RegisterCallback<ChangeEvent<Variable>>(evt => SetInitializer(evt.newValue));

			_advancedControl = new ExpressionControl(Value.Initializer) { tooltip = "The Expression to execute and assign when initializing, resetting, or updating the variable" };
			_advancedControl.AddToClassList(InitializerAdvancedUssClassName);

			_initializer.Add(_modeToggle);
			_initializer.Add(label);
			_initializer.Add(_simpleControl);
			_initializer.Add(_advancedControl);
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

		private void SetInitializer(Variable variable)
		{
			switch (variable.Type)
			{
				case VariableType.Bool: Value.Initializer.SetStatement(variable.AsBool ? "true" : "false"); break;
				case VariableType.Float: Value.Initializer.SetStatement(variable.AsFloat.ToString()); break;
				case VariableType.Int: Value.Initializer.SetStatement(variable.AsInt.ToString()); break;
				case VariableType.Vector2Int: Value.Initializer.SetStatement($"Vector2Int({variable.AsVector2Int.x}, {variable.AsVector2Int.y})"); break;
				case VariableType.Vector3Int: Value.Initializer.SetStatement($"Vector3Int({variable.AsVector3Int.x}, {variable.AsVector3Int.y}, {variable.AsVector3Int.z})"); break;
				case VariableType.RectInt: Value.Initializer.SetStatement($"RectInt({variable.AsRectInt.x}, {variable.AsRectInt.y}, {variable.AsRectInt.width}, {variable.AsRectInt.height})"); break;
				case VariableType.BoundsInt: Value.Initializer.SetStatement($"BoundsInt({variable.AsBoundsInt.x}, {variable.AsBoundsInt.y}, {variable.AsBoundsInt.z}, {variable.AsBoundsInt.size.x}, {variable.AsBoundsInt.size.y}, {variable.AsBoundsInt.size.z})"); break;
				case VariableType.Vector2: Value.Initializer.SetStatement($"Vector2({variable.AsVector2.x}, {variable.AsVector2.y})"); break;
				case VariableType.Vector3: Value.Initializer.SetStatement($"Vector3({variable.AsVector3.x}, {variable.AsVector3.y}, {variable.AsVector3.z})"); break;
				case VariableType.Vector4: Value.Initializer.SetStatement($"Vector4({variable.AsVector4.x}, {variable.AsVector4.y}, {variable.AsVector4.z}, {variable.AsVector4.w})"); break;
				case VariableType.Quaternion: var euler = variable.AsQuaternion.eulerAngles; Value.Initializer.SetStatement($"Quaternion({euler.x}, {euler.y}, {euler.z})"); break;
				case VariableType.Rect: Value.Initializer.SetStatement($"Rect({variable.AsRect.x}, {variable.AsRect.y}, {variable.AsRect.width}, {variable.AsRect.height})"); break;
				case VariableType.Bounds: Value.Initializer.SetStatement($"Bounds({variable.AsBounds.center}, {variable.AsBounds.extents})"); break;
				case VariableType.Color: Value.Initializer.SetStatement(string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", Mathf.RoundToInt(variable.AsColor.r * 255), Mathf.RoundToInt(variable.AsColor.g * 255), Mathf.RoundToInt(variable.AsColor.b * 255), Mathf.RoundToInt(variable.AsColor.a * 255))); break;
				case VariableType.String: Value.Initializer.SetStatement("\"" + variable.AsString + "\""); break;
			}
		}
	}
}
