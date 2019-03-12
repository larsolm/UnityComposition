using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEngine;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionExample
{
	[RequireComponent(typeof(Rigidbody2D))]
	[AddComponentMenu("PiRho Soft/Examples/Player")]
	public class Player : MonoBehaviour, IVariableStore, IReloadable
	{
		[AssetPopup] [ReloadOnChange] public VariableSchema Schema;

		public Camera Camera;
		public WorldManager World;
		public float Acceleration = 1.0f;

		public InstructionCaller OnStart = new InstructionCaller();

		public VariableList Variables;
		public MappedVariableStore Store = new MappedVariableStore();
		public InstructionContext Context = new InstructionContext();


		private Rigidbody2D _body;
		private Collider2D[] _colliders = new Collider2D[6];

		public void OnEnable()
		{
			Context.Stores.Add(nameof(Player), this);
			Context.Stores.Add(nameof(World), World);
			Store.Setup(this, Schema, Variables);
		}

		public void OnDisable()
		{
			Context.Stores.Clear();

			if (_body)
				_body.velocity = Vector2.zero;
		}

		void Awake()
		{
			_body = GetComponent<Rigidbody2D>();
		}

		void Start()
		{
			if (OnStart.Instruction)
				InstructionManager.Instance.RunInstruction(OnStart, Context, this);
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

		void OnCollisionEnter2D(Collision2D collision)
		{
			var interaction = collision.collider.GetComponent<Interaction>();
			if (interaction)
				InstructionManager.Instance.RunInstruction(interaction.OnEnter, Context, interaction);
		}

		void OnCollisionExit2D(Collision2D collision)
		{
			var interaction = collision.collider.GetComponent<Interaction>();
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
