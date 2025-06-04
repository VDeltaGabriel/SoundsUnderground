using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class Sonar : MonoBehaviour
{
    public static event UnityAction<Vector3[]> OnSonarDispatched; 
    
    [Header("Config")]
    [SerializeField] private Camera _cam;
    [SerializeField] private float _rayStep = 0.05f;

    [Header("Debug")]
    [SerializeField] private GameObject _debugPoint;
    private Player _player;

    public void SendSonar(InputAction.CallbackContext ctx)
    {
        List<Vector3> points = new List<Vector3>();
        for (float x = 0; x <= 1; x += _rayStep)
        {
            for (float y = 0; y <= 1; y += _rayStep)
            {
                Ray r = _cam.ViewportPointToRay(new Vector3(x,y,0));

                RaycastHit hit;
                if (Physics.Raycast(r, out hit, Single.MaxValue, LayerMask.NameToLayer("Player")))
                {
                    if (false)
                    {
                        GameObject obj = Instantiate(_debugPoint, hit.point, Quaternion.identity);
                        Destroy(obj, 10f);
                    }

                    points.Add(hit.point);
                }
            }
        }
        Debug.Log($"Sent {points.Count} points");
        OnSonarDispatched?.Invoke(points.ToArray());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        for (float x = 0; x <= 1; x += _rayStep)
        {
            for (float y = 0; y <= 1; y += _rayStep)
            {
                Ray r = _cam.ViewportPointToRay(new Vector3(x,y,0));
                Gizmos.DrawRay(r.origin, r.direction);
            }
        }
    }
}