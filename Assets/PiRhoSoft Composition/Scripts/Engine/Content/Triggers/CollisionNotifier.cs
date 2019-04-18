using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public interface ICollisionTrigger
	{
		void Enter();
		void Exit();
	}

	[HelpURL(Composition.DocumentationUrl + "collision-notifier")]
	[AddComponentMenu("PiRho Soft/Composition/Collision Notifier")]
	public class CollisionNotifier : MonoBehaviour
	{
		void OnCollisionEnter(Collision collision)
		{
			Enter(collision.collider);
		}

		void OnCollisionEnter2D(Collision2D collision)
		{
			Enter(collision.collider);
		}

		void OnTriggerEnter(Collider collider)
		{
			Enter(collider);
		}

		void OnTriggerEnter2D(Collider2D collider)
		{
			Enter(collider);
		}

		void OnCollisionExit(Collision collision)
		{
			Enter(collision.collider);
		}

		void OnCollisionExit2D(Collision2D collision)
		{
			Enter(collision.collider);
		}

		void OnTriggerExit(Collider collider)
		{
			Enter(collider);
		}

		void OnTriggerExit2D(Collider2D collider)
		{
			Enter(collider);
		}

		private void Enter(Component component)
		{
			var trigger = component.GetComponent<ICollisionTrigger>();
			trigger?.Enter();
		}

		private void Exit(Component component)
		{
			var trigger = component.GetComponent<ICollisionTrigger>();
			trigger?.Exit();
		}
	}
}
