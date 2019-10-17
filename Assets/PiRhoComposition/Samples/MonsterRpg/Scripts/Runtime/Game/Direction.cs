using System;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	public enum MovementDirection
	{
		Left,
		Right,
		Down,
		Up
	}

	public static class Direction
	{
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
			var axis = Math.Sign(Math.Abs(direction.x) - Math.Abs(direction.y));

			if (axis <= 0) return direction.y > 0 ? MovementDirection.Up : MovementDirection.Down;
			else return direction.x >= 0 ? MovementDirection.Right : MovementDirection.Left;
		}
	}
}
