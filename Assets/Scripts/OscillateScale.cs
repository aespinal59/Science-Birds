using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscillateScale : MonoBehaviour
{
    public float A;
    public float b;

    private Vector3 initScale;

    void Awake()
    {
        initScale = transform.localScale;
    }

    void FixedUpdate()
    {
        float newScaleShift = A * Mathf.Sin(b * Time.time);
        transform.localScale = new Vector3(initScale.x + newScaleShift, initScale.y + newScaleShift, initScale.z);
    }
}
