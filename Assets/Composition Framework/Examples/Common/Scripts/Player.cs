using PiRhoSoft.Composition;
using PiRhoSoft.Utilities;
using System.Linq;
using UnityEngine;

namespace PiRhoSoft.CompositionExample
{
	[RequireComponent(typeof(Rigidbody2D))]
	[AddComponentMenu("PiRho Soft/Examples/Player")]
	public class Player : VariableSetComponent
	{
		[MappedVariable] public float Acceleration = 1.0f;

		private Rigidbody2D _body;
		private Collider2D[] _colliders = new Collider2D[6];

		void Awake()
		{
			_body = GetComponent<Rigidbody2D>();
		}

		void Update()
		{
			if (InputHelper.GetWasButtonPressed("Submit"))
			{
				var count = Physics2D.OverlapCircle(_body.position, 1.0f, new ContactFilter2D { useTriggers = true }, _colliders);
				for (var i = 0; i < count; i++)
				{
					var interaction = _colliders[i].GetComponent<GraphTrigger>();
					if (interaction && interaction.gameObject != gameObject)
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

#if UNITY_EDITOR
		[UnityEditor.MenuItem("Tools/Add Examples to Build")]
		static void AddExamplesToBuild()
		{
			var scenes = new string[]
			{
				"Assets/Composition Framework/Examples/Loader/Loader.unity",
				"Assets/Composition Framework/Examples/Battle/Scenes/Battle.unity",
				"Assets/Composition Framework/Examples/BoardGame/Board.unity",
				"Assets/Composition Framework/Examples/Calculator/Calculator.unity",
				"Assets/Composition Framework/Examples/CardGame/Scenes/Addiction.unity",
				"Assets/Composition Framework/Examples/Loot/Scenes/LootLevel.unity",
				"Assets/Composition Framework/Examples/Loot/Scenes/LootMenu.unity",
				"Assets/Composition Framework/Examples/Maze/Scenes/Maze1.unity",
				"Assets/Composition Framework/Examples/Maze/Scenes/Maze2.unity",
				"Assets/Composition Framework/Examples/Maze/Scenes/Maze3.unity",
				"Assets/Composition Framework/Examples/Maze/Scenes/MazeUi.unity",
				"Assets/Composition Framework/Examples/Shop/Scenes/Shop.unity"
			};

			var newScenes = scenes.Where(path => !UnityEditor.EditorBuildSettings.scenes.Select(scene => scene.path).Contains(path)).Select(path => new UnityEditor.EditorBuildSettingsScene(path, true));
			UnityEditor.EditorBuildSettings.scenes = UnityEditor.EditorBuildSettings.scenes.Concat(newScenes).ToArray();
		}
#endif
	}
}
