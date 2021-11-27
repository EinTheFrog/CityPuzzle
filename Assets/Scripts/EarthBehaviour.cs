using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthBehaviour : MonoBehaviour
{
    private Animator _anim;

    private void Start()
    {
        _anim = GetComponent<Animator>();
    }

    public void RotateRight()
    {
        _anim.Play("EarthRightRotation");
    }

    public void RotateLeft()
    {
        _anim.Play("EarthLeftRotation");
    }
}
