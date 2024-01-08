using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handle : MonoBehaviour
{
#if UNITY_EDITOR
    private void OnValidate()
    {
        logiHandler = FindAnyObjectByType<LogiHandler>();
    }
#endif

    [ReadOnly(false), SerializeField] private LogiHandler logiHandler;

    private void Update()
    {
        float newXPosition = MapRange(logiHandler.handle, -32768 / 2, 32767 / 2, -10.0f, 10.0f);
        newXPosition = Mathf.Clamp(newXPosition, -65.0f, 70.0f); // x축 위치를 -65에서 +70 사이로 제한
        transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);
    }

    private float MapRange(float value, float fromSource, float toSource, float fromTarget, float toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }
}