using PiRhoSoft.Composition;
using PiRhoSoft.Utilities;
using System;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	public enum AbilityUseLocation
	{
		World,
		Battle
	}

	[Serializable]
	public class AbilityVariableSource : VariableSource<Ability> { }

	[HelpURL(MonsterRpg.DocumentationUrl + "ability")]
	[CreateAssetMenu(menuName = "PiRho Soft/Ability", fileName = nameof(Ability), order = 202)]
	public class Ability : VariableSetAsset, IResource
	{
		private const string _missingEcosystemWarning = "(EAME) The Ability '{0}' has not been assigned an Ecosystem";

		[Tooltip("The display name for this ability")] public string Name;

		#region Moves

		public virtual Move CreateMove(Creature creature)
		{
			var move = CreateInstance<Move>();
			move.Ability = this;
			move.Setup(creature);
			return move;
		}

		#endregion

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
