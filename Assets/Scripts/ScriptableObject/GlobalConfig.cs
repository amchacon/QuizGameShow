using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalConfiguration", menuName = "Global Configuration", order = 1)]
public class GlobalConfig : ScriptableObject
{	
	[SerializeField, Tooltip("If DEV, the correct answer allways will be the first answer")]
	private Environment _environment;
	[SerializeField]
	private string _localURL;
	[SerializeField]
	private string _remoteURL;
	[SerializeField]
	private URLActive _urlActive;
	[SerializeField]
	private GameMode _gameMode;

	public string localURL { get { return _localURL; } }
	public string remoteURL { get { return _remoteURL; } }
	public Environment environment { get { return _environment; } }
	public URLActive urlActive { get { return _urlActive; } }
	public GameMode gameMode { get { return _gameMode; } set { _gameMode = value; } }
}