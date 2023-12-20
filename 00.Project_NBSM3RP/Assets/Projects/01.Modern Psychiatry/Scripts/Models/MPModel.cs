using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public static class MPModel
{
    public static Subject<int> SceneLevel { get; set; } = new();
    public static int CurrentSceneLevel { get; set; } = 0;

    public static int NextSceneLevel()
        => CurrentSceneLevel + 1;

    public static int PrevSceneLevel()
        => CurrentSceneLevel - 1;
}