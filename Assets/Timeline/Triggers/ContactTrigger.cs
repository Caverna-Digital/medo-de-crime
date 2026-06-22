using UnityEngine;
using UnityEngine.Events;

public class ContactTrigger : TriggerEvents
{
    public UnityEvent m_actions;
    public bool m_oneTimeOnly = false;

    void OnTriggerEnter(Collider other)
    {
        RegisterXML();
        m_actions.Invoke();

        if (m_oneTimeOnly)
            this.enabled = false;
    }
}
