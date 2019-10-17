using PiRhoSoft.Composition;
using PiRhoSoft.Utilities;
using System;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[Serializable]
	public struct MountPoint
	{
		public float X;
		public float Y;
		public float Rotation;
	}

	[Serializable] public class MountPointDictionary : SerializedDictionary<string, MountPoint> { }

	[CreateAssetMenu(menuName = "Monster RPG/Species", fileName = nameof(Species), order = 201)]
	public class Species : SchemaVariableAsset
	{
		[Tooltip("The icon for this species")] public Sprite Icon;
		[Tooltip("The animations for this species")] public AnimatorOverrideController Animations;
		[Tooltip("The skills available to Creatures of this Species")] [List(EmptyLabel = "The Species has no Skills")] public SkillList Skills = new SkillList();
		[Tooltip("The mount point locations on the Species")] [Dictionary(EmptyLabel = "The Species has no mount points")] public MountPointDictionary MountPoints = new MountPointDictionary();
		
		public virtual Creature CreateCreature(Trainer trainer)
		{
			var creature = CreateInstance<Creature>();
			creature.Species = this;
			creature.name = name;
			creature.Setup(trainer);
			return creature;
		}
	}
}
