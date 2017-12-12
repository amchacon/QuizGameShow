using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScreen : MonoBehaviour
{
	public AudioClip menuTheme;
	public AudioClip buttonClickFx;

	void Start()
	{
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != "MainMenu")
            SoundController.Instance.PlayMusic(menuTheme);
        else
            SoundController.Instance.StopAllSounds();
	}

	public void SelectCategory(int catId)
	{
		SoundController.Instance.PlaySoundFX (buttonClickFx);
		DataController.Instance.LoadRemoteGameData (catId, LoadReadyScreenSceneCallback);
	}

	private void LoadReadyScreenSceneCallback()
	{
		ScreenManager.Instance.LoadScene ("ReadyScreen");
	}
		
	public void StartGame()
	{
		SoundController.Instance.PlaySoundFX (buttonClickFx);
		ScreenManager.Instance.LoadScene ("Game");
	}

	public void ReturnToMenu()
	{		
		ScreenManager.Instance.LoadScene("MainMenu");
	}
}