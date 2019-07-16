using PiRhoSoft.Composition;
using System;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[Serializable]
	public class NpcSaveData
	{
		public string Id;
		public Vector2 Position;
		public MovementDirection Direction;
		public CollisionLayer Layer;
		public string ControllerData;
		public VariableSet NpcVariables = new VariableSet();
		public VariableSet TrainerVariables = new VariableSet();
	}

	[DisallowMultipleComponent]
	[HelpURL(MonsterRpg.DocumentationUrl + "npc")]
	[AddComponentMenu("Monster RPG/World/NPC")]
	public class Npc : VariableSetComponent
	{
		[Tooltip("The name of the character")]
		public string Name = string.Empty;

		[HideInInspector] public string Guid;

		public Controller Controller { get; private set; }
		public Trainer Trainer { get; private set; }

		public Npc()
		{
			Guid = System.Guid.NewGuid().ToString();
		}

		void Awake()
		{
			Controller = GetComponent<Controller>();
			Trainer = GetComponent<Trainer>();
		}

		#region Persistence

		public void Load(NpcSaveData saveData)
		{
			if (saveData != null)
			{
				Variables.LoadFrom(saveData.NpcVariables, WorldLoader.SavedVariables);

				if (Controller)
				{
					Controller.Mover.WarpToPosition(saveData.Position, saveData.Direction, saveData.Layer);
					Controller.Load(saveData.ControllerData);
				}

				if (Trainer)
					Trainer.Variables.LoadFrom(saveData.TrainerVariables, WorldLoader.SavedVariables);
			}
		}

		public void Save(NpcSaveData saveData)
		{
			Variables.SaveTo(saveData.NpcVariables, WorldLoader.SavedVariables);

			if (Controller)
			{
				saveData.Position = transform.position;
				saveData.Direction = Controller.Mover.MovementDirection;
				saveData.ControllerData = Controller.Save();
			}

			if (Trainer)
				Trainer.Variables.SaveTo(saveData.TrainerVariables, WorldLoader.SavedVariables);
		}

		#endregion
	}
}
