using System;
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
    [SerializeField]
    private Image skipFiller;
    private bool loading = false;
    private bool isSkipping = false;
    private float skipKeyHoldDuration = 1.2f;
    public float fadeOutDelay = 8f;
    public float skipTimer = 0f;
    private bool skipped = false;
    
    private delegate void CutsceneComplete();
    private delegate void OnCutscenePrepared();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this.fadePanel = GameObject.Find("SceneLoader").GetComponentInChildren<FadePanelManager>();
    }

    private void Start()
    {        
        this.PlayVideoCutscene("FalloutNewVegas.mp4", false, this.LoadNextScene);
    }

    private void LoadNextScene()
    {
        SceneLoader.instance.LoadScene("MainMenu");
    }

    #region Utility
    private void PlayImageCutscene(Texture imageTexture)
    {
        this.cutsceneImage.enabled = true;
        this.cutsceneImage.texture = imageTexture;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0))
        {
            isSkipping = true;
        }
        
        if (Input.GetKeyUp(KeyCode.Escape) || Input.GetMouseButtonUp(0))
        {
            skipFiller.fillAmount = 0;
            isSkipping = false;
            skipTimer = 0f;
        }

        if (isSkipping && !skipped)
        {
            skipTimer += Time.deltaTime;
            skipFiller.fillAmount = skipTimer / skipKeyHoldDuration;
            if (skipTimer > skipKeyHoldDuration)
            {
                StartCoroutine(FadeOutVideoVolume());
                LoadNextScene();
                skipped = true;

            }
        }
    }

    public IEnumerator FadeOutVideoVolume()
    {
        AudioSource aSource = cutscenePlayer.GetTargetAudioSource(0);
        if (aSource)
        {
            while (aSource.volume > 0f)
            {
                aSource.volume -= Time.deltaTime * 0.01f;
                yield return 0;
            }    
        }
        else
        {
            float vol = cutscenePlayer.GetDirectAudioVolume(0);
            vol -= Time.deltaTime * 0.01f;
            cutscenePlayer.SetDirectAudioVolume(0,vol);
        }
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
