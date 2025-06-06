using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScrapPickup : MonoBehaviour
{
    public float pickupRange = 3f;

    private PlayerActions _playerActions;
    private PlayerInventory inventory;

    void Awake()
    {
        _playerActions = new PlayerActions();
        inventory = GetComponent<PlayerInventory>();
    }

    void OnEnable() 
    {
        _playerActions.Actions.Enable();
        _playerActions.Actions.pickup.performed += ctx => TryPickup();
    }
    void OnDisable()
    {
        _playerActions.Actions.Disable();
        _playerActions.Actions.pickup.performed += ctx => TryPickup();
    }

    void TryPickup()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("scrap"))
            {
                if (!inventory.canPickUp)
                {
                    return;
                }
                inventory.AddItem("Scrap");
                Destroy(hit.gameObject);
                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}