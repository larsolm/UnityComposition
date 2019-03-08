using UnityEngine;
using UnityEditor;

namespace PiRhoSoft.SnippetsEditor
{
	[InitializeOnLoad]
	public static class AutoSnapping
	{
		public static class Preferences
		{
			private const string _enabled = "PiRhoSoft.AutoSnapping.Enabled";
			private const string _snapPosition = "PiRhoSoft.AutoSnapping.SnapPosition";
			private const string _snapScale = "PiRhoSoft.AutoSnapping.SnapScale";
			private const string _snapRotation = "PiRhoSoft.AutoSnapping.SnapRotation";
			private const string _xPositionIncrement = "PiRhoSoft.AutoSnapping.XPositionIncrement";
			private const string _yPositionIncrement = "PiRhoSoft.AutoSnapping.YPositionIncrement";
			private const string _zPositionIncrement = "PiRhoSoft.AutoSnapping.ZPositionIncrement";
			private const string _xScaleIncrement = "PiRhoSoft.AutoSnapping.XScaleIncrement";
			private const string _yScaleIncrement = "PiRhoSoft.AutoSnapping.YScaleIncrement";
			private const string _zScaleIncrement = "PiRhoSoft.AutoSnapping.ZScaleIncrement";
			private const string _rotationIncrement = "PiRhoSoft.AutoSnapping.RotationIncrement";

			public static bool Enabled { get => EditorPrefs.GetBool(_enabled, false); set => EditorPrefs.SetBool(_enabled, value); }
			public static bool SnapPosition { get => EditorPrefs.GetBool(_snapPosition, true); set => EditorPrefs.SetBool(_snapPosition, value); }
			public static bool SnapScale { get => EditorPrefs.GetBool(_snapScale, true); set => EditorPrefs.SetBool(_snapScale, value); }
			public static bool SnapRotation { get => EditorPrefs.GetBool(_snapRotation, true); set => EditorPrefs.SetBool(_snapRotation, value); }
			public static float XPositionIncrement { get => EditorPrefs.GetFloat(_xPositionIncrement, 0.1f); set => EditorPrefs.SetFloat(_xPositionIncrement, value); }
			public static float YPositionIncrement { get => EditorPrefs.GetFloat(_yPositionIncrement, 0.1f); set => EditorPrefs.SetFloat(_yPositionIncrement, value); }
			public static float ZPositionIncrement { get => EditorPrefs.GetFloat(_zPositionIncrement, 0.1f); set => EditorPrefs.SetFloat(_zPositionIncrement, value); }
			public static float XScaleIncrement { get => EditorPrefs.GetFloat(_xScaleIncrement, 0.1f); set => EditorPrefs.SetFloat(_xScaleIncrement, value); }
			public static float YScaleIncrement { get => EditorPrefs.GetFloat(_yScaleIncrement, 0.1f); set => EditorPrefs.SetFloat(_yScaleIncrement, value); }
			public static float ZScaleIncrement { get => EditorPrefs.GetFloat(_zScaleIncrement, 0.1f); set => EditorPrefs.SetFloat(_zScaleIncrement, value); }
			public static float RotationIncrement { get => EditorPrefs.GetFloat(_rotationIncrement, 15.0f); set => EditorPrefs.SetFloat(_rotationIncrement, value); }
		}

		private static bool _active = false;

		static AutoSnapping()
		{
			SetEnabled(Preferences.Enabled);
		}

		public static void SetEnabled(bool enabled)
		{
			Preferences.Enabled = enabled;

			if (enabled && !_active)
			{
				EditorApplication.update += Update;
				_active = true;
			}
			else if (!enabled && _active)
			{
				EditorApplication.update -= Update;
				_active = false;
			}
		}

		private static void Update()
		{
			if (!EditorApplication.isPlaying && Selection.transforms.Length > 0)
			{
				if (Preferences.SnapPosition)
					SnapPosition();

				if (Preferences.SnapScale)
					SnapScale();

				if (Preferences.SnapRotation)
					SnapRotation();
			}
		}

		private static void SnapPosition()
		{
			foreach (var transform in Selection.transforms)
			{
				if (!(transform is RectTransform))
				{
					var position = transform.position;
					position.x = Snap(position.x, Preferences.XPositionIncrement);
					position.y = Snap(position.y, Preferences.YPositionIncrement);
					position.z = Snap(position.z, Preferences.ZPositionIncrement);
					transform.transform.position = position;
				}
			}
		}

		private static void SnapScale()
		{
			foreach (var transform in Selection.transforms)
			{
				if (!(transform is RectTransform))
				{
					var scale = transform.localScale;
					scale.x = Snap(scale.x, Preferences.XScaleIncrement);
					scale.y = Snap(scale.y, Preferences.YScaleIncrement);
					scale.z = Snap(scale.z, Preferences.ZScaleIncrement);
					transform.transform.localScale = scale;

					var renderer = transform.GetComponent<SpriteRenderer>();
					if (renderer)
					{
						var x = Snap(renderer.size.x, Preferences.XScaleIncrement);
						var y = Snap(renderer.size.y, Preferences.YScaleIncrement);

						renderer.size = new Vector2(x, y);
					}
				}
			}
		}

