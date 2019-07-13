using PiRhoSoft.UtilityEngine;
using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[HelpURL(MonsterRpg.DocumentationUrl + "area-controller")]
	[AddComponentMenu("PiRho Soft/Controllers/Area Controller")]
	public class AreaController : Controller
	{
		[Tooltip("The distance to the left this mover can travel from its starting point (>= 0)")] [Minimum(0)] public int LeftDistance = 0;
		[Tooltip("The distance to the right this mover can travel from its starting point (>= 0)")] [Minimum(0)] public int RightDistance = 0;
		[Tooltip("The distance upward this mover can travel from its starting point (>= 0)")] [Minimum(0)] public int UpDistance = 0;
		[Tooltip("The distance downward this mover can travel from its starting point (>= 0)")] [Minimum(0)] public int DownDistance = 0;

		[Tooltip("The delay before this character moves to a new tile (>= 0)")]
		[Minimum(0.0f)]
		public float MovementDelay = 1.0f;

		private RectInt _area;
		private float _movementDelay = 0.0f;

		void Start()
		{
			Mover.OnTileChanged += OnTileChanged;
			Mover.DirectionDelayFrames = 0;
			_area = new RectInt(Mover.CurrentGridPosition.x - LeftDistance, Mover.CurrentGridPosition.y - DownDistance, 1 + RightDistance + LeftDistance, 1 + UpDistance + DownDistance);
		}

		void FixedUpdate()
		{
			var horizontal = 0.0f;
			var vertical = 0.0f;

			if (!IsFrozen)
			{
				if (!Mover.Moving)
				{
					_movementDelay += Time.fixedDeltaTime;

					if (_movementDelay >= MovementDelay)
					{
						var direction = GetDirection();
						Direction.GetMovement(direction, out horizontal, out vertical);
					}
				}
			}

			UpdateMover(horizontal, vertical);
		}

		private void OnTileChanged(Vector2Int previous, Vector2Int current)
		{
			_movementDelay = 0.0f;
		}

		private MovementDirection GetDirection()
		{
			var direction = (MovementDirection)Random.Range(1, 5);

			if (direction == MovementDirection.Left && Mover.CurrentGridPosition.x > _area.xMin) return direction;
			if (direction == MovementDirection.Right && Mover.CurrentGridPosition.x < _area.xMax - 1) return direction;
			if (direction == MovementDirection.Up && Mover.CurrentGridPosition.y < _area.yMax - 1) return direction;
			if (direction == MovementDirection.Down && Mover.CurrentGridPosition.y > _area.yMin) return direction;

			return MovementDirection.None;
		}

		#region Persistence

		internal override void Load(string saveData)
		{
			var elements = saveData.Split('|');

			if (elements.Length == 5)
			{
				if (int.TryParse(elements[0], out int x)) _area.x = x;
				if (int.TryParse(elements[1], out int y)) _area.y = y;
				if (int.TryParse(elements[2], out int w)) _area.width = w;
				if (int.TryParse(elements[3], out int h)) _area.height = h;
				if (float.TryParse(elements[4], out float d)) _movementDelay = d;
			}
		}

		internal override string Save()
		{
			return string.Format("{0}|{1}|{2}|{3}|{4}", _area.x, _area.y, _area.width, _area.height, _movementDelay);
		}

		#endregion
	}
}
