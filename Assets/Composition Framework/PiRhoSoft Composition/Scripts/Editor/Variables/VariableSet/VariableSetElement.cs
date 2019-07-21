using PiRhoSoft.CompositionEngine;
using PiRhoSoft.PargonUtilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.CompositionEditor
{
	public class VariableSetElement : VisualElement
	{
		public VariableSetElement(SerializedProperty property) : this(property.serializedObject.targetObject, PropertyHelper.GetObject<VariableSet>(property))
		{
		}

		public VariableSetElement(Object owner, VariableSet variables)
		{
			if (owner is ISchemaOwner schemaOwner)
				schemaOwner.SetupSchema();

			if (variables.Owner != null && variables.NeedsUpdate)
			{
				using (new ChangeScope(owner))
					variables.Update();
			}

			//Add(new ListElement(new VariablesProxy(owner, variables), "Variables", "The list of variables defined by this variable set"));
		}

		private class VariablesProxy : ListProxy
		{
			public VariableSet Variables;

			private readonly Object _owner;

			public override int ItemCount => Variables.VariableCount;

			public VariablesProxy(Object owner, VariableSet variables)
			{
				_owner = owner;
				Variables = variables;
			}

			public override VisualElement CreateField(int index)
			{
				var container = new VisualElement();

				var name = Variables.GetVariableName(index);
				var definition = Variables.Schema != null && index < Variables.Schema.Count ? Variables.Schema[index].Definition : ValueDefinition.Create(VariableType.Empty);
				
				if (Variables.Owner != null)
				{
					container.Add(new Label(name));

					container.Add(new VariableValueElement(_owner, () => Variables.GetVariableValue(index), value => Variables.SetVariableValue(index, value), () =>
					{
						return Variables.Schema != null && index < Variables.Schema.Count ? Variables.Schema[index].Definition : ValueDefinition.Create(VariableType.Empty);
					}));
					
					if (Variables.Schema != null&& Variables.Owner != null)
					{
						container.Add(ElementHelper.CreateIconButton(Icon.Refresh.Content, "Re-compute this variable based on the schema initializer", () =>
						{
							var value = Variables.Schema[index].Definition.Generate(Variables.Owner);
							Variables.SetVariableValue(index, value);
						}));
					}
				}
				
				return container;
			}

			public override bool NeedsUpdate(VisualElement item, int index)
			{
				return true;
			}

			public override void AddItem() { }
			public override void RemoveItem(int index) { }
			public override void ReorderItem(int from, int to) { }
		}
	}
}
