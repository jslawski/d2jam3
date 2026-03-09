using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager instance;

    private Vector3 gameplayFacePosition;
    private Vector3 gameplayFaceScale;
    private Vector3 gameplayFaceRotation;

    [SerializeField]
    private RawImage cutsceneImage;

    [SerializeField]
    private RenderTexture videoCutsceneTexture;

    [SerializeField]
    private FadePanelManager fadePanel;
    [SerializeField]
    private VideoPlayer cutscenePlayer;

    private delegate void CutsceneComplete();
    private delegate void OnCutscenePrepared();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        //Cursor.visible = false;
    }

    #region Utility
    private void PlayImageCutscene(Texture imageTexture)
    {
        this.cutsceneImage.enabled = true;
        this.cutsceneImage.texture = imageTexture;
    }

    private void PlayVideoCutscene(string cutsceneFileName, bool looping = false, CutsceneComplete functionAfterComplete = null)
    {
        this.cutscenePlayer.Stop();

        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, cutsceneFileName);
        this.cutscenePlayer.url = filePath;

        this.cutsceneImage.enabled = true;
        this.cutsceneImage.texture = this.videoCutsceneTexture;

        this.cutscenePlayer.renderMode = VideoRenderMode.RenderTexture;
        this.cutscenePlayer.targetCameraAlpha = 1.0f;
        this.cutscenePlayer.Play();

        if (looping == false)
        {
            this.cutscenePlayer.isLooping = false;
            StartCoroutine(this.WaitForCutsceneToFinish(functionAfterComplete));
        }
        else
        {
            this.cutscenePlayer.isLooping = true;
        }
    }

    private IEnumerator WaitForCutsceneToFinish(CutsceneComplete functionAfterComplete)
    {
        while (this.cutscenePlayer.isPlaying == false)
        {
            yield return null;
        }

        while (this.cutscenePlayer.isPlaying)
        {
            yield return null;
        }

        if (functionAfterComplete != null)
        {
            functionAfterComplete();
        }
    }

    private void WaitForCutscenePrepared(OnCutscenePrepared functionAfterComplete)
    {
        StartCoroutine(this.WaitForCutscenePreparedCoroutine(functionAfterComplete));
    }

    private IEnumerator WaitForCutscenePreparedCoroutine(OnCutscenePrepared functionAfterComplete)
    {
        while (this.cutscenePlayer.isPrepared == false)
        {
            yield return null;
        }

        yield return null;

        functionAfterComplete();
    }

    private void FadeToNextScene()
    {
        //this.fadePanel.OnFadeSequenceComplete += this.StartNewScene;
        this.fadePanel.FadeToBlack();
    }
    #endregion
}
