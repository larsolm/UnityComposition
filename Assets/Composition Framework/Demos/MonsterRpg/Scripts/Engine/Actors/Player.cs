using PiRhoSoft.Composition;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[Serializable]
	public class PlayerSaveData
	{
		public VariableStore PlayerVariables = new VariableStore();
		public VariableStore TrainerVariables = new VariableStore();
		public List<CreatureSaveData> Creatures = new List<CreatureSaveData>();
	}

	[DisallowMultipleComponent]
	[RequireComponent(typeof(Mover))]
	[RequireComponent(typeof(Trainer))]
	[RequireComponent(typeof(PlayerController))]
	[RequireComponent(typeof(Rigidbody2D))]
	[HelpURL(MonsterRpg.DocumentationUrl + "player")]
	[AddComponentMenu("Monster RPG/World/Player")]
	public class Player : VariableSetComponent
	{
		private const string _secondInstanceWarning = "(WWMSI) A second instance of WorldManager was created";
		private const string _missingSpeciesWarning = "(WPMS) The Species at the path {0} for the Player's Creature could not be found";

		public static Player Instance { get; private set; }

		public Mover Mover { get; private set; }
		public Trainer Trainer { get; private set; }
		public PlayerController Controller { get; private set; }
		public Interaction Interaction { get; private set; }

		private Rigidbody2D _body;
		private readonly RaycastHit2D[] _collisions = new RaycastHit2D[4];

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

				_body = GetComponent<Rigidbody2D>();
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

		public bool CanInteract => !IsInteracting && !Controller.IsFrozen;
		public bool IsInteracting => Interaction != null && Interaction.IsInteracting;

		public void Interact()
		{
			if (CanInteract)
				Interaction.Run();
		}

		public void ForceInteract(Interaction interaction)
		{
			Interaction = interaction;
			interaction.Run();
		}

		private void UpdateInteraction()
		{
			if (!IsInteracting)
				Interaction = GetInteraction();
		}

		private Interaction GetInteraction()
		{
			var filter = new ContactFilter2D()
			{
				useLayerMask = true,
				layerMask = 1 << gameObject.layer
			};

			var count = _body.Cast(Mover.DirectionVector, _collisions, 1.0f);

			for (var i = 0; i < count; i++)
			{
				var collision = _collisions[i];
				var interaction = collision.collider.GetComponent<Interaction>();
				if (interaction != null && interaction.CanInteract(Mover.MovementDirection, collision.distance))
					return interaction;
			}

			return null;
		}

		#endregion

		#region Persistence

		public void Load(PlayerSaveData saveData, string tag)
		{
			SchemaVariables.CopyFrom(saveData.PlayerVariables, tag);

			// If the save data has creatures or items, anything set on the actual Player needs to be cleared so it
			// isn't duplicated on every save. This means if a new creature or item is added to the Player in the
			// editor existing saves will not include it.

			if (saveData.Creatures.Count > 0) Trainer.Roster.Clear();
			
			Trainer.SchemaVariables.CopyFrom(saveData.TrainerVariables, tag);
			
			foreach (var creatureData in saveData.Creatures)
			{
				var creature = Creature.Create(creatureData, Trainer);
			
				if (creature != null)
					Trainer.Roster.AddCreature(creature);
				else
					Debug.LogWarningFormat(_missingSpeciesWarning, creatureData.SpeciesPath);
			}
		}

		public void Save(PlayerSaveData saveData, string tag)
		{
			SchemaVariables.CopyTo(saveData.PlayerVariables, tag);

			Trainer.SchemaVariables.CopyTo(saveData.TrainerVariables, tag);
			
			foreach (var creature in Trainer.Roster.Creatures)
				saveData.Creatures.Add(Creature.Save(creature, tag));
		}

		#endregion
	}
}
