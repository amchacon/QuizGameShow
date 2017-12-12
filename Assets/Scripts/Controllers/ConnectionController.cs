using UnityEngine;
using System.Collections;
using System;
using System.Text;
using UnityEngine.UI;

public class ConnectionController : Singleton<ConnectionController> 
{
	private string urlBase;
	private Action<int> loadHighScoreCallback;
	private Action<GameData> loadGameDataCallback;
	private Slider loadingBar;
	private Text loadingText;

	public void Initialize()
	{
        loadingBar = GameObject.FindGameObjectWithTag("LoadingBar").GetComponent<Slider>();
        loadingText = GameObject.FindGameObjectWithTag("LoadingBar").GetComponentInChildren<Text>();
        loadingBar.gameObject.SetActive(false);

        if (Config.Instance.globalConfig.urlActive == URLActive.LOCAL)
        {
            urlBase = Config.Instance.globalConfig.localURL;
            Debug.Log("Configurando acesso para dados locais em: " + urlBase);
        }
			
		else if (Config.Instance.globalConfig.urlActive == URLActive.REMOTE)
        {
            urlBase = Config.Instance.globalConfig.remoteURL;
            Debug.Log("Configurando acesso para dados remotos em: " + urlBase);
        }
    }

	#region HighScore
	/// <summary>
	/// Loads the high score
	/// </summary>
	public void LoadHighScore(Action<int> callback)
	{
		loadHighScoreCallback = callback;
		WWWForm highscoreForm = new WWWForm ();
		highscoreForm.AddField ("type", "gethighscore");
		WWW www = new WWW (urlBase, highscoreForm);
		StartCoroutine (LoadHighScoreFromServer(www));
	}

	/// <summary>
	/// Loads the high score from server
	/// </summary>
	IEnumerator LoadHighScoreFromServer(WWW www)
	{
		int hs;
		yield return www;
		if (String.IsNullOrEmpty(www.error))
		{
			string wsReturn = www.text.Trim ();
			if (wsReturn != "0")
			{
				hs = Convert.ToInt32 (wsReturn);
			} 
			else
			{
				hs = 0;
			}
		} 
		else
		{
			Debug.LogWarning ("Failed to load High Score from DB!");
			Config.Instance.globalConfig.gameMode = GameMode.OFFLINE;
			Debug.LogWarning ("Putting game in mode Offline. HighScore will not be persisted!");
			Debug.LogError (www.error);
			hs = -1;
		}
		loadHighScoreCallback (hs);
	}

	/// <summary>
	/// Saves the high score
	/// </summary>
	/// <param name="hs">int highscore</param>
	public void SaveHighScore(int hs)
	{
		WWWForm highscoreForm = new WWWForm ();
		highscoreForm.AddField ("type", "sethighscore");
		highscoreForm.AddField ("points", hs);
		WWW www = new WWW (urlBase, highscoreForm);
		StartCoroutine (SaveHighScoreOnServer(www));
	}

	/// <summary>
	/// Saves the high score on server
	/// </summary>
	IEnumerator SaveHighScoreOnServer(WWW www)
	{
		yield return www;
		if (String.IsNullOrEmpty(www.error))
		{
			string wsReturn = www.text.Trim ();
			if (wsReturn == "1")
			{
				Debug.Log ("High Score saved!");
			} 
			else
			{
				Debug.LogWarning ("Failed to register High Score on DB!");
			}
		} 
		else
		{
			Debug.LogWarning ("Failed to register High Score on DB!");
			Debug.LogError (www.error);
		}
	}
	#endregion

	#region Questions
	/// <summary>
	/// Loads the game data
	/// </summary>
	public void LoadGameData (int catId, Action<GameData> callback)
	{
        Debug.Log("Carregando dados do jogo...");
		loadGameDataCallback = callback;
		WWWForm gamedataForm = new WWWForm ();
		gamedataForm.AddField ("type", "getquestions");
		gamedataForm.AddField ("catId", catId);
		WWW www = new WWW (urlBase, gamedataForm);
		StartCoroutine (LoadGameDataFromServer(www));
	}

	/// <summary>
	/// Loads the game data from server
	/// </summary>
	IEnumerator LoadGameDataFromServer(WWW www)
	{
        loadingBar.gameObject.SetActive(true);
        GameData gd;
		//yield return www;
		while(!www.isDone){
			loadingBar.value = www.progress;
			loadingText.text = (www.progress * 100).ToString ("####") + "%";
			yield return null;
		}
		loadingBar = null;
		if (www.error == null)
		{
			gd = JsonUtility.FromJson<GameData> (www.text.Trim());
		}
		else
		{
			Debug.Log ("Erro na conexão com o banco de dados: " + www.error);
			gd = null;
		}
		loadGameDataCallback (gd);
	}
	#endregion
}