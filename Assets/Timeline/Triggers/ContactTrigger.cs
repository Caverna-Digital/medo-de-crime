using UnityEngine;
using UnityEngine.Events;

public class ContactTrigger : TriggerEvents
{
    public UnityEvent[] m_Actions;
    public bool m_oneTimeOnly = false;

    void OnTriggerEnter(Collider other)
    {
        RegisterXML();
        foreach (UnityEvent action in m_Actions)
        {
            action.Invoke();
        }

        if (m_oneTimeOnly)
            this.gameObject.SetActive(false);
    }
}
