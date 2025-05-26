using System;
using UnityEngine;

public class MainMenucontroller : MenuController
{
    [SerializeField] private MainMenuView _mainMenuView;

    public void ShowOptions()
    {
        SettingsController.ShowMenu(this);
    }
}