		private static void SnapRotation()
		{
			foreach (var transform in Selection.transforms)
			{
				if (!(transform is RectTransform))
				{
					var rotation = transform.eulerAngles;
					rotation.x = Snap(rotation.x, Preferences.RotationIncrement);
					rotation.y = Snap(rotation.y, Preferences.RotationIncrement);
					rotation.z = Snap(rotation.z, Preferences.RotationIncrement);
					transform.transform.eulerAngles = rotation;
				}
			}
		}

		private static float Snap(float value, float snap)
		{
			return snap > 0.0f ? Mathf.Round(value / snap) * snap : value;
		}
	}

	public class AutoSnappingWindow : EditorWindow
	{
		private static readonly GUIContent _enabledContent = new GUIContent("Enabled Snapping", "Enable automatic snapping of transforms to a selected grid");
		private static readonly GUIContent _snapPositionContent = new GUIContent("Snap Positions", "Whether positions should be snapped to increments automatically when moved");
		private static readonly GUIContent _snapScaleContent = new GUIContent("Snap Scales", "Whether scales should be snapped to the increments automatically when scaled");
		private static readonly GUIContent _snapRotationContent = new GUIContent("Snap Rotations", "Whether rotations should be snapped to the increments when rotated");
		private static readonly GUIContent _xPositionContent = new GUIContent("X Position Increment", "The increment to snap the x positions to");
		private static readonly GUIContent _yPositionContent = new GUIContent("Y Position Increment", "The increment to snap the y positions to");
		private static readonly GUIContent _zPositionContent = new GUIContent("Z Position Increment", "The increment to snap the z positions to");
		private static readonly GUIContent _xScaleContent = new GUIContent("X Scale Increment", "The increment to snap the x scales to");
		private static readonly GUIContent _yScaleContent = new GUIContent("Y Scale Increment", "The increment to snap the y scales to");
		private static readonly GUIContent _zScaleContent = new GUIContent("Z Scale Increment", "The increment to snap the z scales to");
		private static readonly GUIContent _rotationContent = new GUIContent("Rotation Increment", "The increment to snap rotations to");

		[MenuItem("Window/PiRho Soft/Auto Snapping")]
		public static void Open()
		{
			GetWindow<AutoSnappingWindow>("Auto Snapping").Show();
		}

		public void OnGUI()
		{
			var enabled = EditorGUILayout.Toggle(_enabledContent, AutoSnapping.Preferences.Enabled);

			if (enabled != AutoSnapping.Preferences.Enabled)
				AutoSnapping.SetEnabled(enabled);

			if (AutoSnapping.Preferences.Enabled)
			{
				AutoSnapping.Preferences.SnapPosition = EditorGUILayout.Toggle(_snapPositionContent, AutoSnapping.Preferences.SnapPosition);

				if (AutoSnapping.Preferences.SnapPosition)
				{
					AutoSnapping.Preferences.XPositionIncrement = Mathf.Max(0.001f, EditorGUILayout.FloatField(_xPositionContent, AutoSnapping.Preferences.XPositionIncrement));
					AutoSnapping.Preferences.YPositionIncrement = Mathf.Max(0.001f, EditorGUILayout.FloatField(_yPositionContent, AutoSnapping.Preferences.YPositionIncrement));
					AutoSnapping.Preferences.ZPositionIncrement = Mathf.Max(0.001f, EditorGUILayout.FloatField(_zPositionContent, AutoSnapping.Preferences.ZPositionIncrement));
				}

				AutoSnapping.Preferences.SnapScale = EditorGUILayout.Toggle(_snapScaleContent, AutoSnapping.Preferences.SnapScale);

				if (AutoSnapping.Preferences.SnapScale)
				{
					AutoSnapping.Preferences.XScaleIncrement = Mathf.Max(0.001f, EditorGUILayout.FloatField(_xScaleContent, AutoSnapping.Preferences.XScaleIncrement));
					AutoSnapping.Preferences.YScaleIncrement = Mathf.Max(0.001f, EditorGUILayout.FloatField(_yScaleContent, AutoSnapping.Preferences.YScaleIncrement));
					AutoSnapping.Preferences.ZScaleIncrement = Mathf.Max(0.001f, EditorGUILayout.FloatField(_zScaleContent, AutoSnapping.Preferences.ZScaleIncrement));
				}

				AutoSnapping.Preferences.SnapRotation = EditorGUILayout.Toggle(_snapRotationContent, AutoSnapping.Preferences.SnapRotation);

				if (AutoSnapping.Preferences.SnapRotation)
					AutoSnapping.Preferences.RotationIncrement = Mathf.Max(0.001f, EditorGUILayout.FloatField(_rotationContent, AutoSnapping.Preferences.RotationIncrement));
			}
		}
	}
}