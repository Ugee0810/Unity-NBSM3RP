using UnityEngine;
using System.Collections;

public class LogiButtonTrigger : MonoBehaviour
{
#if UNITY_EDITOR
    private void OnValidate()
    {
        canvas = GetComponent<CanvasGroup>();
        logiHandler = FindAnyObjectByType<LogiHandler>();
    }
#endif

    [ReadOnly(false), SerializeField] private LogiHandler logiHandler;
    [ReadOnly(false), SerializeField] private CanvasGroup canvas;

    private void Start()
    {
        StartCoroutine(CheckThrottle());
    }

    private IEnumerator CheckThrottle()
    {
        yield return new WaitUntil(() => canvas.alpha == 1);

        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            if (canvas.alpha == 1 && logiHandler.throttle == -32768)
            {
                SceneLevelModel.SceneLevel.OnNext(SceneLevelModel.NextSceneLevel());
            }

            yield return null;
        }
    }
}