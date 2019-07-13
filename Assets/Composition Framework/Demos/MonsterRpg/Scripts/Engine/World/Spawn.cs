using PiRhoSoft.Composition.Engine;
using PiRhoSoft.Utilities.Engine;
using System;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg.Engine
{
	[Serializable]
	public struct SpawnPoint
	{
		public static SpawnPoint Default = new SpawnPoint { Name = null, Direction = MovementDirection.Down, Layer = CollisionLayer.One };

		public Vector2 Position;

		[Tooltip("The name of this spawn point to be referenced by a zone trigger")]
		public string Name;

		[Tooltip("The transition to play when this spawn point is spawned at (if null, DefaultSpawnTransition on the World will be used)")]
		public Transition Transition;

		[Tooltip("The direction the mover will face when spawning at this spawn point")]
		public MovementDirection Direction;

		[Tooltip("The collision layer to set the mover to when they spawn at this spawn point")]
		public CollisionLayer Layer;

		public bool IsNamed => !string.IsNullOrEmpty(Name);
	}

	[AddComponentMenu("Monster RPG/World/Spawn")]
	public class Spawn : MonoBehaviour
	{
		[Inline]
		public SpawnPoint SpawnPoint = SpawnPoint.Default;

		void Awake()
		{
			var zone = WorldManager.Instance.GetZone(this);
			zone.SpawnPoints.Add(SpawnPoint.Name, SpawnPoint);
		}
	}
}
