using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    [HideInInspector]
    public string nextSceneName;
    
    private FadePanelManager fadeManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this.fadeManager = GetComponentInChildren<FadePanelManager>();

        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    PlayerPrefs.DeleteAll();
                    this.LoadScene("LoginScene");
                }
            }
        }
    }

    public void LoadScene(string sceneName)
    {
        this.nextSceneName = sceneName;

        fadeManager.GetComponent<Image>().enabled = true;
        fadeManager.OnFadeSequenceComplete += this.LoadNextScene;
        fadeManager.FadeToBlack();
    }

    private void LoadNextScene()
    {
        fadeManager.OnFadeSequenceComplete -= LoadNextScene;   
        StartCoroutine(LoadNextSceneCoroutine());
    }

    private IEnumerator LoadNextSceneCoroutine()
    {
        SceneManager.LoadScene(this.nextSceneName);

        while (SceneManager.GetActiveScene().name != this.nextSceneName)
        {
            yield return null;
        }
        
        fadeManager.GetComponent<Image>().enabled = true;
        fadeManager.FadeFromBlack();
    }

    public void QuitGame()
    {
        fadeManager.OnFadeSequenceComplete += this.CloseGame;
        fadeManager.FadeToBlack();
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
