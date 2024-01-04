using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.ComponentModel;

public class MainCanvas : MonoBehaviour
{
    [ReadOnly(true), SerializeField] private List<CanvasGroup> canvasGroups;

    private void Awake()
    {
        SceneLevelModel.InitializationSceneLevelHandler();
    }

    private void Start()
    {
        SceneLevelModel.SceneLevel
            .AsObservable()
            .Subscribe(SceneLevelChangedHandler)
            .AddTo(gameObject);

        Observable.EveryUpdate()
            .Where(_ => Input.GetKey(KeyCode.LeftShift))
            .Where(_ => Input.GetKeyDown(KeyCode.Alpha2))
            .Subscribe(delegate { NextSceneHandler(); })
            .AddTo(gameObject);

        Observable.EveryUpdate()
            .Where(_ => Input.GetKey(KeyCode.LeftShift))
            .Where(_ => Input.GetKeyDown(KeyCode.Alpha1))
            .Subscribe(delegate { PreviousSceneHandler(); })
            .AddTo(gameObject);

        SceneLevelModel.SceneLevel
            .OnNext(SceneLevelModel.CurrentSceneLevel);
    }

    private void SceneLevelChangedHandler(int changedSceneLevel)
    {
        SceneLevelModel.CurrentSceneLevel = changedSceneLevel;

        for (int i = 0; i < canvasGroups.Count; i++)
        {
            canvasGroups[i].alpha = (i == changedSceneLevel) ? 1 : 0;
            canvasGroups[i].interactable = (i == changedSceneLevel);
            canvasGroups[i].blocksRaycasts = (i == changedSceneLevel);
        }

        Debug.Log($"MainCanvas.SceneLevelChangedHandler(): SceneLevelModel.CurrentSceneLevel Value is [{changedSceneLevel}]");
    }

    private void NextSceneHandler()
    {
        if (SceneLevelModel.CurrentSceneLevel == canvasGroups.Count - 1) return;

        SceneLevelModel.SceneLevel
            .OnNext(SceneLevelModel.NextSceneLevel());

        Debug.Log("MainCanvas.NextSceneHandler(): Shift+2 Pressed");
    }

    private void PreviousSceneHandler()
    {
        if (SceneLevelModel.CurrentSceneLevel == 0) return;

        SceneLevelModel.SceneLevel
            .OnNext(SceneLevelModel.PrevSceneLevel());

        Debug.Log("MainCanvas.PreviousSceneHandler(): Shift+1 Pressed");
    }
}