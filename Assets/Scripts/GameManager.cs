using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this) Destroy(gameObject);
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void StartGame()
    {
        LoadScene("game");
    }
    
    public void QuitGame()
    {
        Debug.Log("Quitting game");
        Application.Quit();
    }
}
