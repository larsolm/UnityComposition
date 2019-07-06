using System.Reflection;
using PiRhoSoft.CompositionEngine;
using PiRhoSoft.PargonInspector.Editor;
using PiRhoSoft.PargonUtilities.Editor;
using PiRhoSoft.PargonUtilities.Engine;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.CompositionEditor
{
	public class VariableReferenceControl : ObjectControl<VariableReference>
	{
		private VariableReference _variableReference;

		public static float GetHeight()
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public static void Draw(GUIContent label, VariableReference reference)
		{
			var rect = EditorGUILayout.GetControlRect(false);
			Draw(rect, reference, label);
		}

		public static void Draw(Rect position, VariableReference reference, GUIContent label)
		{
			using (new InvalidScope(reference.IsValid))
			{
				using (var changes = new EditorGUI.ChangeCheckScope())
				{
					var variable = EditorGUI.TextField(position, label, reference.Variable);

					if (changes.changed)
						reference.Variable = variable;
				}
			}
		}

		public override void Setup(VariableReference target, SerializedProperty property, FieldInfo fieldInfo, PropertyAttribute attribute)
		{
			_variableReference = target;
		}

		public override float GetHeight(GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public override void Draw(Rect position, GUIContent label)
		{
			Draw(position, _variableReference, label);
		}
	}

	public class VariableReferenceElement : TextField, IAutocompleteElement
	{
		private static readonly KeyCode[] _selectKeys = new KeyCode[] { KeyCode.Tab, KeyCode.Space, KeyCode.KeypadEnter, KeyCode.Return, KeyCode.Period, KeyCode.KeypadPeriod, KeyCode.LeftBracket, KeyCode.RightBracket };

		private Object _owner;
		private VariableReference _reference;
		private AutocompleteSource _source;
		private AutocompletePopup _popup;

		public void Setup(SerializedProperty property, AutocompleteSource source)
		{
			var reference = PargonUtilities.Editor.PropertyHelper.GetObject<VariableReference>(property);
			Setup(property.serializedObject.targetObject, reference, source);
		}

		public void Setup(Object owner, VariableReference reference, AutocompleteSource source)
		{
			_owner = owner;
			_reference = reference;
			_source = source;
			_popup = new AutocompletePopup(this, this, source);
			_popup.SelectKeys = _selectKeys;

			EditHelper.Bind(this, _owner, () => text, () => _reference.Variable, value => SetValueWithoutNotify(value), value => _reference.Variable = value);

			style.flexGrow = 1;
		}

		#region IAutocompleteElement Implementation

		public string GetAutocompleteFilter(TextField input)
		{
			return text; // TODO: should be text of current token
		}

		public Vector2 GetAutocompleteLocation(TextField input)
		{
			return worldBound.position + new Vector2(0 /* TODO: should be position of token start */, resolvedStyle.unityFont.lineHeight);
		}

		public void ApplyAutocomplete(TextField input, KeyCode selector, AutocompleteItem item)
		{
			var text = item.Name;

			switch (selector)
			{
				case KeyCode.Space: text += ' '; break;
				case KeyCode.Period: text += '.'; break;
				case KeyCode.KeypadPeriod: text += '.'; break;
				case KeyCode.LeftBracket: text += '['; break;
				case KeyCode.RightBracket: text += ']'; break;
			}

			this.text = text; // TODO: should replace current token
		}

		#endregion
	}

	[CustomPropertyDrawer(typeof(VariableReference))]
	public class VariableReferenceDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var source = property.serializedObject.targetObject as IAutocompleteSource;
			var container = ElementHelper.CreatePropertyContainer(property.displayName);
			var element = new VariableReferenceElement();

			element.Setup(property, source.AutocompleteSource);
			container.Add(element);

			return container;
		}
	}
}
