using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class GroundGameFailInfo : MonoBehaviour
{
    [ReadOnly(false), SerializeField] private Button nextButton;
    [ReadOnly(false), SerializeField] private Button mainButton;

    private void Start()
    {
        // GroundModel.OnGameFail += OnGameFail;

        nextButton
            .OnClickAsObservable()
            .Subscribe(delegate { OnClickNext(); })
            .AddTo(gameObject);

        mainButton
            .OnClickAsObservable()
            .Subscribe(delegate { OnClickMain(); })
            .AddTo(gameObject);
    }

    // private void OnGameFail()
    // {
    //     SceneLevelModel.CurrentSceneLevel = 14;
    // }

    private void OnClickNext()
    {
        switch (GroundModel.CurrentDifficulty.Value)
        {
            case GroundModel.Difficulty.Easy:
                GroundModel.RaiseGameStart(GroundModel.Difficulty.Easy);
                break;
            case GroundModel.Difficulty.Normal:
                GroundModel.RaiseGameStart(GroundModel.Difficulty.Normal);
                break;
            case GroundModel.Difficulty.Hard:
                GroundModel.RaiseGameStart(GroundModel.Difficulty.Hard);
                break;
        }

        GroundModel.RaiseGameFail();
    }

    private void OnClickMain()
    {
        GroundModel.InitializationGameDifficulty();
    }
}