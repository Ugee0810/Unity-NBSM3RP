using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class GroundGameFailInfo : MonoBehaviour
{
    [ReadOnly(false), SerializeField] private Button nextButton;

    private void Start()
    {
        GroundModel.OnGameFail += OnGameFail;

        nextButton
            .OnClickAsObservable()
            .Subscribe(delegate { OnClickNext(); })
            .AddTo(gameObject);
    }

    private void OnGameFail()
    {
        SceneLevelModel.CurrentSceneLevel = 14;
    }

    private void OnClickNext()
    {
        switch (GroundModel.CurrentDifficulty.Value)
        {
            case GroundModel.Difficulty.Easy:
                GroundModel.RaiseMainGameStart(GroundModel.Difficulty.Easy);
                break;
            case GroundModel.Difficulty.Normal:
                GroundModel.RaiseMainGameStart(GroundModel.Difficulty.Normal);
                break;
            case GroundModel.Difficulty.Hard:
                GroundModel.RaiseMainGameStart(GroundModel.Difficulty.Hard);
                break;
        }
    }
}