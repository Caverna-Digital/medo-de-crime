using UnityEngine;

public class TriggerEvents : MonoBehaviour
{
     static CreateXML createXML;
     public string eventType;
    
   
    
   public  void RegisterXML()
    {
        CreateXML.Instance.EventTriggered(eventType);
        
    }
}
