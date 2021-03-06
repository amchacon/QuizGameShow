using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class GameController : MonoBehaviour
{
	#region Variables
	public AnswerButtonObjectPool answerButtonObjectPool;
	public Text questionText;
    public Text timeRemainingDisplay;
    public Slider timeRemainingSlider;
    public Transform answerButtonParent;
	public Text scoreDisplay;
	public Text highScoreDisplay;
	public GameObject newHighScoreMsg;
	public GameObject questionDisplay;
	public GameObject roundEndDisplay;
	public GameObject levelChangeGo;
	public Text levelChangeText;
    public Text resultText;
	public AudioClip gameTheme;
	public AudioClip rightAnswerClip;
	public AudioClip wrongAnswerClip;
	public AudioClip levelUpClip;
	public AudioClip levelDownClip;
	public AudioClip menuClickClip;

	private DataController _dataController;
	private RoundData _currentRoundData;
	private QuestionData[] _questionPool;
	private bool _isRoundActive = false;
    private float _baseTimeRemaining;
    private float _timeRemaining;
    private int _questionIndex;
	private List<GameObject> _answerButtonGameObjects = new List<GameObject>();
    private int _wrongAnswerCount;
    private int _rightAnswerCount;
	private int[] randomIds = new int[4];
	#endregion

	#region Unity Methods
	void Awake()
	{
		PlayerProgress.Instance.LoadPlayerLevel ();
		_dataController = FindObjectOfType<DataController>();
		_currentRoundData = _dataController.GetCurrentRoundData();
	}

    void Start()
	{
		_questionPool = _currentRoundData.questions;
		_questionPool.Shuffle ();
		_baseTimeRemaining = 10;
        _questionIndex = 0;
        _rightAnswerCount = 0;
        _wrongAnswerCount = 0;

		randomIds [0] = 0;
		randomIds [1] = 1;
		randomIds [2] = 2;
		randomIds [3] = 3;

		ScoreManager.Instance.RestartPoints ();
		ScoreManager.Instance.LoadHighScore (RefresHighScoreHUD);
		SoundController.Instance.PlayMusic (gameTheme);
		ShowQuestion();
		_isRoundActive = true;
	}

	void Update()
	{
		if (_isRoundActive)
		{
			_timeRemaining -= Time.deltaTime;
			UpdateTimeRemainingDisplay();
			if (_timeRemaining <= 0f)
			{
                AnswerButtonClicked(false);
            }
		}

		if (levelChangeGo.activeInHierarchy)
		{
			levelChangeText.color -= new Color (0,0,0, 0.8f * Time.deltaTime);

			if (levelChangeText.color.a <= 0)
				levelChangeGo.SetActive (false);
		}
	}
	#endregion

	#region Private Methods
	private void ShowQuestion()
	{
		RemoveAnswerButtons();
		if(Config.Instance.globalConfig.environment == Environment.PRODUCTION) //
			randomIds.Shuffle ();
		_timeRemaining = _baseTimeRemaining - (_currentRoundData.difficultyLevel * 2);
		timeRemainingSlider.maxValue = _timeRemaining;
        UpdateTimeRemainingDisplay();

        QuestionData questionData = _questionPool[_questionIndex];
		questionText.text = "Level " +_currentRoundData.difficultyLevel + ": " + questionData.questionText;

		for (int i = 0; i < questionData.answers.Length; i ++)
		{
			GameObject answerButtonGameObject = answerButtonObjectPool.GetObject();
			_answerButtonGameObjects.Add(answerButtonGameObject);
			answerButtonGameObject.transform.SetParent(answerButtonParent);
			answerButtonGameObject.transform.localScale = Vector3.one;

			AnswerButton answerButton = answerButtonGameObject.GetComponent<AnswerButton>();
			answerButton.SetUp(questionData.answers[randomIds[i]]);
		}
	}

	private void RemoveAnswerButtons()
	{
		while (_answerButtonGameObjects.Count > 0)
		{
			answerButtonObjectPool.ReturnObject(_answerButtonGameObjects[0]);
			_answerButtonGameObjects.RemoveAt(0);
		}
	}

	private void LevelCheck()
	{
		if (_rightAnswerCount == 3 && PlayerProgress.Instance.currentLevel < 3)
			IncreaseLevel();
		if (_wrongAnswerCount == 2 && PlayerProgress.Instance.currentLevel > 1)
			DecreaseLevel ();
	}

	private void IncreaseLevel()
	{
		PlayerProgress.Instance.currentLevel++;
        levelChangeText.text = "Level Up :)";
        levelChangeText.color = new Color (0, 1, 0, 1);
		levelChangeGo.SetActive (true);
		SoundController.Instance.PlaySoundFX (levelUpClip);
		_rightAnswerCount = 0;
		ReloadCurrentRoundData ();
	}

	private void DecreaseLevel()
	{
		PlayerProgress.Instance.currentLevel--;
        levelChangeText.text = "Level Down :(";
        levelChangeText.color = new Color (1, 0, 0, 1);
		levelChangeGo.SetActive (true);
		SoundController.Instance.PlaySoundFX (levelDownClip);
		_wrongAnswerCount = 0;
		ReloadCurrentRoundData ();
	}

	private void ReloadCurrentRoundData()
    {
		_currentRoundData = _dataController.GetCurrentRoundData();
        _questionPool = _currentRoundData.questions;
    }

    private void GameOver()
    {
        // Player Lose
        resultText.color = Color.red;
        resultText.text = "You Lose";
        EndRound();
    }

    private void GameWin()
    {
        // Player Win
        resultText.color = Color.green;
        resultText.text = "You Win";
        EndRound();
    }

    private void UpdateTimeRemainingDisplay()
	{
		timeRemainingDisplay.text = Mathf.Round(_timeRemaining).ToString();
        timeRemainingSlider.value = _timeRemaining;
    }

	private void EndRound()
	{
		_isRoundActive = false;
		questionDisplay.SetActive(false);
		roundEndDisplay.SetActive(true);

		if (ScoreManager.Instance.CheckNewHighScore ())
			newHighScoreMsg.SetActive (true);
			
		PlayerProgress.Instance.SavePlayerLevel ();
	}

	private void RefresHighScoreHUD(int hs)
	{
		highScoreDisplay.text = hs.ToString ();
	}
	#endregion

	#region Public Methods
	public void AnswerButtonClicked(bool isCorrect)
	{
		if (isCorrect)
		{
			ScoreManager.Instance.AddPoints (_currentRoundData.difficultyLevel);
			scoreDisplay.text = ScoreManager.Instance.GetScore().ToString();
			SoundController.Instance.PlaySoundFX (rightAnswerClip);

			_wrongAnswerCount = 0;
			_rightAnswerCount++;

			if (_rightAnswerCount == 5)
				GameWin();
		}
		else
		{
			SoundController.Instance.PlaySoundFX (wrongAnswerClip);
			_rightAnswerCount = 0;
			_wrongAnswerCount++;

			if (_wrongAnswerCount == 3)
				GameOver();
		}
		LevelCheck ();

		if (_questionPool.Length > _questionIndex + 1)
		{
			_questionIndex++;
			ShowQuestion ();
		} 
		else
		{
			EndRound ();
		}
	}

	public void ReturnToMenu()
	{		
		SoundController.Instance.PlaySoundFX (menuClickClip);
		SceneManager.LoadScene("ReadyScreen");
		ReloadCurrentRoundData();
	}
	#endregion
}