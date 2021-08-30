using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthBehaviour : MonoBehaviour
{
    public void RotateBy90()
    {
        var anim = GetComponent<Animator>();
        anim.Play("EarthRotation");
    }
}
