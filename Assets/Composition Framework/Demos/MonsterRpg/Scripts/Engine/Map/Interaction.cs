using PiRhoSoft.Composition;
using PiRhoSoft.Utilities;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[AddComponentMenu("PiRho Soft/Monster RPG/Interaction")]
	public class Interaction : GraphTrigger
	{
		private const string _missingOccupierWarning = "(WIMO) The Interaction '{0}' needs either a Mover or StaticCollider";

		[Tooltip("The directions that the player can be in order to interact with this object")]
		[EnumButtons]
		public InteractionDirection Directions;

		[Tooltip("The distance from which this can be interacted from")]
		[Minimum(0.0f)]
		public float Distance = 1.0f;

		public bool IsInteracting => Graph.Graph && Graph.IsRunning;

		public bool CanInteract(MovementDirection direction, float distance)
		{
			return !IsInteracting && Direction.Contains(Directions, Direction.Opposite(direction)) && distance <= Distance;
		}
	}
}
