using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class MSEScene01 : MonoBehaviour
{
    [ReadOnly(true), SerializeField] private Button topButton;
    [ReadOnly(true), SerializeField] private Button bottomButton;


    private void Start()
    {
        topButton
            .OnClickAsObservable()
            .Subscribe(OnClickTop)
            .AddTo(gameObject);

        bottomButton
            .OnClickAsObservable()
            .Subscribe(OnClickBottom)
            .AddTo(gameObject);
    }

    private void OnClickTop(Unit _)
    {
        SceneLevelModel.SetCurrentSceneLevel(2);
        SceneLevelModel.GetSceneLevel().OnNext(SceneLevelModel.GetCurrentSceneLevel());
    }

    private void OnClickBottom(Unit _)
    {
        SceneLevelModel.SetCurrentSceneLevel(4);
        SceneLevelModel.GetSceneLevel().OnNext(SceneLevelModel.GetCurrentSceneLevel());
    }
}