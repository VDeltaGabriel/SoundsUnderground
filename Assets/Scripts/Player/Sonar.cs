using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Profiling;

[Serializable]
public struct SonarEntry
{
    public Vector3 Position;
    public SonarEntity Entity;
}

[DisallowMultipleComponent]
public class Sonar : MonoBehaviour
{
    public static event UnityAction<SonarEntry[]> OnSonarDispatched; 
    
    [Header("Config")]
    [SerializeField] private Camera _cam;
    [SerializeField] private float _rayStep = 0.05f;

    [Header("Debug")]
    [SerializeField] private GameObject _debugPoint;
    private Player _player;

    private NativeArray<RaycastCommand> _rays;
    private NativeArray<RaycastHit> _hits;
    private int _row;

    private QueryParameters _query = new QueryParameters();

    private void Awake()
    {
        _row = (int)(1.0f / _rayStep) + 1;
        _rays = new NativeArray<RaycastCommand>(_row*_row, Allocator.Persistent);
        _hits = new NativeArray<RaycastHit>(_row*_row, Allocator.Persistent);
        _query.layerMask = LayerMask.NameToLayer("Player");
    }

    private void OnDestroy()
    {
        _rays.Dispose();
        _hits.Dispose();
    }

    private RaycastCommand CreateSonarCommand(Vector3 origin, Vector3 dir, float dst)
    {
        return new RaycastCommand
        {
            queryParameters = _query,
            direction = dir,
            distance = dst,
            from = origin,
        };
    }
    
    private JobHandle CreateSonarJob(NativeArray<RaycastCommand> cmds, NativeArray<RaycastHit> hits)
    {
        JobHandle job = RaycastCommand.ScheduleBatch(cmds, hits, 1);
        return job;
    }

    public void SendSonar(InputAction.CallbackContext ctx)
    {
        Profiler.BeginSample("SonarScan");
        List<SonarEntry> points = new List<SonarEntry>();
        
        int idx = 0;
        for (float x = 0; x <= 1; x += _rayStep)
        {
            for (float y = 1; y >= 0; y -= _rayStep)
            {
                Ray r = _cam.ViewportPointToRay(new Vector3(x,y,0));

                _rays[idx] = CreateSonarCommand(r.origin, r.direction, Single.MaxValue);

                idx++;
            }
        }
        
        JobHandle sonarJob = CreateSonarJob(_rays, _hits);
        sonarJob.Complete();

        foreach (RaycastHit hit in _hits)
        {
            SonarEntry entry = new SonarEntry();
            entry.Position = hit.point;

            SonarEntity ent = hit.transform.GetComponent<SonarEntity>();
            if (ent)
            {
                entry.Entity = ent;
            }
                    
            points.Add(entry);
        }
        
        Profiler.EndSample();
        OnSonarDispatched?.Invoke(points.ToArray());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        for (float x = 0; x <= 1; x += _rayStep)
        {
            for (float y = 1; y >= 0; y -= _rayStep)
            {
                Ray r = _cam.ViewportPointToRay(new Vector3(x,y,0));
                Gizmos.DrawRay(r.origin, r.direction);
            }
        }
    }
}