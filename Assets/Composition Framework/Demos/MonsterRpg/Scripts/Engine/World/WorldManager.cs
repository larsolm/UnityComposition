using PiRhoSoft.Composition.Engine;
using PiRhoSoft.Utilities.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace PiRhoSoft.MonsterRpg.Engine
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

		public static WorldManager Instance { get; private set; }

		[Tooltip("The world asset that this manager will load zones and scenes from")]
		[ObjectPicker]
		[Required(MessageBoxType.Error, "A world asset is required")]
		public World World;

		[MappedVariable]
		public Zone CurrentZone { get; set; }

		[MappedVariable]
		public string SaveFilename { get; private set; }

		public List<Zone> LoadedZones { get; private set; }

		//private Dictionary<Vector2Int, CollisionLayer> _occupiedTiles = new Dictionary<Vector2Int, CollisionLayer>();
		//private Dictionary<Vector2Int, Interaction> _interactionTiles = new Dictionary<Vector2Int, Interaction>();

		private Zone[] _zones;

		private int _freezeCount = 0;
		private int _transitionCount = 0;
		private int _pendingLoads = 0;

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

		#region Zones

		private void CreateZones()
		{
			_zones = new Zone[SceneManager.sceneCountInBuildSettings];
			LoadedZones = new List<Zone>();

			foreach (var zone in World.Zones)
			{
				var index = zone.SceneIndex;

				if (index < 0)
					Debug.LogWarningFormat(zone, _missingZoneSceneWarning, zone.name);
				else
					_zones[index] = zone;
			}
		}

		public Zone GetZone(Object o)
		{
			switch (o)
			{
				case GameObject gameObject: return _zones[gameObject.scene.buildIndex];
				case MonoBehaviour behaviour: return _zones[behaviour.gameObject.scene.buildIndex];
				default: return null;
			}
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
				zone.Save(zoneData);
			}

			SaveFilename = filename;
		}

		public string Save(VariableSet header, WorldSaveData saveData)
		{
			Variables.SaveTo(header, WorldLoader.HeaderVariables);
			Variables.SaveTo(saveData.Variables, WorldLoader.SavedVariables);

			foreach (var zone in World.Zones)
			{
				var zoneData = new ZoneSaveData { Name = zone.name };
				zone.Save(zoneData);
				saveData.Zones.Add(zoneData);
			}

			return SaveFilename;
		}

		#endregion

		#region Freezing

		public bool IsFrozen => _freezeCount > 0;

		public void Freeze()
		{
			Time.timeScale = 0.0f;
			_freezeCount++;
		}

		public void Thaw()
		{
			_freezeCount--;

			if (_freezeCount == 0)
				Time.timeScale = 1.0f;
		}

		#endregion

		#region Zone Loading

		public IEnumerator LoadUi()
		{
			foreach (var scene in World.UiScenes)
			{
				if (scene.IsAssigned)
				{
					var loader = SceneManager.LoadSceneAsync(scene.Path, LoadSceneMode.Additive);
					if (loader != null)
					{
						while (!loader.isDone)
							yield return null;
					}
				}
			}
		}

		private IEnumerator LoadZone(ZoneData data)
		{
			while (data.State == ZoneState.Unloading)
				yield return null;

			if (data.State != ZoneState.Unloaded)
			{
				Debug.LogErrorFormat(this, _zoneAlreadyLoadedError, data.Zone.name);
				yield break;
			}

			var loader = SceneManager.LoadSceneAsync(data.Zone.Scene.Index, LoadSceneMode.Additive);

			if (loader == null)
			{
				Debug.LogErrorFormat(this, _zoneNotAssignedError, data.Zone.name);
				yield break;
			}

			_pendingLoads++;
			data.State = ZoneState.Loading;

			while (!loader.isDone)
				yield return null;

			var valid = data.Loaded();

			if (valid)
			{
				data.State = ZoneState.Loaded;
				LoadedZones.Add(data);
			}
			else
			{
				SceneManager.UnloadSceneAsync(data.SceneIndex);
				data.State = ZoneState.Unloaded;
			}

			_pendingLoads--;
		}

		private IEnumerator UnloadZone(ZoneData data)
		{
			if (data.State != ZoneState.Loaded)
			{
				Debug.LogErrorFormat(this, _zoneNotLoadedError, data.Zone.name);
				yield break;
			}

			data.Unloading();
			data.State = ZoneState.Unloading;
			LoadedZones.Remove(data);

			var loader = SceneManager.UnloadSceneAsync(data.SceneIndex);

			if (loader == null)
			{
				Debug.LogErrorFormat(this, _zoneUnloadFailedError, data.Zone.name);
				yield break;
			}

			while (!loader.isDone)
				yield return null;

			data.State = ZoneState.Unloaded;
		}

		private void EnteringLoadedZone(ZoneLoadStatus status, ZoneData to, ZoneData from)
		{
			StartCoroutine(LoadConnections(to, from));
			EnteringZone(to, from);
			status.IsDone = true;
		}

		private IEnumerator EnteringUnloadedZone(ZoneLoadStatus status, ZoneData from, ZoneData to, bool waitForConnections)
		{
			Freeze();

			if (to.State == ZoneState.Unloaded || to.State == ZoneState.Unloading)
				yield return LoadZone(to);

			while (to.State == ZoneState.Loading)
				yield return null;

			if (waitForConnections)
				yield return LoadConnections(to, from);
			else
				StartCoroutine(LoadConnections(to, from));

			EnteringZone(to, from);
			status.IsDone = true;

			Thaw();
		}

		private IEnumerator LoadConnections(ZoneData to, ZoneData from)
		{
			foreach (var index in to.Connections)
			{
				var data = Zones[index];

				if (data.State == ZoneState.Unloaded || data.State == ZoneState.Unloading)
					StartCoroutine(LoadZone(data));
			}

			while (_pendingLoads > 0)
				yield return null;

			foreach (var loadedZone in LoadedZones)
			{
				var enabled = loadedZone.Zone.MapLayer == to.Zone.MapLayer;

				if (enabled && !loadedZone.IsEnabled)
				{
					loadedZone.Enabled();
					yield return null; // wait for Start and OnEnabled calls
					loadedZone.RestoreNpcData();
				}

				if (!enabled && loadedZone.IsEnabled)
				{
					loadedZone.PersistNpcData();
					loadedZone.Disabled();
				}
			}
		}

		private void UnloadConnections(ZoneData from, ZoneData to)
		{
			foreach (var index in from.Connections)
			{
				var data = Zones[index];

				if (to == null || (data != to && !to.Connections.Contains(data.SceneIndex)))
					StartCoroutine(UnloadZone(data));
			}

			if (to == null || !to.Connections.Contains(from.SceneIndex))
				StartCoroutine(UnloadZone(from));
		}

		#endregion

		#region Zone Changing

		public bool IsTransitioning => _transitionCount > 0;

		public void ChangeZone(Zone zone)
		{
			LeavingZone();
	
			var to = GetZone(zone);
			var status = ChangeZone(to, false);

			EnterZone();

			StartCoroutine(UnloadAssets(status));
		}

		public void TransitionZone(Zone zone, SpawnPoint spawnPoint, Transition transition)
		{
			StartCoroutine(DoZoneTransition(zone, spawnPoint, transition));
		}

		private IEnumerator DoZoneTransition(Zone zone, SpawnPoint spawnPoint, Transition transition)
		{
			_transitionCount++;

			LeavingZone();

			var from = Player.Instance.Zone;
			var to = Zones[zone.Scene.Index];

			yield return TransitionManager.Instance.StartTransition(transition == null ? World.DefaultZoneTransition : transition, TransitionPhase.Out); // Don't null coalesce these

			var status = ChangeZone(to, true);

			while (!status.IsDone)
				yield return null;

			yield return UnloadAssets(status);
			yield return SpawnPlayer(to, spawnPoint);

			EnterZone();

			_transitionCount--;
		}

		private IEnumerator UnloadAssets(ZoneLoadStatus status)
		{
			while (!status.IsDone)
				yield return null;

			yield return Resources.UnloadUnusedAssets();
		}

		private IEnumerator SpawnPlayer(ZoneData zone, SpawnPoint spawnPoint)
		{
			if (spawnPoint.IsNamed)
				spawnPoint = zone.GetSpawnPoint(spawnPoint.Name);
			else
				spawnPoint = zone.SpawnPoints.Count > 0 ? zone.SpawnPoints.Values.First() : SpawnPoint.Default;

			Player.Instance.Mover.WarpToPosition(spawnPoint.Position, spawnPoint.Direction, spawnPoint.Layer);

			yield return TransitionManager.Instance.RunTransition(spawnPoint.Transition == null ? World.DefaultSpawnTransition : spawnPoint.Transition, TransitionPhase.In); // Don't null coalesce these

			if (spawnPoint.Move)
				Player.Instance.Mover.Move(spawnPoint.Direction);
		}

		private ZoneLoadStatus ChangeZone(ZoneData to, bool waitForConnections)
		{
			var from = Player.Instance.Zone;
			var loader = new ZoneLoadStatus { IsDone = false };

			if (from != null)
				LeaveZone(from, to);

			if (to != null)
			{
				if (to.State == ZoneState.Loaded && !waitForConnections)
					EnteringLoadedZone(loader, to, from);
				else
					StartCoroutine(EnteringUnloadedZone(loader, from, to, waitForConnections));
			}
			else
			{
				loader.IsDone = true;
			}

			return loader;
		}

		private void LeavingZone()
		{
			var zone = Player.Instance.Zone;
			if (zone != null)
				zone.Leaving();
		}

		private void LeaveZone(ZoneData from, ZoneData to)
		{
			from.Left(to);
			Player.Instance.Zone = null;
			SceneManager.SetActiveScene(gameObject.scene);

			UnloadConnections(from, to);

			_context.SetStore(nameof(Zone), null);
		}

		private void EnteringZone(ZoneData to, ZoneData from)
		{
			_context.SetStore(nameof(Zone), to);

			SceneManager.SetActiveScene(to.Zone.Scene.Scene);
			Player.Instance.Zone = to;
			to.Entering(from);
		}

		private void EnterZone()
		{
			var zone = Player.Instance.Zone;
			if (zone != null)
				zone.Entered();
		}

		#endregion
	}
}
