using UnityEngine;
using System.Collections;

/**
 * Helper Behavior for IFinishable.
 */
public abstract class FinishableBehavior : GameBehavior, IFinishable {
	private IFinishables.OnFinish finishCallbacks;
	private bool callbacksCalled = false;

	public void Finish() {
		if (finishCallbacks != null) {
			finishCallbacks(this);
			clearFinishCallbacks();
		}
	}

	abstract public bool IsFinished();

	void Update() {
		if (IsFinished()) {
			Finish();
		}
	}

	protected void clearFinishCallbacks() {
		foreach (var cb in finishCallbacks.GetInvocationList()) {
			finishCallbacks -= (IFinishables.OnFinish)cb;
		}
	}

	public void AddFinishCallback(IFinishables.OnFinish cb) {
		// If we're adding a callback to a finishable that's already finished,
		// just invoke the callback now, otherwise, add it for later.
		if (IsFinished()) {
			cb(this);
			return;
		}

		finishCallbacks += cb;
	}

	public void RemoveFinishCallback(IFinishables.OnFinish cb) {
		finishCallbacks -= cb;
	}

	public IEnumerator Gen() {
		return new IFinishables.FinishableWaiter(this);
	}
}
	

/**
 * Interface for things that will finish later, and that you want to be able
 * to force to "finish".
 * 
 * You should probably be extending FinishableBehavior, instead, so you only
 * have to define IsFinished().
 * 
 * NOTE: If you define Update in your child class, you MUST do either:
 * - Call ((FinishableBehavior)this).Update() somewhere
 * - Call if (IsFinished()) { Finish() }
 */
public interface IFinishable {
	/**
	 * Adds a callback to be invoked when this action finishes 
	 */
	void AddFinishCallback(IFinishables.OnFinish cb);

	/**
	 * Adds a callback to be invoked when this action finishes 
	 */
	void RemoveFinishCallback(IFinishables.OnFinish cb);

	/**
	 * Force the action to finish.  Invokes
	 */
	void Finish();

	/**
	 * Returns true of the action is finished
	 */
	bool IsFinished();

	/**
	 * Generally, `return new IFinishables.FinishableAwaiter(this);`
	 */
	IEnumerator Gen();
}


/**
 * Helper class for IFinishable.
 * 
 * Since we can't define delegates or local classes on an interface, do this here.
 */
public static class IFinishables {
	public delegate void OnFinish(IFinishable f);

	public class FinishableWaiter : CustomYieldInstruction {
		private IFinishable finishable;
		private bool f_keepWaiting = true;

		override public bool keepWaiting { get {
				return f_keepWaiting;
			}}

		public FinishableWaiter(IFinishable f) {
			f.AddFinishCallback((IFinishable f2) => {f_keepWaiting = false;});
		}
	}
}
