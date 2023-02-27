using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffectHandler : MonoBehaviour
{
    public static CameraEffectHandler Current;
    private float m_cameraFov = 90;
    private float m_duration = 0;
    // Start is called before the first frame update
    void Start()
    {
        m_cameraFov = Camera.main.fieldOfView;
        Current= this;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_duration >= 0)
        {
            m_duration -= Time.unscaledDeltaTime;
            if (m_duration < 0)
            {
                Camera.main.fieldOfView = m_cameraFov;
            }
        }
    }

    public void CameraFOVEffect(float duration, float fov) { 
        Camera.main.fieldOfView = fov;
        m_duration = duration;
    }
}
