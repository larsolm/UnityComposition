using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities.Editor
{
	public class ChangeScope : IDisposable
	{
		private bool _isDisposed;
		private SerializedObject _serializedObject;
		private Object _object;

		public ChangeScope(Object objectToTrack)
		{
			_object = objectToTrack;
			ChangeHelper.Start(_object);
		}

		public ChangeScope(SerializedObject serializedObject)
		{
			_serializedObject = serializedObject;
			ChangeHelper.Start(_serializedObject);
		}

		public void Dispose()
		{
			if (!_isDisposed)
			{
				_isDisposed = true;

				if (_object != null)
					ChangeHelper.Finish(_object);
				else if (_serializedObject != null)
					ChangeHelper.Finish(_serializedObject);
			}
		}
	}

	public static class ChangeHelper
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
	}
}
