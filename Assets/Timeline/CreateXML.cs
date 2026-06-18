using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Text;


public class CreateXML : MonoBehaviour
{
      public static CreateXML Instance { get; private set; }
      public Timer time;

      private PlayerStats PlayerStats = new PlayerStats();


    private void Awake()
    {
         // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
      string path = Application.persistentDataPath + "/data_DATA_jogador_INPUT.csv";
        var content = CreateHeadline();
        File.AppendAllText(path, content);
        Debug.Log("CALLED START");
    }

    private string CreateHeadline()
    {
         var sb = new StringBuilder("Seção,Evento,Tempo em segundos");
          //  sb.Append('\n').Append(PlayerStats.Secao).Append(',').Append(PlayerStats.Points).Append(',').Append(PlayerStats.Tempo_em_segundos).Append(',').Append(PlayerStats.Mistakes);
           
        return sb.ToString();
    }
   
    public void EnteredSection(string section)
    {
        string path = Application.persistentDataPath + "/data_DATA_jogador_INPUT.csv";
        bool retorno = PlayerStats.ChangeSection(section);
        var content = retorno?AddToFileSection($"Voltou Seção ({PlayerStats.LastSection} -> {section})"):AddToFileSection($"Nova Seção ({PlayerStats.LastSection} -> {section})");
        File.AppendAllText(path, content);
                
    }

    public void EventTriggered(string eventType)
    {
        string path = Application.persistentDataPath + "/data_DATA_jogador_INPUT.csv";
        var content = AddToFile(eventType);
        File.AppendAllText(path, content);
    }

    
     private string AddToFile(string eventType)
    {
        string tempo = time.tempo.ToString("N0");      
        var sb = new StringBuilder();
        sb.Append('\n').Append(PlayerStats.Section).Append(',').Append(eventType).Append(',').Append(tempo);
        return sb.ToString();
        
    }

    private string AddToFileSection(string eventType)
    {
        string tempo = time.tempo.ToString("N0");      
        var sb = new StringBuilder();
        sb.Append('\n').Append(PlayerStats.LastSection).Append(',').Append(eventType).Append(',').Append(tempo);
        return sb.ToString();
        
    }



[ContextMenu("END")]
    public void ChangePath()
    {       
        DateTime data_hora = DateTime.Now;
        string data = data_hora.ToString("dd.MM.yyyy_HH.mm");      
        string playerID = PlayerPrefs.GetString("TesterID", "00");

        var folder = new DirectoryInfo(Application.persistentDataPath);
        var files = folder.EnumerateFiles();
        var lastModifiedFile = files.OrderBy(fi => fi.LastWriteTime).Last();
        var filePath = lastModifiedFile.FullName;
        var prev_path = filePath;
        Debug.Log("prev path: " + prev_path);

        string pos_path = Application.persistentDataPath + $"/data_e_hora_{data}_jogador_{playerID}.csv";
        Debug.Log(pos_path);
        
        File.Move(prev_path, pos_path);
    }
}
