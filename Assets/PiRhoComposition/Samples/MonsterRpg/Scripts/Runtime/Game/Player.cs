using PiRhoSoft.Composition;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PiRhoSoft.MonsterRpg
{
	[Serializable]
	public class SaveData
	{
		public static string GetSavePath(string filename) => $"{Application.persistentDataPath}/{filename}.json";

		public Vector2 Position = SpawnPoint.Default.Position;
		public MovementDirection Direction = SpawnPoint.Default.Direction;
		public SerializedVariableDictionary PlayerVariables = new SerializedVariableDictionary();
		public SerializedVariableDictionary TrainerVariables = new SerializedVariableDictionary();
		public List<CreatureSaveData> Creatures = new List<CreatureSaveData>();
	}

	[DisallowMultipleComponent]
	[RequireComponent(typeof(Mover))]
	[RequireComponent(typeof(Trainer))]
	[RequireComponent(typeof(PlayerInput))]
	[RequireComponent(typeof(Rigidbody2D))]
	[AddComponentMenu("Monster RPG/Player")]
	public class Player : SchemaVariableComponent
	{
		private const string _missingSpeciesWarning = "(MRPGPMS) The Species at the path {0} for the Player's Creature could not be found";

		private Mover _mover;
		private Trainer _trainer;
		private Rigidbody2D _body;
		private Interaction _interaction;
		private Animator _animator;

		private Vector2 _move = Vector2.zero;

		private readonly RaycastHit2D[] _collisions = new RaycastHit2D[4];

		void Awake()
		{
			_mover = GetComponent<Mover>();
			_trainer = GetComponent<Trainer>();
			_animator = GetComponent<Animator>();
			_body = GetComponent<Rigidbody2D>();
		}

		void FixedUpdate()
		{
			_mover.UpdateMove(_move);
			UpdateInteraction();
		}

		#region Interaction

		public bool CanInteract => !IsInteracting && !_mover.IsFrozen;
		public bool IsInteracting => _interaction && _interaction.Graph.IsRunning;

		public void Interact()
		{
			if (CanInteract)
				_interaction.Run();
		}

		public void ForceInteract(Interaction interaction)
		{
			_interaction = interaction;
			interaction.Run();
		}

		private void UpdateInteraction()
		{
			if (!IsInteracting)
				_interaction = GetInteraction();
		}

		private Interaction GetInteraction()
		{
			var filter = new ContactFilter2D()
			{
				useLayerMask = true,
				layerMask = 1 << gameObject.layer
			};

			var direction = Direction.GetVector(_mover.MovementDirection);
			var count = _body.Cast(direction, _collisions, 1.0f);

			for (var i = 0; i < count; i++)
			{
				var collision = _collisions[i];
				var interaction = collision.collider.GetComponent<Interaction>();
				if (interaction != null)
					return interaction;
			}

			return null;
		}

		#endregion

		private void OnMove(InputValue value)
		{
			_move = value.Get<Vector2>();
		}

		private void OnInteract(InputValue value)
		{
			if (!_mover.IsFrozen)
			{
				var input = value.Get<float>();
				if (input > 0.0f)
					Interact();
			}
		}

		#region Persistence

		public void Load(SaveData saveData)
		{
			_mover.WarpToPosition(saveData.Position, saveData.Direction);

			//Variables.CopyFrom(saveData.PlayerVariables, tag);

			// If the save data has creatures or items, anything set on the actual Player needs to be cleared so it
			// isn't duplicated on every save. This means if a new creature or item is added to the Player in the
			// editor existing saves will not include it.

			if (saveData.Creatures.Count > 0) _trainer.Roster.Clear();
			
			//_trainer.Variables.CopyFrom(saveData.TrainerVariables, tag);
			
			foreach (var creatureData in saveData.Creatures)
			{
				var creature = Creature.Create(creatureData, _trainer);
			
				if (creature != null)
					_trainer.Roster.AddCreature(creature);
				else
					Debug.LogWarningFormat(_missingSpeciesWarning, creatureData.SpeciesPath);
			}
		}

		public void Save(SaveData saveData)
		{
			saveData.Position = transform.position;
			saveData.Direction = _mover.MovementDirection;

			for (var i = 0; i < Variables.VariableCount; i++)
			{
				var name = Variables.VariableNames[i];
				var variable = Variables.GetVariable(i);
				saveData.PlayerVariables.AddVariable(name, variable);
			}
			
			foreach (var creature in _trainer.Roster.Creatures)
				saveData.Creatures.Add(Creature.Save(creature, tag));
		}

		#endregion
	}
}
