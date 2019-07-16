using PiRhoSoft.Utilities;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Player))]
	[HelpURL(MonsterRpg.DocumentationUrl + "player-controller")]
	[AddComponentMenu("PiRho Soft/Controllers/Player Controller")]
	public class PlayerController : Controller
	{
		[Tooltip("The input axis to use for horizontal movement")] public string HorizontalAxis = "Horizontal";
		[Tooltip("The input axis to use for vertical movement")] public string VerticalAxis = "Vertical";
		[Tooltip("The input button to use for interacting")] public string InteractButton = "Submit";

		protected float _horizontal = 0.0f;
		protected float _vertical = 0.0f;
		protected bool _interact = false;

		void Update()
		{
			if (IsFrozen)
				UpdateInput();
			else
				ClearInput();
		}

		void FixedUpdate()
		{
			ProcessInput();

			_interact = false;
		}

		private void UpdateInput()
		{
			_horizontal = InputHelper.GetAxis(HorizontalAxis);
			_vertical = InputHelper.GetAxis(VerticalAxis);
			_interact = _interact || InputHelper.GetWasButtonPressed(InteractButton); // _interact should stay true until the first FixedUpdate after the button is pressed
		}

		private void ClearInput()
		{
			_horizontal = 0.0f;
			_vertical = 0.0f;
			_interact = false;
		}

		private void ProcessInput()
		{
			UpdateMover(_horizontal, _vertical);

			if (_interact)
				Player.Instance.Interact();
		}
	}
}
