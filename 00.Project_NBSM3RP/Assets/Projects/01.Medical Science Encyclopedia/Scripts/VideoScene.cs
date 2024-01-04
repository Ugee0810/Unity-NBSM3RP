using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UniRx;

public class VideoScene : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RawImage rawImage;
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private Sprite titleSprite;
    [SerializeField] private VideoClip videoClip;
    [SerializeField] private Image titleImage;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private TimeRange[] timeRanges;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rawImage = GetComponentInChildren<RawImage>();
        renderTexture = new(width: 1920, height: 1080, depth: 24);

        rawImage.texture = renderTexture;
        videoPlayer.targetTexture = renderTexture;

        SceneLevelModel.SceneLevel
            .AsObservable()
            .Subscribe(SceneLevelChangedHandler)
            .AddTo(gameObject);

        Observable
            .EveryUpdate()
            .Where(_ => canvasGroup.alpha == 0)
            .Subscribe(delegate { OnCanvasChanged(); })
            .AddTo(gameObject);

        Observable
            .EveryUpdate()
            .Where(_ => videoPlayer != null && timeRanges != null && canvasGroup.alpha == 1)
            .Where(_ => videoPlayer.isPlaying == true)
            .Subscribe(delegate { AdjustPlaybackSpeed(videoPlayer.time); })
            .AddTo(gameObject);

        videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void SceneLevelChangedHandler(int changedSceneLevel)
    {
        switch (changedSceneLevel)
        {
            case 3 or 4 or 6 or 7 or 8 or 9 or 10 or 11 or 12:
                SetTitleImage();
                SetVideoClip();
                PlayVideo();
                break;
        }
    }

    private void OnCanvasChanged()
    {
        if (videoPlayer.isPlaying)
        {
            titleImage.sprite = null;
            titleImage.rectTransform.sizeDelta = Vector2.zero;
            videoPlayer.Stop();
            videoPlayer.clip = null;
        }
    }

    private void SetTitleImage()
    {
        titleImage.sprite = titleSprite;
        titleImage.SetNativeSize();
    }

    private void AdjustPlaybackSpeed(double currentTime)
    {
        bool isWithinRange = false;

        foreach (var range in timeRanges)
        {
            if (currentTime >= range.startTime && currentTime <= range.endTime)
            {
                isWithinRange = true;
                break;
            }
        }

        videoPlayer.playbackSpeed = isWithinRange ? 2.0f : 1.0f;
    }

    private void SetVideoClip()
        => videoPlayer.clip = videoClip;

    private void PlayVideo()
        => videoPlayer.Play();

    private void OnVideoEnd(VideoPlayer vp)
    {
        SceneLevelModel.SceneLevel.OnNext(0);
    }
}