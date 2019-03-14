using PiRhoSoft.UtilityEngine;
using System;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
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
		private const string _missingShaderError = "(CTMS) Failed to load Transition: shader {0} not found";

		[Tooltip("The duration of the transition")] public float Duration = 1.0f;

		protected Material Material { get; private set; }

		protected void SetShader(string name)
		{
			var shader = Shader.Find(name);

			if (shader != null)
				Material = new Material(shader);
			else if (ApplicationHelper.IsPlaying)
				Debug.LogErrorFormat(this, _missingShaderError, name);
		}

		void OnDisable()
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
