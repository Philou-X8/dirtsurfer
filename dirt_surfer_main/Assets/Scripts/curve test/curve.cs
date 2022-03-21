using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class curve : MonoBehaviour
{
    public Transform p1;
    public Transform p2;
    public Transform p3;
    public Transform p4;

    private Vector3 a12;
    private Vector3 a23;
    private Vector3 a34;

    private Vector3 b12;
    private Vector3 b23;

    private Vector3 c12;

    public float pos;

    // Start is called before the first frame update
    void Start()
    {
        pos = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        pos = (pos + 0.01f) % 1;

        a12 = Vector3.Lerp(p1.position, p2.position, pos);
        a23 = Vector3.Lerp(p2.position, p3.position, pos);
        a34 = Vector3.Lerp(p3.position, p4.position, pos);

        b12 = Vector3.Lerp(a12, a23, pos);
        b23 = Vector3.Lerp(a23, a34, pos);

        c12 = Vector3.Lerp(b12, b23, pos);

        transform.position = c12;
    }

    
}
