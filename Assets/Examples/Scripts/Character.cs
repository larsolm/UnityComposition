using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionExample
{
	[RequireComponent(typeof(Rigidbody2D))]
	[AddComponentMenu("PiRho Soft/Examples/Character")]
	public class Character : MonoBehaviour, IVariableStore
	{
		[AssetPopup] [ChangeTrigger(nameof(SetupSchema))] public VariableSchema Schema;

		public Camera Camera;
		public WorldManager World;
		public float Acceleration = 1.0f;

		public VariableList Variables;
		public MappedVariableStore Store = new MappedVariableStore();
		public InstructionContext Context = new InstructionContext();

		private Rigidbody2D _body;
		private Collider2D[] _colliders = new Collider2D[6];

		void Awake()
		{
			_body = GetComponent<Rigidbody2D>();
		}

		void OnEnable()
		{
			Context.Stores.Add(nameof(Character), this);
			Context.Stores.Add(nameof(World), World);
			SetupSchema();
		}

		void OnDisable()
		{
			Context.Stores.Clear();
			_body.velocity = Vector2.zero;
		}

		private void SetupSchema()
		{
			Store.Setup(this, Schema, Variables);
		}

		void Update()
		{
			if (Camera)
				Camera.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.transform.position.z);

			if (InputHelper.GetWasButtonPressed(KeyCode.Space, "Submit"))
			{
				var count = Physics2D.OverlapCircle(_body.position, 1.0f, new ContactFilter2D { useTriggers = true }, _colliders);
				for (var i = 0; i < count; i++)
				{
					var interaction = _colliders[i].GetComponent<Interaction>();
					if (interaction)
					{
						InstructionManager.Instance.RunInstruction(interaction.OnInteract, Context, interaction);
						break;
					}
				}
			}

			if (Camera)
				Camera.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.transform.position.z);
		}

		void FixedUpdate()
		{
			var horizontal = InputHelper.GetAxis("Horizontal");
			var vertical = InputHelper.GetAxis("Vertical");

			_body.AddForce(new Vector2(horizontal * Acceleration, vertical * Acceleration), ForceMode2D.Impulse);
		}

		void OnTriggerEnter2D(Collider2D collision)
		{
			var interaction = collision.GetComponent<Interaction>();
			if (interaction)
				InstructionManager.Instance.RunInstruction(interaction.OnEnter, Context, interaction);
		}

		void OnTriggerExit2D(Collider2D collision)
		{
			var interaction = collision.GetComponent<Interaction>();
			if (interaction)
				InstructionManager.Instance.RunInstruction(interaction.OnLeave, Context, interaction);
		}

		#region IVariableStore Implementation

			public VariableValue GetVariable(string name) => Store.GetVariable(name);
		public SetVariableResult SetVariable(string name, VariableValue value) => Store.SetVariable(name, value);
		public IEnumerable<string> GetVariableNames() => Store.GetVariableNames();

		#endregion
	}
}
