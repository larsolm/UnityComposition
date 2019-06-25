using PiRhoSoft.CompositionEngine;
using PiRhoSoft.PargonUtilities.Editor;
using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEditor
{
	public class VariableSetElement : VisualElement, IBindableProperty<VariableSet>, IBindableObject<VariableSet>
	{
		private VariablesProxy _proxy;

		private readonly Object _owner;
		private readonly Func<VariableSet> _getValue;
		private readonly Action<VariableSet> _setValue;

		public VariableSetElement(SerializedProperty property)
		{
			_owner = property.serializedObject.targetObject;

			ElementHelper.Bind(this,this, property);
		}

		public VariableSetElement(Object owner, Func<VariableSet> getValue, Action<VariableSet> setValue)
		{
			_owner = owner;
			_getValue = getValue;
			_setValue = setValue;

			ElementHelper.Bind(this, this, owner);
		}

		#region IBindable Implementation

		public VariableSet GetValueFromElement(VisualElement element)
		{
			return _proxy.Variables;
		}

		public VariableSet GetValueFromProperty(SerializedProperty property)
		{
			return PropertyHelper.GetObject<VariableSet>(property);
		}

		public VariableSet GetValueFromObject(Object owner)
		{
			return _getValue();
		}

		public void UpdateElement(VariableSet value, VisualElement element, SerializedProperty property)
		{
			Setup(value);
		}

		public void UpdateElement(VariableSet value, VisualElement element, Object owner)
		{
			Setup(value);
		}

		public void UpdateProperty(VariableSet value, VisualElement element, SerializedProperty property)
		{
		}

		public void UpdateObject(VariableSet value, VisualElement element, Object owner)
		{
			_setValue(value);
		}

		#endregion

		public void Setup(VariableSet variables)
		{
			_proxy = new VariablesProxy(_owner, variables);

			if (_owner is ISchemaOwner owner)
				owner.SetupSchema();

			if (variables.Owner != null && variables.NeedsUpdate)
			{
				using (new ChangeScope(_owner))
					variables.Update();
			}
		}

		private class VariablesProxy : ListProxy
		{
			public VariableSet Variables;

			private readonly Object _owner;

			public override int Count => Variables.VariableCount;

			public VariablesProxy(Object owner, VariableSet variables)
			{
				_owner = owner;
				Variables = variables;
			}

			public override VisualElement CreateElement(int index)
			{
				var container = new VisualElement();

				var name = Variables.GetVariableName(index);
				var definition = Variables.Schema != null && index < Variables.Schema.Count ? Variables.Schema[index].Definition : ValueDefinition.Create(VariableType.Empty);
				
				if (Variables.Owner != null)
				{
					container.Add(new VariableValueElement(_owner, name, () => Variables.GetVariableValue(index), value => Variables.SetVariableValue(index, value), () =>
					{
						return Variables.Schema != null && index < Variables.Schema.Count ? Variables.Schema[index].Definition : ValueDefinition.Create(VariableType.Empty);
					}));
					
					if (Variables.Schema != null&& Variables.Owner != null)
					{
						var refreshButton = new Image { image = Icon.Refresh.Content, tooltip = "Re-compute this variable based on the schema initializer" };
						refreshButton.AddManipulator(new Clickable(() =>
						{
							var value = Variables.Schema[index].Definition.Generate(Variables.Owner);
							Variables.SetVariableValue(index, value);
						}));
					}
				}
				
				return container;
			}

			public override void AddItem() { }
			public override void RemoveItem(int index) { }
			public override void MoveItem(int from, int to) { }
		}
	}
}
