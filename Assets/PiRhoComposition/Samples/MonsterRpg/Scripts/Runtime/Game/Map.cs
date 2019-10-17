using PiRhoSoft.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PiRhoSoft.MonsterRpg
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Monster RPG/Map")]
	public class Map : MonoBehaviour, ISerializationCallbackReceiver
	{
		private const string _noSpawnPointsWarning = "(MMMNSP) The map {0} does not have any spawn points";
		private const string _missingSpawnPointWarning = "(MMMMSP) The spawn point {0} could not be found in map {1}";

		[Tooltip("Whether or not the camera should clamp to the left point of this Map")] public bool ClampLeftBounds;
		[Tooltip("Whether or not the camera should clamp to the right point of this Map")] public bool ClampRightBounds;
		[Tooltip("Whether or not the camera should clamp to the bottom point of this Map")] public bool ClampBottomBounds;
		[Tooltip("Whether or not the camera should clamp to the top point of this Map")] public bool ClampTopBounds;

		[Tooltip("The left point at which the camera will clamp")] [Button(nameof(CalculateLeft), Label = "Calculate", Location = TraitLocation.Right)] public float LeftBounds;
		[Tooltip("The right point at which the camera will clamp")] [Button(nameof(CalculateRight), Label = "Calculate", Location = TraitLocation.Right)] public float RightBounds;
		[Tooltip("The bottom point at which the camera will clamp")] [Button(nameof(CalculateBottom), Label = "Calculate", Location = TraitLocation.Right)] public float BottomBounds;
		[Tooltip("The top point at which the camera will clamp")] [Button(nameof(CalculateTop), Label = "Calculate", Location = TraitLocation.Right)] public float TopBounds;

		[SerializeField] [HideInInspector] private List<Spawn> _spawns = new List<Spawn>();

		public bool ClampBounds => ClampLeftBounds || ClampRightBounds || ClampBottomBounds || ClampTopBounds;

		private Dictionary<string, SpawnPoint> _spawnPoints = new Dictionary<string, SpawnPoint>();

		public SpawnPoint GetSpawnPoint(string name)
		{
			var spawn = SpawnPoint.Default;

			if (_spawnPoints.Count == 0)
				Debug.LogWarningFormat(_noSpawnPointsWarning, name);
			else if (!_spawnPoints.TryGetValue(name, out spawn))
				Debug.LogWarningFormat(_missingSpawnPointWarning, name, name);

			return spawn;
		}

		public RectInt GetBounds()
		{
			var tilemaps = GetComponentsInChildren<Tilemap>();
			var left = int.MaxValue;
			var right = int.MinValue;
			var top = int.MinValue;
			var bottom = int.MaxValue;

			foreach (var tilemap in tilemaps)
			{
				tilemap.CompressBounds();
				if (tilemap.GetUsedTilesCount() == 0)
					continue;

				var bounds = tilemap.cellBounds;
				var position = tilemap.transform.position;

				left = Math.Min(left, Mathf.RoundToInt(bounds.xMin + position.x));
				right = Math.Max(right, Mathf.RoundToInt(bounds.xMax + position.x));
				top = Math.Max(top, Mathf.RoundToInt(bounds.yMax + position.y));
				bottom = Math.Min(bottom, Mathf.RoundToInt(bounds.yMin + position.y));
			}

			return new RectInt(left, bottom, right - left, top - bottom);
		}

		public void CalculateLeft()
		{
			var bounds = GetBounds();
			LeftBounds = bounds.xMin;
		}

		public void CalculateRight()
		{
			var bounds = GetBounds();
			RightBounds = bounds.xMax;
		}

		public void CalculateTop()
		{
			var bounds = GetBounds();
			TopBounds = bounds.yMax;
		}

		public void CalculateBottom()
		{
			var bounds = GetBounds();
			BottomBounds = bounds.yMin;
		}

		#region ISerializationCallbackReceiver Implementation

		public void OnBeforeSerialize()
		{
			_spawns = ObjectHelper.GetComponentsInScene<Spawn>(gameObject.scene.buildIndex, true);
		}

		public void OnAfterDeserialize()
		{
			_spawnPoints.Clear();

			foreach (var spawn in _spawns)
				_spawnPoints.Add(spawn.name, spawn.SpawnPoint);
		}

		#endregion

		#region Gizmos

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.magenta;

			if (ClampLeftBounds)
				Gizmos.DrawLine(new Vector2(LeftBounds, BottomBounds), new Vector2(LeftBounds, TopBounds));
			if (ClampRightBounds)
				Gizmos.DrawLine(new Vector2(RightBounds, BottomBounds), new Vector2(RightBounds, TopBounds));
			if (ClampTopBounds)
				Gizmos.DrawLine(new Vector2(LeftBounds, TopBounds), new Vector2(RightBounds, TopBounds));
			if (ClampBottomBounds)
				Gizmos.DrawLine(new Vector2(LeftBounds, BottomBounds), new Vector2(RightBounds, BottomBounds));
		}

		#endregion
	}
}