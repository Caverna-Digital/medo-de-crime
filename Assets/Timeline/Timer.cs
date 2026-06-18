using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float tempo;
   // public Demo demo;

    private bool contar = false;

    void Update()
    {
        if(contar){
            tempo += Time.deltaTime;
        }
    }

[ContextMenu("START")]
    public void StartTimer()
    {
       // demo.isPlaying = false;
        contar = true;
        Debug.Log("Iniciou o timer");
    }

    public void StopTimer()
    {
        contar = false;
        Debug.Log("Parou o timer");
       // demo.ChangePathWithoutID();
    }

    public void ResetTimer()
    {
        tempo = 0;
        Debug.Log("Resetou o timer");
    }
}