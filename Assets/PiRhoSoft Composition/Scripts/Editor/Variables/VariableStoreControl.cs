using System.Collections.Generic;
using PiRhoSoft.CompositionEngine;
using UnityEditor;

namespace PiRhoSoft.CompositionEditor
{
	public class VariableStoreControl
	{
		public static void Draw(IVariableStore store)
		{
			var names = store.GetVariableNames();
			DrawStore(store, names);
		}

		private static void DrawStore(IVariableStore store, IEnumerable<string> names)
		{
			foreach (var name in names)
			{
				var value = store.GetVariable(name);

				using (var changes = new EditorGUI.ChangeCheckScope())
				{
					switch (value.Type)
					{
						case VariableType.Empty: DrawEmpty(name, ref value); break;
						case VariableType.Boolean: DrawBoolean(name, ref value); break;
						case VariableType.Integer: DrawInteger(name, ref value); break;
						case VariableType.Number: DrawNumber(name, ref value); break;
						case VariableType.String: DrawString(name, ref value); break;
						case VariableType.Null: DrawNull(name, ref value); break;
					}

					if (value.Object != null)
						DrawObject(name, ref value);

					if (changes.changed)
						store.SetVariable(name, value);
				}

				if (value.Store != null)
				{
					using (new EditorGUI.IndentLevelScope(1))
					{
						DrawStore(name, ref value);
					}
				}
			}
		}

		private static void DrawEmptyStore(string name, IVariableStore store)
		{
			EditorGUILayout.LabelField(name, "(empty store)");
		}

		private static void DrawEmpty(string name, ref VariableValue variable)
		{
			EditorGUILayout.LabelField(name, variable.ToString());
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

		private static void DrawNull(string name, ref VariableValue variable)
		{
			EditorGUILayout.LabelField(name, variable.ToString());
		}

		private static void DrawObject(string name, ref VariableValue variable)
		{
			var value = EditorGUILayout.ObjectField(name, variable.Object, variable.Object.GetType(), true);
			variable = VariableValue.Create(value);
		}

		private static void DrawStore(string name, ref VariableValue variable)
		{
			var names = variable.Store.GetVariableNames();

			Draw(variable.Store);
		}
	}
}
