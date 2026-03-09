using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public GameTimer timer;

    [SerializeField]
    private ResultScreen _resultScreen;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        this.StartGame();
    }

    private void StartGame()
    {
        this.timer.StartTimer();
    }

    public void EndGame()
    {
        this.timer.StopTimer();

        float bestTime = PlayerPrefs.GetFloat("record", float.PositiveInfinity);

        if (this.timer.GetRawTime() < bestTime)
        {
            PlayerPrefs.SetFloat("record", this.timer.GetRawTime());
        }

        this._resultScreen.Setup();
        this._resultScreen.gameObject.SetActive(true);

        PlayerControlsManager.instance.enabled = false;

        Cursor.lockState = CursorLockMode.None;
    }
}
