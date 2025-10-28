#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private string skinMenuSceneName = "SkinMenu";
    [SerializeField] private string mainGameSceneName = "MainGame";

    public void OpenSkins()
    {
        SceneManager.LoadScene(skinMenuSceneName);
    }

    public void Play()
    {
        SceneManager.LoadScene(mainGameSceneName);
    }

    public void Exit()
    {
        var mgr = ShellCoinManager.instance;
        if (mgr != null)
        {
            mgr.SaveNow();
        }

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
