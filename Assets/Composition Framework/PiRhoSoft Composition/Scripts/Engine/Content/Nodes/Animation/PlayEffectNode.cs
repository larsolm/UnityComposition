using PiRhoSoft.PargonUtilities.Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Animation/Play Effect", 100)]
	[HelpURL(Composition.DocumentationUrl + "play-effect-node")]
	public class PlayEffectNode : InstructionGraphNode
	{
		public enum ObjectPositioning
		{
			Absolute,
			Relative,
			Child
		}

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The GameObject to spawn")]
		[Inline]
		public GameObjectVariableSource Effect = new GameObjectVariableSource();

		[Tooltip("The name of the spawned effect object")]
		[Inline]
		public StringVariableSource EffectName = new StringVariableSource("Spawned Effect");

		[Tooltip("A variable reference to assign the created effect to so that it can be referenced later")]
		public VariableReference EffectVariable = new VariableReference();

		[Tooltip("How to create and position the effect, with an exact position, relative to another object, or as a child of another object")]
		[EnumButtons]
		public ObjectPositioning Positioning = ObjectPositioning.Absolute;

		[Tooltip("The object to position the effect relative to")]
		[Conditional(nameof(Positioning), (int)ObjectPositioning.Relative)]
		public VariableReference Object = new VariableReference();

		[Tooltip("The parent object to make the effect a child of")]
		[Conditional(nameof(Positioning), (int)ObjectPositioning.Child)]
		public VariableReference Parent = new VariableReference();

		[Tooltip("The position to spawn the object at")]
		[Inline]
		public Vector3VariableSource Position = new Vector3VariableSource();

		[Tooltip("The rotation to spawn the object at")]
		[Inline]
		public Vector3VariableSource Rotation = new Vector3VariableSource();

		[Tooltip("Whether to wait for the effect to finish before moving to Next")]
		public bool WaitForCompletion = false;

		[Tooltip("Whether to destroy the effect object when it is finished playing")]
		public bool DestroyOnComplete = true;

		public override Color NodeColor => Colors.Animation;

		private List<ICompletionNotifier> _animations = new List<ICompletionNotifier>(5);

		public override void GetInputs(IList<VariableDefinition> inputs)
		{
			Effect.GetInputs(inputs);
			EffectName.GetInputs(inputs);
			Position.GetInputs(inputs);
			Rotation.GetInputs(inputs);

			if (Positioning == ObjectPositioning.Child && InstructionStore.IsInput(Parent))
				inputs.Add(new VariableDefinition { Name = Parent.RootName, Definition = ValueDefinition.Create<GameObject>() });

			if (Positioning == ObjectPositioning.Relative && InstructionStore.IsInput(Object))
				inputs.Add(new VariableDefinition { Name = Object.RootName, Definition = ValueDefinition.Create<GameObject>() });
		}

		public override void GetOutputs(IList<VariableDefinition> outputs)
		{
			if (InstructionStore.IsOutput(EffectVariable))
				outputs.Add(new VariableDefinition { Name = EffectVariable.RootName, Definition = ValueDefinition.Create<GameObject>() });
		}

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (ResolveObject(variables, Effect, out var effect))
			{
				GameObject spawned = null;

				Resolve(variables, Position, out var position);
				Resolve(variables, Rotation, out var rotation);

				if (Positioning == ObjectPositioning.Absolute)
				{
					spawned = Instantiate(effect, position, Quaternion.Euler(rotation));
				}
				else if (Positioning == ObjectPositioning.Relative)
				{
					if (ResolveObject(variables, Object, out GameObject obj))
						spawned = Instantiate(effect, obj.transform.position + position, Quaternion.Euler(rotation));
				}
				else if (Positioning == ObjectPositioning.Child)
				{
					if (ResolveObject(variables, Parent, out GameObject parent))
						spawned = Instantiate(effect, parent.transform.position + position, Quaternion.Euler(rotation), parent.transform);
				}

				if (spawned)
				{
					if (Resolve(variables, EffectName, out var effectName) && !string.IsNullOrEmpty(effectName))
						spawned.name = effectName;

					if (EffectVariable.IsAssigned)
						Assign(variables, EffectVariable, VariableValue.Create(spawned));
				}

				if (WaitForCompletion)
					yield return WaitForFinish(spawned);
				else if (DestroyOnComplete)
					CompositionManager.Instance.StartCoroutine(WaitForFinish(spawned));
			}

			graph.GoTo(Next, nameof(Next));
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
