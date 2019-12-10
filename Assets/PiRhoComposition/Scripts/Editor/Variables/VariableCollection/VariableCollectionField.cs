using PiRhoSoft.Utilities;
using PiRhoSoft.Utilities.Editor;
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableCollectionField : BindableElement
	{
		public const string Stylesheet = "Variables/VariableCollection/VariableCollectionStyle.uss";
		public const string UssClassName = "pirho-variable-collection";
		public const string ItemUssClassName = UssClassName + "__item";
		public const string LabelUssClassName = ItemUssClassName + "__label";
		public const string VariableUssClassName = ItemUssClassName + "__variable";
		public const string SchemaLabelUssClassName = UssClassName + "__schema-label";
		public const string RefreshUssClassName = UssClassName + "__refresh";
		public const string SettingsUssClassName = UssClassName + "__settings";
		public const string PopupUssClassName = UssClassName + "__popup";
		public const string PopupOpenUssClassName = PopupUssClassName + "--open";
		public const string PopupDefinitionUssClassName = PopupUssClassName + "__definition";
		public const string PopupCloseUssClassName = PopupUssClassName + "__close";

		private static readonly Icon _refreshIcon = Icon.BuiltIn("preAudioLoopOff");

		public VariableCollectionField(SerializedProperty property)
		{
			bindingPath = property.propertyPath;

			var dictionaryField = new DictionaryField();
			var variables = property.GetObject<VariableCollection>();
			var proxy = new VariableCollectionProxy(property, variables);

			var schemaProperty = property.FindPropertyRelative(nameof(VariableCollection.Schema));
			var schemaPicker = new ObjectPickerField(schemaProperty, typeof(VariableSchema));
			schemaPicker.RegisterCallback<ChangeEvent<Object>>(evt =>
			{
				variables.ApplySchema(evt.newValue as VariableSchema, variables);
				property.serializedObject.Update();
				dictionaryField.Control.Refresh();
			});

			var listProperty = property
				.FindPropertyRelative(VariableCollection.DataProperty)
				.FindPropertyRelative(SerializedDataList.ContentProperty);

			dictionaryField.Setup(listProperty, proxy);

			Add(schemaPicker);
			Add(dictionaryField);

			RegisterCallback<AttachToPanelEvent>(evt => proxy.AttachToPanel(evt.destinationPanel.visualTree.Q<TemplateContainer>()));
			RegisterCallback<DetachFromPanelEvent>(evt => proxy.DetachFromPanel());

			this.AddStyleSheet(Configuration.EditorPath, Stylesheet);
			AddToClassList(UssClassName);
		}

		private class VariableCollectionProxy : IDictionaryProxy
		{
			public string Label => "Variables";
			public string Tooltip => "The list of variables in this collection";
			public string EmptyLabel => "No variables exist in this collection";
			public string EmptyTooltip => "Add variables to the collection to edit them";
			public string AddPlaceholder => "(Add variable)";
			public string AddTooltip => "Add a variable to this collection";
			public string RemoveTooltip => "Remove this variable from the collection";
			public string ReorderTooltip => "Reorder this variable in the collection";

			public bool AllowAdd => true;
			public bool AllowRemove => true;
			public bool AllowReorder => false;

			public bool CanAdd(string key) => !_variables.TryGetIndex(key, out _);
			public bool CanRemove(int index) => true;
			public bool CanReorder(int from, int to) => false;
			public void ReorderItem(int from, int to) { }

			public int KeyCount => _variables.VariableNames.Count;

			private readonly VariableCollection _variables;
			private readonly SerializedProperty _property;
			private readonly SerializedProperty _dataProperty;
			private readonly SerializedProperty _definitionsProperty;
			private readonly SerializedDataList _data;
			private readonly DefinitionPopup _definitionPopup;

			public VariableCollectionProxy(SerializedProperty property, VariableCollection variables)
			{
				_variables = variables;
				_property = property;
				_data = _variables.Data;
				_dataProperty = _property
					.FindPropertyRelative(VariableCollection.DataProperty)
					.FindPropertyRelative(SerializedDataList.ContentProperty);

				_definitionsProperty = property
					.FindPropertyRelative(nameof(VariableCollection.Definitions));

				_definitionPopup = new DefinitionPopup();
			}

			public VisualElement CreateField(int index)
			{
				var variable = _variables.GetVariable(index);
				var container = CreateContainer(index);

				if (_variables.TryGetName(index, out var name))
				{
					var definition = GetDefinition(name);
					var control = CreateVariable(index, variable, definition);
					
					if (_variables.Schema != null && _variables.Schema.HasEntry(name))
					{
						var definitionProperty = new SerializedObject(_variables.Schema)
							.FindProperty(VariableSchema.EntriesField)
							.FindPropertyRelative(SerializedList<string>.ItemsProperty)
							.GetArrayElementAtIndex(index)
							.FindPropertyRelative(nameof(VariableSchemaEntry.Definition));

						var label = CreateLabel(definitionProperty);
						var entry = _variables.Schema.GetEntry(name);

						var refreshButton = new IconButton(_refreshIcon.Texture, "Recompute this variable based on the schema initializer", () =>
						{
							var newValue = entry.GenerateVariable(_variables);
							control.SetValue(newValue);
						});

						WatchDefinition(container, control, definitionProperty);

						refreshButton.AddToClassList(RefreshUssClassName);

						container.Add(label);
						container.Add(control);
						container.Add(refreshButton);
					}
					else
					{
						var definitionProperty = GetDefinitionProperty(name);
						var icon = CreateSettingsIcon(index, definitionProperty);
						var label = CreateLabel(definitionProperty);

						WatchDefinition(container, control, definitionProperty);

						container.Add(icon);
						container.Add(label);
						container.Add(control);
					}
				}

				return container;
			}

			public void AddItem(string key)
			{
				using (new ChangeScope(_property.serializedObject.targetObject))
				{
					var definition = new VariableDefinition(key);
					if (_variables.AddVariable(key, definition.Generate()) == SetVariableResult.Success)
						_variables.Definitions.Add(definition);
				}

				_property.serializedObject.Update();
				_definitionsProperty.serializedObject.Update();
			}

			public void RemoveItem(int index)
			{
				if (_variables.TryGetName(index, out var name))
				{
					using (new ChangeScope(_property.serializedObject.targetObject))
					{
						if (_variables.RemoveVariable(name) == SetVariableResult.Success)
						{
							for (var i = 0; i < _variables.Definitions.Count; i++)
							{
								if (_variables.Definitions[i].Name == name)
									_variables.Definitions.RemoveAt(i--);
							}
						}
					}

					_property.serializedObject.Update();
					_definitionsProperty.serializedObject.Update();
				}
			}

			public void AttachToPanel(TemplateContainer panel)
			{
				panel.Add(_definitionPopup);
			}

			public void DetachFromPanel()
			{
				_definitionPopup.RemoveFromHierarchy();
			}

			private VariableDefinition GetDefinition(string name)
			{
				var definition = _variables.GetDefinition(name);

				if (definition == null)
				{
					definition = new VariableDefinition(name);
					_variables.Definitions.Add(definition);
					_property.serializedObject.Update();
					_definitionsProperty.serializedObject.Update();
				}

				return definition;
			}

			private SerializedProperty GetDefinitionProperty(string name)
			{
				for (var i = 0; i < _definitionsProperty.arraySize; i++)
				{
					var property = _definitionsProperty.GetArrayElementAtIndex(i);
					if (property.FindPropertyRelative(nameof(VariableDefinition.Name)).stringValue == name)
						return property;
				}

				return null;
			}

			private IconButton CreateSettingsIcon(int index, SerializedProperty property)
			{
				var position = new VisualElement();

				var icon = new IconButton(Icon.Settings.Texture, "Edit the definition for this variable", () =>
				{
					_definitionPopup.Show(position, property);
				});

				icon.Add(position);
				icon.AddToClassList(SettingsUssClassName);

				return icon;
			}

			public bool NeedsUpdate(VisualElement item, int index)
			{
				return !(item.userData is int i) || i != index;
			}

			private VisualElement CreateContainer(int index)
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

			private void SetVariable(int index, Variable variable)
			{
				using (new ChangeScope(_property.serializedObject.targetObject))
					_variables.SetVariable(index, variable);

				_property.serializedObject.Update();
			}

			private VariableControl CreateVariable(int index, Variable variable, VariableDefinition definition)
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

			private void WatchDefinition(VisualElement container, VariableControl control, SerializedProperty property)
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

			private class DefinitionPopup : VisualElement
			{
				public DefinitionPopup()
				{
					this.AddStyleSheet(Configuration.EditorPath, Stylesheet);
					AddToClassList(PopupUssClassName);
				}

				public void Show(VisualElement position, SerializedProperty definitionProperty)
				{
					var worldPosition = position.worldBound.position;
					var local = parent.WorldToLocal(worldPosition);

					style.left = local.x;
					style.top = local.y;
					style.width = position.parent.parent.worldBound.width;

					var definitionField = new VariableDefinitionField(definitionProperty, true);
					definitionField.AddToClassList(PopupDefinitionUssClassName);

					var close = new IconButton(Icon.Close.Texture, "Stop editing this definition", Hide);
					close.AddToClassList(PopupCloseUssClassName);

					Add(definitionField);
					Add(close);
					AddToClassList(PopupOpenUssClassName);
				}

				public void Hide()
				{
					Clear();
					RemoveFromClassList(PopupOpenUssClassName);
				}
			}
		}
	}
}
