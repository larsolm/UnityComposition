using PiRhoSoft.Composition.Engine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg.Engine
{
	[Serializable]
	public class PlayerSaveData
	{
		public string Name = string.Empty;
		public VariableSet PlayerVariables = new VariableSet();
		public VariableSet TrainerVariables = new VariableSet();
		public List<CreatureSaveData> Creatures = new List<CreatureSaveData>();
	}

	[DisallowMultipleComponent]
	[RequireComponent(typeof(Mover))]
	[RequireComponent(typeof(Trainer))]
	[RequireComponent(typeof(PlayerController))]
	[HelpURL(MonsterRpg.DocumentationUrl + "player")]
	[AddComponentMenu("Monster RPG/World/Player")]
	public class Player : VariableSetComponent
	{
		private const string _secondInstanceWarning = "(WWMSI) A second instance of WorldManager was created";
		private const string _missingSpeciesWarning = "(WPMS) The Species at the path {0} for the Player's Creature could not be found";

		public static Player Instance { get; private set; }

		[Tooltip("The name of the player")]
		public string Name = "Player";

		public Mover Mover { get; private set; }
		public Trainer Trainer { get; private set; }
		public PlayerController Controller { get; private set; }
		public IInteractable Interaction { get; private set; }

		public Player()
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
				Mover = GetComponent<Mover>();
				Trainer = GetComponent<Trainer>();
				Controller = GetComponent<PlayerController>();
			}
		}

		void OnDestroy()
		{
			if (Instance == this)
				Instance = null;
		}

		void FixedUpdate()
		{
			UpdateInteraction();
		}

		#region Interaction

		public bool CanInteract => Interaction != null && !Interaction.IsInteracting() && Interaction.CanInteract(_isCurrentTile ? MovementDirection.None : Mover.MovementDirection) && Mover.CanInteract && !Controller.IsFrozen;
		public bool IsInteracting => Interaction != null && Interaction.IsInteracting();

		private bool _isCurrentTile;

		public void Interact()
		{
			if (CanInteract)
				Interaction.Interact();
		}

		public void ForceInteract(Interaction interaction)
		{
			Interaction = interaction;
			interaction.Interact();
		}

		protected void UpdateInteraction()
		{
			if (!IsInteracting)
			{
				_isCurrentTile = false;
				Interaction = WorldManager.Instance.GetInteraction(Mover.CurrentGridPosition + Mover.DirectionVector);

				if (Interaction == null)
				{
					Interaction = WorldManager.Instance.GetInteraction(Mover.CurrentGridPosition);
					_isCurrentTile = true;
				}
			}
		}

		#endregion

		#region Persistence

		public void Load(VariableSet header, PlayerSaveData saveData)
		{
			Variables.LoadFrom(header, WorldLoader.HeaderVariables);
			Variables.LoadFrom(saveData.PlayerVariables, WorldLoader.SavedVariables);

			// If the save data has creatures or items, anything set on the actual Player needs to be cleared so it
			// isn't duplicated on every save. This means if a new creature or item is added to the Player in the
			// editor existing saves will not include it.

			if (saveData.Creatures.Count > 0) Trainer.Roster.Clear();
			if (!string.IsNullOrEmpty(saveData.Name)) Name = saveData.Name;
			
			Trainer.Variables.LoadFrom(saveData.TrainerVariables, WorldLoader.SavedVariables);
			
			foreach (var creatureData in saveData.Creatures)
			{
				var creature = Creature.Create(creatureData, Trainer);
			
				if (creature != null)
					Trainer.Roster.AddCreature(creature);
				else
					Debug.LogWarningFormat(_missingSpeciesWarning, creatureData.SpeciesPath);
			}
		}

		public void Save(VariableSet header, PlayerSaveData saveData)
		{
			Variables.SaveTo(header, WorldLoader.HeaderVariables);
			Variables.SaveTo(saveData.PlayerVariables, WorldLoader.SavedVariables);

			saveData.Name = Name;
			Trainer.Variables.SaveTo(saveData.TrainerVariables, WorldLoader.SavedVariables);
			
			foreach (var creature in Trainer.Roster.Creatures)
				saveData.Creatures.Add(Creature.Save(creature));
		}

		#endregion
	}
}
