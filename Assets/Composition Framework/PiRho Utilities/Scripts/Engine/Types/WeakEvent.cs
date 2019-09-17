using System;

namespace PiRhoSoft.Utilities
{
	public class WeakEvent
	{
		private event Action _event;

		public void Subscribe<T>(T target, Action<T> callback) where T : class
		{
			var reference = new WeakReference(target, false);
			var handler = (Action)null;

			handler = () =>
			{
				var t = reference.Target as T;

				if (t != null)
					callback(t);
				else
					_event -= handler;
			};

			_event += handler;
		}

		public void Trigger()
		{
			_event?.Invoke();
		}
	}
}
