using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenManager : Singleton<ScreenManager>
{
    private FadeTransition m_FadeTransition;

    void Awake()
    {
        m_FadeTransition = FadeTransition.Instance;
    }

    public void LoadScene(string sceneNameToLoad)
    {
        StartCoroutine("Loading", sceneNameToLoad);
    }

    IEnumerator Loading(string name)
    {
        yield return new WaitForSeconds(m_FadeTransition.BeginFade(FadeDirection.Out));

        if (!name.Equals("Quit"))
        {
            SceneManager.LoadScene(name);
        }
        else
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    public void Quit()
    {
        StartCoroutine("Loading", "Quit");
    }

    public void Pause()
    {
        Time.timeScale = 1.0f - Time.timeScale;
    }
}