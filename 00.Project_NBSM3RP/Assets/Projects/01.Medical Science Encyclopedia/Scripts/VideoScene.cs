using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UniRx;

public class VideoScene : MonoBehaviour
{
    [ReadOnly(true), SerializeField] private List<Sprite> titleSprites;
    [ReadOnly(true), SerializeField] private List<VideoClip> videoClips;
    [ReadOnly(true), SerializeField] private Image titleImage;
    [ReadOnly(true), SerializeField] private VideoPlayer videoPlayer;

    private void Start()
    {
        SceneLevelModel.GetSceneLevel()
            .AsObservable()
            .Subscribe(SceneLevelChangedHandler)
            .AddTo(gameObject);
    }

    private void SceneLevelChangedHandler(int changedSceneLevel)
    {
        switch (changedSceneLevel)
        {
            case 3:
                SetTitleImage(0);
                SetVideoClip(0);
                SetVideoPlaybackSpeed(1);
                PlayVideo();
                break;
            case 4:
                SetTitleImage(1);
                SetVideoClip(1);
                SetVideoPlaybackSpeed(1);
                PlayVideo();
                break;

            default:
                if (videoPlayer.isPlaying)
                {
                    titleImage.sprite = null;
                    titleImage.rectTransform.sizeDelta = Vector2.zero;
                    videoPlayer.Stop();
                    videoPlayer.clip = null;
                }
                break;
        }
    }

    private void SetTitleImage(int countValue)
    {
        titleImage.sprite = titleSprites[countValue];
        titleImage.SetNativeSize();
    }

    private void SetVideoPlaybackSpeed(int speedValue)
        => videoPlayer.playbackSpeed = speedValue;

    private void SetVideoClip(int countValue)
        => videoPlayer.clip = videoClips[countValue];

    private void PlayVideo()
        => videoPlayer.Play();
}