using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PiRhoSoft.CompositionDemo
{
	[RequireComponent(typeof(Rigidbody2D))]
	[AddComponentMenu("PiRho Soft/Examples/Character")]
	public class Character : MonoBehaviour, IVariableStore
	{
		public float Speed = 5.0f;

		private Rigidbody2D _body;
		private Collider2D[] _colliders = new Collider2D[2];

		void Awake()
		{
			_body = GetComponent<Rigidbody2D>();
		}

		void Update()
		{
			if (InputHelper.GetWasButtonPressed(KeyCode.Space, "Submit"))
			{
				var count = Physics2D.OverlapCircle(_body.position, 1.0f, new ContactFilter2D { useTriggers = false }, _colliders);
				for (var i = 0; i < count; i++)
				{
					var interaction = _colliders[i].GetComponent<Interaction>();
					if (interaction)
					{
						InstructionManager.Instance.RunInstruction(interaction.Caller, null, this);
						break;
					}
				}
			}
		}

		void FixedUpdate()
		{
			var velocity = _body.velocity;
			var horizontal = InputHelper.GetAxis("Horizontal");
			var vertical = InputHelper.GetAxis("Vertical");

			velocity.x = horizontal * Speed;
			velocity.y = vertical * Speed;

			_body.velocity = velocity;
		}

		void OnTriggerEnter2D(Collider2D collision)
		{
			var interaction = collision.GetComponent<Interaction>();
			if (interaction)
				InstructionManager.Instance.RunInstruction(interaction.Caller, null, this);
		}

		public VariableValue GetVariable(string name)
		{
			return VariableValue.Empty;
		}

		public SetVariableResult SetVariable(string name, VariableValue value)
		{
			return SetVariableResult.NotFound;
		}

		public IEnumerable<string> GetVariableNames()
		{
			return Enumerable.Empty<string>();
		}
	}
}
