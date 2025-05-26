using System;
using UnityEngine;

public abstract class MenuController : MonoBehaviour
{
    protected MenuController _previousMenu;
    
    public virtual void Show(MenuController previous)
    {
        if (previous != null)
        {
            _previousMenu = previous;
            _previousMenu.Hide();
        }
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
        if (_previousMenu != null) _previousMenu.Show(null);
    }
}