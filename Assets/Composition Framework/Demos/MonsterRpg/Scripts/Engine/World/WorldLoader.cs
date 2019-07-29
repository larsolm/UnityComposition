using PiRhoSoft.Composition;
using PiRhoSoft.Utilities;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	public enum LoadState
	{
		Loading,
		Error,
		Complete
	}

	public enum SaveState
	{
		Saving,
		Error,
		Complete
	}

	public class LoadStatus
	{
		public Action<Zone> OnComplete;
		public Action<float> OnProgress;
		public Action<string> OnError;

		public LoadState State { get; private set; }
		public float Progress { get; private set; }
		public string Message { get; private set; }
		public Zone StartingZone { get; private set; }

		public void UpdateProgress(float progress)
		{
			Progress = progress;
			OnProgress?.Invoke(progress);
		}

		public void SetError(string message)
		{
			State = LoadState.Error;
			Progress = 0.0f;
			Message = message;
			OnError?.Invoke(message);
		}

		public void SetComplete(Zone startingZone)
		{
			State = LoadState.Complete;
			Progress = 1.0f;
			StartingZone = startingZone;
			OnComplete?.Invoke(startingZone);
		}
	}

	public class SaveStatus
	{
		public Action OnComplete;
		public Action<float> OnProgress;
		public Action<string> OnError;

		public SaveState State { get; private set; }
		public float Progress { get; private set; }
		public string Message { get; private set; }

		public void UpdateProgress(float progress)
		{
			Progress = progress;
			OnProgress?.Invoke(progress);
		}

		public void SetError(string message)
		{
			State = SaveState.Error;
			Progress = 0.0f;
			Message = message;
			OnError?.Invoke(message);
		}

		public void SetComplete()
		{
			State = SaveState.Complete;
			Progress = 1.0f;
			OnComplete?.Invoke();
		}
	}

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
		public string StartingZone;
		public SpawnPoint PlayerSpawn;
	}

	[HelpURL(MonsterRpg.DocumentationUrl + "world-loader")]
	public class WorldLoader : GlobalBehaviour<WorldLoader>
	{
		public const string HeaderVariables = "Header";
		public const string SavedVariables = "Persistent";

		private const string _invalidLoadError = "(MWWLIL) Failed to load save file {0}: {1}";
		private const string _invalidSaveError = "(MWWLIS) Failed to write save file {0}: {1}";
		private const string _invalidZoneError = "(MWWLIZ) Failed to load starting zone: the zone {0} does not exist on the world";
		private const string _missingPlayerError = "(MWWLMP) Failed to load world: the main scene does not contain a Player";
		private const string _missingWorldAssetError = "(MWWLMWA) Failed to load world: the WorldManager in main scene does not have a World set";
		private const string _missingWorldManagerError = "(MMWWLMWA) Failed to load world: the main scene does not contain a WorldManager";

		public LoadStatus LoadHeader(string filename, ConstrainedStore header)
		{
			var info = new LoadStatus();
			var data = new VariableStore();
			Read(info, filename + HeaderVariables, data);
			header.CopyFrom(data, HeaderVariables);
			return info;
		}

		public SaveStatus SaveHeader(string filename, ConstrainedStore header)
		{
			var info = new SaveStatus();
			var data = new VariableStore();
			header.CopyTo(data, HeaderVariables);
			Write(info, filename + HeaderVariables, data);
			return info;
		}

		public LoadStatus Load(IVariableCollection header, string defaultZone, string defaultSpawn, string filename)
		{
			var info = new LoadStatus();
			StartCoroutine(Load(info, header, defaultZone, defaultSpawn, filename));
			return info;
		}

		public SaveStatus Save()
		{
			var info = new SaveStatus();
			StartCoroutine(Save(info));
			return info;
		}

		private IEnumerator Load(LoadStatus info, IVariableCollection header, string defaultZone, string defaultSpawn, string filename)
		{
			var data = new SaveData();

			if (!string.IsNullOrEmpty(filename))
				Read(info, filename, data);

			if (info.State == LoadState.Error)
				yield break;

			var gameData = data.Game ?? new GameSaveData { StartingZone = defaultZone, PlayerSpawn = new SpawnPoint { Name = defaultSpawn } };
			var worldData = data.World ?? new WorldSaveData();
			var playerData = data.Player ?? new PlayerSaveData();

			info.UpdateProgress(0.5f);

			if (WorldManager.Instance == null)
			{
				info.SetError(_missingWorldManagerError);
				yield break;
			}

			if (WorldManager.Instance.World == null)
			{
				info.SetError(_missingWorldAssetError);
				yield break;
			}

			if (Player.Instance == null)
			{
				info.SetError(_missingPlayerError);
				yield break;
			}

			var zone = WorldManager.Instance.World.GetZone(gameData.StartingZone);
			if (zone == null)
			{
				info.SetError(string.Format(_invalidZoneError, gameData.StartingZone));
				yield break;
			}

			WorldManager.Instance.Load(filename, header, worldData);
			Player.Instance.Load(header, playerData);

			info.SetComplete(zone);
		}

		private IEnumerator Save(SaveStatus info)
		{
			var header = new VariableStore();
			var data = new SaveData { Game = new GameSaveData(), World = new WorldSaveData(), Player = new PlayerSaveData() };
			data.Game.StartingZone = WorldManager.Instance.CurrentZone.name;
			data.Game.PlayerSpawn.Direction = Player.Instance.Mover.MovementDirection;
			data.Game.PlayerSpawn.Position = Player.Instance.transform.position;
			data.Game.PlayerSpawn.Layer = Player.Instance.Mover.MovementLayer;

			Player.Instance.Save(header, data.Player);

			var filename = WorldManager.Instance.Save(header, data.World);

			info.UpdateProgress(0.5f);

			Write(info, filename + HeaderVariables, header);
			Write(info, filename, data);

			if (info.State != SaveState.Error)
				info.SetComplete();

			yield break;
		}

		// These could be threaded but the Unity web platform doesn't support System.Thread and all platforms warn when
		// reading and writing Json from a background thread despite seeming to work correctly.

		private void Read<T>(LoadStatus info, string filename, T data)
		{
			try
			{
				var path = GetSavePath(filename);
				if (File.Exists(path))
				{
					var json = File.ReadAllText(path);
					JsonUtility.FromJsonOverwrite(json, data);
				}
			}
			catch (Exception e)
			{
				info.SetError(string.Format(_invalidLoadError, filename, e.Message));
			}
		}

		private void Write<T>(SaveStatus info, string filename, T data)
		{
			try
			{
				var path = GetSavePath(filename);
				var json = JsonUtility.ToJson(data, true);
				File.WriteAllText(path, json);
			}
			catch (Exception e)
			{
				info.SetError(string.Format(_invalidSaveError, filename, e.Message));
			}
		}

		private string GetSavePath(string filename)
		{
			return $"{Application.persistentDataPath}/{filename}.json";
		}
	}
}
