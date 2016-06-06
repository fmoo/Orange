using UnityEngine;
using System;
using System.Collections;

public class GameTimer : MonoBehaviour {
	/**
	 * Publicly visible TimeSinceStart.  This should probably be a read-only property.
	 */
	public float TimeSinceStart;

	/**
	 * Whether this timer has been paused by the user
	 */
	public bool Paused = false;
	
	/**
	 * Delegate type for callbacks.  Maybe there should be a parameters...?  *shrug*
	 */
	public delegate void Action();

	/**
	 * Whether this timer has been paused (internally) due to processing an Action callback
	 */
	private bool _ActionPaused = false;

	/**
	 * Internal class for encapsulating and ordering action callbacks based on when they
	 * should be started.
	 */
	private class TimedAction : IComparable<TimedAction> {
		public float StartTime;
		public bool Cancelled = false;
		private Action _Action;

		public TimedAction(GameTimer timer, Action action, float seconds) {
			StartTime = timer.TimeSinceStart + seconds;
			_Action = action;
		}

		internal void Call() {
			_Action ();
		}

		public void Cancel() {
			Cancelled = true;
			_Action = _Noop;
		}

		private void _Noop() { }

		public int CompareTo(TimedAction other) {
			return StartTime.CompareTo(other.StartTime);
		}
	}

	/**
	 * Heap of pending actions.
	 */
	private Heap<TimedAction> _Pending;

	// Use this for initialization
	void Start () {
		_Pending = new MinHeap<TimedAction> ();
	}

	TimedAction CallLater(float callIn, Action action) {
		TimedAction ta = new TimedAction (this, action, callIn);
		_Pending.Push (ta);
		return ta;
	}

	void UpdateFixed () {
		// If the timer is Paused, don't process any new Actions
		if (Paused || _ActionPaused) {
			return;
		}

		// Increment the time.
		TimeSinceStart += Time.fixedDeltaTime;

		// Now, see what needs to run
		while (_Pending.Count > 0 &&
		       _Pending.Peek().StartTime < TimeSinceStart) {
			TimedAction act = _Pending.Pop();

			// TODO: This code blocks while it calls Call(), and it really shouldn't.  Instead we should:
			// - Pause the timer
			// - Call this action in another thread
			// - Wait for that action to finish before un-pausing.
			if (!act.Cancelled) {
				act.Call ();
			}
		}
	}
}
