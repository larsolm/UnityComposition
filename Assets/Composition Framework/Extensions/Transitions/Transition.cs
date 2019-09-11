using PiRhoSoft.Utilities;
using System;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	public enum TransitionPhase
	{
		Out,
		Obscure,
		In
	}

	[Serializable] public class TransitionList : SerializedList<Transition> { }
	[Serializable] public class TransitionVariableSource : VariableSource<Transition> { }

	public abstract class Transition : ScriptableObject
	{
		private const string _missingShaderError = "(CTMS) Failed to load Transition '{0}': the shader has not been set";

		[Tooltip("The shader to use to render the transition")] public Shader Shader;
		[Tooltip("The duration of the transition")] public float Duration = 1.0f;

		protected Material Material { get; private set; }

		protected virtual void OnEnable()
		{
			if (Shader)
				Material = new Material(Shader);
			else if (ApplicationHelper.IsPlaying)
				Debug.LogErrorFormat(this, _missingShaderError, name);
		}

		protected virtual void OnDisable()
		{
			if (Material != null)
				DestroyImmediate(Material);
		}

		public virtual void Begin(TransitionPhase phase) { }
		public virtual void Process(float time, TransitionPhase phase) { }
		public virtual void End() { }

		public virtual void Render(RenderTexture source, RenderTexture destination)
		{
			Update();

			if (Material != null)
				Graphics.Blit(source, destination, Material);
			else
				Graphics.Blit(source, destination);
		}

		protected virtual void Update() { }
	}
}
