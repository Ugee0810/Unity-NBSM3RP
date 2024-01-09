using UnityEngine;
using UniRx;

public class LogiHandler : MonoBehaviour
{
    private LogitechGSDK.DIJOYSTATE2ENGINES rec;
    [ReadOnly(false)] public int handle;
    [ReadOnly(false)] public int throttle;
    [ReadOnly(false)] public int brake;

    private void Start()
    {
        LogitechGSDK.LogiSteeringInitialize(false);
    }

    private void Update()
    {
        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
        {
            rec = LogitechGSDK.LogiGetStateUnity(0);

            handle = rec.lX;
            throttle = rec.lY;
            brake = rec.lRz;
        }
    }
}