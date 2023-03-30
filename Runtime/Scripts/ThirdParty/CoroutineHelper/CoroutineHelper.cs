using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public class CoroutineHelper : MonoBehaviourSingleton<CoroutineHelper>
{
	private List<Run> m_OnGUIObjects = new List<Run>();
	public int ScheduledOnGUIItems
	{
		get {return m_OnGUIObjects.Count;}
	}
	public Run Add(Run aRun)
	{
		if (aRun != null)
			m_OnGUIObjects.Add(aRun);
		return aRun;
	}
	void OnGUI()
	{
		for(int i = 0; i < m_OnGUIObjects.Count; i++)
		{
			Run R = m_OnGUIObjects[i];
			if (!R.abort && !R.isDone && R.onGUIaction != null)
				R.onGUIaction();
			else
				R.isDone = true;
		}
	}
	void Update()
	{
		for(int i = m_OnGUIObjects.Count-1; i >= 0; i--)
		{
			if (m_OnGUIObjects[i].isDone)
				m_OnGUIObjects.RemoveAt(i);
		}
	}
}
