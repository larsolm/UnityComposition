using PiRhoSoft.Composition;
using System;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[Serializable]
	public class SaveData
	{
		public GameSaveData Game;
		public WorldSaveData World;
		public PlayerSaveData Player;
	}

	[Serializable]
	public class GameSaveData
	{
		public SpawnPoint PlayerSpawn;
	}

	[Serializable]
	public class WorldSaveData
	{
		public VariableStore Variables = new VariableStore();
	}

	[DisallowMultipleComponent]
	[AddComponentMenu("PiRho Soft/Monster RPG/World Manager")]
	public class WorldManager : VariableSetComponent
	{
		private const string _secondInstanceWarning = "(MRPGWMSI) A second instance of WorldManager was created";

		public static WorldManager Instance { get; private set; }

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
				Debug.LogWarning(_secondInstanceWarning, this);
				Destroy(this);
			}
		}

		void OnDestroy()
		{
			if (Instance == this)
				Instance = null;
		}
		
		public void Load(string filename, WorldSaveData saveData, string tag)
		{
			SchemaVariables.CopyFrom(saveData.Variables, tag);
			SaveFilename = filename;
		}

		public string Save(WorldSaveData saveData, string tag)
		{
			SchemaVariables.CopyTo(saveData.Variables, tag);
			return SaveFilename;
		}

		public string GetSavePath(string filename)
		{
			return $"{Application.persistentDataPath}/{filename}.json";
		}
	}
}
