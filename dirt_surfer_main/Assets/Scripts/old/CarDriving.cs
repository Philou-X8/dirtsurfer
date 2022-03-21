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

    //public float engineRPM = 1000;
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
        verticalInput = input.Get<float>();
        engine.input = input.Get<float>();

        if (verticalInput == 0) 
        {
            ApplyPedalBrake(0f);
            print("case 1 / vInput: " + verticalInput);
        }
        //else if( (Mathf.Abs(carSpeed) > 10) == (verticalInput < 0) )
        else if ( ((verticalInput>0) == (carSpeed<-10)) && (Mathf.Abs(carSpeed) > 10))
        {
            ApplyPedalBrake(Mathf.Abs(verticalInput));
            ApplyTorque(0f);
            print("case 2 / vInput: " + verticalInput);

        }
        else if (true)
        {
            ApplyPedalBrake(0f);
            ApplyTorque(verticalInput);
            print("case 3 / vInput: " + verticalInput);

        }

    }
    public void OnSteering(InputValue input)
    {
        horizontalInput = input.Get<float>();
        //print("steer: " + horizontalInput);
    }
    public void OnHandbrake(InputValue input)
    {
        handBrakeInput = input.Get<float>();
        //print("handBrake: " + handBrakeInput);

        ApplyHandBrake();
    }

    void Start()
    {
        engine.RPM++;
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

        //ApplyTorque();
        UpdateEngineRPM();
        HandleSteering();
        //ApplyHandBrake();
        
    }

    private void UpdateEngineRPM()
    {
        
        if (verticalInput != 0)
        {
            engine.RPM += 100 * Mathf.Abs(verticalInput); 
        }
        else
        {
            engine.RPM -= 200;
        }
        engine.RPM = Mathf.Clamp(engine.RPM, idleRPM, maxRPM);
    }
    private void ApplyTorque(float torqueInput)
    {

        // (0.5f + torqueInput/2)
        float outputTorque = torqueInput * maxMotorForce; // * engineRPM / maxRPM;
        wColliderFL.motorTorque = outputTorque;
        wColliderFR.motorTorque = outputTorque;
        wColliderBL.motorTorque = outputTorque;
        wColliderBR.motorTorque = outputTorque;

    }
    private void ApplyPedalBrake(float brakeInput)
    {
        wColliderFL.brakeTorque = brakeInput * breakForce;
        wColliderFR.brakeTorque = brakeInput * breakForce;
        wColliderBL.brakeTorque = brakeInput * breakForce;
        wColliderBR.brakeTorque = brakeInput * breakForce;
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

    private void ApplyHandBrake()
    {
        wColliderBL.brakeTorque = handBrakeInput * breakForce;
        wColliderBR.brakeTorque = handBrakeInput * breakForce;
    }

    [System.Serializable]
    public class Engine
    {
        public float input;
        public float RPM;
        public const float idleRPM = 0;
        public const float maxRPM = 10000;

        public const float maxForce = 500;
        public float outputForce;
    }
    public Engine engine;
}
