using PiRhoSoft.CompositionEngine;
using PiRhoSoft.PargonUtilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEditor
{
	public class VariablePoolElement : VisualElement
	{
		public VariablePoolElement(SerializedProperty property) : this(property.serializedObject.targetObject, PropertyHelper.GetObject<VariablePool>(property))
		{
		}

		public VariablePoolElement(Object owner, VariablePool variables)
		{
			if (owner is ISchemaOwner schemaOwner)
				schemaOwner.SetupSchema();

			Add(new ListElement(new VariablesProxy(owner, variables), "Variables", "The variables defined by this variable pool"));
		}

		private class VariablesProxy : ListProxy
		{
			public VariablePool Variables;

			private readonly Object _owner;

			public override int Count => Variables.Variables.Count;
			
			public VariablesProxy(Object owner, VariablePool variables)
			{
				_owner = owner;
				Variables = variables;
			}

			public override VisualElement CreateElement(int index)
			{
				var container = new VisualElement();

				var edit = new Image { image = Icon.Settings.Content, tooltip = "Edit the definition of this variable" };
				edit.AddManipulator(new Clickable(() => EditVariablePopup.Show(edit.worldBound, _owner, Variables, index)));

				var label = new TextField() { value = Variables.Names[index] };
				ElementHelper.Bind(label, label, _owner, () => Variables.Names[index], value => Variables.Names[index] = value);

				container.Add(edit);
				container.Add(label);
				container.Add(new VariableValueElement(_owner, () => Variables.Variables[index], value => Variables.SetVariable(index, value), () => Variables.Definitions[index]));

				return container;
			}

			public override void AddItem()
			{
				Variables.AddVariable("", VariableValue.Empty);
			}

			public override void RemoveItem(int index)
			{
				Variables.RemoveVariable(index);
			}

			public override void MoveItem(int from, int to)
			{
				Variables.VariableMoved(from, to);
			}
		}

		private class EditVariablePopup : EditorWindow
		{
			private static EditVariablePopup _instance;

			public static void Show(Rect rect, Object owner, VariablePool variables, int index)
			{
				if (_instance != null)
					_instance.Close();

				var position = GUIUtility.GUIToScreenPoint(rect.position);

				var name = new TextField("Name") { value = variables.Names[index] };
				name.RegisterValueChangedCallback(evt =>
				{
					if (!variables.Map.ContainsKey(evt.newValue))
						variables.ChangeName(index, evt.newValue);
				});

				var definition = new ValueDefinitionElement(owner, () => variables.Definitions[index], value =>
				{
					variables.ChangeDefinition(index, value);
				}, () => VariableInitializerType.None, () => null, false);

				_instance = CreateInstance<EditVariablePopup>();
				_instance.rootVisualElement.Add(name);
				_instance.rootVisualElement.Add(definition);
				_instance.ShowAsDropDown(new Rect(position, rect.size), rect.size);
			}
		}
	}
}
