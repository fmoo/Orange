using UnityEngine;

public class MonoBehaviourSingleton<TSelfType> : MonoBehaviour where TSelfType : MonoBehaviour
{
    virtual protected void Start() {
        if (this != Instance) {
            Destroy(gameObject);
        }
    }

    void Awake() {
        DontDestroyOnLoad(transform.gameObject);
    }

    private static TSelfType m_Instance = null;
	public static TSelfType Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = (TSelfType)FindObjectOfType(typeof(TSelfType));
				if (m_Instance == null)
				{
					m_Instance = (new GameObject(typeof(TSelfType).Name)).AddComponent<TSelfType>();
				}
                if (!Application.isEditor) {
                    DontDestroyOnLoad(m_Instance.gameObject);
                }
            }
			return m_Instance;
		}
	}
}
