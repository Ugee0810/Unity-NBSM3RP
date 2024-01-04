using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public static partial class SceneLevelModel
{
    public static Subject<int> SceneLevel {get; set;}
    public static int CurrentSceneLevel {get; set;}

    public static void InitializationSceneLevelHandler()
    {
        SceneLevel = new();
        CurrentSceneLevel = 0;
    }

    public static int NextSceneLevel()
        => CurrentSceneLevel + 1;

    public static int PrevSceneLevel()
        => CurrentSceneLevel - 1;
}