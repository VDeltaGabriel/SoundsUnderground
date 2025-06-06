using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    public int maxItemCount = 3;
    public bool canPickUp = true;
    private PlayerActions playerActions;
    public List<string> inventory = new List<string>();
    private int selectedSlot = 0;

    public PlayerActions PlayerActions => playerActions;
    void Awake()
    {
        playerActions = new PlayerActions();     
    }

    void OnEnable() { 
        playerActions.Actions.Enable();
        playerActions.Actions.select1.performed += ctx => SelectSlot(0);
        playerActions.Actions.select2.performed += ctx => SelectSlot(1);
        playerActions.Actions.select3.performed += ctx => SelectSlot(2);
    }
    void OnDisable() { 
        playerActions.Actions.Disable();
        playerActions.Actions.select1.performed -= ctx => SelectSlot(0);
        playerActions.Actions.select2.performed -= ctx => SelectSlot(1);
        playerActions.Actions.select3.performed -= ctx => SelectSlot(2);
    }

    void SelectSlot(int slot)
    {
        if(slot < inventory.Count)
        {
            Debug.Log($"Selected: {inventory[slot]}");
            selectedSlot = slot + 1;
        }
        else
        {
            Debug.Log("Empty slot");
        }
        
    }

    private void Update()
    {
        if (inventory.Count >= maxItemCount)
        {
            Debug.Log("Inventory full");
            canPickUp = false;
        }
        else canPickUp = true;       
    }
    public void AddItem(string itemName)
    {
        
        inventory.Add(itemName);
        Debug.Log($"Added to inventory: {itemName}");
        
    }
}