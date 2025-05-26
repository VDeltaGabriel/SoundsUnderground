using System;
using UnityEngine;

public class SettingsController : MenuController
{
    public static event Action<MenuController> EvShowMenu;

    public static void ShowMenu(MenuController menu)
    {
        EvShowMenu?.Invoke(menu);
    }
    
    [SerializeField] private SettingsView _settingsView;

    private void Awake()
    {
        EvShowMenu += Show;
        gameObject.SetActive(false);
    }
}