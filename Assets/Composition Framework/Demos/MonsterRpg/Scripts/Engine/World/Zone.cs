using PiRhoSoft.Composition.Engine;
using PiRhoSoft.Utilities.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg.Engine
{
	public enum ZoneState
	{
		Unloaded,
		Loading,
		Loaded,
		Unloading
	}

	[Serializable]
	public class ZoneSaveData
	{
		public string Name = string.Empty;
		public VariableSet Variables = new VariableSet();
		public List<NpcSaveData> Npcs = new List<NpcSaveData>();
	}

	[Serializable]
	public class ZoneVariableSource : VariableSource<Zone> { }

	[Serializable]
	public class ZoneList : SerializedList<Zone> { }

	[HelpURL(MonsterRpg.DocumentationUrl + "zone")]
	[CreateAssetMenu(fileName = nameof(Zone), menuName = "PiRho Soft/Zone", order = 301)]
	public class Zone : VariableSetAsset
	{
		private const string _noSpawnPointsWarning = "(WZDNSP) The zone {0} does not have any spawn points";
		private const string _missingSpawnPointWarning = "(WZDMSP) The spawn point {0} could not be found in zone {1}";

		[Tooltip("The World this Zone is a part of")]
		public World World;

		[Tooltip("The scene that this zone loads")]
		[ScenePicker(nameof(CreateZone))]
		public SceneReference Scene = new SceneReference();

		[MappedVariable]
		public int SceneIndex => Scene.Index;

		public Map Map { get; private set; }
		public ZoneState State { get; private set; }
		public bool IsActive { get; private set; }
		public bool IsEnabled { get; private set; }

		//private Dictionary<string, NpcSaveData> _npcData = new Dictionary<string, NpcSaveData>();

		public SpawnPoint GetSpawnPoint(string name)
		{
			var spawn = SpawnPoint.Default;

			//if (SpawnPoints.Count == 0)
			//{
			//	Debug.LogWarningFormat(_noSpawnPointsWarning, this.name);
			//}
			//else if (!SpawnPoints.TryGetValue(name, out spawn))
			//{
			//	Debug.LogWarningFormat(_missingSpawnPointWarning, name, this.name);
			//	spawn = SpawnPoints.Values.First();
			//}

			return spawn;
		}

		#region Persistence

		public void Load(ZoneSaveData saveData)
		{
			Variables.LoadFrom(saveData.Variables, WorldLoader.SavedVariables);
			//_npcData = saveData.Npcs.ToDictionary(npc => npc.Id);
		}

		public void Save(ZoneSaveData saveData)
		{
			Variables.SaveTo(saveData.Variables, WorldLoader.SavedVariables);
			//if (IsEnabled)
			//	PersistNpcData();
			//
			//saveData.Npcs = _npcData.Select(npc => npc.Value).ToList();
		}

		public void RestoreNpcData()
		{
			//foreach (var npc in Npcs)
			//{
			//	_npcData.TryGetValue(npc.Guid, out NpcSaveData npcData);
			//	npc.Load(npcData);
			//}
		}
		
		public void PersistNpcData()
		{
			//foreach (var npc in Npcs)
			//{
			//	if (!_npcData.TryGetValue(npc.Guid, out NpcSaveData npcData))
			//	{
			//		npcData = new NpcSaveData { Id = npc.Guid.ToString() };
			//		_npcData.Add(npc.Guid, npcData);
			//	}
			//
			//	npc.Save(npcData);
			//}
		}

		#endregion

		#region Scene Maintenance

		protected override void OnEnable()
		{
			Scene.Setup(this);
		}

		void OnDisable()
		{
			Scene.Teardown();
		}

		private void CreateZone()
		{
			var map = new GameObject() { isStatic = true };
			map.AddComponent<Map>();
		}

		#endregion
	}
}
