using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dashboard : MonoBehaviour
{
    public PlayerCar playerCarObj;
    public TextMeshProUGUI speedTxt;
    
    public CarEngine carEngine;
    public TextMeshProUGUI rpmTxt;

    public RectTransform rpmTransf;

    public TextMeshProUGUI gearTxt;

    // Start is called before the first frame update
    void Start()
    {
        //rpmTransf = rpmNeedle.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        speedTxt.text = (Mathf.Round(playerCarObj.carSpeed * 36) / 10).ToString() + 
            " km/h\n" + 
            (Mathf.Round(playerCarObj.carSpeed * 10) / 10).ToString() + 
            " m/s";

        rpmTxt.text = (Mathf.Round(carEngine.RPM/100)*100).ToString() + " rpm";

        //rpmTransf.localPosition.Set(267 + carEngine.RPM / 200, -280, 0);
        //rpmTransf.anchoredPosition.Set(267 + carEngine.RPM / 200, -280);
        rpmTransf.localPosition = new Vector3(267 + carEngine.RPM / 52, -280, 0);

        gearTxt.text = carEngine.gear.ToString();
    }
}
