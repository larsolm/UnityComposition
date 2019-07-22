using PiRhoSoft.Composition;
using PiRhoSoft.Utilities;
using System;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[Serializable]
	public class ZoneSaveData
	{
		public string Name = string.Empty;
		public VariableSet Variables = new VariableSet();
		public MapSaveData Map = new MapSaveData();
	}

	[Serializable]
	public class ZoneVariableSource : VariableSource<Zone> { }

	[Serializable]
	public class ZoneList : SerializedList<Zone> { }

	[HelpURL(MonsterRpg.DocumentationUrl + "zone")]
	[CreateAssetMenu(fileName = nameof(Zone), menuName = "PiRho Soft/Zone", order = 301)]
	public class Zone : VariableSetAsset
	{
		[Tooltip("The World this Zone is a part of")]
		[ChangeTrigger(nameof(WorldChanged))]
		public World World;

		[Tooltip("The scene that this zone loads")]
		[ScenePicker(nameof(CreateZone))]
		public SceneReference Scene = new SceneReference();

		[Tooltip("The map layer that this zone is on. Zones on the same layer will be enabled/disabled together")]
		[SerializeField]
		[Popup(new string[] { "Indoor", "Outdoor" })]
		private string _mapLayer = "Indoor";

		[MappedVariable]
		public string MapLayer => _mapLayer;

		[MappedVariable]
		public int SceneIndex => Scene.Index;

		#region Persistence

		public void Load(ZoneSaveData saveData)
		{
			Variables.LoadFrom(saveData.Variables, WorldLoader.SavedVariables);
		}

		public void Save(ZoneSaveData saveData)
		{
			Variables.SaveTo(saveData.Variables, WorldLoader.SavedVariables);
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

		#region Editor Interface

		public void WorldChanged(World from, World to)
		{
			if (from != null)
				from.Zones.Remove(this);

			if (to != null)
				to.Zones.Add(this);
		}

		#endregion
	}
}
