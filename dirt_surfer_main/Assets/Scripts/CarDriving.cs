using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class CarDriving : MonoBehaviour
{
    //public InputAction throttle;
    //public InputAction steering;
    //public InputAction handbrake;

    public Vector3 CoM;
    private Rigidbody carRigidbody;

    private float horizontalInput;
    private float verticalInput;
    private float handBrakeInput;

    public float maxMotorForce;
    public float breakForce;
    public float maxSteerAngle;
    public float steeringAssist = 30;
    private float currentSteerAngle = 0f;

    public float engineRPM = 1000;
    private const float idleRPM = 0;
    private const float maxRPM = 10000;

    public float carSpeed;
    public float wheelSpeed;
    public float deltaSpeed;

    public WheelCollider wColliderFL;
    public WheelCollider wColliderFR;
    public WheelCollider wColliderBL;
    public WheelCollider wColliderBR;

    
    public void OnThrottle(InputValue input)
    {
        print("gas: " + input.Get<float>());
        verticalInput = input.Get<float>();
    }
    public void OnSteering(InputValue input)
    {
        print("steer: " + input.Get<float>());
        horizontalInput = input.Get<float>();
    }
    public void OnHandbrake(InputValue input)
    {
        print("handBrake: " + input.Get<float>());
        handBrakeInput = input.Get<float>();
    }

    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        carRigidbody.centerOfMass = CoM;

        steeringAssist = Mathf.Clamp(steeringAssist, 10, 50);

        horizontalInput = 0f;
        verticalInput = 0f;
        handBrakeInput = 0f;
    }

    
    void FixedUpdate()
    {
        carSpeed = transform.InverseTransformDirection(carRigidbody.velocity).z;

        //carSpeed = carRigidbody.velocity.magnitude;
        float agvWheelRpm = (wColliderBR.rpm + wColliderBL.rpm + wColliderFR.rpm + wColliderFL.rpm) / 4;
        wheelSpeed = Mathf.PI * wColliderBR.radius * agvWheelRpm / 60;
        deltaSpeed = wheelSpeed - carSpeed;

        ApplyTorque();
        HandleSteering();
        ApplyBrakeHand();
        
    }

    private void ApplyTorque()
    {
        
        if (verticalInput != 0)
        {
            engineRPM += 100 * Mathf.Abs(verticalInput);
        }
        else
        {
            engineRPM -= 100;
        }
        engineRPM = Mathf.Clamp(engineRPM, idleRPM, maxRPM);
        float outputTorque = verticalInput * maxMotorForce * engineRPM / maxRPM;
        wColliderFL.motorTorque = outputTorque;
        wColliderFR.motorTorque = outputTorque;
        wColliderBL.motorTorque = outputTorque;
        wColliderBR.motorTorque = outputTorque;

    }

    private void HandleSteering()
    {
        //smooth the wheel turning speed
        currentSteerAngle = Mathf.Clamp(currentSteerAngle + (horizontalInput-currentSteerAngle)/5, -1f, 1f);

        //limit the turn angle based on current speed
        float targetSteer = maxSteerAngle * currentSteerAngle * (steeringAssist/(Mathf.Abs(carSpeed)+steeringAssist));
        wColliderFL.steerAngle = targetSteer;
        wColliderFR.steerAngle = targetSteer;
    }

    private void ApplyBrakeHand()
    {
        wColliderBL.brakeTorque = handBrakeInput * breakForce;
        wColliderBR.brakeTorque = handBrakeInput * breakForce;
    }


}
