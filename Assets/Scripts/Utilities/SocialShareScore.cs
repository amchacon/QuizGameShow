using UnityEngine;
using System.Collections;

public class SocialShareScore : MonoBehaviour {

	private int _scoreToShare;

	#region Facebook Share
	private string _appID = "568599286667441";
	private string _link = "https://play.google.com/store/apps/developer?id=PlayO+Studio";
	private string _picture = "http://playo.com.br/digitalair/images/quizfaceimg.png";
	private string _name;
	private string _caption;
	private string _description;

	public void ShareScoreOnFB(){
		_scoreToShare = ScoreManager.Instance.GetScore ();
		_name = "Quiz Game Shoooow!";
		_caption = "I got " + _scoreToShare + " points, bro! Can you beat it?";
		_description = "Enjoy fun, free games! Challenge yourself or share with friends. Fun and easy-to-use game.";

		Application.OpenURL("https://www.facebook.com/dialog/feed?"+ "app_id="+_appID+ "&link="+
			_link+ "&picture="+_picture+ "&name="+ReplaceSpace(_name)+ "&caption="+
			ReplaceSpace(_caption)+ "&description="+ReplaceSpace(_description)+
			"&redirect_uri=https://facebook.com/");
	}

	string ReplaceSpace (string val) {
		return val.Replace(" ", "%20");
	}
	#endregion


	#region Twitter Share
	private const string _address = "http://twitter.com/intent/tweet";
	private const string _language = "en";
	public static string _descriptionParam;
	private string _appStoreLink = "https://play.google.com/store/apps/developer?id=PlayO+Studio";

	public void ShareScoreOnTW()
	{
		_scoreToShare = ScoreManager.Instance.GetScore ();
		string nameParameter = "I got " + _scoreToShare + " points, bro! Can you beat it?";
		Application.OpenURL(_address +
			"?text=" + WWW.EscapeURL(nameParameter + "\n" + _descriptionParam + "\n" + "Get the Game:\n" + _appStoreLink));
	}
	#endregion
}