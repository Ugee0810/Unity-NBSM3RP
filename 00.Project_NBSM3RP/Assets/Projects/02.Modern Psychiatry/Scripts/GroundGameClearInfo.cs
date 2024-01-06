using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class GroundGameClearInfo : MonoBehaviour
{
    [ReadOnly(false), SerializeField] private List<Sprite> nextButtonSprites;
    [ReadOnly(false), SerializeField] private Button nextButton;

    private void Start()
    {
        GroundModel.OnGameClear += OnGameClear;

        nextButton
            .OnClickAsObservable()
            .Subscribe(delegate { OnClickNext(); })
            .AddTo(gameObject);
    }

    private void OnGameClear()
    {
        SceneLevelModel.CurrentSceneLevel = 13;

        switch (GroundModel.CurrentDifficulty.Value)
        {
            case GroundModel.Difficulty.Easy:
                nextButton.image.sprite = nextButtonSprites[0];
                break;
            case GroundModel.Difficulty.Normal:
                nextButton.image.sprite = nextButtonSprites[1];
                break;
            case GroundModel.Difficulty.Hard:
                nextButton.image.sprite = nextButtonSprites[2];
                break;
        }

        nextButton.image.SetNativeSize();
    }

    private void OnClickNext()
    {
        switch (GroundModel.CurrentDifficulty.Value)
        {
            case GroundModel.Difficulty.Easy:
                GroundModel.RaiseMainGameStart(GroundModel.Difficulty.Normal);
                break;
            case GroundModel.Difficulty.Normal:
                GroundModel.RaiseMainGameStart(GroundModel.Difficulty.Hard);
                break;
            case GroundModel.Difficulty.Hard:
                SceneLevelModel.CurrentSceneLevel = 0;
                GroundModel.InitializationModel();
                break;
        }
    }
}