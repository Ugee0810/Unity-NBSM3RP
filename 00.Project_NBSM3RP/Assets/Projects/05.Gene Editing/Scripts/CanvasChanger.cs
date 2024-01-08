using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasChanger : MonoBehaviour
{
#if UNITY_EDITOR
    private void OnValidate()
    {
        logiHandler = FindAnyObjectByType<LogiHandler>();
    }
#endif

    [ReadOnly(false), SerializeField] private LogiHandler logiHandler;
    [ReadOnly(true), SerializeField] private CanvasGroup canvas2D;
    [ReadOnly(true), SerializeField] private CanvasGroup nextCanvas;

    private void Update()
    {
        if (nextCanvas.alpha == 1 && logiHandler.throttle == -32768)
        {
            canvas2D.alpha = 0;
        }
    }
}