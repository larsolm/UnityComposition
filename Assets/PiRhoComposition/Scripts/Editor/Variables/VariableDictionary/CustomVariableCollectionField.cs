using PiRhoSoft.Utilities;
using PiRhoSoft.Utilities.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class CustomVariableCollectionField : SerializedVariableDictionaryField
	{
		public new const string Stylesheet = "Variables/VariableDictionary/CustomVariableCollectionStyle.uss";
		public new const string UssClassName = "pirho-custom-variable-collection";

		public CustomVariableCollectionField(SerializedProperty property)
		{
			var variables = property.GetObject<CustomVariableCollection>();
			var proxy = new CustomVariableCollectionProxy(property, variables);

			Setup(property, proxy);

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

			public CustomVariableCollectionProxy(SerializedProperty property, CustomVariableCollection variables) : base(property, variables)
			{
			}

			public override VisualElement CreateField(int index)
			{
				var variable = _variables.GetVariable(index);
				var definition = _collection.GetDefinition(index);

				var container = CreateContainer(index);
				var label = CreateLabel(definition.Name);
				var control = CreateVariable(index, variable, definition);

				container.Add(label);
				container.Add(control);

				return container;
			}

			public override void AddItem(string key)
			{
				_collection.Add(new VariableDefinition(key));
				UpdateValue();
			}

			public override void RemoveItem(int index)
			{
				var name = _variables.VariableNames[index];
				_variables.RemoveVariable(name);
				UpdateValue();
			}
		}
	}
}