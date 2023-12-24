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
            .Subscribe(NextSceneHandler)
            .AddTo(gameObject);

        Observable.EveryUpdate()
            .Where(_ => Input.GetKey(KeyCode.LeftShift))
            .Where(_ => Input.GetKeyDown(KeyCode.Alpha1))
            .Subscribe(PreviousSceneHandler)
            .AddTo(gameObject);

        SceneLevelModel.GetSceneLevel().OnNext(SceneLevelModel.GetCurrentSceneLevel());
    }

    private void SceneLevelChangedHandler(int changedSceneLevel)
    {
        Debug.Log($"MainCanvas.SceneLevelChangedHandler(): changedSceneLevel is {changedSceneLevel}");

        switch (true)
        {
            case var _ when SceneLevelModel.GetCurrentSceneLevel() > changedSceneLevel:
                OnChangeSceneState(changedSceneLevel + 1, false);
                break;
            case var _ when SceneLevelModel.GetCurrentSceneLevel() < changedSceneLevel:
                OnChangeSceneState(changedSceneLevel - 1, false);
                break;
        }

        SceneLevelModel.SetCurrentSceneLevel(changedSceneLevel);
        
        OnChangeSceneState(changedSceneLevel, true);
    }

    private void NextSceneHandler(long _)
    {
        Debug.Log("MainCanvas.NextSceneHandler(): Shift+2 Pressed");

        if (SceneLevelModel.GetCurrentSceneLevel() == canvasGroups.Count - 1) return;

        SceneLevelModel.GetSceneLevel().OnNext(SceneLevelModel.NextSceneLevel());
    }

    private void PreviousSceneHandler(long _)
    {
        Debug.Log("MainCanvas.PreviousSceneHandler(): Shift+1 Pressed");

        if (SceneLevelModel.GetCurrentSceneLevel() == 0) return;

        SceneLevelModel.GetSceneLevel().OnNext(SceneLevelModel.PrevSceneLevel());
    }

    private void OnChangeSceneState(int changedSceneLevel, bool IsEnabled)
    {
        canvasGroups[changedSceneLevel].alpha = IsEnabled
            ? 1
            : 0;
        canvasGroups[changedSceneLevel].interactable = IsEnabled;
        canvasGroups[changedSceneLevel].blocksRaycasts = IsEnabled;
    }
}