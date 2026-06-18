using UnityEngine;

public class ExampleFear : TriggerEvents
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [ContextMenu("TRIGGER")]
    public void ExecuteEvent()
    {
        //DO WHATEVER


        RegisterXML();  //NEEDS THIS EVENT TO REGISTER
    }
}
