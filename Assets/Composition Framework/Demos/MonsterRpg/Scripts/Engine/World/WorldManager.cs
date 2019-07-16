using PiRhoSoft.Composition;
using PiRhoSoft.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PiRhoSoft.MonsterRpg
{
	[Serializable]
	public class WorldSaveData
	{
		public VariableSet Variables = new VariableSet();
		public List<ZoneSaveData> Zones = new List<ZoneSaveData>();
	}

	public class ZoneLoadStatus
	{
		public bool IsDone;
	}

	[DisallowMultipleComponent]
	[HelpURL(MonsterRpg.DocumentationUrl + "world-manager")]
	[AddComponentMenu("PiRho Soft/Managers/World Manager")]
	public class WorldManager : VariableSetComponent
	{
		private const string _secondInstanceWarning = "(WWMSI) A second instance of WorldManager was created";
		private const string _zoneAlreadyLoadedError = "(WWMZAL) Failed to load zone {0}: the zone is already loaded";
		private const string _zoneNotAssignedError = "(WWMZNA) Failed to load zone {0}: the zone has not been assigned a scene";
		private const string _zoneNotLoadedError = "(WWMZNL) Failed to unload zone {0}: the zone is not loaded";
		private const string _zoneUnloadFailedError = "(WWMZUF) Failed to unload zone {0}";
		private const string _missingZoneSceneWarning = "(WWMMZS) Zone {0} does not have a valid scene assigned";
		private const string _missingMapError = "(WWMMM) Failed to load zone {0}: the zone does not contain a Map component";

		public static WorldManager Instance { get; private set; }

		[Tooltip("The world asset that this manager will load zones and scenes from")]
		[ObjectPicker]
		[Required(MessageBoxType.Error, "A world asset is required")]
		public World World;

		[MappedVariable]
		public Zone CurrentZone { get; set; }

		[MappedVariable]
		public string SaveFilename { get; private set; }

		public WorldManager()
		{
			Instance = this;
		}

		void Awake()
		{
			if (Instance != this)
			{
				Debug.LogWarningFormat(_secondInstanceWarning, GetType().Name);
				Destroy(this);
			}
			else
			{
				CreateZones();
			}
		}

		void OnDestroy()
		{
			if (Instance == this)
				Instance = null;
		}

		public SpawnPoint GetSpawnPoint(string name)
		{
			var zone = _zones[CurrentZone.SceneIndex];
			return zone.Map.GetSpawnPoint(name);
		}

		#region Zones

		private ZoneData[] _zones;
		private readonly List<ZoneData> _zoadedZones = new List<ZoneData>();

		public enum ZoneState
		{
			Unloaded,
			Loading,
			Loaded,
			Unloading
		}

		private class ZoneData
		{
			public Zone Zone;
			public Map Map;
			public ZoneState State = ZoneState.Unloaded;
			public bool IsActive = false;
			public bool IsEnabled = false;

			public void Loaded()
			{
				var objects = Zone.Scene.Scene.GetRootGameObjects();

				foreach (var obj in objects)
				{
					if (Map = obj.GetComponent<Map>())
						break;
				}

				if (Map == null)
					Debug.LogErrorFormat(Zone, _missingMapError, Zone.name);

				State = ZoneState.Loaded;
			}

			public void Unloading()
			{
				State = ZoneState.Unloading;

				IsEnabled = false;
				Map = null;
			}

			public void Enabled()
			{
				var objects = Zone.Scene.Scene.GetRootGameObjects();

				foreach (var obj in objects)
					obj.SetActive(true);

				Map.RestoreNpcData();
				IsEnabled = true;
			}

			public void Disabled()
			{
				var objects = Zone.Scene.Scene.GetRootGameObjects();

				foreach (var obj in objects)
					obj.SetActive(false);

				Map.PersistNpcData();
				IsEnabled = false;
			}
		}

		private void CreateZones()
		{
			_zones = new ZoneData[SceneManager.sceneCountInBuildSettings];

			foreach (var zone in World.Zones)
			{
				var index = zone.SceneIndex;

				if (index < 0)
					Debug.LogWarningFormat(zone, _missingZoneSceneWarning, zone.name);
				else
					_zones[index] = new ZoneData { Zone = zone };
			}
		}

		#endregion

		#region Zone Changing

		public ZoneLoadStatus ChangeZone(Zone zone)
		{
			var from = _zones[CurrentZone.SceneIndex];
			var to = _zones[zone.SceneIndex];
			var loader = new ZoneLoadStatus { IsDone = false };

			LeavingZone(from, to);
			StartCoroutine(EnterZone(loader, from, to));
			StartCoroutine(UnloadAssets(loader));

			return loader;
		}

		private IEnumerator UnloadAssets(ZoneLoadStatus status)
		{
			while (!status.IsDone)
				yield return null;

			yield return Resources.UnloadUnusedAssets();
		}

		private void LeavingZone(ZoneData from, ZoneData to)
		{
			SceneManager.SetActiveScene(gameObject.scene);
			UnloadConnections(from, to);
		}

		private void EnteredZone(ZoneData to, ZoneData from)
		{
			SceneManager.SetActiveScene(to.Zone.Scene.Scene);
			CurrentZone = to.Zone;
		}

		#endregion

		#region Zone Loading

		private int _pendingLoads = 0;

		private IEnumerator LoadZone(ZoneData zone)
		{
			while (zone.State == ZoneState.Unloading)
				yield return null;

			if (zone.State != ZoneState.Unloaded)
			{
				Debug.LogErrorFormat(this, _zoneAlreadyLoadedError, zone.Zone.name);
				yield break;
			}

			var loader = SceneManager.LoadSceneAsync(zone.Zone.SceneIndex, LoadSceneMode.Additive);

			if (loader == null)
			{
				Debug.LogErrorFormat(this, _zoneNotAssignedError, zone.Zone.name);
				yield break;
			}

			_pendingLoads++;

			zone.State = ZoneState.Loading;

			while (!loader.isDone)
				yield return null;

			_zoadedZones.Add(zone);

			zone.Loaded();

			_pendingLoads--;
		}

		private IEnumerator UnloadZone(ZoneData zone)
		{
			if (zone.State != ZoneState.Loaded)
			{
				Debug.LogErrorFormat(this, _zoneNotLoadedError, zone.Zone.name);
				yield break;
			}

			zone.Unloading();

			_zoadedZones.Remove(zone);

			var loader = SceneManager.UnloadSceneAsync(zone.Zone.SceneIndex);

			if (loader == null)
			{
				Debug.LogErrorFormat(this, _zoneUnloadFailedError, zone.Zone.name);
				yield break;
			}

			while (!loader.isDone)
				yield return null;

			zone.State = ZoneState.Unloaded;
		}

		private IEnumerator EnterZone(ZoneLoadStatus status, ZoneData from, ZoneData to)
		{
			if (to.State == ZoneState.Unloaded || to.State == ZoneState.Unloading)
				yield return LoadZone(to);

			while (to.State == ZoneState.Loading)
				yield return null;

			yield return LoadConnections(to, from);

			EnteredZone(to, from);
			status.IsDone = true;
		}

		private IEnumerator LoadConnections(ZoneData to, ZoneData from)
		{
			foreach (var zone in to.Map.Connections)
			{
				var data = _zones[zone.SceneIndex];

				if (data.State == ZoneState.Unloaded || data.State == ZoneState.Unloading)
					StartCoroutine(LoadZone(data));
			}

			while (_pendingLoads > 0)
				yield return null;

			foreach (var loadedZone in _zoadedZones)
			{
				var enabled = loadedZone.Zone.MapLayer == to.Zone.MapLayer;

				if (enabled && !loadedZone.IsEnabled)
					loadedZone.Enabled();

				if (!enabled && loadedZone.IsEnabled)
					loadedZone.Disabled();
			}
		}

		private void UnloadConnections(ZoneData from, ZoneData to)
		{
			foreach (var zone in from.Map.Connections)
			{
				var data = _zones[zone.SceneIndex];

				if (data.Zone != to.Zone && !to.Map.Connections.Contains(zone))
					StartCoroutine(UnloadZone(data));
			}

			if (!to.Map.Connections.Contains(from.Zone))
				StartCoroutine(UnloadZone(from));
		}

		#endregion

		#region Persistence

		public void Load(string filename, VariableSet header, WorldSaveData saveData)
		{
			Variables.LoadFrom(header, WorldLoader.HeaderVariables);
			Variables.LoadFrom(saveData.Variables, WorldLoader.SavedVariables);

			foreach (var zoneData in saveData.Zones)
			{
				var zone = World.GetZone(zoneData.Name);
				var data = _zones[zone.SceneIndex];
				zone.Save(zoneData);
				data.Map.Save(zoneData.Map);
			}

			SaveFilename = filename;
		}

		public string Save(VariableSet header, WorldSaveData saveData)
		{
			Variables.SaveTo(header, WorldLoader.HeaderVariables);
			Variables.SaveTo(saveData.Variables, WorldLoader.SavedVariables);

			foreach (var zone in World.Zones)
			{
				var data = _zones[zone.SceneIndex];
				var zoneData = new ZoneSaveData { Name = zone.name };
				zone.Save(zoneData);
				data.Map.Save(zoneData.Map);
				saveData.Zones.Add(zoneData);
			}

			return SaveFilename;
		}

		#endregion
	}
}
