using PiRhoSoft.MonsterRpgEngine;
using PiRhoSoft.UtilityEngine;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
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
		[Minimum(0.0f)]
		public float SpinDelay = 1.0f;

		private float _spinDelay = 0.0f;

		void Start()
		{
			Mover.OnDirectionChanged += OnDirectionChanged;
			Mover.DirectionDelayFrames = 1;
		}

		void FixedUpdate()
		{
			var horizontal = 0.0f;
			var vertical = 0.0f;

			_spinDelay += Time.fixedDeltaTime;

			if (_spinDelay >= SpinDelay)
			{
				var direction = GetDirection();
				Direction.GetMovement(direction, out horizontal, out vertical);
			}

			UpdateMover(horizontal, vertical);
		}

		private void OnDirectionChanged(MovementDirection previous, MovementDirection current)
		{
			_spinDelay = 0.0f;
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

		internal override void Load(string saveData)
		{
			float.TryParse(saveData, out _spinDelay);
		}

		internal override string Save()
		{
			return _spinDelay.ToString();
		}

		#endregion
	}
}
