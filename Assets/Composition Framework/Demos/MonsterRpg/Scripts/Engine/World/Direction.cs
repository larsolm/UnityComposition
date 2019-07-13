using System;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg.Engine
{
	public enum MovementDirection
	{
		None,
		Left,
		Right,
		Down,
		Up
	}

	[Flags]
	public enum InteractionDirection
	{
		Any = 0,
		Left = 1 << 0,
		Right = 1 << 1,
		Down = 1 << 2,
		Up = 1 << 3
	}

	public static class Direction
	{
		public static bool Contains(InteractionDirection interactionDirection, MovementDirection movementDirection)
		{
			// If the movement direction that is sent in is None it means that the interaction is on the current tile, not the one the player is facing
			var direction = ToInteractionDirection(movementDirection);

			return interactionDirection == InteractionDirection.Any || (((int)direction & (int)interactionDirection) > 0);
		}

		public static MovementDirection Opposite(MovementDirection direction)
		{
			switch (direction)
			{
				case MovementDirection.Left: return MovementDirection.Right;
				case MovementDirection.Right: return MovementDirection.Left;
				case MovementDirection.Down: return MovementDirection.Up;
				case MovementDirection.Up: return MovementDirection.Down;
				default: return MovementDirection.None;
			}
		}

		private static InteractionDirection ToInteractionDirection(MovementDirection direction)
		{
			switch (direction)
			{
				case MovementDirection.Left: return InteractionDirection.Left;
				case MovementDirection.Right: return InteractionDirection.Right;
				case MovementDirection.Down: return InteractionDirection.Down;
				case MovementDirection.Up: return InteractionDirection.Up;
				default: return InteractionDirection.Any;
			}
		}

		public static Vector2 GetVector(MovementDirection direction)
		{
			switch (direction)
			{
				case MovementDirection.Left: return Vector2.left;
				case MovementDirection.Right: return Vector2.right;
				case MovementDirection.Down: return Vector2.down;
				case MovementDirection.Up: return Vector2.up;
				default: return Vector2.zero;
			}
		}

		public static MovementDirection GetDirection(Vector2 direction)
		{
			if (direction == Vector2.zero)
				return MovementDirection.None;

			var axis = Math.Sign(Math.Abs(direction.x) - Math.Abs(direction.y));

			if (axis < 0) return direction.y > 0 ? MovementDirection.Up : MovementDirection.Down;
			else return direction.x > 0 ? MovementDirection.Right : MovementDirection.Left;
		}
	}
}
