using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	[CustomEditor(typeof(CallMethodNode))]
	class CallMethodNodeEditor : Editor
	{
		private static readonly Label _nextContent = new Label(typeof(CallMethodNode), nameof(CallMethodNode.Next));
		private static readonly Label _outputContent = new Label(typeof(CallMethodNode), nameof(CallMethodNode.Output));
		private static readonly Label _parametersContent = new Label(typeof(CallMethodNode), nameof(CallMethodNode.Parameters));

		private static readonly GUIContent _targetTypeContent = new GUIContent("Object Type", "The Type of the object to call the method on");
		private static readonly GUIContent _methodContent = new GUIContent("Method", "The method to call on the target object");

		private CallMethodNode _node;
		private MethodInfo[] _methods;
		private string[] _methodNames;
		private string[] _parameterNames;
		private SerializedProperty _targetProperty;

		private PropertyListControl _parametersControl;

		void OnEnable()
		{
			_node = target as CallMethodNode;
			_targetProperty = serializedObject.FindProperty(nameof(CallMethodNode.Target));

			BuildMethodList();
			SetupParameters();
		}

		private void BuildMethodList()
		{
			if (_node.TargetType != null)
			{
				_methods = _node.TargetType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(method => !method.IsSpecialName)
					.Where(method => method.ReturnType == typeof(void) || VariableValue.GetType(method.ReturnType) != VariableType.Empty)
					.Where(method => !method.GetParameters().Any(parameter => VariableValue.GetType(parameter.ParameterType) == VariableType.Empty))
					.Where(method => method.GetCustomAttribute<ObsoleteAttribute>() == null).ToArray();

				_methodNames = _methods.Select(method => method.Name).ToArray();
			}
			else
			{
				_methods = null;
				_methodNames = null;
			}
		}

		private void SetupParameters()
		{
			if (_node.Method != null)
			{
				var parameters = _node.Method.GetParameters();
				_parameterNames = parameters.Select(parameter => parameter.Name).ToArray();

				_parametersControl = new PropertyListControl();
				_parametersControl.Setup(serializedObject.FindProperty(nameof(CallMethodNode.Parameters)))
					.MakeDrawable(DrawParameter)
					.MakeCustomHeight((index) => 2 * RectHelper.LineHeight);
			}
		}

		public override void OnInspectorGUI()
		{
			using (new UndoScope(_node, false))
				InstructionGraphNodeDrawer.Draw(_nextContent.Content, _node.Next);

			using (new UndoScope(serializedObject))
				EditorGUILayout.PropertyField(_targetProperty);

			using (new UndoScope(_node, false))
			{
				var selectedTargetType = TypePopupDrawer.Draw<Component>(_targetTypeContent, _node.TargetType, false);
				if (selectedTargetType != _node.TargetType)
					SetTargetType(selectedTargetType);

				if (_node.TargetType != null)
				{
					var method = Array.IndexOf(_methodNames, _node.MethodName);
					var selectedMethod = EditorGUILayout.Popup(_methodContent, method, _methodNames);

					if (selectedMethod != method)
						SetMethod(_methods[selectedMethod]);

					if (_node.Method != null)
					{
						if (_node.Method.ReturnType != typeof(void))
							VariableReferenceControl.Draw(_outputContent.Content, _node.Output);
					}
				}
			}

			if (_node.TargetType != null && _node.Method != null && _node.Parameters.Count > 0)
			{
				using (new UndoScope(serializedObject))
					_parametersControl.Draw(_parametersContent.Content);
			}
		}

		private void SetTargetType(Type type)
		{
			_node.TargetType = type;
			_node.Method = null;
			_node.MethodName = null;
			_node.ParameterTypes = null;
			_node.Parameters.Clear();

			BuildMethodList();
		}

		private void SetMethod(MethodInfo method)
		{
			_node.Method = method;
			_node.MethodName = method?.Name;

			var parameters = _node.Method.GetParameters();

			_node.ParameterTypes = parameters.Select(parameter => parameter.ParameterType).ToArray();
			_node.Parameters.Clear();

			foreach (var parameter in parameters)
			{
				var variableType = VariableValue.GetType(parameter.ParameterType);
				var definition = variableType == VariableType.Object ? VariableDefinition.Create(string.Empty, parameter.ParameterType) : VariableDefinition.Create(string.Empty, variableType);

				_node.Parameters.Add(new VariableValueSource(variableType, definition));
			}

			SetupParameters();
		}

		private void DrawParameter(Rect rect, SerializedProperty property, int index)
		{
			var element = property.GetArrayElementAtIndex(index);
			EditorGUI.PropertyField(rect, element, new GUIContent(_parameterNames[index]));
		}
	}
}
