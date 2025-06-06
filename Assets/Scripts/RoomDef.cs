using System;
using System.Collections;
using System.Collections.Generic;
using Facility;
using UnityEngine;

[DisallowMultipleComponent]
public class RoomDef : MonoBehaviour
{
    [SerializeField] private GameObject _model;
    public Direction[] AvailableDirections;
    
    public GameObject Model => _model;

    public Bounds ModelBounds => _model.GetComponent<MeshFilter>().sharedMesh.bounds;

    public void InitRoom()
    {
        _model.GetComponent<MeshRenderer>().enabled = false;
    }
}
