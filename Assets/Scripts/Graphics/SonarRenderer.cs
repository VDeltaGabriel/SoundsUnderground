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
    
    private LimitedList<Vector3> _points;
    const int batchSize = 1023;

    private void Awake()
    {
        _points = new LimitedList<Vector3>(_maxDots);
    }

    private void OnEnable()
    {
        Sonar.OnSonarDispatched += UpdateSonar;
    }

    private void OnDisable()
    {
        Sonar.OnSonarDispatched -= UpdateSonar;
    }

    private void UpdateSonar(Vector3[] points)
    {
        foreach (Vector3 point in points) _points.Add(point);
    }

    private void Update()
    {
        if (_dotMesh == null || _sonarMaterial == null || _points.Count == 0) return;
        int amount = _points.Count;

        int fullBatches = amount / batchSize; // Get full batches (that is full batchSize)
        int remaining = amount - fullBatches * batchSize; // Remaining points (one batch that will be below batchSize)

        Debug.Log($"");
        
        for (int batch = 0; batch < fullBatches; batch++)
        {
            RenderParams rp = new RenderParams(_sonarMaterial);
            Matrix4x4[] matrices = new Matrix4x4[batchSize];
            for (int p = 0; p < batchSize; p++)
            {
                Vector3 point = _points[batch * batchSize + p];
                matrices[p] = Matrix4x4.TRS(point, Quaternion.identity, Vector3.one * _radius);
            }
            Graphics.RenderMeshInstanced(rp, _dotMesh, 0, matrices);
        }
    }
}
