using PiRhoSoft.CompositionEngine;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	public class VariableStoreControl
	{
		public static void Draw(IVariableStore store)
		{
			switch (store)
			{
				case IVariableList list: Draw(list); break;
				case VariableStore variables: Draw(variables); break;
				default: break;
			}
		}

		private static void Draw(VariableStore store)
		{
			for (var i = 0; i < store.Variables.Count; i++)
			{
				var name = store.Variables[i].Name;
				var value = store.Variables[i].Value;

				using (var changes = new EditorGUI.ChangeCheckScope())
				{
					switch (value.Type)
					{
						case VariableType.Empty: break;
						case VariableType.Boolean: DrawBoolean(name, ref value); break;
						case VariableType.Integer: DrawInteger(name, ref value); break;
						case VariableType.Number: DrawNumber(name, ref value); break;
						case VariableType.String: DrawString(name, ref value); break;
						case VariableType.Object: DrawObject(name, ref value); break;
					}

					if (changes.changed)
						store.SetVariable(name, value);
				}
			}
		}

		private static void Draw(IVariableList store)
		{
			for (var i = 0; i < store.VariableCount; i++)
			{
				var name = store.GetVariableName(i);
				var value = store.GetVariableValue(i);

				using (var changes = new EditorGUI.ChangeCheckScope())
				{
					switch (value.Type)
					{
						case VariableType.Empty: break;
						case VariableType.Boolean: DrawBoolean(name, ref value); break;
						case VariableType.Integer: DrawInteger(name, ref value); break;
						case VariableType.Number: DrawNumber(name, ref value); break;
						case VariableType.String: DrawString(name, ref value); break;
						case VariableType.Object: DrawObject(name, ref value); break;
					}

					if (changes.changed)
						store.SetVariableValue(i, value);
				}
			}
		}

		private static void DrawBoolean(string name, ref VariableValue variable)
		{
			var value = EditorGUILayout.Toggle(name, variable.Boolean);
			variable = VariableValue.Create(value);
		}

		private static void DrawInteger(string name, ref VariableValue variable)
		{
			var value = EditorGUILayout.IntField(name, variable.Integer);
			variable = VariableValue.Create(value);
		}

		private static void DrawNumber(string name, ref VariableValue variable)
		{
			var value = EditorGUILayout.FloatField(name, variable.Number);
			variable = VariableValue.Create(value);
		}

		private static void DrawString(string name, ref VariableValue variable)
		{
			var value = EditorGUILayout.TextField(name, variable.String);
			variable = VariableValue.Create(value);
		}

		private static void DrawObject(string name, ref VariableValue variable)
		{
			var value = EditorGUILayout.ObjectField(name, variable.Object, typeof(Object), true);
			variable = VariableValue.Create(value);
		}
	}
}
