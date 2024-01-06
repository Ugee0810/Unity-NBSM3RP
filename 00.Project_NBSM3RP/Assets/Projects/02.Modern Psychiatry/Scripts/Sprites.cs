using UnityEngine;

[System.Serializable]
public class Sprites
{
    public Sprite quest;
    public Sprite hint;
    public Sprite item;

    public Sprites(Sprite quest, Sprite hint, Sprite item)
    {
        this.quest = quest;
        this.hint = hint;
        this.item = item;
    }
}