using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private string _nextSceneToLoad;

    public void StartButtonPressed()
    {
        SceneLoader.instance.LoadScene(this._nextSceneToLoad);
    }

    public void ExitButtonPressed()
    {
        Application.Quit();
    }
}
