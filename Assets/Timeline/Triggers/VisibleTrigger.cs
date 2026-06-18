using UnityEngine;
using UnityEngine.Events;

public class VisibleTrigger : TriggerEvents
{
    public UnityEvent[] m_Actions;
    public bool m_oneTimeOnly = false;
    public ObjectOnFOV m_objectToBeVisible;

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
