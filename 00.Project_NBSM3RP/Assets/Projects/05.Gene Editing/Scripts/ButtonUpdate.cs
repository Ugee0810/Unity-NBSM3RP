using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUpdate : MonoBehaviour
{

#if UNITY_EDITOR
    private void OnValidate()
    {
        logiHandler = FindAnyObjectByType<LogiHandler>();
        image = GetComponent<Image>();
    }
#endif

    [ReadOnly(true), SerializeField] private List<Sprite> sprites;
    [ReadOnly(false), SerializeField] private LogiHandler logiHandler;
    [ReadOnly(false), SerializeField] private Image image;

    private void Update()
    {
        if (logiHandler.throttle == -32768)
        {
            image.sprite = sprites[1];
        }
        else
        {
            image.sprite = sprites[0];
        }
    }
}