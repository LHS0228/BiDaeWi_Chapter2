using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    private void Awake()
    {
        Screen.SetResolution(1920, 1080, true);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ScreenSystem.instance.ScreenPlay(true);
    }
    
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void LoadSceneDelay(string sceneName, float time)
    {
        StartCoroutine(LoadSceneStart(sceneName, time));
    }

    private IEnumerator LoadSceneStart(string sceneName, float time)
    {
        yield return new WaitForSecondsRealtime(time);

        SceneManager.LoadScene(sceneName);
    }
}
