using UnityEngine;

public class CameraMouv : MonoBehaviour
{
    public Transform target;
    public Vector3 cameraOffset;
    public float smoothTime;
    private Vector3 velocity = Vector3.zero;

    void FixedUpdate()
    {
        transform.position = Vector3.SmoothDamp(
            transform.position,
            target.TransformPoint(cameraOffset), 
            ref velocity, 
            smoothTime
            );

        transform.LookAt(target);
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, 0.3f);
        
    }
}
