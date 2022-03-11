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
    private bool handBrakeInput;

    public float maxMotorForce;
    public float breakForce;
    public float maxSteerAngle;
    public float steeringAssist = 30;

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
        handBrakeInput = input.Get<float>() == 1;
    }

    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        carRigidbody.centerOfMass = CoM;

        steeringAssist = Mathf.Clamp(steeringAssist, 10, 50);

        horizontalInput = 0f;
        verticalInput = 0f;
        handBrakeInput = false;
    }

    private void Update()
    {
        
        GetInput();
    }

    void FixedUpdate()
    {
        carSpeed = transform.InverseTransformDirection(carRigidbody.velocity).z;

        //carSpeed = carRigidbody.velocity.magnitude;
        wheelSpeed = Mathf.PI * wColliderBR.radius * wColliderBR.rpm / 60;
        deltaSpeed = wheelSpeed - carSpeed;

        ApplyTorque();
        HandleSteering();

        wColliderBL.brakeTorque = handBrakeInput ? breakForce : 0f;
        wColliderBR.brakeTorque = handBrakeInput ? breakForce : 0f;
    }

    private void GetInput()
    {
        
        //horizontalInput = Input.GetAxis("Horizontal");
        //verticalInput = Input.GetAxis("Vertical");
        //handBrakeInput = ( Input.GetKey(KeyCode.Space) || Input.GetButton("Fire1") );
        //Input.GetAxis("10th Axis");
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
        wColliderFL.motorTorque = verticalInput * maxMotorForce * engineRPM / maxRPM;
        wColliderFR.motorTorque = verticalInput * maxMotorForce * engineRPM / maxRPM;
        wColliderBL.motorTorque = verticalInput * maxMotorForce * engineRPM / maxRPM;
        wColliderBR.motorTorque = verticalInput * maxMotorForce * engineRPM / maxRPM;

    }

    private void HandleSteering()
    {

        float targetSteer = maxSteerAngle * horizontalInput * (steeringAssist/(carSpeed+steeringAssist));
        wColliderFL.steerAngle = targetSteer;
        wColliderFR.steerAngle = targetSteer;
    }
    
}
