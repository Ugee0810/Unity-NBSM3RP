using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public static class SceneLevelModel
{
    private static Subject<int> sceneLevel;

    public static Subject<int> GetSceneLevel()
        => sceneLevel;

    public static void SetSceneLevel(Subject<int> value)
        => sceneLevel = value;

    private static int currentSceneLevel;

    public static int GetCurrentSceneLevel()
        => currentSceneLevel;

    public static void SetCurrentSceneLevel(int value)
        => currentSceneLevel = value;

    public static void Initialization()
    {
        SetSceneLevel(new());
        SetCurrentSceneLevel(0);
    }

    public static int NextSceneLevel()
        => GetCurrentSceneLevel() + 1;

    public static int PrevSceneLevel()
        => GetCurrentSceneLevel() - 1;
}