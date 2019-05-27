using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.UtilityEngine
{
	public struct ButtonState
	{
		public bool Pressed;
		public bool Held;
		public bool Released;

		public ButtonState(bool pressed, bool held, bool released)
		{
			Pressed = pressed;
			Held = held;
			Released = released;
		}

		public ButtonState(string button)
		{
			Pressed = Input.GetButtonDown(button);
			Held = Input.GetButton(button);
			Released = Input.GetButtonUp(button);
		}

		public ButtonState(KeyCode key)
		{
			Pressed = Input.GetKeyDown(key);
			Held = Input.GetKey(key);
			Released = Input.GetKeyUp(key);
		}

		public ButtonState(int touch)
		{
			if (Input.touchCount > touch)
			{
				var phase = Input.GetTouch(touch).phase;

				Pressed = phase == TouchPhase.Began;
				Held = phase != TouchPhase.Canceled && phase != TouchPhase.Ended;
				Released = phase == TouchPhase.Canceled || phase == TouchPhase.Ended;
			}
			else
			{
				Pressed = false;
				Held = false;
				Released = false;
			}
		}
	}

	public static class InputHelper
	{
		private static Dictionary<string, ButtonData> _manualButtons = new Dictionary<string, ButtonData>();
		private static Dictionary<string, AxisData> _manualAxes = new Dictionary<string, AxisData>();
		private static Dictionary<string, AxisValue> _previousAxes = new Dictionary<string, AxisValue>();

		public static void LateUpdate()
		{
			UpdateManualButtons();
			UpdatePreviousAxisValues();
		}

		#region Manual Input

		public static void SetButton(string button, bool down)
		{
			var data = GetManualButtonData(button);
			data.Pressed = !data.Held && down;
			data.Released = data.Held && !down;
			data.Held = down;
		}

		public static void RemoveButton(string button)
		{
			_manualButtons.Remove(button);
		}

		public static void SetAxis(string axis, float value)
		{
			var data = GetManualAxisData(axis);
			data.Value = value;
		}

		public static void RemoveAxis(string axis)
		{
			_manualAxes.Remove(axis);
		}

		private static ButtonData GetManualButtonData(string button)
		{
			if (!_manualButtons.TryGetValue(button, out ButtonData data))
			{
				data = new ButtonData();
				_manualButtons.Add(button, data);
			}

			return data;
		}

		private static AxisData GetManualAxisData(string axis)
		{
			if (!_manualAxes.TryGetValue(axis, out AxisData data))
			{
				data = new AxisData();
				_manualAxes.Add(axis, data);
			}

			return data;
		}

		private static void UpdateManualButtons()
		{
			foreach (var button in _manualButtons)
			{
				button.Value.Pressed = false;
				button.Value.Released = false;
			}
		}

		#endregion

		#region Key State

		public static ButtonState GetKeyState(KeyCode key)
		{
			return new ButtonState(key);
		}

		public static bool GetKeyDown(KeyCode key)
		{
			return Input.GetKey(key);
		}

		public static bool GetWasKeyPressed(KeyCode key)
		{
			return Input.GetKeyDown(key);
		}

		public static bool GetWasKeyReleased(KeyCode key)
		{
			return Input.GetKeyUp(key);
		}

		#endregion

		#region Pointer State

		public static bool GetIsPointerDown()
		{
			return GetIsTouching(0) || Input.GetMouseButton(0);
		}

		public static bool GetWasPointerPressed()
		{
			return GetTouchStarted(0) || Input.GetMouseButtonDown(0);
		}

		public static bool GetWasPointerReleased()
		{
			return GetTouchEnded(0) || Input.GetMouseButtonUp(0);
		}

		#endregion

		#region Touch State

		public static ButtonState GetTouchState(int touch)
		{
			return new ButtonState(touch);
		}

		public static TouchPhase? GetTouchPhase(int touch)
		{
			return Input.touchCount > touch ? Input.GetTouch(touch).phase : (TouchPhase?)null;
		}

		public static bool GetIsTouching(int touch)
		{
			var phase = GetTouchPhase(touch);
			return phase.HasValue && (phase.Value == TouchPhase.Began || phase.Value == TouchPhase.Stationary || phase.Value == TouchPhase.Moved);
		}

		public static bool GetTouchStarted(int touch)
		{
			var phase = GetTouchPhase(touch);
			return phase.HasValue && (phase.Value == TouchPhase.Began);
		}

		public static bool GetTouchEnded(int touch)
		{
			var phase = GetTouchPhase(touch);
			return phase.HasValue && (phase.Value == TouchPhase.Ended || phase.Value == TouchPhase.Canceled);
		}

		#endregion

		#region Button State

		public static bool IsButtonAvailable(string button)
		{
			if (_manualButtons.ContainsKey(button))
				return true;

			try
			{
				Input.GetButton(button);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static ButtonState GetButtonState(string button)
		{
			if (_manualButtons.TryGetValue(button, out ButtonData state))
				return new ButtonState(state.Pressed, state.Held, state.Released);

			try { return new ButtonState(button); }
			catch { return new ButtonState(false, false, false); }
		}

		public static bool GetButtonDown(string button)
		{
			var down = false;

			if (_manualButtons.TryGetValue(button, out ButtonData state))
				down = state.Held;

			try { return down || Input.GetButton(button); }
			catch { return false; }
		}
		
		public static bool GetWasButtonPressed(string button)
		{
			var pressed = false;

			if (_manualButtons.TryGetValue(button, out ButtonData state))
				pressed = state.Pressed;

			try { return pressed || Input.GetButtonDown(button); }
			catch { return false; }
		}
		
		public static bool GetWasButtonReleased(string button)
		{
			var released = false;

			if (_manualButtons.TryGetValue(button, out ButtonData state))
				released = state.Released;

			try { return released || Input.GetButtonUp(button); }
			catch { return false; }
		}

		#endregion

		#region Axis State

		public static float GetAxis(string axis)
		{
			return GetCurrentAxisValue(axis);
		}

		public static ButtonState GetAxisState(string axis, float magnitude)
		{
			var previousValue = GetPreviousAxisValue(axis);
			var currentValue = GetCurrentAxisValue(axis);

			var wasPressed = IsAxisPressed(previousValue, magnitude);
			var isPressed = IsAxisPressed(currentValue, magnitude);

			return new ButtonState
			{
				Pressed = !wasPressed && isPressed,
				Held = isPressed,
				Released = wasPressed && !isPressed
			};
		}

		public static bool GetAxisDown(string axis, float magnitude)
		{
			var currentValue = GetCurrentAxisValue(axis);

			return IsAxisPressed(currentValue, magnitude);
		}

		public static bool GetWasAxisPressed(string axis, float magnitude)
		{
			var previousValue = GetPreviousAxisValue(axis);
			var currentValue = GetCurrentAxisValue(axis);

			return !IsAxisPressed(previousValue, magnitude) && IsAxisPressed(currentValue, magnitude);
		}

		public static bool GetWasAxisReleased(string axis, float magnitude)
		{
			var previousValue = GetPreviousAxisValue(axis);
			var currentValue = GetCurrentAxisValue(axis);

			return IsAxisPressed(previousValue, magnitude) && !IsAxisPressed(currentValue, magnitude);
		}

		private static float GetCurrentAxisValue(string axis)
		{
			var value = Input.GetAxisRaw(axis);

			if (_manualAxes.TryGetValue(axis, out AxisData data) && Math.Abs(data.Value) > Math.Abs(value))
				return data.Value;

			return value;
		}

		private static float GetPreviousAxisValue(string axis)
		{
			if (!_previousAxes.TryGetValue(axis, out AxisValue value))
			{
				value = new AxisValue { Value = 0.0f };
				_previousAxes.Add(axis, value);
			}

			return value.Value;
		}

		private static void UpdatePreviousAxisValues()
		{
			foreach (var axis in _previousAxes)
				axis.Value.Value = GetCurrentAxisValue(axis.Key);
		}

		private static bool IsAxisPressed(float value, float magnitude)
		{
			return (magnitude < 0.0f && value < magnitude) || (magnitude > 0.0f && value > magnitude);
		}

		#endregion

		#region Support Classes

		private class ButtonData
		{
			public bool Pressed;
			public bool Held;
			public bool Released;
		}

		private class AxisData
		{
			public float Value;
		}

		private class AxisValue
		{
			public float Value;
		}

		#endregion
	}
}
