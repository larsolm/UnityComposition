﻿using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PiRhoSoft.CompositionDemo
{
	[RequireComponent(typeof(Rigidbody2D))]
	[AddComponentMenu("PiRho Soft/Demo/Demo Character")]
	public class DemoBehaviour : MonoBehaviour, IVariableStore
	{
		public float Speed = 5.0f;

		private Rigidbody2D _body;
		private Collider2D[] _colliders = new Collider2D[1];

		void Awake()
		{
			_body = GetComponent<Rigidbody2D>();
		}

		void Update()
		{
			if (InputHelper.GetWasButtonPressed(KeyCode.Space, "Submit"))
			{
				var count = _body.GetContacts(_colliders);

				if (count > 0)
				{
					var interaction = _colliders[0].GetComponent<DemoInteraction>();
					if (interaction != null)
						InstructionManager.Instance.RunInstruction(interaction.Caller, null, this);
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
