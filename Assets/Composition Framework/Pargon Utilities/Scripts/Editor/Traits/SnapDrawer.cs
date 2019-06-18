﻿using PiRhoSoft.PargonUtilities.Engine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	[CustomPropertyDrawer(typeof(SnapAttribute))]
	class SnapDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PITSDIT) Invalid type for SnapAttribute on field {0}: Snap can only be applied to int, float, vector, rect, or bounds fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var snap = attribute as SnapAttribute;
			var container = ElementHelper.GetPropertyContainer(property, property.displayName, fieldInfo, attribute);

			switch (property.propertyType)
			{
				case SerializedPropertyType.Integer:
				{
					Snap(property, Mathf.RoundToInt(snap.Number));
					container.RegisterCallback<FocusOutEvent>(e => Snap(property, Mathf.RoundToInt(snap.Number)));
					break;
				}
				case SerializedPropertyType.Float:
				{
					Snap(property, snap.Number);
					container.RegisterCallback<FocusOutEvent>(e => Snap(property, snap.Number));
					break;
				}
				case SerializedPropertyType.Vector2:
				{
					Snap(property, (Vector2)snap.Vector);
					container.RegisterCallback<FocusOutEvent>(e => Snap(property, (Vector2)snap.Vector));
					break;
				}
				case SerializedPropertyType.Vector2Int:
				{
					Snap(property, Vector2Int.RoundToInt(snap.Vector));
					container.RegisterCallback<FocusOutEvent>(e => Snap(property, Vector2Int.RoundToInt(snap.Vector)));
					break;
				}
				case SerializedPropertyType.Vector3:
				{
					Snap(property, (Vector3)snap.Vector);
					container.RegisterCallback<FocusOutEvent>(e => Snap(property, (Vector3)snap.Vector));
					break;
				}
				case SerializedPropertyType.Vector3Int:
				{
					Snap(property, Vector3Int.RoundToInt(snap.Vector));
					container.RegisterCallback<FocusOutEvent>(e => Snap(property, Vector3Int.RoundToInt(snap.Vector)));
					break;
				}
				case SerializedPropertyType.Vector4:
				{
					Snap(property, snap.Vector);
					container.RegisterCallback<FocusOutEvent>(e => Snap(property, snap.Number));
					break;
				}
				case SerializedPropertyType.Rect:
				{
					Snap(property, new Rect(snap.Vector.x, snap.Vector.y, snap.Vector.y, snap.Vector.w));
					container.RegisterCallback<FocusOutEvent>(e => Snap(property, new Rect(snap.Vector.x, snap.Vector.y, snap.Vector.y, snap.Vector.w)));
					break;
				}
				case SerializedPropertyType.RectInt:
				{
					Snap(property, new RectInt(Mathf.RoundToInt(snap.Vector.x), Mathf.RoundToInt(snap.Vector.y), Mathf.RoundToInt(snap.Vector.y), Mathf.RoundToInt(snap.Vector.w)));
					container.RegisterCallback<FocusOutEvent>(e => Snap(property, snap.Number));
					break;
				}
				case SerializedPropertyType.Bounds:
				{
					Snap(property, snap.Bounds);
					container.RegisterCallback<FocusOutEvent>(e => Snap(property, snap.Bounds));
					break;
				}
				case SerializedPropertyType.BoundsInt:
				{
					Snap(property, new BoundsInt(Vector3Int.RoundToInt(snap.Bounds.center), Vector3Int.RoundToInt(snap.Bounds.extents)));
					container.RegisterCallback<FocusOutEvent>(e => Snap(property, new BoundsInt(Vector3Int.RoundToInt(snap.Bounds.center), Vector3Int.RoundToInt(snap.Bounds.extents))));
					break;
				}
				default:
				{
					Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
					break;
				}
			}

			return container;
		}

		private void Snap(SerializedProperty property, int snap)
		{
			property.intValue = Snap(property.intValue, snap);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void Snap(SerializedProperty property, float snap)
		{
			property.floatValue = Snap(property.floatValue, snap);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void Snap(SerializedProperty property, Vector2 snap)
		{
			var x = Snap(property.vector2Value.x, snap.x);
			var y = Snap(property.vector2Value.y, snap.y);

			property.vector2Value = new Vector2(x, y);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void Snap(SerializedProperty property, Vector2Int snap)
		{
			var x = Snap(property.vector2IntValue.x, snap.x);
			var y = Snap(property.vector2IntValue.y, snap.y);

			property.vector2IntValue = new Vector2Int(x, y);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void Snap(SerializedProperty property, Vector3 snap)
		{
			var x = Snap(property.vector3Value.x, snap.x);
			var y = Snap(property.vector3Value.y, snap.y);
			var z = Snap(property.vector3Value.z, snap.z);

			property.vector3Value = new Vector3(x, y, z);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void Snap(SerializedProperty property, Vector3Int snap)
		{
			var x = Snap(property.vector3IntValue.x, snap.x);
			var y = Snap(property.vector3IntValue.y, snap.y);
			var z = Snap(property.vector3IntValue.z, snap.z);

			property.vector3IntValue = new Vector3Int(x, y, z);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void Snap(SerializedProperty property, Vector4 snap)
		{
			var x = Snap(property.vector4Value.x, snap.x);
			var y = Snap(property.vector4Value.y, snap.y);
			var z = Snap(property.vector4Value.z, snap.z);
			var w = Snap(property.vector4Value.w, snap.w);

			property.vector4Value = new Vector4(x, y, z, w);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void Snap(SerializedProperty property, Rect snap)
		{
			var x = Snap(property.rectValue.x, snap.x);
			var y = Snap(property.rectValue.y, snap.y);
			var width = Snap(property.rectValue.width, snap.width);
			var height = Snap(property.rectValue.height, snap.height);

			property.rectValue = new Rect(x, y, width, height);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void Snap(SerializedProperty property, RectInt snap)
		{
			var x = Snap(property.rectIntValue.x, snap.x);
			var y = Snap(property.rectIntValue.y, snap.y);
			var width = Snap(property.rectIntValue.width, snap.width);
			var height = Snap(property.rectIntValue.height, snap.height);

			property.rectIntValue = new RectInt(x, y, width, height);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void Snap(SerializedProperty property, Bounds snap)
		{
			var x = Snap(property.boundsValue.center.x, snap.center.x);
			var y = Snap(property.boundsValue.center.y, snap.center.y);
			var z = Snap(property.boundsValue.center.z, snap.center.z);
			var width = Snap(property.boundsValue.extents.x, snap.extents.x);
			var height = Snap(property.boundsValue.extents.y, snap.extents.y);
			var depth = Snap(property.boundsValue.extents.z, snap.extents.z);

			property.boundsValue = new Bounds(new Vector3(x, y, z), new Vector3(width, height, depth));
			property.serializedObject.ApplyModifiedProperties();
		}

		private void Snap(SerializedProperty property, BoundsInt snap)
		{
			var x = Snap(property.boundsIntValue.x, snap.x);
			var y = Snap(property.boundsIntValue.y, snap.y);
			var z = Snap(property.boundsIntValue.z, snap.z);
			var width = Snap(property.boundsIntValue.x, snap.size.x);
			var height = Snap(property.boundsIntValue.y, snap.size.y);
			var depth = Snap(property.boundsIntValue.z, snap.size.z);

			property.boundsIntValue = new BoundsInt(new Vector3Int(x, y, z), new Vector3Int(width, height, depth));
			property.serializedObject.ApplyModifiedProperties();
		}

		private int Snap(int value, int snap)
		{
			return snap > 0 ? Mathf.RoundToInt(value / (float)snap) * snap : value;
		}

		private float Snap(float value, float snap)
		{
			return snap > 0.0f ? Mathf.Round(value / snap) * snap : value;
		}
	}
}
