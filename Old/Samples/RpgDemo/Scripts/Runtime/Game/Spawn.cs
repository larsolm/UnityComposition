using PiRhoSoft.Utilities;
using System;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[Serializable]
	public struct SpawnPoint
	{
		public static SpawnPoint Default = new SpawnPoint { Position = Vector2.zero, Direction = MovementDirection.Down };

		[HideInInspector]
		public Vector2 Position;

		[Tooltip("The direction the mover will face when spawning at this spawn point")]
		public MovementDirection Direction;
	}

	[AddComponentMenu("Monster RPG/Spawn")]
	public class Spawn : MonoBehaviour, ISerializationCallbackReceiver
	{
		[Inline]
		public SpawnPoint SpawnPoint = SpawnPoint.Default;

		public void OnAfterDeserialize()
		{
		}

		public void OnBeforeSerialize()
		{
			SpawnPoint.Position = transform.position;
		}
	}
}
