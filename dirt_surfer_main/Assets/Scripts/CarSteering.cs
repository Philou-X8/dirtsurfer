using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSteering : MonoBehaviour
{
    private float forwardSpeed; // forward speed of the car

    public float input;
    private float currentSteer; // from -1 to 1
    public float maxAngle; // range of the wheel (in angle)
    public float assistStrengh;

    public float outputAngle;

    

    void Start()
    {
        assistStrengh = Mathf.Clamp(assistStrengh, 10, 50);
    }
    void FixedUpdate()
    {
        //UpdateSteer();
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

        //limit the turn angle based on current speed
        outputAngle = maxAngle * currentSteer * (assistStrengh / (Mathf.Abs(forwardSpeed) + assistStrengh));
    }
}
