using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GameItem
{
    public Button item;
    public Vector2 anchoredPos;

    public GameItem(Button item, Vector2 anchoredPos)
    {
        this.item = item;
        this.anchoredPos = anchoredPos;
    }
}