using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class OpenCabinet : MonoBehaviour
{
    public float closeAngle = 0f;
    public float openAngle = 90f;
    public float rotationSpeed = 180f;

    private float _targetY;
    private bool _statusKabinet;

    void Start()
    {
        Vector3 e = transform.localEulerAngles;
        e.y = closeAngle;
        transform.localEulerAngles = e;
        _targetY = closeAngle;
    }

    public void InteractKeCabinet()
    {
        if (_statusKabinet == false)
        {
            _targetY = openAngle;
            _statusKabinet = !_statusKabinet;
        }
        else
        {
            _targetY = closeAngle;
            _statusKabinet = !_statusKabinet;
        }
        StartCoroutine(RotateDoor(_targetY));
    }

    private IEnumerator RotateDoor(float targetY)
    {
        Vector3 e = transform.localEulerAngles;
        
        while (Mathf.Abs(Mathf.DeltaAngle(e.y, targetY)) >= 0.1f)
        {
            e.y = Mathf.MoveTowardsAngle(e.y, targetY, Time.deltaTime * rotationSpeed);
            transform.localEulerAngles = e;

            yield return null;
        }

        e.y = targetY;
        transform.localEulerAngles = e;
    }
}
