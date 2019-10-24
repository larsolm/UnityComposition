using PiRhoSoft.Utilities;
using PiRhoSoft.Utilities.Editor;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class CustomVariableCollectionField : SerializedVariableDictionaryField
	{
		public new const string Stylesheet = "Variables/VariableDictionary/CustomVariableCollectionStyle.uss";
		public new const string UssClassName = "pirho-custom-variable-collection";
		public const string SettingsUssClassName = UssClassName + "__settings";
		public const string PopupUssClassName = UssClassName + "__popup";
		public const string PopupOpenUssClassName = PopupUssClassName + "--open";
		public const string PopupDefinitionUssClassName = PopupUssClassName + "__definition";
		public const string PopupCloseUssClassName = PopupUssClassName + "__close";

		public CustomVariableCollectionField(SerializedProperty property)
		{
			var variables = property.GetObject<CustomVariableCollection>();
			var proxy = new CustomVariableCollectionProxy(property, variables);

			Setup(property, proxy);

			RegisterCallback<AttachToPanelEvent>(evt => proxy.AttachToPanel(evt.destinationPanel.visualTree.Q<TemplateContainer>()));
			RegisterCallback<DetachFromPanelEvent>(evt => proxy.DetachFromPanel());

			this.AddStyleSheet(Configuration.EditorPath, Stylesheet);
			AddToClassList(UssClassName);
		}

		private class CustomVariableCollectionProxy : VariableDictionaryProxy
		{
			public override string Label => "Custom Variables";
			public override string Tooltip => "The list of variables in this collection";
			public override string EmptyLabel => "No variables exist in this collection";
			public override string EmptyTooltip => "Add variables to the collection to edit them";
			public override string AddPlaceholder => "(Add variable)";
			public override string AddTooltip => "Add a variable to this collection";
			public override string RemoveTooltip => "Remove this variable from the collection";
			public override string ReorderTooltip => "Reorder this variable in the collection";

			public override bool AllowAdd => true;
			public override bool AllowRemove => true;

			public override bool CanAdd(string key) => !_variables.VariableNames.Contains(key);
			public override bool CanRemove(int index) => true;

			private CustomVariableCollection _collection => _variables as CustomVariableCollection;

			private readonly SerializedProperty _definitionsProperty;
			private readonly DefinitionPopup _definitionPopup;

			public CustomVariableCollectionProxy(SerializedProperty property, CustomVariableCollection variables) : base(property, variables)
			{
				_definitionPopup = new DefinitionPopup();
				_definitionsProperty = property
					.FindPropertyRelative(CustomVariableCollection.DefinitionsProperty)
					.FindPropertyRelative(SerializedList<string>.ItemsProperty);
			}

			public override VisualElement CreateField(int index)
			{
				var variable = _variables.GetVariable(index);
				var definition = _collection.GetDefinition(index);
				var definitionProperty = _definitionsProperty.GetArrayElementAtIndex(index);
				var container = CreateContainer(index);
				var icon = CreateSettingsIcon(index, definitionProperty);
				var label = CreateLabel(definitionProperty);
				var control = CreateVariable(index, variable, definition);

				container.Add(icon);
				container.Add(label);
				container.Add(control);

				WatchDefinition(container, control, definitionProperty);

				return container;
			}

			public override void AddItem(string key)
			{
				using (new ChangeScope(_property.serializedObject.targetObject))
					_collection.Add(new VariableDefinition(key));

				_property.serializedObject.Update();
				_definitionsProperty.serializedObject.Update();
			}

			public override void RemoveItem(int index)
			{
				var name = _variables.VariableNames[index];

				using (new ChangeScope(_property.serializedObject.targetObject))
					_variables.RemoveVariable(name);

				_property.serializedObject.Update();
				_definitionsProperty.serializedObject.Update();
			}

			public void AttachToPanel(TemplateContainer panel)
			{
				panel.Add(_definitionPopup);
			}

			public void DetachFromPanel()
			{
				_definitionPopup.RemoveFromHierarchy();
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

					var definitionField = new VariableDefinitionField(definitionProperty, false);
					definitionField.AddToClassList(PopupDefinitionUssClassName);

					var close = new IconButton(Icon.Close.Texture, "Close this definition window", Hide);
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