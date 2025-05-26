using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryDef
{
    public EntryDef(GameObject point, RoomDef.EntryDir dir)
    {
        spawnPoint = point;
        direction = dir;
    }

    public Vector3 position => spawnPoint.transform.position;
    public GameObject spawnPoint;
    public RoomDef.EntryDir direction;
}

[DisallowMultipleComponent]
public class RoomDef : MonoBehaviour
{
    public enum EntryDir
    {
        UP,LEFT,DOWN,RIGHT
    }

    [SerializeField] private bool _isEntry = false;
    [SerializeField] private GameObject[] _spawnPoints;
    [SerializeField] private EntryDir[] _spawnPointsDirs;

    private List<EntryDef> _entries = new List<EntryDef>();
    private Transform _tr;

    private void Awake()
    {
        _tr = transform;
        for (int i = 0; i < _spawnPoints.Length; i++) _entries.Add(new EntryDef(_spawnPoints[i], _spawnPointsDirs[i]));
    }

    public bool IsEntry => _isEntry;
    public GameObject[] SpawnPoints => _spawnPoints;
    public EntryDir[] SpawnDirs => _spawnPointsDirs;
    public Vector3 Position => _tr.position;
    public List<EntryDef> Entries => _entries;
    
    public void SetPos(Vector3 pos)
    {
        _tr.position = pos;
    }
}
