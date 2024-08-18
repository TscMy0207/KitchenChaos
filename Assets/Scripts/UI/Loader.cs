using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader {
    public enum Scene
    {
        MainMenuScene,
        GameScenes,
        LoadScenes,
    }
    public static Scene targetScene;

    public static void Load(Scene targetScene)
    {
        Loader.targetScene = targetScene;
        SceneManager.LoadScene(Scene.LoadScenes.ToString());
    }

    public static void LoadCallBack()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
