using PiRhoSoft.Utilities;
using UnityEngine;
using UnityEngine.Rendering;

namespace PiRhoSoft.MonsterRpg
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody2D))]
	public class Mover : MonoBehaviour
	{
		[Tooltip("The speed at which the mover moves")]
		[Range(0.0f, 10.0f)]
		[CustomLabel("Move Speed (units/second)")]
		public float MoveSpeed = 5.0f;

		[Tooltip("The collision layer this mover starts on")]
		[EnumButtons]
		public CollisionLayer MovementLayer = CollisionLayer.One; // Need to change layer for collisions when this changes

		[Tooltip("The direction this mover begins facing")]
		[EnumButtons]
		[SerializeField]
		private MovementDirection _movementDirection;
		public MovementDirection MovementDirection
		{
			get { return _movementDirection; }
			set { _movementDirection = value; UpdateLayer(); }
		}

		public float Speed => _velocity.sqrMagnitude;
		public Vector2 DirectionVector => Direction.GetVector(MovementDirection);

		protected Vector2 _velocity = Vector2.zero;

		private SortingGroup _sorting;
		private SpriteRenderer _renderer;
		private Rigidbody2D _body;

		void Awake()
		{
			if (MovementDirection == MovementDirection.None)
				FaceDirection(MovementDirection.Down);

			_sorting = GetComponent<SortingGroup>();
			_renderer = GetComponent<SpriteRenderer>();
			_body = GetComponent<Rigidbody2D>();

			gameObject.layer = LayerCollision.GetLayer(MovementLayer);
		}

		void LateUpdate()
		{
			var order = LayerSorting.GetSortingOrder(MovementLayer);

			if (_sorting)
				_sorting.sortingOrder = order;
			else if (_renderer)
				_renderer.sortingOrder = order;
		}

		public void WarpToPosition(Vector2 position, MovementDirection direction, CollisionLayer layer)
		{
			transform.position = position;

			FaceDirection(direction);

			if (layer != CollisionLayer.All)
				MovementLayer = layer;
		}

		public void FaceDirection(MovementDirection direction)
		{
			if (direction == MovementDirection.None)
				return;

			MovementDirection = direction;
		}

		public void UpdateMove(Vector2 move)
		{
			_velocity = move.normalized * MoveSpeed;

			UpdateMoveDirection(move);
			Move();
		}

		private void Move()
		{
			_body.MovePosition(_body.position + (_velocity * Time.fixedDeltaTime));
		}

		private void UpdateMoveDirection(Vector2 move)
		{
			if (move.x == 0.0f)
			{
				if (move.y != 0.0f)
					FaceDirection(move.y < 0.0f ? MovementDirection.Down : MovementDirection.Up);
			}
			else if (move.y == 0.0f)
			{
				if (move.x != 0.0f)
					FaceDirection(move.x < 0.0f ? MovementDirection.Left : MovementDirection.Right);
			}
			else
			{
				switch (MovementDirection)
				{
					case MovementDirection.Left: if (move.x > 0) FaceDirection(MovementDirection.Right); break;
					case MovementDirection.Right: if (move.x < 0) FaceDirection(MovementDirection.Left); break;
					case MovementDirection.Down: if (move.y > 0) FaceDirection(MovementDirection.Up); break;
					case MovementDirection.Up: if (move.y < 0) FaceDirection(MovementDirection.Down); break;
				}
			}
		}

		private void UpdateLayer()
		{
			gameObject.layer = LayerCollision.GetLayer(MovementLayer);
		}
	}
}