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
        SceneLevelModel.Initialization();
    }

    private void Start()
    {
        SceneLevelModel.GetSceneLevel()
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

        SceneLevelModel.GetSceneLevel().OnNext(SceneLevelModel.GetCurrentSceneLevel());
    }

    private void SceneLevelChangedHandler(int changedSceneLevel)
    {
        Debug.Log($"MainCanvas.SceneLevelChangedHandler(): changedSceneLevel is {changedSceneLevel}");

        for (int i = 0; i < canvasGroups.Count; i++)
        {
            canvasGroups[i].alpha = (i == changedSceneLevel) ? 1 : 0;
            canvasGroups[i].interactable = (i == changedSceneLevel);
            canvasGroups[i].blocksRaycasts = (i == changedSceneLevel);
        }

        SceneLevelModel.SetCurrentSceneLevel(changedSceneLevel);
    }

    private void NextSceneHandler()
    {
        Debug.Log("MainCanvas.NextSceneHandler(): Shift+2 Pressed");

        if (SceneLevelModel.GetCurrentSceneLevel() == canvasGroups.Count - 1) return;

        SceneLevelModel.GetSceneLevel().OnNext(SceneLevelModel.NextSceneLevel());
    }

    private void PreviousSceneHandler()
    {
        Debug.Log("MainCanvas.PreviousSceneHandler(): Shift+1 Pressed");

        if (SceneLevelModel.GetCurrentSceneLevel() == 0) return;

        SceneLevelModel.GetSceneLevel().OnNext(SceneLevelModel.PrevSceneLevel());
    }
}