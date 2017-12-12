using UnityEngine;

public class Config : MonoBehaviour {

	public static Config Instance;
	public GlobalConfig globalConfig;

	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else if (Instance != this)
			Destroy (gameObject);
		DontDestroyOnLoad (gameObject);

		Instance.globalConfig.gameMode = GameMode.ONLINE;
		ConnectionController.Instance.Initialize ();
        FileManagement.Instance.Initialize();
	}
}
