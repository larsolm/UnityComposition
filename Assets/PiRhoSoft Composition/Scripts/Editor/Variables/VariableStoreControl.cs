using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	public class VariableStoreControl
	{
		// this is a resized version of UnityEditor.LookDevView
		private const string _viewIcon = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAOCAYAAAAmL5yKAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAACXBIWXMAAA7DAAAOwwHHb6hkAAAAB3RJTUUH4wEOBR4qiIqp7QAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMS41ZEdYUgAAAfZJREFUOE+1kM+LEnEYxodClBaR9uDFTsJ2qiAo8C9wQIIOXryIJ82ZFUUP0iFm8KircxD3MII3D5u0YNuhWjqsZbqiI2rjD2ocxLUudQ3KWt7edxyIoKDLPvDAy/t+nvf7zjAXosiDyGWe527yu/w9MsdxN8KR8CVz/G8h6Ein048KhXzr9LStLRaLpa7ry2bzjba3l2vi7GE0Gt0y8T+VSCTuiqLwan+/9ElV1R+apsFwOIR+vw/j8Rg6nc66WCx+FAThRTwev23GNorFYndEUXwrSYV1o3ECq9UKKpUKsCwLXq8XyuUyzGYzOD5+CZIkfUf2NWY2S0Kh0HYqlXqey+XOy2UZdH0Oo9EIPB4PWCwWw1T3ej2YTMYgyzJks9nzZDL5DLNXGXxlBxes8/k8viTD2dkS2u02OBwOwP2GqW61WjCfa8YCYjHzDa/bYdxu93W/39/PZDJ0HqjqO+O76XSr1Qo2m82oqTcYDgyGWMpQlr5i2+VyJQOBwAD/8tdarQbT6RSq1SoEg0HAM42aegePD4AYYilDWVpActvt9l2fz3fC8dz7er3+udvt/lSUHiiKAlRj7wvNkGkQS5lN9LeuoQNOp1NgWe+TUqk0OTp6+oFMNfYOaUaMyf5VFvQt9H10GM2bppp6NCPmv3XF9EWJYX4BCfkg+lRSTokAAAAASUVORK5CYII=";

		public static IconButton EditButton = new IconButton(IconButton.Edit, "View this object in the Inpsector");
		public static Base64Button ViewButton = new Base64Button(_viewIcon, "View the contents of this store");

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
