using PiRhoSoft.Composition;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[CreateGraphNodeMenu("General/Load Game", order: 0)]
	public class LoadGame : GraphNode
	{
		private const string _invalidLoadError = "(MNLGIL) Failed to load save file {0}: {1}";

		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The filename to load")]
		public StringVariableSource Filename = new StringVariableSource();

		[Tooltip("The tag of the variables to load")]
		public StringVariableSource Tag = new StringVariableSource();

		public override Color NodeColor => Colors.ExecutionLight;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (variables.Resolve(this, Filename, out var filename) && variables.Resolve(this, Tag, out var tag))
			{
				var data = new SaveData();

				if (!string.IsNullOrEmpty(filename))
					Read(filename, data);

				var gameData = data.Game ?? new GameSaveData { PlayerSpawn = SpawnPoint.Default };
				var worldData = data.World ?? new WorldSaveData();
				var playerData = data.Player ?? new PlayerSaveData();

				WorldManager.Instance.Load(filename, worldData, tag);
				Player.Instance.Load(playerData, tag);
			}

			graph.GoTo(Next, nameof(Next));
			yield break;
		}

		private void Read<T>(string filename, T data)
		{
			try
			{
				var path = WorldManager.Instance.GetSavePath(filename);
				if (File.Exists(path))
				{
					var json = File.ReadAllText(path);
					JsonUtility.FromJsonOverwrite(json, data);
				}
			}
			catch (Exception e)
			{
				Debug.LogErrorFormat(this, _invalidLoadError, filename, e.Message);
			}
		}
	}
}
