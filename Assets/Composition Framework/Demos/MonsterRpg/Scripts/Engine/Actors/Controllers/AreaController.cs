using PiRhoSoft.Utilities;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[HelpURL(MonsterRpg.DocumentationUrl + "area-controller")]
	[AddComponentMenu("PiRho Soft/Controllers/Area Controller")]
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

		private float _horizontal = 0.0f;
		private float _vertical = 0.0f;

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
						var movement = Direction.GetVector(direction);

						_horizontal = movement.x;
						_vertical = movement.y;
					}
					else
					{
						_horizontal = 0.0f;
						_vertical = 0.0f;
					}
				}
			}

			UpdateMover(_horizontal, _vertical);
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

		#region Persistence

		public override void Load(string saveData)
		{
			var elements = saveData.Split('|');

			if (elements.Length == 7)
			{
				if (int.TryParse(elements[0], out var x)) _area.x = x;
				if (int.TryParse(elements[1], out var y)) _area.y = y;
				if (int.TryParse(elements[2], out var width)) _area.width = width;
				if (int.TryParse(elements[3], out var height)) _area.height = height;
				if (float.TryParse(elements[4], out var delay)) _delay = delay;
				if (float.TryParse(elements[5], out var range)) _range = range;
				if (bool.TryParse(elements[6], out var moving)) _moving = moving;
			}
		}

		public override string Save()
		{
			return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}", _area.x, _area.y, _area.width, _area.height, _delay, _range, _moving);
		}

		#endregion
	}
}
