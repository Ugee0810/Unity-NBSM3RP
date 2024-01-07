using UnityEngine;

[System.Serializable]
public class Sprites
{
    public Sprite quest;
    public Sprite hint;
    public Sprite target;

    public Sprites(Sprite quest, Sprite hint, Sprite target)
    {
        this.quest = quest;
        this.hint = hint;
        this.target = target;
    }
}