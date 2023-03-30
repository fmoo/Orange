using System;

public class SimpleEvent
{
	private System.Action m_Delegate = ()=>{};
	public void Add(System.Action aDelegate)
	{
		m_Delegate += aDelegate;
	}
	public void Remove(System.Action aDelegate)
	{
		m_Delegate -= aDelegate;
	}
	public void Run()
	{
		m_Delegate();
	}
}
