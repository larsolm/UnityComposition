using PiRhoSoft.Composition;
using PiRhoSoft.Utilities;
using System;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[Serializable]
	public struct SpawnPoint
	{
		public static SpawnPoint Default = new SpawnPoint { Direction = MovementDirection.Down, Layer = CollisionLayer.One };

		[HideInInspector]
		public string Name;

		[HideInInspector]
		public Vector2 Position;

		[Tooltip("The transition to play when this spawn point is spawned at (if null, DefaultSpawnTransition on the World will be used)")]
		public Transition Transition;

		[Tooltip("The direction the mover will face when spawning at this spawn point")]
		public MovementDirection Direction;

		[Tooltip("The collision layer to set the mover to when they spawn at this spawn point")]
		public CollisionLayer Layer;
	}

	[AddComponentMenu("PiRho Soft/Monster RPG/Spawn")]
	public class Spawn : MonoBehaviour, ISerializationCallbackReceiver
	{
		[Inline]
		public SpawnPoint SpawnPoint = SpawnPoint.Default;

		public void OnAfterDeserialize()
		{
		}

		public void OnBeforeSerialize()
		{
			SpawnPoint.Name = name;
			SpawnPoint.Position = transform.position;
		}
	}
}
