using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public static class GroundModel
{
    public enum Difficulty { Easy, Normal, Hard }

    public static ReactiveProperty<Difficulty> CurrentDifficulty { get; private set; }

    public static event Action<Difficulty, int> OnGameStart;
    public static event Action OnGameClear;
    public static event Action OnGameFail;

    public static void InitializationModel()
    {
        CurrentDifficulty = new();
    }

    public static void RaiseMainGameStart(Difficulty curDifficulty)
    {
        int randomStageNumber = 0;

        switch (curDifficulty)
        {
            case Difficulty.Easy:
                randomStageNumber = UnityEngine.Random.Range(0, 2);
                break;
            case Difficulty.Normal:
                randomStageNumber = UnityEngine.Random.Range(0, 4);
                break;
            case Difficulty.Hard:
                randomStageNumber = UnityEngine.Random.Range(0, 5);
                break;
        }

        OnGameStart?.Invoke(curDifficulty, randomStageNumber);
    }

    public static void RaiseOnGameClear()
    {
        switch (CurrentDifficulty.Value)
        {
            case Difficulty.Easy:
                CurrentDifficulty.Value = Difficulty.Normal;
                break;
            case Difficulty.Normal:
                CurrentDifficulty.Value = Difficulty.Hard;
                break;
        }

        OnGameClear?.Invoke();
    }

    public static void RaiseOnGameFail()
    {
        OnGameFail?.Invoke();
    }


    //////////////
    /// InGame ///
    //////////////
    public static event Action OnNextItem;

    public static void RaiseOnNextItem()
    {
        OnNextItem?.Invoke();
    }
}