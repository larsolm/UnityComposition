using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using UnityEngine;

namespace PiRhoSoft.CompositionExample
{
	[RequireComponent(typeof(Rigidbody2D))]
	[AddComponentMenu("PiRho Soft/Examples/Player")]
	public class Player : VariableSetComponent
	{
		[MappedVariable] public Camera Camera;
		[MappedVariable] public float Acceleration = 1.0f;

		private Rigidbody2D _body;
		private Collider2D[] _colliders = new Collider2D[6];

		void Awake()
		{
			_body = GetComponent<Rigidbody2D>();
		}

		void Update()
		{
			if (Camera)
				Camera.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.transform.position.z);

			if (InputHelper.GetWasButtonPressed("Submit"))
			{
				var count = Physics2D.OverlapCircle(_body.position, 1.0f, new ContactFilter2D { useTriggers = true }, _colliders);
				for (var i = 0; i < count; i++)
				{
					var interaction = _colliders[i].GetComponent<GraphTrigger>();
					if (interaction)
					{
						interaction.Run();
						break;
					}
				}
			}
		}

		void FixedUpdate()
		{
			var horizontal = InputHelper.GetAxis("Horizontal");
			var vertical = InputHelper.GetAxis("Vertical");

			_body.AddForce(new Vector2(horizontal * Acceleration, vertical * Acceleration), ForceMode2D.Impulse);
		}

		void LateUpdate()
		{
			if (Camera)
				Camera.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.transform.position.z);
		}
	}
}
