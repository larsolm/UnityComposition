using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.DocGen.Editor
{
	public class SettingsObject : ScriptableObject
	{
		[NonSerialized] public string Path;
		public Settings Settings;

		public void Reset()
		{
			Path = string.Empty;
			Settings = new Settings();
		}

		public void Load()
		{
			Path = EditorUtility.OpenFilePanel("Open Settings File", Generator.RootPath, "json");

			if (!string.IsNullOrEmpty(Path))
			{
				var content = File.ReadAllText(Path);
				Settings = JsonUtility.FromJson<Settings>(content);
			}
		}

		public void Save()
		{
			if (string.IsNullOrEmpty(Path))
				Path = EditorUtility.SaveFilePanel("Save Documentation Settings", Generator.RootPath, "settings", "json");

			if (!string.IsNullOrEmpty(Path))
			{
				var content = JsonUtility.ToJson(Settings, true);
				var outputFile = new FileInfo(Path);

				Directory.CreateDirectory(outputFile.Directory.FullName);
				File.WriteAllText(outputFile.FullName, content);
			}
		}
	}
	
	public class Window : EditorWindow
	{
		private static SettingsObject _settings;

		private Generator _generator;
		private UnityEditor.Editor _editor;
		private Vector2 _scrollPosition;
		private string _message = string.Empty;
		private float _progress = 0.0f;

		[MenuItem("Window/PiRho Soft/Documentation Generator")]
		public static void Open()
		{
			GetWindow<Window>("Documentation Generator").Show();
		}

		void OnEnable()
		{
			if (_settings == null)
			{
				_settings = CreateInstance<SettingsObject>();
				_settings.Reset();
			}

			_generator = new Generator();
		}

		void OnDisable()
		{
			if (_editor)
			{
				Destroy(_editor);
				_editor = null;
			}

			_generator = null;
		}

		void OnInspectorUpdate()
		{
			if (_progress > 0.0f)
				Repaint();
		}

		void OnGUI()
		{
			using (new EditorGUI.DisabledScope(_progress > 0.0f))
			{
				EditorGUILayout.Space();

				using (new GUILayout.HorizontalScope())
				{
					GUILayout.FlexibleSpace();

					var popupRect = GUILayoutUtility.GetRect(0.0f, 20.0f);

					if (GUILayout.Button("New", GUILayout.MinWidth(20), GUILayout.MaxWidth(80.0f)))
						_settings.Reset();

					if (GUILayout.Button("Open", GUILayout.MinWidth(20), GUILayout.MaxWidth(80.0f)))
						_settings.Load();
				}

				EditorGUILayout.Space();
				EditorGUILayout.Space();

				if (_settings != null)
				{
					using (new EditorGUILayout.HorizontalScope())
					{
						EditorGUILayout.SelectableLabel(_settings.Path);

						if (GUILayout.Button("Generate", GUILayout.MaxWidth(80.0f)))
						{
							_generator.Generate(_settings.Settings, (message, progress) =>
							{
								_message = message;
								_progress = progress;
							});
						}

						if (GUILayout.Button("Save", GUILayout.MaxWidth(80.0f)))
							_settings.Save();
					}

					EditorGUILayout.Space();

					using (var scrolling = new EditorGUILayout.ScrollViewScope(_scrollPosition))
					{
						var editor = UnityEditor.Editor.CreateEditor(_settings);
						editor.DrawDefaultInspector();
						_scrollPosition = scrolling.scrollPosition;
					}
				}
			}

			if (_progress > 0.0f)
			{
				if (_progress < 1.0f)
				{
					EditorUtility.DisplayProgressBar("Generating Documentation", _message, _progress);
				}
				else
				{
					_progress = 0.0f;
					EditorUtility.ClearProgressBar();
				}
			}
		}
	}
}
