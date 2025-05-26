using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class Sonar : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Camera _cam;
    [SerializeField] private float _rayStep = 0.05f;

    [Header("Debug")]
    [SerializeField] private GameObject _debugPoint;
    private Player _player;

    public void SendSonar(InputAction.CallbackContext ctx)
    {
        for (float x = 0; x <= 1; x += _rayStep)
        {
            for (float y = 0; y <= 1; y += _rayStep)
            {
                Ray r = _cam.ViewportPointToRay(new Vector3(x,y,0));

                RaycastHit hit;
                if (Physics.Raycast(r, out hit, Single.MaxValue, LayerMask.NameToLayer("Player")))
                {
                    GameObject obj = Instantiate(_debugPoint, hit.point, Quaternion.identity);
                    Destroy(obj, 10f);
                }
            }
        }
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