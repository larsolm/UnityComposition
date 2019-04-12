using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using PiRhoSoft.UtilityEngine;
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEditor
{
	public class VariableValueDrawer
	{
		private static readonly Label _addStoreButton = new Label(Icon.BuiltIn(Icon.CustomAdd), "", "Add an item to the store");
		private static readonly Label _addListButton = new Label(Icon.BuiltIn(Icon.Add), "", "Add an item to the list");
		private static readonly Label _removeStoreButton = new Label(Icon.BuiltIn(Icon.Remove), "", "Remove this item from the store");
		private static readonly Label _removeListButton = new Label(Icon.BuiltIn(Icon.Remove), "", "Remove this item from the list");
		private const float _storeLabelWidth = 100.0f;

		public static float GetHeight(VariableValue value, ValueDefinition definition, bool drawStores)
		{
			var type = definition.Type != VariableType.Empty ? definition.Type : value.Type;

			switch (type)
			{
				case VariableType.Empty: return GetEmptyHeight();
				case VariableType.Bool: return GetBoolHeight();
				case VariableType.Int: return GetIntHeight();
				case VariableType.Float: return GetFloatHeight();
				case VariableType.Int2: return GetInt2Height();
				case VariableType.Int3: return GetInt3Height();
				case VariableType.IntRect: return GetIntRectHeight();
				case VariableType.IntBounds: return GetIntBoundsHeight();
				case VariableType.Vector2: return GetVector2Height();
				case VariableType.Vector3: return GetVector3Height();
				case VariableType.Vector4: return GetVector4Height();
				case VariableType.Quaternion: return GetQuaternionHeight();
				case VariableType.Rect: return GetRectHeight();
				case VariableType.Bounds: return GetBoundsHeight();
				case VariableType.Color: return GetColorHeight();
				case VariableType.String: return GetStringHeight();
				case VariableType.Enum: return GetEnumHeight();
				case VariableType.Object: return GetObjectHeight();
				case VariableType.Store: return GetStoreHeight(value, definition.Constraint as StoreVariableConstraint, drawStores);
				case VariableType.List: return GetListHeight(value, definition.Constraint as ListVariableConstraint, drawStores);
				default: return EditorGUIUtility.singleLineHeight;
			}
		}

		public static VariableValue Draw(GUIContent label, VariableValue value, ValueDefinition definition, bool drawStores)
		{
			var height = GetHeight(value, definition, drawStores);
			var rect = EditorGUILayout.GetControlRect(true, height);
			return Draw(rect, label, value, definition, drawStores);
		}

		public static VariableValue Draw(Rect position, GUIContent label, VariableValue value, ValueDefinition definition, bool drawStores)
		{
			position = EditorGUI.PrefixLabel(position, label);

			switch (value.Type)
			{
				case VariableType.Empty: return DrawEmpty(position);
				case VariableType.Bool: return DrawBool(position, value);
				case VariableType.Int: return DrawInt(position, value, definition.Constraint as IntVariableConstraint);
				case VariableType.Float: return DrawFloat(position, value, definition.Constraint as FloatVariableConstraint);
				case VariableType.Int2: return DrawInt2(position, value);
				case VariableType.Int3: return DrawInt3(position, value);
				case VariableType.IntRect: return DrawIntRect(position, value);
				case VariableType.IntBounds: return DrawIntBounds(position, value);
				case VariableType.Vector2: return DrawVector2(position, value);
				case VariableType.Vector3: return DrawVector3(position, value);
				case VariableType.Vector4: return DrawVector4(position, value);
				case VariableType.Quaternion: return DrawQuaternion(position, value);
				case VariableType.Rect: return DrawRect(position, value);
				case VariableType.Bounds: return DrawBounds(position, value);
				case VariableType.Color: return DrawColor(position, value);
				case VariableType.String: return DrawString(position, value, definition.Constraint as StringVariableConstraint);
				case VariableType.Enum: return DrawEnum(position, value, definition.Constraint as EnumVariableConstraint);
				case VariableType.Object: return DrawObject(position, value, definition.Constraint as ObjectVariableConstraint);
				case VariableType.Store: return DrawStore(position, value, definition.Constraint as StoreVariableConstraint, drawStores);
				case VariableType.List: return DrawList(position, value, definition.Constraint as ListVariableConstraint, drawStores);
				default: return value;
			}
		}

		#region Empty

		public static float GetEmptyHeight()
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public static VariableValue DrawEmpty(Rect position)
		{
			var type = (VariableType)EditorGUI.EnumPopup(position, VariableType.Empty);
			return VariableHandler.CreateDefault(type, null);
		}

		#endregion

		#region Bool

		public static float GetBoolHeight()
		{
			return EditorGUI.GetPropertyHeight(SerializedPropertyType.Boolean, GUIContent.none);
		}

		private static VariableValue DrawBool(Rect rect, VariableValue value)
		{
			var boolean = EditorGUI.ToggleLeft(rect, GUIContent.none, value.Bool);
			return VariableValue.Create(boolean);
		}

		#endregion

		#region Int

		public static float GetIntHeight()
		{
			return EditorGUI.GetPropertyHeight(SerializedPropertyType.Integer, GUIContent.none);
		}

		private static VariableValue DrawInt(Rect rect, VariableValue value, IntVariableConstraint constraint)
		{
			var integer = value.Int;

			if (constraint != null)
				integer = EditorGUI.IntSlider(rect, GUIContent.none, integer, constraint.Minimum, constraint.Maximum);
			else
				integer = EditorGUI.IntField(rect, GUIContent.none, integer);

			return VariableValue.Create(integer);
		}

		#endregion

		#region Float

		public static float GetFloatHeight()
		{
			return EditorGUI.GetPropertyHeight(SerializedPropertyType.Float, GUIContent.none);
		}

		private static VariableValue DrawFloat(Rect rect, VariableValue value, FloatVariableConstraint constraint)
		{
			var number = value.Float;

			if (constraint != null)
				number = EditorGUI.Slider(rect, GUIContent.none, number, constraint.Minimum, constraint.Maximum);
			else
				number = EditorGUI.FloatField(rect, GUIContent.none, number);

			return VariableValue.Create(number);
		}

		#endregion

		#region Int2

		public static float GetInt2Height()
		{
			return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector2Int, GUIContent.none);
		}

		private static VariableValue DrawInt2(Rect rect, VariableValue value)
		{
			var result = EditorGUI.Vector2IntField(rect, GUIContent.none, value.Int2);
			return VariableValue.Create(result);
		}

		#endregion

		#region Int3

		public static float GetInt3Height()
		{
			return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3Int, GUIContent.none);
		}

		private static VariableValue DrawInt3(Rect rect, VariableValue value)
		{
			var result = EditorGUI.Vector3IntField(rect, GUIContent.none, value.Int3);
			return VariableValue.Create(result);
		}

		#endregion

		#region IntRect

		public static float GetIntRectHeight()
		{
			return EditorGUI.GetPropertyHeight(SerializedPropertyType.RectInt, GUIContent.none);
		}

		private static VariableValue DrawIntRect(Rect rect, VariableValue value)
		{
			var result = EditorGUI.RectIntField(rect, GUIContent.none, value.IntRect);
			return VariableValue.Create(result);
		}

		#endregion

		#region IntBounds

		public static float GetIntBoundsHeight()
		{
			return EditorGUI.GetPropertyHeight(SerializedPropertyType.BoundsInt, GUIContent.none);
		}

		private static VariableValue DrawIntBounds(Rect rect, VariableValue value)
		{
			var result = EditorGUI.BoundsIntField(rect, GUIContent.none, value.IntBounds);
			return VariableValue.Create(result);
		}

		#endregion

		#region Vector2

		public static float GetVector2Height()
		{
			return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector2, GUIContent.none);
		}

		private static VariableValue DrawVector2(Rect rect, VariableValue value)
		{
			var result = EditorGUI.Vector2Field(rect, GUIContent.none, value.Vector2);
			return VariableValue.Create(result);
		}

		#endregion

		#region Vector3

		public static float GetVector3Height()
		{
			return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3, GUIContent.none);
		}

		private static VariableValue DrawVector3(Rect rect, VariableValue value)
		{
			var result = EditorGUI.Vector3Field(rect, GUIContent.none, value.Vector3);
			return VariableValue.Create(result);
		}

		#endregion

		#region Vector4

		public static float GetVector4Height()
		{
			return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector4, GUIContent.none);
		}

		private static VariableValue DrawVector4(Rect rect, VariableValue value)
		{
			var result = EditorGUI.Vector4Field(rect, GUIContent.none, value.Vector4);
			return VariableValue.Create(result);
		}

		#endregion

		#region Quaternion

		public static float GetQuaternionHeight()
		{
			return AngleDisplayDrawer.GetHeight(GUIContent.none, AngleDisplayType.Euler);
		}

		private static VariableValue DrawQuaternion(Rect rect, VariableValue value)
		{
			var result = AngleDisplayDrawer.Draw(rect, GUIContent.none, value.Quaternion, AngleDisplayType.Euler);
			return VariableValue.Create(result);
		}

		#endregion

		#region Rect

		public static float GetRectHeight()
		{
			return EditorGUI.GetPropertyHeight(SerializedPropertyType.Rect, GUIContent.none);
		}

		private static VariableValue DrawRect(Rect rect, VariableValue value)
		{
			var result = EditorGUI.RectField(rect, GUIContent.none, value.Rect);
			return VariableValue.Create(result);
		}

		#endregion

		#region Bounds

		public static float GetBoundsHeight()
		{
			return EditorGUI.GetPropertyHeight(SerializedPropertyType.Bounds, GUIContent.none);
		}

		private static VariableValue DrawBounds(Rect rect, VariableValue value)
		{
			var result = EditorGUI.BoundsField(rect, GUIContent.none, value.Bounds);
			return VariableValue.Create(result);
		}

		#endregion

		#region Color

		public static float GetColorHeight()
		{
			return EditorGUI.GetPropertyHeight(SerializedPropertyType.Boolean, GUIContent.none);
		}

		private static VariableValue DrawColor(Rect rect, VariableValue value)
		{
			var result = EditorGUI.ColorField(rect, GUIContent.none, value.Color);
			return VariableValue.Create(result);
		}

		#endregion

		#region String

		public static float GetStringHeight()
		{
			return EditorGUI.GetPropertyHeight(SerializedPropertyType.String, GUIContent.none);
		}

		private static VariableValue DrawString(Rect rect, VariableValue value, StringVariableConstraint constraint)
		{
			var s = value.String;

			if (constraint != null && constraint.Values.Length > 0)
			{
				var index = Array.IndexOf(constraint.Values, s);
				index = EditorGUI.Popup(rect, index, constraint.Values);
				s = index >= 0 ? constraint.Values[index] : string.Empty;
			}
			else
			{
				s = EditorGUI.TextField(rect, GUIContent.none, s);
			}

			return s == null ? VariableValue.Create(string.Empty) : VariableValue.Create(s);
		}

		#endregion

		#region Enum

		public static float GetEnumHeight()
		{
			return EditorGUI.GetPropertyHeight(SerializedPropertyType.Enum, GUIContent.none);
		}

		private static VariableValue DrawEnum(Rect rect, VariableValue value, EnumVariableConstraint constraint)
		{
			var e = EditorGUI.EnumPopup(rect, GUIContent.none, value.Enum);
			return VariableValue.Create(e);
		}

		#endregion

		#region Object

		public static float GetObjectHeight()
		{
			return EditorGUI.GetPropertyHeight(SerializedPropertyType.ObjectReference, GUIContent.none);
		}

		private static VariableValue DrawObject(Rect rect, VariableValue value, ObjectVariableConstraint constraint)
		{
			var objectType = constraint?.Type ?? value.ReferenceType ?? typeof(Object);
			var unityObject = value.Object;

			if (typeof(Component).IsAssignableFrom(objectType) || typeof(GameObject) == objectType || typeof(Object) == objectType)
				unityObject = EditorGUI.ObjectField(rect, GUIContent.none, value.Object, objectType, true);
			else
				unityObject = AssetDisplayDrawer.Draw(rect, GUIContent.none, unityObject, objectType, true, true, AssetLocation.None, null);

			return VariableValue.Create(unityObject);
		}

		#endregion

		#region Store

		public static float GetStoreHeight(VariableValue value, StoreVariableConstraint constraint, bool drawStores)
		{
			var height = EditorGUIUtility.singleLineHeight;

			if (drawStores)
			{
				var index = 0;
				var names = value.Store.GetVariableNames();

				foreach (var name in names)
				{
					var variable = value.Store.GetVariable(name);
					var itemDefinition = constraint?.Schema != null ? constraint.Schema[index].Definition : ValueDefinition.Create(VariableType.Empty);

					height += RectHelper.VerticalSpace;
					height += GetHeight(variable, itemDefinition, true);
					index++;
				}
			}

			return height;
		}

		private static VariableValue DrawStore(Rect rect, VariableValue value, StoreVariableConstraint constraint, bool drawStores)
		{
			if (drawStores)
			{
				var names = value.Store.GetVariableNames();
				var remove = string.Empty;
				var first = true;
				var empty = ValueDefinition.Create(VariableType.Empty);

				foreach (var name in names)
				{
					if (!first)
						RectHelper.TakeVerticalSpace(ref rect);

					var index = constraint?.Schema != null ? constraint.Schema.GetIndex(name) : -1;
					var definition = index >= 0 ? constraint.Schema[index].Definition : empty;

					var item = value.Store.GetVariable(name);
					var height = GetHeight(item, definition, true);
					var itemRect = RectHelper.TakeHeight(ref rect, height);
					var labelRect = RectHelper.TakeWidth(ref itemRect, _storeLabelWidth);
					labelRect = RectHelper.TakeLine(ref labelRect);

					EditorGUI.LabelField(labelRect, name);

					if (constraint?.Schema == null && value.Store is VariableStore)
					{
						var removeRect = RectHelper.TakeTrailingIcon(ref itemRect);

						if (GUI.Button(removeRect, _removeStoreButton.Content, GUIStyle.none))
							remove = name;
					}

					item = Draw(itemRect, GUIContent.none, item, definition, true);
					value.Store.SetVariable(name, item);

					first = false;
				}

				if (constraint?.Schema == null && value.Store is VariableStore store)
				{
					var addRect = RectHelper.TakeTrailingIcon(ref rect);

					if (GUI.Button(addRect, _addStoreButton.Content, GUIStyle.none))
					{
						AddToStorePopup.Store = store;
						AddToStorePopup.Name = "";
						PopupWindow.Show(addRect, AddToStorePopup.Instance);
					}

					if (!string.IsNullOrEmpty(remove))
						(value.Store as VariableStore).RemoveVariable(remove);
				}
			}
			else
			{
				EditorGUI.LabelField(rect, value.Store.ToString());
			}

			return value;
		}

		private class AddToStorePopup : PopupWindowContent
		{
			public static AddToStorePopup Instance = new AddToStorePopup();
			public static VariableStore Store;
			public static string Name;

			public override Vector2 GetWindowSize()
			{
				var size = base.GetWindowSize();

				size.y = 10.0f; // padding
				size.y += RectHelper.LineHeight; // name
				size.y += RectHelper.LineHeight; // button

				return size;
			}

			public override void OnGUI(Rect rect)
			{
				rect = RectHelper.Inset(rect, 5.0f);
				var nameRect = RectHelper.TakeLine(ref rect);
				var buttonRect = RectHelper.TakeTrailingHeight(ref rect, EditorGUIUtility.singleLineHeight);
				RectHelper.TakeTrailingHeight(ref rect, RectHelper.VerticalSpace);

				Name = EditorGUI.TextField(nameRect, Name);
				var valid = !string.IsNullOrEmpty(Name) && !Store.Map.ContainsKey(Name);

				using (new EditorGUI.DisabledScope(!valid))
				{
					if (GUI.Button(buttonRect, "Add"))
					{
						Store.AddVariable(Name, VariableValue.Empty);
						editorWindow.Close();
					}
				}
			}
		}

		#endregion

		#region List

		public static float GetListHeight(VariableValue value, ListVariableConstraint constraint, bool drawStores)
		{
			var height = EditorGUIUtility.singleLineHeight;

			var itemDefinition = constraint != null
				? ValueDefinition.Create(constraint.ItemType, constraint.ItemConstraint)
				: ValueDefinition.Create(VariableType.Empty);

			for (var i = 0; i < value.List.Count; i++)
			{
				if (i != 0)
					height += EditorGUIUtility.standardVerticalSpacing;

				var item = value.List.GetVariable(i);
				height += GetHeight(item, itemDefinition, drawStores);
			}

			return height;
		}

		private static VariableValue DrawList(Rect rect, VariableValue value, ListVariableConstraint constraint, bool drawStores)
		{
			var itemDefinition = constraint != null
				? ValueDefinition.Create(constraint.ItemType, constraint.ItemConstraint)
				: ValueDefinition.Create(VariableType.Empty);

			var remove = -1;

			for (var i = 0; i < value.List.Count; i++)
			{
				if (i != 0)
					RectHelper.TakeVerticalSpace(ref rect);

				var item = value.List.GetVariable(i);
				var height = GetHeight(item, itemDefinition, drawStores);
				var itemRect = RectHelper.TakeHeight(ref rect, height);
				var removeRect = RectHelper.TakeTrailingIcon(ref itemRect);

				item = Draw(itemRect, GUIContent.none, item, itemDefinition, drawStores);
				value.List.SetVariable(i, item);

				if (GUI.Button(removeRect, _removeListButton.Content, GUIStyle.none))
					remove = i;
			}

			var addRect = RectHelper.TakeTrailingIcon(ref rect);

			if (GUI.Button(addRect, _addListButton.Content, GUIStyle.none))
				value.List.AddVariable(itemDefinition.Generate(null));

			if (remove >= 0)
				value.List.RemoveVariable(remove);

			return value;
		}

		#endregion
	}
}
