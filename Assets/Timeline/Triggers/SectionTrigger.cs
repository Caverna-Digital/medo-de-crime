using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
    public string m_newSection;
    public bool m_oneTimeOnly = false;

    public void SectionAction()
    {
        CreateXML.Instance.EnteredSection(m_newSection);
    }

    void OnTriggerEnter(Collider other)
    {
        SectionAction();
        if (m_oneTimeOnly)
            this.gameObject.SetActive(false);
    }
}
