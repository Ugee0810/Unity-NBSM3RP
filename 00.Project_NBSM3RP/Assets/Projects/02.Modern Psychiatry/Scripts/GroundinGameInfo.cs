using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GroundinGameInfo : MonoBehaviour
{
    [ReadOnly(false), SerializeField] private List<Sprite> textInfoOnDifficultySprites;
    [ReadOnly(false), SerializeField] private Image textInfoOnDifficultyImage;
    [ReadOnly(false), SerializeField] private Button startButton;

    private void Start()
    {
        GroundModel.InitializationModel();

        GroundModel.CurrentDifficulty
            .AsObservable()
            .Subscribe(OnChangeTextImage)
            .AddTo(gameObject);

        startButton
            .OnClickAsObservable()
            .Subscribe(delegate { GroundModel.RaiseMainGameStart(GroundModel.CurrentDifficulty.Value); })
            .AddTo(gameObject);
    }

    private void OnChangeTextImage(GroundModel.Difficulty difficulty)
    {
        switch (difficulty)
        {
            case GroundModel.Difficulty.Easy:
                textInfoOnDifficultyImage.sprite = textInfoOnDifficultySprites[0];
                break;
            case GroundModel.Difficulty.Normal:
                textInfoOnDifficultyImage.sprite = textInfoOnDifficultySprites[1];
                break;
            case GroundModel.Difficulty.Hard:
                textInfoOnDifficultyImage.sprite = textInfoOnDifficultySprites[2];
                break;
        }

        textInfoOnDifficultyImage.SetNativeSize();
    }
}
