using UnityEngine;
using UnityEngine.Events;

public class VisibleTrigger : TriggerEvents
{
    public UnityEvent m_actions;

    void OnBecameVisible()
    {
        m_actions.Invoke();
    }
}
