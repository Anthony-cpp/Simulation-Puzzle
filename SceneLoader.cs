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
        // ���̃V�[���œǂݍ��� CSV ��ۑ�
        PlayerPrefs.SetString("NextCSV", csvFileName);
        PlayerPrefs.Save(); // �O�̂���

        SceneManager.LoadScene(sceneName);
    }

}
