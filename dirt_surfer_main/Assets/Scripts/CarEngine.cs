using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour
{
    public float input;
    public float RPM;
    private const float idleRPM = 0;
    
    private const float maxForce = 500;
    public float outputForce;

    private const float maxRPM = 10000;
    public int gear;
    public float wheelRPM;
    public float efficiency;

    void Start()
    {
        gear = 1;
        input = 0f;
        RPM = 0f;
        //UpdateRPM();
    }

    void FixedUpdate()
    {
        //UpdateRPM();
    }

    public float GetTorque(float avgWheelRPM)
    {
        wheelRPM = Mathf.Clamp(avgWheelRPM, 0f, 2000f);
        ShiftGear(); 
        WheelToEngineRPM();
        EngineEfficiency();
        ApplyEfficiency();
        return outputForce;
    }
    private void ShiftGear()
    {
        gear = Mathf.Clamp(Mathf.CeilToInt(wheelRPM / 200), 1, 10);
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
    /*
    private void ConvertRPM()
    {
        gear = Mathf.Clamp( Mathf.RoundToInt(wheelRPM / 200) , 1, 10 );
        RPM = Mathf.Clamp(50 * wheelRPM / gear, idleRPM, 100000f);
        //RPM = 50 * wheelRPM / gear;
    }
    
    private void EngineEfficiency()
    {
        float var1 = gear * RPM / 10000f;
        float var2 = 1.2f * (var1 - gear) / Mathf.Sqrt(gear);
        efficiency = Mathf.Pow(Mathf.Sin(var2) / var2, 5) - Mathf.Pow(var1 / 10f, 2);
        efficiency = Mathf.Clamp01(efficiency);
    }
    
    private void UpdateRPM()
    {
        if (input != 0)
        {
            RPM += 200 * Mathf.Abs(input);
        }
        else
        {
            RPM -= 300;
        }
        RPM = Mathf.Clamp(RPM, idleRPM, maxRPM);
        outputForce = input * efficiency * maxForce;
    }
    */
}
