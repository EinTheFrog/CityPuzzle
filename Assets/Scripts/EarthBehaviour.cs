using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthBehaviour : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1;
    
    private float _targetY;
    private int _rotationDir;

    private void Start()
    {
        _targetY = transform.eulerAngles.y;
        _rotationDir = 0;
    }

    private void Update()
    {
        var eulerAngles = transform.eulerAngles;
        var step = rotationSpeed * Time.deltaTime;
        var distance = Mathf.Abs(_targetY - eulerAngles.y);
        if (distance < step)
        {
            transform.eulerAngles = new Vector3(eulerAngles.x, _targetY, eulerAngles.z);
            return;
        }

        var newY = (eulerAngles.y + _rotationDir * step) % 360;
        transform.eulerAngles = new Vector3(eulerAngles.x, newY, eulerAngles.z);
    }

    public void RotateRight()
    {
        if (Math.Abs(transform.eulerAngles.y - _targetY) > 0.01) return;
        var eulerAngles = transform.eulerAngles;
        _targetY = (eulerAngles.y + 90) % 360;
        _rotationDir = 1;
    }

    public void RotateLeft()
    {
        if (Math.Abs(transform.eulerAngles.y - _targetY) > 0.01) return;
        var eulerAngles = transform.eulerAngles;
        _targetY = (eulerAngles.y + 270) % 360;
        _rotationDir = -1;
    }
}
