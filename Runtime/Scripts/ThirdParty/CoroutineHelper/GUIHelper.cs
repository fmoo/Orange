public static class GUIHelper
{
	private static int m_WinIDCounter = 2000;
	public static int GetFreeWindowID()
	{
		return m_WinIDCounter++;
	}
}
