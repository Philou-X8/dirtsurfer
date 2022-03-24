using System.Collections;
using System.Collections.Generic;

using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerCar : MonoBehaviour
{
    public CarEngine carEngine;
    public CarSteering carSteering;
    private Rigidbody carRigidbody;

    public Vector3 CoM;

    private float forwardInput; // how much you want to accelerate (0 to 1)
    private float reverseInput; // how much you want to decelerate (0 to -1)
    private float throttleInput; // computed result of forward and reverse
    private float steerInput; // steering scale from -1 (left) to 1 (right)
    
    public float carSpeed; // speed parallel to the car body (m/s)
    public float wheelRPM; // average wheel rpm

    public float driftScale;

    private float steerAngle;
    private float wheelTorque;
    private float pedalBrakeForce;
    private float handBrakeForce;

    public WheelCollider wColliderFL;
    public WheelCollider wColliderFR;
    public WheelCollider wColliderBL;
    public WheelCollider wColliderBR;

    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        //Set the car's center of mass (CoM)
        carRigidbody.centerOfMass = CoM;

    }

    void FixedUpdate()
    {
        // average the rpm of all 4 wheel
        wheelRPM = (wColliderBR.rpm + wColliderBL.rpm + wColliderFR.rpm + wColliderFL.rpm) / 4;

        carSpeed = transform.InverseTransformDirection(carRigidbody.velocity).z;
        float xSpeed = transform.InverseTransformDirection(carRigidbody.velocity).x;

        ConvertThrottle();
        steerAngle = carSteering.GetSteer(steerInput, carSpeed);
        wheelTorque = carEngine.GetTorque(throttleInput, wheelRPM, 60 * carSpeed / (wColliderFR.radius * Mathf.PI));
        ApplySteering();
        ApplyTorque();
        ApplyBrake();

        
        driftScale = Mathf.Rad2Deg * Mathf.Asin(xSpeed / carRigidbody.velocity.magnitude);
        
    }
    // --------------------------- user input ---------------------
    public void OnSteering(InputValue input)
    {
        steerInput = input.Get<float>();
    }
    public void OnForward(InputValue input)
    {
        forwardInput = input.Get<float>();
    }
    public void OnReverse(InputValue input)
    {
        reverseInput = -input.Get<float>();
    }
    public void OnHandbrake(InputValue input)
    {
        handBrakeForce = input.Get<float>();
    }
    public void OnGearUp(InputValue input)
    {
        if(carEngine.gear<10) carEngine.gear++;
    }
    public void OnGearDown(InputValue input)
    {
        if (carEngine.gear > 1) carEngine.gear--;
    }
    public void OnRespawn()
    {
        transform.position = new Vector3(0, 11, 0);
    }
    // --------------------------- user input end -----------------
    private void ConvertThrottle()
    {
        throttleInput = forwardInput+reverseInput;

        carSpeed = transform.InverseTransformDirection(carRigidbody.velocity).z;
        if (carSpeed > 5)
        {
            throttleInput = Mathf.Clamp01(throttleInput);
            pedalBrakeForce = -reverseInput;
        } else if (carSpeed < -5)
        {
            throttleInput = Mathf.Clamp(throttleInput, -1, 0);
            pedalBrakeForce = forwardInput;
        } else
        {
            pedalBrakeForce = 0f;
        }
    }
    private void ApplyTorque()
    { 
        wColliderFL.motorTorque = wheelTorque;
        wColliderFR.motorTorque = wheelTorque;
        wColliderBL.motorTorque = wheelTorque;
        wColliderBR.motorTorque = wheelTorque;
    }
    private void ApplyBrake()
    {
        wColliderFL.brakeTorque = pedalBrakeForce * 500;
        wColliderFR.brakeTorque = pedalBrakeForce * 500;
        wColliderBL.brakeTorque = Mathf.Max(pedalBrakeForce*500, handBrakeForce*1000);
        wColliderBR.brakeTorque = Mathf.Max(pedalBrakeForce*500, handBrakeForce*1000);
    }
    private void ApplySteering()
    {
        wColliderFL.steerAngle = steerAngle;
        wColliderFR.steerAngle = steerAngle;
    }


}
