using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    public string Section = "Rua Inicial";

    public string LastSection = "Rua Inicial";

     public List<string> AllPastSections = new List<string>();
    public string Tempo_em_segundos = "0";

    public bool ChangeSection(string newSection)
    {
        
        if(!AllPastSections.Contains(Section) )AllPastSections.Add(Section);

        LastSection = Section;

        Section = newSection;

        if( AllPastSections.Contains(newSection)) return true;
        else return false;
    }
   
}

