using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class MPScene00 : MonoBehaviour
{
    [ReadOnly(true), SerializeField] private Button mainButton;

    private void Start()
    {
        mainButton
            .OnClickAsObservable()
            .Subscribe(OnClickMain)
            .AddTo(gameObject);
    }

    private void OnClickMain(Unit _)
    {
        SceneLevelModel.SceneLevel.OnNext(SceneLevelModel.NextSceneLevel());
    }
}