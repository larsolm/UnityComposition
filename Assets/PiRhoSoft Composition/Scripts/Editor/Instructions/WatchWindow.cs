using PiRhoSoft.CompositionEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	public class WatchWindow : EditorWindow
	{
		private Vector2 _scrollPosition;
		private const float _minimumWidth = 200.0f;

		private VariableStoreControl _globalStore;
		private List<VariableStoreControl> _instructionStores = new List<VariableStoreControl>();
		private List<VariableStoreControl> _watchedStores = new List<VariableStoreControl>();

		[MenuItem("Window/PiRho Soft/Watch Window")]
		public static void ShowWindow()
		{
			GetWindow<WatchWindow>("Watch Window").Show();
		}

		void OnEnable()
		{
			EditorApplication.playModeStateChanged += PlayModeStateChanged;
			minSize = new Vector2(_minimumWidth, minSize.y);
		}

		void OnDisable()
		{
			EditorApplication.playModeStateChanged -= PlayModeStateChanged;
		}

		private void PlayModeStateChanged(PlayModeStateChange state)
		{
			if (state == PlayModeStateChange.ExitingPlayMode)
			{
				_globalStore = null;
				_instructionStores.Clear();
				_watchedStores.Clear();
			}
		}

		void OnInspectorUpdate()
		{
			if (Application.isPlaying)
			{
				UpdateGlobalStore();
				UpdateInstructionStores();
				Repaint();
			}
		}

		void OnGUI()
		{
			using (var scroller = new EditorGUILayout.ScrollViewScope(_scrollPosition))
			{
				DrawGlobalStore();
				DrawInstructionStores();
				DrawWatchedStores();

				_scrollPosition = scroller.scrollPosition;
			}
		}

		#region Global Store

		private void UpdateGlobalStore()
		{
			if (CompositionManager.Exists && (_globalStore == null || _globalStore.Store != CompositionManager.Instance.GlobalStore))
			{
				_globalStore = new VariableStoreControl();
				_globalStore.Setup(CompositionManager.GlobalStoreName, CompositionManager.Instance.GlobalStore, false, false);
			}
		}

		private void DrawGlobalStore()
		{
			if (_globalStore != null)
			{
				_globalStore.Draw();

				if (_globalStore.Selected != null)
					AddWatch(_globalStore.SelectedName, _globalStore.Selected);
			}
		}

		#endregion

		#region Instruction Stores

		private void UpdateInstructionStores()
		{
			if (CompositionManager.Exists)
			{
				for (var i = 0; i < _instructionStores.Count; i++)
				{
					if (!CompositionManager.Instance.InstructionState.Any(data => data.Value.Variables == _instructionStores[i].Store))
						_instructionStores.RemoveAt(i--);
				}

				foreach (var instruction in CompositionManager.Instance.InstructionState)
				{
					if (!_instructionStores.Any(control => control.Store == instruction.Value.Variables))
					{
						var control = new VariableStoreControl();
						control.Setup(instruction.Key.name, instruction.Value.Variables, true, false);
						_instructionStores.Add(control);
					}
				}
			}
		}

		private void DrawInstructionStores()
		{
			foreach (var instruction in _instructionStores)
			{
				instruction.Draw();

				if (instruction.Selected != null)
					AddWatch(instruction.SelectedName, instruction.Selected);
			}
		}

		#endregion

		#region Watched Stores

		private void AddWatch(string name, IVariableStore store)
		{
			var control = new VariableStoreControl();
			control.Setup(name, store, false, true);
			_watchedStores.Add(control);
		}

		private void RemoveWatch(VariableStoreControl control)
		{
			_watchedStores.RemoveAll(c => c == control);
		}

		private void DrawWatchedStores()
		{
			var name = string.Empty;
			IVariableStore add = null;
			VariableStoreControl remove = null;

			foreach (var watched in _watchedStores)
			{
				watched.Draw();

				if (watched.Selected != null)
				{
					name = watched.SelectedName;
					add = watched.Selected;
				}

				if (watched.ShouldClose)
					remove = watched;
			}

			if (add != null)
				AddWatch(name, add);

			if (remove != null)
				RemoveWatch(remove);
		}

		#endregion
	}
}
