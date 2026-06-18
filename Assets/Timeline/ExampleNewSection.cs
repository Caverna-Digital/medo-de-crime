using UnityEngine;

public class ExampleNewSection : MonoBehaviour
{
    public string newSection;

    [ContextMenu("TRIGGER")]
    public void TriggerTEST()
    {
        CreateXML.Instance.EnteredSection(newSection);
    }
    
}
