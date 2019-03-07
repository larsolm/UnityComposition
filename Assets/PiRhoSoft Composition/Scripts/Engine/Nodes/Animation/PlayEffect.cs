using PiRhoSoft.UtilityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Animation/Play Effect", 100)]
	[HelpURL(Composition.DocumentationUrl + "play-effect")]
	public class PlayEffect : InstructionGraphNode
	{
		private const string _missingObjectWarning = "(CAPEMO) Unable to create object for {0}: the specified object could not be found";
		private const string _missingParentWarning = "(CAPEMP) Unable to assign parent object for {0}: the specified object '{1}' could not be found";
		private const string _missingNameWarning = "(CAPEMN) Unable to assign name for {0}: the specified name could not could not be found";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The GameObject to spawn")]
		[InlineDisplay(PropagateLabel = true)]
		public GameObjectVariableSource Effect = new GameObjectVariableSource();

		[Tooltip("The name of the spawned effect object")]
		[InlineDisplay(PropagateLabel = true)]
		public StringVariableSource EffectName = new StringVariableSource("Spawned Effect");

		[Tooltip("The parent object to attach the object to (optional) - if set, position will be in local space")]
		public VariableReference Parent = new VariableReference();

		[Tooltip("The position to spawn the object at - in local space if parent is set")]
		public Vector2 Position;

		[Tooltip("Whether to wait for the effect to finish before moving to Next")]
		public bool WaitForCompletion = false;

		[Tooltip("Whether to destroy the effect object when it is finished playing")]
		public bool DestroyOnComplete = true;

		public override Color NodeColor => Colors.Animation;

		private List<ICompletionNotifier> _animations = new List<ICompletionNotifier>(5);

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			Effect.GetInputs(inputs);
			EffectName.GetInputs(inputs);

			if (InstructionStore.IsInput(Parent))
				inputs.Add(VariableDefinition.Create<GameObject>(Parent.RootName));
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (Effect.TryGetValue(variables, this, out var effect))
			{
				GameObject parent = null;

				if (Parent.IsAssigned && !Parent.GetValue(variables).TryGetObject(out parent))
					Debug.LogWarningFormat(this, _missingParentWarning, Name, Parent);

				var spawned = parent ? Instantiate(effect, parent.transform.position + (Vector3)Position, Quaternion.identity, parent.transform) : Instantiate(effect, Position, Quaternion.identity);
				if (EffectName.TryGetValue(variables, this, out var objectName))
					spawned.name = objectName;
				else
					Debug.LogWarningFormat(this, _missingNameWarning, Name, Parent);

				if (WaitForCompletion)
					yield return WaitForFinish(spawned);
				else if (DestroyOnComplete)
					InstructionManager.Instance.StartCoroutine(WaitForFinish(spawned));
			}
			else
			{
				Debug.LogWarningFormat(this, _missingObjectWarning, Name);
			}

			graph.GoTo(Next, variables.This, nameof(Next));
		}

		private IEnumerator WaitForFinish(GameObject obj)
		{
			var particles = obj.GetComponentInChildren<ParticleSystem>(); // Don't need a cached list because ParticleSystem.IsAlive() will check all its children
			obj.GetComponentsInChildren(_animations);

			var isPlaying = true;
			while (isPlaying)
			{
				isPlaying = false;

				foreach (var animation in _animations)
				{
					if (!animation.IsComplete)
						isPlaying = true;
				}

				if (particles != null && particles.IsAlive(true))
					isPlaying = true;

				if (isPlaying)
					yield return null;
			}

			if (DestroyOnComplete)
				Destroy(obj);
		}
	}
}
