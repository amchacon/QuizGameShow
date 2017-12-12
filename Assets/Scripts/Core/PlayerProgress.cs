using UnityEngine;
using System.Collections;

/// <summary>
/// Manages player level (difficulty level)
/// </summary>
public class PlayerProgress : Singleton<PlayerProgress>
{
	private int _currentLevel = 1;

	public int currentLevel
	{
		get { return _currentLevel; }
		set { 
			_currentLevel = value;
			if (_currentLevel > 3)
				_currentLevel = 3;
			else if (_currentLevel < 1)
				_currentLevel = 1;
		}
	}

	/// <summary>
	/// Loads the Playerlevel = Difficulty Level
	/// </summary>
	/// <returns>The player level.</returns>
	public void LoadPlayerLevel()
	{
		if (PlayerPrefs.HasKey("CurrentDifficultyLevel"))
		{
			_currentLevel = PlayerPrefs.GetInt ("CurrentDifficultyLevel");
		} 
		else
		{
			_currentLevel = 1;
		}
	}

	/// <summary>
	/// Saves the player level
	/// </summary>
	public void SavePlayerLevel()
	{
		PlayerPrefs.SetInt ("CurrentDifficultyLevel", currentLevel);
	}
}
