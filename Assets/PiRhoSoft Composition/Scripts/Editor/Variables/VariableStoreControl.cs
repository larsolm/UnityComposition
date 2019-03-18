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

			if (store is IIndexedVariableStore indexed)
				_names = store.GetVariableNames().Concat(Enumerable.Repeat(string.Empty, indexed.Count).Select((value, index) => VariableReference.LookupOpen + index.ToString() + VariableReference.LookupClose)).ToList();
			else
				_names = store.GetVariableNames().ToList();

			Setup(_names)
				.MakeEmptyLabel(new GUIContent("The store is empty"))
				.MakeCollapsable(nameof(VariableStore) + "." + Name + ".IsOpen")
				.MakeCustomHeight(GetHeight);

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

		private float GetHeight(int index)
		{
			var name = _names[index];
			var value = Store.GetVariable(name);
			var definition = VariableDefinition.Create(string.Empty, VariableType.Empty);

			return VariableValueDrawer.GetHeight(value, definition);
		}

		protected override void Draw(Rect rect, int index)
		{
			Selected = null;

			var name = _names[index];
			var definition = VariableDefinition.Create(string.Empty, VariableType.Empty);
			var value = VariableValue.Empty;

			if (name.Length > 0 && name[0] == VariableReference.LookupOpen && Store is IIndexedVariableStore indexed)
			{
				var i = int.Parse(name.Substring(1, name.Length - 2));
				value = VariableValue.Create(indexed.GetItem(i));
			}
			else
			{
				value = Store.GetVariable(name);
			}

			if (value.Type == VariableType.Store)
			{
				if (DrawStore(rect, name, value.Store)) // TODO: show object picker
				{
					Selected = value.Store;
					SelectedName = name;
				}
			}
			else
			{
				using (var changes = new EditorGUI.ChangeCheckScope())
				{
					value = VariableValueDrawer.Draw(new GUIContent(name), value, definition);

					if (changes.changed)
						Store.SetVariable(name, value);
				}
			}
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
	}
}
