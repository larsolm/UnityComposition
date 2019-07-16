using PiRhoSoft.Utilities;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[AddComponentMenu("PiRho Soft/Controllers/Spinner Controller")]
	[HelpURL(MonsterRpg.DocumentationUrl + "spinner-controller")]
	public class SpinnerController : Controller
	{
		public enum SpinType
		{
			Clockwise,
			CounterClockwise,
			Random
		}

		[Tooltip("The type of behaviour of the spinner")]
		[EnumButtons]
		public SpinType Type = SpinType.Clockwise;

		[Tooltip("The delay before the controller changes direction")]
		public Vector2 SpinDelay = Vector2.up;

		private float _delay;
		private float _range;

		void FixedUpdate()
		{
			_delay += Time.fixedDeltaTime;

			if (_delay >= _range)
			{
				_delay = 0.0f;
				_range = Random.Range(SpinDelay.x, SpinDelay.y);

				var direction = GetDirection();
				Mover.FaceDirection(direction);
			}
		}

		private MovementDirection GetDirection()
		{
			switch (Type)
			{
				case SpinType.Clockwise:
				{
					if (Mover.MovementDirection == MovementDirection.Left) return MovementDirection.Up;
					if (Mover.MovementDirection == MovementDirection.Right) return MovementDirection.Down;
					if (Mover.MovementDirection == MovementDirection.Up) return MovementDirection.Right;
					if (Mover.MovementDirection == MovementDirection.Down) return MovementDirection.Left;
					break;
				}
				case SpinType.CounterClockwise:
				{
					if (Mover.MovementDirection == MovementDirection.Left) return MovementDirection.Down;
					if (Mover.MovementDirection == MovementDirection.Right) return MovementDirection.Up;
					if (Mover.MovementDirection == MovementDirection.Up) return MovementDirection.Left;
					if (Mover.MovementDirection == MovementDirection.Down) return MovementDirection.Right;
					break;
				}
				case SpinType.Random:
				{
					return (MovementDirection)Random.Range(1, 5);
				}
			}

			return MovementDirection.None;
		}

		#region Persistence

		public override void Load(string saveData)
		{
			var elements = saveData.Split('|');

			if (elements.Length == 2)
			{
				if (float.TryParse(elements[0], out var delay)) _delay = delay;
				if (float.TryParse(elements[1], out var range)) _range = range;
			}
		}

		public override string Save()
		{
			return string.Format("{0}|{1}", _delay, _range);
		}

		#endregion
	}
}
