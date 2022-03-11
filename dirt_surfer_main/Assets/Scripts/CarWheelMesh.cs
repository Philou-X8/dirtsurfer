using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarWheelMesh : MonoBehaviour
{
    public WheelCollider wheelCollider;
    private Vector3 wPosition;
    private Quaternion wRotation;

    
    void Update()
    {
        wheelCollider.GetWorldPose(out wPosition, out wRotation);
        transform.position = wPosition;
        transform.rotation = wRotation;
    }
}
