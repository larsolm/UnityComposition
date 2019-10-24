using PiRhoSoft.Utilities;
using PiRhoSoft.Utilities.Editor;
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class SerializedVariableDictionaryField : BindableElement
	{
		public const string Stylesheet = "Variables/VariableDictionary/SerializedVariableDictionaryStyle.uss";
		public const string UssClassName = "pirho-serialized-variable-dictionary";
		public const string ItemUssClassName = UssClassName + "__item";
		public const string LabelUssClassName = ItemUssClassName + "__label";
		public const string VariableUssClassName = ItemUssClassName + "__variable";

		protected readonly DictionaryField _dictionaryField;

		public SerializedVariableDictionaryField()
		{
			_dictionaryField = new DictionaryField();

			this.AddStyleSheet(Configuration.EditorPath, Stylesheet);
			AddToClassList(UssClassName);
		}

		protected void Setup(SerializedProperty property, VariableDictionaryProxy proxy)
		{
			bindingPath = property.propertyPath;

			var listProperty = property
				.FindPropertyRelative(SerializedVariableDictionary.DataProperty)
				.FindPropertyRelative(SerializedDataList.ContentProperty);

			_dictionaryField.Setup(listProperty, proxy);

			Add(_dictionaryField);

			this.AddStyleSheet(Configuration.EditorPath, Stylesheet);
			AddToClassList(UssClassName);
		}

		protected abstract class VariableDictionaryProxy : IDictionaryProxy
		{
			public abstract string Label { get; }
			public abstract string Tooltip { get; }
			public abstract string EmptyLabel { get; }
			public abstract string EmptyTooltip { get; }
			public virtual string AddPlaceholder => string.Empty;
			public virtual string AddTooltip => string.Empty;
			public virtual string RemoveTooltip => string.Empty;
			public virtual string ReorderTooltip => string.Empty;

			public virtual bool AllowAdd => false;
			public virtual bool AllowRemove => false;
			public virtual bool AllowReorder => false;

			public virtual void AddItem(string key) { }
			public virtual void RemoveItem(int index) { }
			public virtual void ReorderItem(int from, int to) { }

			public virtual bool CanAdd(string key) => false;
			public virtual bool CanRemove(int index) => false;
			public virtual bool CanReorder(int from, int to) => false;

			public int KeyCount => _variables.VariableNames.Count;

			protected readonly SerializedProperty _property;
			protected readonly SerializedProperty _dataProperty;
			protected readonly SerializedVariableDictionary _variables;
			protected readonly SerializedDataList _data;

			public VariableDictionaryProxy(SerializedProperty property, SerializedVariableDictionary variables)
			{
				_property = property;
				_variables = variables;
				_dataProperty = _property
					.FindPropertyRelative(SerializedVariableDictionary.DataProperty)
					.FindPropertyRelative(SerializedDataList.ContentProperty);
				_data = _variables.Data;
			}

			public abstract VisualElement CreateField(int index);

			protected void SetVariable(int index, Variable variable)
			{
				using (new ChangeScope(_property.serializedObject.targetObject))
					_variables.SetVariable(index, variable);

				_property.serializedObject.Update();
			}

			public bool NeedsUpdate(VisualElement item, int index)
			{
				// Definition !=, index !=, name !=
				return !(item.userData is int i) || i != index;
			}

			protected VisualElement CreateContainer(int index)
			{
				var container = new VisualElement { userData = index };
				container.AddToClassList(ItemUssClassName);

				return container;
			}

			protected Label CreateLabel(SerializedProperty definitionProperty)
			{
				var nameProperty = definitionProperty.FindPropertyRelative(nameof(VariableDefinition.Name));
				var label = new Label();
				label.BindProperty(nameProperty);
				label.AddToClassList(LabelUssClassName);

				return label;
			}

			protected VariableControl CreateVariable(int index, Variable variable, VariableDefinition definition)
			{
				var control = new VariableControl(variable, definition, _property.serializedObject.targetObject);
				control.AddToClassList(VariableUssClassName);
				control.RegisterCallback<ChangeEvent<Variable>>(evt => SetVariable(index, evt.newValue));

				var dataWatcher = new ChangeTriggerControl<string>(_dataProperty.GetArrayElementAtIndex(index), (oldValue, newValue) =>
				{
					control.SetValueWithoutNotify(_variables.GetVariable(index));
				});

				control.Add(dataWatcher);

				return control;
			}

			protected void WatchDefinition(VisualElement container, VariableControl control, SerializedProperty property)
			{
				var typeProperty = property.FindPropertyRelative(VariableDefinition.TypeProperty);
				var definitionDataProperty = property
					.FindPropertyRelative(VariableDefinition.ConstraintProperty)
					.FindPropertyRelative(SerializedDataItem.ContentProperty);

				var definitionDataWatcher = new ChangeTriggerControl<string>(definitionDataProperty, (oldValue, newValue) => control.Refresh());
				var typeWatcher = new ChangeTriggerControl<Enum>(typeProperty, (oldValue, newValue) => control.Refresh());

				container.Add(definitionDataWatcher);
				container.Add(typeWatcher);
			}
		}
	}
}
