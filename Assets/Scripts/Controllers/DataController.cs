using UnityEngine;
using System.IO;
using System;
using System.Linq;

public class DataController : Singleton<DataController>
{
	[SerializeField]
	private RoundData[] _allRoundData;
	private string _gameDataFileName;
	Action ExecuteCallback;

	/// <summary>
	/// Load game data from local file
	/// </summary>
	private void LoadLocalGameData()
	{
        Debug.Log("Carregando dados locais");
        string dataAsJson = FileManagement.Instance.ReadFile("LocalData", _gameDataFileName, "json");
		GameData loadedData = JsonUtility.FromJson<GameData>(dataAsJson);
		_allRoundData = loadedData.allRoundData;
	}

	/// <summary>
	/// Load game data from remote data base
	/// </summary>
	public void LoadRemoteGameData(int catId, Action LoadReadyScreenSceneCallback)
	{
		ExecuteCallback = LoadReadyScreenSceneCallback;
		SetGameDataFileName (catId);
		ConnectionController.Instance.LoadGameData (catId, LoadGameDataCallback);
	}

	/// <summary>
	/// Loads the game data callback.
	/// </summary>
	private void LoadGameDataCallback(GameData gd)
	{
		if (gd != null)
		{
            Debug.Log("Dados remotos carregados com sucesso!");
            _allRoundData = gd.allRoundData;
			string dataAsJson = JsonUtility.ToJson (gd);
            Debug.Log("Atualizando base local");
            FileManagement.Instance.UpdateFile("LocalData", _gameDataFileName, "json", dataAsJson, "replace");
		}
		else
		{
			Debug.Log("Tentando carregar dados locais...");
			LoadLocalGameData ();
		}
		ExecuteCallback ();
	}

    /// <summary>
    /// Gets the current round data by difficulty level
    /// </summary>
    public RoundData GetCurrentRoundData()
	{
		return _allRoundData.FirstOrDefault(rd => rd.difficultyLevel == PlayerProgress.Instance.currentLevel);
	}

	private void SetGameDataFileName(int id)
	{
		switch (id)
		{
		case 1:
			_gameDataFileName = "GameData";
			break;
		case 2:
			_gameDataFileName = "GeneralData";
			break;
		case 3:
			_gameDataFileName = "FilmData";
			break;
		case 4:
			_gameDataFileName = "MusicData";
			break;
		}
	}

   
}



