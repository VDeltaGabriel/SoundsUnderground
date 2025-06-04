using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class RoomDef : MonoBehaviour
{
    [SerializeField] private GameObject _model;
    
    public GameObject Model => _model;

    public Bounds ModelBounds => _model.GetComponent<MeshFilter>().sharedMesh.bounds;

    public void InitRoom()
    {
        _model.GetComponent<MeshRenderer>().enabled = false;
    }
}
