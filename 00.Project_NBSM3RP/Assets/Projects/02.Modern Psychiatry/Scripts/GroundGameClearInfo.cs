using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class GroundGameClearInfo : MonoBehaviour
{
    [ReadOnly(false), SerializeField] private List<Sprite> nextButtonSprites;
    [ReadOnly(false), SerializeField] private Button nextButton;
    [ReadOnly(false), SerializeField] private Button mainButton;

    private void Start()
    {
        GroundModel.OnGameClear += OnGameClear;

        nextButton
            .OnClickAsObservable()
            .Subscribe(delegate { OnClickNext(); })
            .AddTo(gameObject);

        mainButton
            .OnClickAsObservable()
            .Subscribe(delegate { OnClickMain(); })
            .AddTo(gameObject);
    }

    private void OnGameClear()
    {
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

        Debug.Log($"GroundGameClearInfo.OnGameClear()");
    }

    private void OnClickNext()
    {
        switch (GroundModel.CurrentDifficulty.Value)
        {
            case GroundModel.Difficulty.Easy:
                GroundModel.RaiseIncreaseDifficulty();
                GroundModel.RaiseGameStart(GroundModel.Difficulty.Normal);
                SceneLevelModel.CurrentSceneLevel = 12;
                SceneLevelModel.SceneLevel.OnNext(12);
                break;
            case GroundModel.Difficulty.Normal:
                GroundModel.RaiseIncreaseDifficulty();
                GroundModel.RaiseGameStart(GroundModel.Difficulty.Hard);
                SceneLevelModel.CurrentSceneLevel = 12;
                SceneLevelModel.SceneLevel.OnNext(12);
                break;
            case GroundModel.Difficulty.Hard:
                GroundModel.InitializationGameDifficulty();
                SceneLevelModel.CurrentSceneLevel = 0;
                SceneLevelModel.SceneLevel.OnNext(0);
                break;
        }
    }

    private void OnClickMain()
    {
        GroundModel.InitializationGameDifficulty();
    }
}