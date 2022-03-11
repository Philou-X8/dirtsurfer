using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{

    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private float currentBreakForce;
    private float currentSteerAngle;
    private bool isBreaking;

    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;

    [SerializeField] private WheelCollider wColliderFL;
    [SerializeField] private WheelCollider wColliderFR;
    [SerializeField] private WheelCollider wColliderBL;
    [SerializeField] private WheelCollider wColliderBR;

    [SerializeField] private Transform wTransformFL;
    [SerializeField] private Transform wTransformFR;
    [SerializeField] private Transform wTransformBL;
    [SerializeField] private Transform wTransformBR;

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        wColliderFL.motorTorque = verticalInput * motorForce;
        wColliderFR.motorTorque = verticalInput * motorForce;
        wColliderBL.motorTorque = verticalInput * motorForce;
        wColliderBR.motorTorque = verticalInput * motorForce;
        currentBreakForce = isBreaking ? breakForce : 0;
        
        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        wColliderFL.brakeTorque = currentBreakForce;
        wColliderFR.brakeTorque = currentBreakForce;
        wColliderBL.brakeTorque = currentBreakForce;
        wColliderBR.brakeTorque = currentBreakForce;
    }

    

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        wColliderFL.steerAngle = currentSteerAngle;
        wColliderFR.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(wColliderFL, wTransformFL);
        UpdateSingleWheel(wColliderFR, wTransformFR);
        UpdateSingleWheel(wColliderBL, wTransformBL);
        UpdateSingleWheel(wColliderBR, wTransformBR);
    }

    private void UpdateSingleWheel(WheelCollider wCollider, Transform wTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wCollider.GetWorldPose(out pos, out rot);
        wTransform.rotation = rot;
        wTransform.position = pos;
    }
}
