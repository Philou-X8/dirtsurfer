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

    private float forwardInput;
    private float reverseInput;
    private float throttleInput;
    public float carSpeed;

    public float wheelRPM;

    public float steerAngle;
    public float wheelTorque;
    public float pedalBrakeForce;
    public float handBrakeForce;

    public WheelCollider wColliderFL;
    public WheelCollider wColliderFR;
    public WheelCollider wColliderBL;
    public WheelCollider wColliderBR;

    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        //Set the car's center of mass (CoM)
        carRigidbody.centerOfMass = CoM;

        wheelTorque = carEngine.GetTorque(wheelRPM);
    }

    void FixedUpdate()
    {
        wheelRPM = (wColliderBR.rpm + wColliderBL.rpm + wColliderFR.rpm + wColliderFL.rpm) / 4;

        carSpeed = transform.InverseTransformDirection(carRigidbody.velocity).z;

        ConvertThrottle();

        steerAngle = carSteering.GetSteer(carSpeed);
        wheelTorque = carEngine.GetTorque(wheelRPM);
        ApplySteering();
        ApplyTorque();
        ApplyBrake();
    }

    public void OnSteering(InputValue input)
    {
        carSteering.input = input.Get<float>();
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
        handBrakeForce *= 1000;
    }

    public void OnGearUp(InputValue input)
    {
        if(carEngine.gear<10) carEngine.gear++;
    }
    public void OnGearDown(InputValue input)
    {
        if (carEngine.gear > 1) carEngine.gear--;
    }

    private void ConvertThrottle()
    {
        throttleInput = forwardInput+reverseInput;

        carSpeed = transform.InverseTransformDirection(carRigidbody.velocity).z;
        if (carSpeed > 5)
        {
            carEngine.input = Mathf.Clamp01(throttleInput);
            pedalBrakeForce = -reverseInput;
        } else if (carSpeed < -5)
        {
            carEngine.input = Mathf.Clamp(throttleInput, -1, 0);
            pedalBrakeForce = forwardInput;
        } else
        {
            carEngine.input = throttleInput;
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
        wColliderBL.brakeTorque = Mathf.Max(pedalBrakeForce*500, handBrakeForce);
        wColliderBR.brakeTorque = Mathf.Max(pedalBrakeForce*500, handBrakeForce);
    }
    private void ApplySteering()
    {
        wColliderFL.steerAngle = steerAngle;
        wColliderFR.steerAngle = steerAngle;
    }


}
