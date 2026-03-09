using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class ResultScreen : MonoBehaviour
{
    [SerializeField]
    private RawImage backgroundImage;

    [SerializeField]
    private RenderTexture videoTexture;

    [SerializeField]
    private VideoPlayer resultVideoPlayer;

    [SerializeField]
    private string cutsceneFileName;

    [SerializeField]
    private TextMeshProUGUI timerText;

    [SerializeField]
    private TextMeshProUGUI bestTimeText;

    public void Setup()
    {
        this.PlayVideo();
        this.timerText.text = GameManager.instance.timer.GetCurrentTimerString();
        this.bestTimeText.text = GameManager.instance.timer.ConvertTimeToString(PlayerPrefs.GetFloat("record", float.PositiveInfinity));
    }

    private void PlayVideo()
    {
        this.resultVideoPlayer.Stop();

        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, this.cutsceneFileName);
        this.resultVideoPlayer.url = filePath;

        this.backgroundImage.enabled = true;
        this.backgroundImage.texture = this.videoTexture;

        this.resultVideoPlayer.renderMode = VideoRenderMode.RenderTexture;
        this.resultVideoPlayer.targetCameraAlpha = 1.0f;
        this.resultVideoPlayer.Play();

        this.resultVideoPlayer.isLooping = true;
    }

    public void RestartButtonPressed()
    {
        SceneLoader.instance.LoadScene("MainScene");
    }

    public void MenuButtonPressed()
    {
        SceneLoader.instance.LoadScene("MainMenu");
    }
}
