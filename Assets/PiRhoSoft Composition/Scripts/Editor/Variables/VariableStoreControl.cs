using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	public class VariableStoreControl : ObjectListControl
	{
		// this is a resized version of UnityEditor.LookDevView
		private const string _viewIcon = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAOCAYAAAAmL5yKAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAACXBIWXMAAA7DAAAOwwHHb6hkAAAAB3RJTUUH4wEOBR4qiIqp7QAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMS41ZEdYUgAAAfZJREFUOE+1kM+LEnEYxodClBaR9uDFTsJ2qiAo8C9wQIIOXryIJ82ZFUUP0iFm8KircxD3MII3D5u0YNuhWjqsZbqiI2rjD2ocxLUudQ3KWt7edxyIoKDLPvDAy/t+nvf7zjAXosiDyGWe527yu/w9MsdxN8KR8CVz/G8h6Ein048KhXzr9LStLRaLpa7ry2bzjba3l2vi7GE0Gt0y8T+VSCTuiqLwan+/9ElV1R+apsFwOIR+vw/j8Rg6nc66WCx+FAThRTwev23GNorFYndEUXwrSYV1o3ECq9UKKpUKsCwLXq8XyuUyzGYzOD5+CZIkfUf2NWY2S0Kh0HYqlXqey+XOy2UZdH0Oo9EIPB4PWCwWw1T3ej2YTMYgyzJks9nzZDL5DLNXGXxlBxes8/k8viTD2dkS2u02OBwOwP2GqW61WjCfa8YCYjHzDa/bYdxu93W/39/PZDJ0HqjqO+O76XSr1Qo2m82oqTcYDgyGWMpQlr5i2+VyJQOBwAD/8tdarQbT6RSq1SoEg0HAM42aegePD4AYYilDWVpActvt9l2fz3fC8dz7er3+udvt/lSUHiiKAlRj7wvNkGkQS5lN9LeuoQNOp1NgWe+TUqk0OTp6+oFMNfYOaUaMyf5VFvQt9H10GM2bppp6NCPmv3XF9EWJYX4BCfkg+lRSTokAAAAASUVORK5CYII=";

		public static IconButton EditButton = new IconButton(IconButton.Edit, "View this object in the Inpsector");
		public static Base64Button ViewButton = new Base64Button(_viewIcon, "View the contents of this store");

		public string Name { get; private set; }
		public IVariableStore Store { get; private set; }

		private GUIContent _label;
		private List<string> _names;

		public IVariableStore Selected { get; private set; }
		public string SelectedName { get; private set; }

		public VariableStoreControl Setup(string name, IVariableStore store)
		{
			Name = name;
			Store = store;
			Selected = null;
			SelectedName = string.Empty;

			_label = new GUIContent(name);
			_names = store.GetVariableNames().ToList();

			Setup(_names)
				.MakeEmptyLabel(new GUIContent("The store is empty"))
				.MakeCollapsable(nameof(VariableStore) + "." + Name + ".IsOpen");

			if (store is Object obj)
				MakeHeaderButton(EditButton, rect => Selection.activeObject = obj, Color.white);

			return this;
		}

		public void Draw()
		{
			Draw(_label);
		}

		public void Draw(Rect rect)
		{
			Draw(rect, _label);
		}

		protected override void Draw(Rect rect, int index)
		{
			Selected = null;

			var name = _names[index];
			var value = Store.GetVariable(name);

			using (var changes = new EditorGUI.ChangeCheckScope())
			{
				switch (value.Type)
				{
					case VariableType.Empty: DrawEmpty(rect, name, ref value); break;
					case VariableType.Boolean: DrawBoolean(rect, name, ref value); break;
					case VariableType.Integer: DrawInteger(rect, name, ref value); break;
					case VariableType.Number: DrawNumber(rect, name, ref value); break;
					case VariableType.String: DrawString(rect, name, ref value); break;
					case VariableType.Raw: DrawRaw(rect, name, ref value); break;
					case VariableType.Null: DrawNull(rect, name, ref value); break;
					default:
					{
						if (DrawStore(rect, name, value.RawObject)) // TODO: show object picker
						{
							Selected = value.Store;
							SelectedName = name;
						}

						break;
					}
				}

				if (changes.changed)
					Store.SetVariable(name, value);
			}
		}

		private void DrawEmpty(Rect rect, string name, ref VariableValue variable)
		{
			EditorGUI.LabelField(rect, name, variable.ToString());
		}

		private void DrawBoolean(Rect rect, string name, ref VariableValue variable)
		{
			var value = EditorGUI.Toggle(rect, name, variable.Boolean);
			variable = VariableValue.Create(value);
		}

		private void DrawInteger(Rect rect, string name, ref VariableValue variable)
		{
			var value = EditorGUI.IntField(rect, name, variable.Integer);
			variable = VariableValue.Create(value);
		}

		private void DrawNumber(Rect rect, string name, ref VariableValue variable)
		{
			var value = EditorGUI.FloatField(rect, name, variable.Number);
			variable = VariableValue.Create(value);
		}

		private void DrawString(Rect rect, string name, ref VariableValue variable)
		{
			var value = EditorGUI.TextField(rect, name, variable.String);
			variable = VariableValue.Create(value);
		}

		private void DrawNull(Rect rect, string name, ref VariableValue variable)
		{
			EditorGUI.LabelField(rect, name, variable.ToString());
		}

		private void DrawObject(Rect rect, string name, ref VariableValue variable)
		{
			var value = EditorGUI.ObjectField(rect, name, variable.Object, variable.Object.GetType(), true);
			variable = VariableValue.Create(value);
		}

		private bool DrawStore(Rect rect, string name, object obj)
		{
			var selected = false;
			var unity = obj as Object;
			var store = obj as IVariableStore;

			var viewRect = store != null ? RectHelper.TakeTrailingIcon(ref rect) : rect;
			var editRect = unity != null ? RectHelper.TakeTrailingIcon(ref rect) : rect;

			GUI.Label(rect, name);

			if (unity != null)
			{
				if (GUI.Button(editRect, EditButton.Content, GUIStyle.none))
					Selection.activeObject = unity;
			}

			if (store != null)
			{
				if (GUI.Button(viewRect, ViewButton.Content, GUIStyle.none))
					selected = true;
			}

			return selected;
		}

		private void DrawRaw(Rect rect, string name, ref VariableValue variable)
		{
			EditorGUI.LabelField(rect, name, variable.ToString());
		}

		public static bool DrawLink(string name, object obj)
		{
			var selected = false;

			using (new GUILayout.HorizontalScope())
			{
				GUILayout.Label(name);
				GUILayout.FlexibleSpace();

				if (obj is Object unityObject)
				{
					if (GUILayout.Button(EditButton.Content, GUIStyle.none))
						Selection.activeObject = unityObject;
				}

				if (obj is IVariableStore store)
				{
					if (GUILayout.Button(ViewButton.Content, GUIStyle.none))
						selected = true;
				}
			}

			return selected;
		}

		public static IVariableStore DrawTable(IVariableStore store, bool allowAddRemove)
		{
			var names = store.GetVariableNames();
			IVariableStore selectedStore = null;

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
						default:
						{
							if (DrawLink(name, value.RawObject)) // TODO: show object picker
								selectedStore = value.Store;

							break;
						}
					}

					if (changes.changed)
						store.SetVariable(name, value);
				}
			}

			return selectedStore;
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
	}
}
