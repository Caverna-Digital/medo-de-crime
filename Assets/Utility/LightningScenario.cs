using UnityEngine;
//using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "LightningScenario", menuName = "Scriptable Objects/LightningScenario")]
public class LightningScenario : ScriptableObject
{
    public Texture2D[] lightmapsColor;
    public Texture2D[] lightmapsDir;

    //public Material skybox;
    public string baseScene;             //--> geometry scene (different lightining settings must use the exact same static geometry)
    public Cubemap reflectionCubemap;   //--> reflection probe 0 -created on bake lightings
    public Vector3 sunRotation;
    public float sunIntensity;
}
