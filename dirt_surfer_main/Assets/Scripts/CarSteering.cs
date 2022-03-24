using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSteering : MonoBehaviour
{
    private Rigidbody carRigidbody;
    private float forwardSpeed; // Speed along the local Z axis
    private float lateralSpeed; // Speed along the local X axis
    private Vector3 localVelocity;
    // localVelocity.z is forward speed
    // localVelocity.x is sideway speed
    public float slipAngle;

    public float input;
    private float currentSteer; // from -1 to 1
    public float maxAngle; // range of the wheel (in angle)
    public float assistStrengh;

    public float outputAngle;

    

    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        assistStrengh = Mathf.Clamp(assistStrengh, 10, 60);
        maxAngle = Mathf.Clamp(maxAngle, 10, 60);
    }
    void FixedUpdate()
    {
        localVelocity = transform.InverseTransformDirection(carRigidbody.velocity);
        slipAngle = Mathf.Rad2Deg * Mathf.Asin(localVelocity.x / carRigidbody.velocity.magnitude);

        slipAngle = 1 + Mathf.Clamp((input * slipAngle / 15), 0, 9);
        
    }
    public float GetSteer(float masterInput, float carSpeed)
    {
        input = masterInput;
        return GetSteer(carSpeed);
    }
    public float GetSteer(float carSpeed)
    {
        forwardSpeed = carSpeed;
        UpdateSteer();
        return outputAngle;
    }
    private void UpdateSteer()
    {
        //smooth the wheel turning speed
        currentSteer = Mathf.Clamp(currentSteer + (input - currentSteer) / 5, -1f, 1f);
        currentSteer = Mathf.Round(currentSteer * 100) / 100;

        //limit the turn angle based on current speed
        outputAngle = maxAngle * currentSteer * (assistStrengh*slipAngle / (Mathf.Abs(localVelocity.z) + slipAngle * assistStrengh));
    }
}
