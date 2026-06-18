//using NUnit.Framework;
//using System.Collections;
using System.Collections.Generic;
//using UnityEditor.SceneManagement;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
//using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    public List<LightningScenario> LightScenes;

    private string currentScene;
    private LightningScenario currentLight;
    private int current = 0;

    public InputAction switchButton;

    void OnEnable()
    {
        //-> preserve objects when changing scenes
        DontDestroyOnLoad(gameObject); 

        switchButton.started += SwitchScene;
        switchButton.Enable();
    }

    private void Start()
    {
        LoadScene( LightScenes[current] );
    }

    void SwitchScene(InputAction.CallbackContext context)
    {
        if (LightScenes.Count > 1) 
        {
            current = (current + 1) % LightScenes.Count;
            LoadScene( LightScenes[current] );
        }
    }

    void LoadScene(LightningScenario lightscene)
    {
        int newscene = LightScenes.IndexOf(lightscene);
        //check base scene
        if (SceneManager.GetActiveScene().name != lightscene.baseScene)
        {
            SceneManager.LoadSceneAsync(lightscene.baseScene, LoadSceneMode.Single);
        }
        ApplyLightSettings(lightscene);
        current = newscene;
    }

    public void ApplyLightSettings(LightningScenario scenario)
    {
        LightmapData[] maps = new LightmapData[scenario.lightmapsColor.Length];

        for (int i = 0; i < maps.Length; i++)
        {
            maps[i] = new LightmapData();
            maps[i].lightmapColor = scenario.lightmapsColor[i];
            maps[i].lightmapDir = scenario.lightmapsDir[i];
        }

        LightmapSettings.lightmaps = maps;
        //RenderSettings.skybox = scenario.skybox;
        RenderSettings.sun.transform.eulerAngles = scenario.sunRotation;
        RenderSettings.sun.intensity = scenario.sunIntensity;

        RenderSettings.customReflectionTexture = scenario.reflectionCubemap;
        RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Custom;
        DynamicGI.UpdateEnvironment();
    }

    /*
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //set active?
        ApplyLightSettings(currentLight);
    }*/
}