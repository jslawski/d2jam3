using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private Slider _mouseSensitivitySlider;

    [SerializeField]
    private GameObject _pauseMenuUI;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) == true)
        {
            if (this._pauseMenuUI.activeSelf == false)
            {
                this.OpenMenu();
            }
            else
            {
                this.CloseMenu();
            }
        }
        else if (Input.GetKeyUp(KeyCode.R) == true)
        {
            this.RestartGame();
        }
    }

    public void SliderChanged()
    {
        PlayerControlsManager.instance.UpdateMouseSensitivity(this._mouseSensitivitySlider.value);
    }

    public void OpenMenu()
    {
        Time.timeScale = 0.0f;
        Cursor.lockState = CursorLockMode.None;
        this._pauseMenuUI.SetActive(true);
    }

    public void CloseMenu()
    {
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
        this._pauseMenuUI.SetActive(false);
    }

    public void RestartGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
