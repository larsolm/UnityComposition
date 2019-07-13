using PiRhoSoft.Composition.Engine;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg.Engine
{
	[HelpURL(MonsterRpg.DocumentationUrl + "world")]
	[CreateAssetMenu(fileName = nameof(World), menuName = "PiRho Soft/World", order = 300)]
	public class World : ScriptableObject
	{
		[Tooltip("The graph to run to load the world")]
		public GraphCaller LoadGraph = new GraphCaller();

		[Tooltip("The zones in this world")]
		public ZoneList Zones = new ZoneList();

		public Zone GetZone(string name)
		{
			foreach (var zone in Zones)
			{
				if (zone.name == name)
					return zone;
			}

			return null;
		}

		public Zone GetZone(int index)
		{
			foreach (var zone in Zones)
			{
				if (zone.Scene.Index == index)
					return zone;
			}

			return null;
		}
	}
}
