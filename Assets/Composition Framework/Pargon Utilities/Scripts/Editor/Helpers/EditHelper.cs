using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class EditScope : IDisposable
	{
		private bool _isDisposed;
		private SerializedObject _serializedObject;
		private Object _object;

		public EditScope(Object objectToTrack)
		{
			_object = objectToTrack;
			EditHelper.Start(_object);
		}

		public EditScope(SerializedObject serializedObject)
		{
			_serializedObject = serializedObject;
			EditHelper.Start(_serializedObject);
		}

		public void Dispose()
		{
			if (!_isDisposed)
			{
				_isDisposed = true;

				if (_object != null)
					EditHelper.Finish(_object);
				else if (_serializedObject != null)
					EditHelper.Finish(_serializedObject);
			}
		}
	}

	public static class EditHelper
	{
		public static void Start(Object obj)
		{
			Undo.RecordObject(obj, obj.name);
			PrefabUtility.RecordPrefabInstancePropertyModifications(obj);
		}

		public static void Start(SerializedObject obj)
		{
			obj.Update();
		}

		public static void Finish(Object obj)
		{
			Undo.FlushUndoRecordObjects();

			// SetDirty is for assets (including prefabs), MarkSceneDirty is for GameObjects

			if (!Application.isPlaying)
			{
				EditorUtility.SetDirty(obj);

				if (obj is GameObject gameObject)
					EditorSceneManager.MarkSceneDirty(gameObject.scene);
				else if (obj is Component component)
					EditorSceneManager.MarkSceneDirty(component.gameObject.scene);
			}
		}

		public static void Finish(SerializedObject obj)
		{
			obj.ApplyModifiedProperties();
		}

		public static void Bind<T>(VisualElement element, SerializedProperty property, Func<T> getValueFromElement, Func<T> getValueFromProperty, Action<T> updateElement, Action<T> updateProperty) where T : IEquatable<T>
		{
			element.RegisterCallback<ChangeEvent<T>>(evt =>
			{
				using (new EditScope(property.serializedObject))
					updateProperty(evt.newValue);
			});

			element.schedule.Execute(() =>
			{
				var fromElement = getValueFromElement();
				var fromProperty = getValueFromProperty();

				if (!fromElement.Equals(fromProperty))
					updateElement(fromProperty);

			}).Every(0);
		}

		public static void Bind<T>(VisualElement element, Object owner, Func<T> getValueFromElement, Func<T> getValueFromObject, Action<T> updateElement, Action<T> updateObject) where T : IEquatable<T>
		{
			element.RegisterCallback<ChangeEvent<T>>(evt =>
			{
				using (new EditScope(owner))
					updateObject(evt.newValue);
			});

			element.schedule.Execute(() =>
			{
				var fromElement = getValueFromElement();
				var fromObject = getValueFromObject();

				if (!fromElement.Equals(fromObject))
					updateElement(fromObject);

			}).Every(0);
		}
	}
}
