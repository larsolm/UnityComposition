using PiRhoSoft.Composition.Engine;
using PiRhoSoft.Utilities.Engine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg.Engine
{
	public interface IInteractable
	{
		bool IsInteracting();
		bool CanInteract(MovementDirection direction);
		void Interact();
	}

	[HelpURL(MonsterRpg.DocumentationUrl + "interaction")]
	[AddComponentMenu("Monster RPG/World/Interaction")]
	public class Interaction : MonoBehaviour, IInteractable
	{
		private const string _missingOccupierWarning = "(WIMO) The Interaction '{0}' needs either a Mover or StaticCollider";

		[Tooltip("The directions that the player can be in order to interact with this object")]
		[EnumButtons]
		public InteractionDirection Directions;

		[Tooltip("The graph to run when the player interacts with this object")]
		public GraphCaller Graph = new GraphCaller();

		protected virtual void Awake()
		{
			if (GetComponent<Mover>() == null && GetComponent<StaticCollider>() == null)
				Debug.LogWarningFormat(this, _missingOccupierWarning, gameObject.name);
		}

		public virtual bool IsInteracting()
		{
			return Graph != null && Graph.IsRunning;
		}

		public virtual bool CanInteract(MovementDirection direction)
		{
			return Direction.Contains(Directions, Direction.Opposite(direction)) && !IsInteracting();
		}

		public virtual void Interact()
		{
			if (Graph != null && Graph.Graph != null)
				WorldManager.Instance.StartCoroutine(DoInteract());
		}

		private IEnumerator DoInteract()
		{
			CompositionManager.Instance.RunGraph(Graph, VariableValue.CreateReference(this));

			while (Graph.IsRunning)
				yield return null;
		}
	}
}
