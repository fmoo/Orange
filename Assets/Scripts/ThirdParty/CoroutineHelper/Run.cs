using UnityEngine;
using System;
using System.Collections;
 
public class Run
{
	public bool isDone;
	public bool abort;
	private IEnumerator action;
	public System.Action onGUIaction = null;
 
	#region Run.EachFrame
	public static Run EachFrame(System.Action aAction)
	{
		var tmp = new Run();
		tmp.action = _RunEachFrame(tmp, aAction);
		tmp.Start();
		return tmp;
	}
	private static IEnumerator _RunEachFrame(Run aRun, System.Action aAction)
	{
		aRun.isDone = false;
		while (true)
		{
			if (!aRun.abort && aAction != null)
				aAction();
			else
				break;
			yield return null;
		}
		aRun.isDone = true;
	}
	#endregion Run.EachFrame
 
	#region Run.Every
	public static Run Every(float aInitialDelay, float aDelay, System.Action aAction)
	{
		var tmp = new Run();
		tmp.action = _RunEvery(tmp,aInitialDelay, aDelay, aAction);
		tmp.Start();
		return tmp;
	}
	private static IEnumerator _RunEvery(Run aRun, float aInitialDelay, float aSeconds, System.Action aAction)
	{
		aRun.isDone = false;
		if (aInitialDelay > 0f)
			yield return new WaitForSeconds(aInitialDelay);
		else
		{
			int FrameCount = Mathf.RoundToInt(-aInitialDelay);
			for (int i = 0; i < FrameCount; i++)
				yield return null;
		}
		while (true)
		{
			if (!aRun.abort && aAction != null)
				aAction();
			else
				break;
			if (aSeconds > 0)
				yield return new WaitForSeconds(aSeconds);
			else
			{
				int FrameCount = Mathf.Max(1,Mathf.RoundToInt(-aSeconds));
				for (int i = 0; i < FrameCount; i++)
					yield return null;
			}
		}
		aRun.isDone = true;
	}
	#endregion Run.Every
 
	#region Run.After
	public static Run After(float aDelay, System.Action aAction)
	{
		var tmp = new Run();
		tmp.action = _RunAfter(tmp, aDelay, aAction);
		tmp.Start();
		return tmp;
	}
	private static IEnumerator _RunAfter(Run aRun, float aDelay, System.Action aAction)
	{
		aRun.isDone = false;
		yield return new WaitForSeconds(aDelay);
		if (!aRun.abort && aAction != null)
			aAction();
		aRun.isDone = true;
	}
	#endregion Run.After
 
	#region Run.Lerp
	public static Run Lerp(float aDuration, System.Action<float> aAction)
	{
		var tmp = new Run();
		tmp.action = _RunLerp(tmp, aDuration, aAction);
		tmp.Start();
		return tmp;
	}
	private static IEnumerator _RunLerp(Run aRun, float aDuration, System.Action<float> aAction)
	{
		aRun.isDone = false;
		float t = 0f;
		while (t < 1.0f)
		{
			t = Mathf.Clamp01(t + Time.deltaTime / aDuration);
			if (!aRun.abort && aAction != null)
				aAction(t);
			yield return null;
		}
		aRun.isDone = true;
 
	}
	#endregion Run.Lerp
 
	#region Run.OnDelegate
	public static Run OnDelegate(SimpleEvent aDelegate, System.Action aAction)
	{
		var tmp = new Run();
		tmp.action = _RunOnDelegate(tmp, aDelegate, aAction);
		tmp.Start();
		return tmp;
	}
 
	private static IEnumerator _RunOnDelegate(Run aRun, SimpleEvent aDelegate, System.Action aAction)
	{
		aRun.isDone = false;
		System.Action action = ()=>{
			aAction();
		};
		aDelegate.Add(action);
		while (!aRun.abort && aAction != null)
		{
			yield return null;
		}
		aDelegate.Remove(action);
		aRun.isDone = true;
	}
	#endregion Run.OnDelegate
 
	#region Run.Coroutine
	public static Run Coroutine(IEnumerator aCoroutine)
	{
		var tmp = new Run();
		tmp.action = _Coroutine(tmp, aCoroutine);
		tmp.Start();
		return tmp;
	}
 
	private static IEnumerator _Coroutine(Run aRun, IEnumerator aCoroutine)
	{
		yield return CoroutineHelper.Instance.StartCoroutine(aCoroutine);
		aRun.isDone = true;
	}
	#endregion Run.Coroutine
 
	public static Run OnGUI(float aDuration, System.Action aAction)
	{
		var tmp = new Run();
		tmp.onGUIaction = aAction;
		if (aDuration > 0.0f)
			tmp.action = _RunAfter(tmp, aDuration, null);
		else
			tmp.action = null;
		tmp.Start();
		CoroutineHelper.Instance.Add(tmp);
		return tmp;
	}
 
	public class CTempWindow
	{
		public Run inst;
		public Rect pos;
		public string title;
		public int winID = GUIHelper.GetFreeWindowID();
		public void Close(){ inst.Abort();}
	}
 
	public static CTempWindow CreateGUIWindow(Rect aPos, string aTitle, System.Action<CTempWindow> aAction)
	{
		CTempWindow tmp = new CTempWindow();
		tmp.pos = aPos;
		tmp.title = aTitle;
		tmp.inst = OnGUI(0,()=>{
			tmp.pos = GUI.Window(tmp.winID, tmp.pos, (id)=>{
				aAction(tmp);
			},tmp.title);
		});
		return tmp;
	}
 
 
	private void Start()
	{
		if (action != null)
		CoroutineHelper.Instance.StartCoroutine(action);
	}
 
	public Coroutine WaitFor
	{
		get
		{
			return CoroutineHelper.Instance.StartCoroutine(_WaitFor(null));
		}
	}
	public IEnumerator _WaitFor(System.Action aOnDone)
	{
		while(!isDone)
			yield return null;
		if (aOnDone != null)
			aOnDone();
	}
	public void Abort()
	{
		abort = true;
	}
	public Run ExecuteWhenDone(System.Action aAction)
	{
		var tmp = new Run();
		tmp.action = _WaitFor(aAction);
		tmp.Start();
		return tmp;
	}
}
