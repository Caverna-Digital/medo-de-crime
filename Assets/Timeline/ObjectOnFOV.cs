using UnityEngine;

public class ObjectOnFOV : TriggerEvents
{
    public bool isVisible = false;
    void OnBecameVisible()
    {
        RegisterXML();
        Debug.Log("VISIBLE");
        isVisible = true;
    }

    void OnBecameInvisible()
    {
        Debug.Log("INVISIBLE");
        isVisible = true;
    }
}
