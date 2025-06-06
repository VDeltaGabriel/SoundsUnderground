using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SonarRenderer : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Material _sonarMaterial;
    [SerializeField] private Mesh _dotMesh;
    [SerializeField, Min(0.01f)] private float _radius = 1f;
    [SerializeField] private int _maxDots = 32000;
    
    private LimitedArray<SonarEntry> _points;
    const int batchSize = 1023;
    private Matrix4x4[] _matrices = new Matrix4x4[batchSize];
    private long _lastPointsHash = 0;
    private bool _updateNeeded = false;

    private void Awake()
    {
        _points = new LimitedArray<SonarEntry>(_maxDots);
    }

    private void OnEnable()
    {
        Sonar.OnSonarDispatched += UpdateSonar;
    }

    private void OnDisable()
    {
        Sonar.OnSonarDispatched -= UpdateSonar;
    }

    private void UpdateSonar(SonarEntry[] points)
    {
        foreach (SonarEntry entry in points) _points.Add(entry);
    }

    private long GetPointsHash()
    {
        Crc32 crc = new Crc32();
        foreach (SonarEntry entry in _points.Data)
        {
            crc.Update(entry.Position.GetHashCode());
        }

        return crc.Value;
    }
    
    private void Update()
    {
        if (_dotMesh == null || _sonarMaterial == null || _points.Count == 0) return;
        long pointsHash = GetPointsHash();
        if (_lastPointsHash != pointsHash)
        {
            _lastPointsHash = pointsHash;
            _updateNeeded = true;
        }
        
        int amount = _points.Count;

        int fullBatches = amount / batchSize; // Get full batches (that is full batchSize)
        int remaining = amount - fullBatches * batchSize; // Remaining points (one batch that will be below batchSize)

        for (int batch = 0; batch < fullBatches; batch++)
        {
            RenderParams rp = new RenderParams(_sonarMaterial);
            if (_updateNeeded)
            {
                // Only do all this if points were updated
                Quaternion facingRot = Quaternion.LookRotation(transform.forward, transform.up);
                for (int p = 0; p < batchSize; p++)
                {
                    SonarEntry entry = _points[batch * batchSize + p];
                    Vector3 point = entry.Position;
                    _matrices[p] = Matrix4x4.TRS(point, facingRot, Vector3.one * _radius);
                }
            }
            Graphics.RenderMeshInstanced(rp, _dotMesh, 0, _matrices);
        }
    }
}
