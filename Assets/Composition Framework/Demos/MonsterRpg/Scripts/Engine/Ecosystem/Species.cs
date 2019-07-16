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

	[HelpURL(MonsterRpg.DocumentationUrl + "species")]
	[CreateAssetMenu(menuName = "PiRho Soft/Species", fileName = nameof(Species), order = 201)]
	public class Species : VariableSetAsset, IResource
	{
		[Tooltip("The name of this species")] public string Name;
		[Tooltip("The icon for this species")] public Sprite Icon;
		[Tooltip("The animations for this species")] public AnimatorOverrideController Animations;
		[Tooltip("The skills available to Creatures of this Species")] [List(EmptyLabel = "The Species has no Skills")] public SkillList Skills = new SkillList();
		[Tooltip("The mount point locations on the Species")] [Dictionary(EmptyText = "The Species has no mount points")] public MountPointDictionary MountPoints = new MountPointDictionary();
		
		public virtual Creature CreateCreature(ITrainer trainer)
		{
			var creature = CreateInstance<Creature>();
			creature.Species = this;
			creature.name = name;
			creature.Name = Name;
			creature.Setup(trainer);
			return creature;
		}

		#region IResource Implementation

		public string Path => _path;

		[SerializeField, HideInInspector]
		private string _path;

		public void OnBeforeSerialize()
		{
			_path = Resource.GetResourcePath(this);
		}

		public void OnAfterDeserialize()
		{
		}

		#endregion
	}
}
