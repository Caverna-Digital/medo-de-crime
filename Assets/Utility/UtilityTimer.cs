using System;
using UnityEngine;
using UnityEngine.Events;
public class UtilityTimer : MonoBehaviour
{
    public float m_time = 3f;
    private bool starttimer = false;
    private float beginningTime = 0f;

    public UnityEvent m_timeEvent;

    void Update()
    {
        if (!starttimer)
            return;

        if (Time.time - beginningTime > m_time)
            m_timeEvent.Invoke();
    }

    public void StartTime()
    {
        beginningTime = Time.time;
        starttimer = true;
    }
}
