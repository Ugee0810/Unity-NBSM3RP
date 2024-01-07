using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public static class GroundModel
{
    public enum Difficulty { Easy, Normal, Hard }

    public static ReactiveProperty<Difficulty> CurrentDifficulty { get; private set; }
    public static ReactiveProperty<int> StageLevel { get; private set; }

    public static event Action<Difficulty, int> OnGameStart;
    public static event Action OnGameClear;
    public static event Action OnGameFail;

    public static void InitializationGameDifficulty()
    {
        if (CurrentDifficulty != null)
        {
            CurrentDifficulty.Value = Difficulty.Easy;
        }
        else
        {
            CurrentDifficulty = new(Difficulty.Easy);
        }

        Debug.Log($"GroundModel.InitializationGameDifficulty()");
    }

    public static void RaiseGameStart(Difficulty curDifficulty)
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

        Debug.Log($"GroundModel.RaiseMainGameStart({curDifficulty})");
    }

    public static void RaiseIncreaseDifficulty()
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

        Debug.Log($"GroundModel.RaiseOnIncDifficulty()");
    }

    public static void InitializationStageLevel()
    {
        if (StageLevel != null)
        {
            StageLevel.Value = 0;
        }
        else
        {
            StageLevel = new(0);
        }

        Debug.Log($"GroundModel.InitializationStageLevel()");
    }

    public static void RaiseNextLevel()
    {
        StageLevel.Value++;

        Debug.Log($"GroundModel.RaiseNextLevel()");
    }
    
    public static void RaiseGameClear()
    {
        OnGameClear?.Invoke();

        Debug.Log($"GroundModel.RaiseGameClear()");
    }

    public static void RaiseGameFail()
    {
        OnGameFail?.Invoke();

        Debug.Log($"GroundModel.RaiseOnGameFail()");
    }
}