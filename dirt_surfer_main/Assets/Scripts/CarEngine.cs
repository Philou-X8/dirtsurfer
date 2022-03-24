using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour
{
    private Rigidbody carRigidbody;

    public float maxForce;
    public float maxRPM;
    public const float idleRPM = 0;

    public float input;
    public float RPM;
    public int gear;

    private float wheelRPM;
    public float efficiency;
    private float outputForce;

    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        gear = 1;
        input = 0f;
        RPM = 0f;
        maxRPM = Mathf.Clamp(maxRPM, 1000, 10000);
        maxForce = Mathf.Clamp(maxForce, 10, 10000);
    }
    private void FixedUpdate()
    {
        
    }

    public float GetTorque(float masterInput, float avgWheelRPM, float convertedSpeed)
    {
        input = masterInput;
        return GetTorque(avgWheelRPM, convertedSpeed);
    }
    public float GetTorque(float avgWheelRPM, float convertedSpeed)
    {
        wheelRPM = Mathf.Clamp(avgWheelRPM, 0f, 2000f);
        ShiftGear(convertedSpeed); 
        WheelToEngineRPM();
        EngineEfficiency();
        ApplyEfficiency();
        return outputForce;
    }
    private void ShiftGear(float convertedSpeed)
    {
        gear = Mathf.Clamp(Mathf.CeilToInt(wheelRPM / 200), 1, 10);
        //gear = Mathf.Clamp(Mathf.CeilToInt((wheelRPM+convertedSpeed)/2 / 200), 1, 10);
    }
    private void WheelToEngineRPM()
    {
        //convert wheel RPM to Engine RPM
        RPM = Mathf.Clamp(
            10f * maxRPM * wheelRPM / (2000f * gear), 
            idleRPM, 
            1.1f * maxRPM
            );
    }
    private void EngineEfficiency()
    {
        float var1 = gear * RPM / maxRPM + 0.2f;
        float var2 = 1.2f * (var1 - gear) / Mathf.Sqrt(gear);
        float var3 = Mathf.Pow((Mathf.Sin(var2) / var2), 5f) - Mathf.Pow(var1/10f, 2f);
        float var4 = var3 - Mathf.Pow(10f, (RPM - 1.1f * maxRPM) / 500f);
        efficiency = Mathf.Clamp01(var4);
    }
    private void ApplyEfficiency()
    {
        outputForce = input * efficiency * maxForce;
    }
    
}
