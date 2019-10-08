using PiRhoSoft.Utilities;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[AddComponentMenu("PiRho Soft/Monster RPG/Area Controller")]
	public class AreaController : Controller
	{
		[Tooltip("The distance to the left this mover can travel from its starting point")] [Minimum(0.0f)] public float LeftDistance = 0;
		[Tooltip("The distance to the right this mover can travel from its starting point")] [Minimum(0.0f)] public float RightDistance = 0;
		[Tooltip("The distance upward this mover can travel from its starting point")] [Minimum(0.0f)] public float UpDistance = 0;
		[Tooltip("The distance downward this mover can travel from its starting point")] [Minimum(0.0f)] public float DownDistance = 0;

		[Tooltip("The delay before this character moves to a new tile (>= 0)")] [Minimum(0.0f)]
		public Vector2 MovementDelay = Vector2.up;
		public Vector2 MovementTime = Vector2.up;

		private Rect _area;
		private float _delay;
		private float _range;
		private bool _moving;

		private Vector2 _move = Vector2.zero;

		void Start()
		{
			_area = new Rect(transform.position.x - LeftDistance, transform.position.y - DownDistance, RightDistance + LeftDistance, UpDistance + DownDistance);
			_delay = 0.0f;
			_moving = false;
			_range = GetNextRange();
		}

		void FixedUpdate()
		{
			if (!IsFrozen)
			{
				_delay += Time.fixedDeltaTime;

				if (_delay >= _range)
				{
					_moving = !_moving;
					_delay = 0.0f;
					_range = GetNextRange();

					if (_moving)
					{
						var direction = GetDirection();
						_move = Direction.GetVector(direction);
					}
					else
					{
						_move = Vector2.zero;
					}
				}
			}

			UpdateMover(_move);
		}

		public float GetNextRange()
		{
			return _moving ? Random.Range(MovementTime.x, MovementTime.y) : Random.Range(MovementDelay.x, MovementDelay.y);
		}

		private MovementDirection GetDirection()
		{
			var direction = (MovementDirection)Random.Range(1, 5);

			if (direction == MovementDirection.Left && transform.position.x > _area.xMin) return direction;
			if (direction == MovementDirection.Right && transform.position.x < _area.xMax) return direction;
			if (direction == MovementDirection.Up && transform.position.y < _area.yMax - 1) return direction;
			if (direction == MovementDirection.Down && transform.position.y > _area.yMin) return direction;

			return MovementDirection.None;
		}
	}
}
