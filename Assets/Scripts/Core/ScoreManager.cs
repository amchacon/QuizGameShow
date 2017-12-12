using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScoreManager : Singleton<ScoreManager>
{
	private int _score;
	private int _highScore = 0;
	private int _baseQuestionPoints = 10;
	private Action<int> hudRefreshCallback;

	public int highscore
	{
		get { return _highScore; }
	}


	/// <summary>
	/// Gets the current score
	/// </summary>
	/// <returns>int score</returns>
	public int GetScore() 
	{
		return _score;
	}

	/// <summary>
	/// Restart score
	/// </summary>
	public void RestartPoints()
	{
		_score = 0;
	}

	/// <summary>
	/// Add points to score
	/// </summary>
	/// <param name="factor">Difficulty level as multiplier factor</param>
	public void AddPoints(int factor)
	{
		_score = _score + (_baseQuestionPoints * factor);
	}

	/// <summary>
	/// Check if the final score is bigger then high score
	/// </summary>
	/// <returns><c>true</c>, if new high score was checked, <c>false</c> otherwise.</returns>
	public bool CheckNewHighScore()
	{
		if (_score > _highScore && Config.Instance.globalConfig.gameMode == GameMode.ONLINE)
		{
			_highScore = _score;
			SaveHighScore ();
			return true;
		}
		return false;
	}

	/// <summary>
	/// Calls connection manager to load the high score from server with a callback
	/// </summary>
	public void LoadHighScore(Action<int> callback)
	{
		//Load from private backend
		hudRefreshCallback = callback;
		ConnectionController.Instance.LoadHighScore(LoadHighScoreFromServerCallback);
	}

	/// <summary>
	/// Callback called form connection manager with server response
	/// </summary>
	/// <param name="hs">Hs.</param>
	private void LoadHighScoreFromServerCallback(int hs)
	{
		_highScore = hs;
		if (_highScore == -1) //not connected
		{
			if (PlayerPrefs.HasKey("HighScore"))
			{
				Debug.LogWarning ("Loading local highscore!");
				_highScore = PlayerPrefs.GetInt ("HighScore");
			} 
			else
			{
				_highScore = 0;
			}
		}
		hudRefreshCallback (_highScore);
	}

	/// <summary>
	/// Saves the high score
	/// </summary>
	private void SaveHighScore()
	{
		Debug.LogWarning ("Savig High Score local!");
		PlayerPrefs.SetInt ("HighScore", _highScore);
		//Save on private backend
		ConnectionController.Instance.SaveHighScore(_highScore);


	}
}
