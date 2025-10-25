using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{
    private StageManager stage;

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneWithCSV(string sceneName, string csvFileName)
    {
        // Ÿ‚ÌƒV[ƒ“‚Å“Ç‚İ‚Ş CSV ‚ğ•Û‘¶
        PlayerPrefs.SetString("NextCSV", csvFileName);
        PlayerPrefs.Save(); // ”O‚Ì‚½‚ß

        SceneManager.LoadScene(sceneName);
    }

}
