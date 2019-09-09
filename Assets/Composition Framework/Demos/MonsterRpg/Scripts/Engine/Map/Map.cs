using PiRhoSoft.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PiRhoSoft.MonsterRpg
{
	[DisallowMultipleComponent]
	[HelpURL(MonsterRpg.DocumentationUrl + "map-properties")]
	[AddComponentMenu("Monster RPG/World/Map")]
	public class Map : MonoBehaviour, ISerializationCallbackReceiver
	{
		private const string _noSpawnPointsWarning = "(MMMNSP) The map {0} does not have any spawn points";
		private const string _missingSpawnPointWarning = "(MMMMSP) The spawn point {0} could not be found in map {1}";

		[Tooltip("Whether or not the camera should clamp to the left point of this Map")] public bool ClampLeftBounds;
		[Tooltip("Whether or not the camera should clamp to the right point of this Map")] public bool ClampRightBounds;
		[Tooltip("Whether or not the camera should clamp to the bottom point of this Map")] public bool ClampBottomBounds;
		[Tooltip("Whether or not the camera should clamp to the top point of this Zone")] public bool ClampTopBounds;

		[Tooltip("The left point at which the camera will clamp")] [Button(nameof(CalculateLeft), Label = "Calculate", Location = TraitLocation.After)] public float LeftBounds;
		[Tooltip("The right point at which the camera will clamp")] [Button(nameof(CalculateRight), Label = "Calculate", Location = TraitLocation.After)] public float RightBounds;
		[Tooltip("The bottom point at which the camera will clamp")] [Button(nameof(CalculateBottom), Label = "Calculate", Location = TraitLocation.After)] public float BottomBounds;
		[Tooltip("The top point at which the camera will clamp")] [Button(nameof(CalculateTop), Label = "Calculate", Location = TraitLocation.After)] public float TopBounds;

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

		public float CalculateLeft()
		{
			var bounds = GetBounds();
			return bounds.xMin;
		}

		public float CalculateRight()
		{
			var bounds = GetBounds();
			return bounds.xMax;
		}

		public float CalculateTop()
		{
			var bounds = GetBounds();
			return bounds.yMax;
		}

		public float CalculateBottom()
		{
			var bounds = GetBounds();
			return bounds.yMin;
		}

		#region ISerializationCallbackReceiver Implementation

		public void OnBeforeSerialize()
		{
			ComponentHelper.GetComponentsInScene(gameObject.scene.buildIndex, _spawns, true);
		}

		public void OnAfterDeserialize()
		{
			_spawnPoints.Clear();

			foreach (var spawn in _spawns)
				_spawnPoints.Add(spawn.name, spawn.SpawnPoint);
		}

		#endregion
	}
}