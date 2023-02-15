using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeEffectHandler : MonoBehaviour
{
    public static TimeEffectHandler Current;
    private float m_duration;
    // Start is called before the first frame update
    void Start()
    {
        Current = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_duration >= 0)
        {
            m_duration -= Time.unscaledDeltaTime;
            if(m_duration < 0)
            {
                Time.timeScale = 1;
            }
        }
    }

    public void SetTemporaryTimescale(float duration, float scale)
    {
        Time.timeScale = scale;
        m_duration = duration;
    }
}
