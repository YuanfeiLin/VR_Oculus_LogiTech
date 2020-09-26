using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveZinkStelle : MonoBehaviour
{
    /**************************************************
     * MoveZinkStelle.cs
     * 
     * Lin Yuanfei 23.11.2019, yuanfei.lin@tum.de
     * 
     * die Bewegung der Zinkenstelle kontrollieren
     * (links und rechts)
     *   
     * Geschwindigkeit: 0.1m/s
     * 
     **************************************************/
    // Start is called before the first frame update
    public Rigidbody Zinkenstelle;
    public Text ZinkenMoveIndex;
    private int IsZinkenMove=0;
    const float MoveVel = 10f;

    //Makro
    const float ZS_LOWER_LIMIT = -220f;// ZS : Zinkenstelle
    const float ZS_UPPER_LIMIT = 220f;
    // Update is called once per frame
    void Update()
    {
        // Wenn Zinkenstelle sich nicht bewegt, wird der Zutstand immer auf null gesetzt
        IsZinkenMove = 0;
        // .text Element dient zur Teilung des Zustands von Zinkenstelle, ist dies änhlich wie Palette
        ZinkenMoveIndex.text = IsZinkenMove.ToString();

        // Wenn beide Joysticks nach links oder rechts bewegen, bewegt die Zinkenstelle nach links oder rechts 
        if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight) || Input.GetKey("l") )
        {
            if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight) || Input.GetKey("d"))
            {
                //Zustand verändert sich und zur .text senden
                IsZinkenMove = 1;
                ZinkenMoveIndex.text = IsZinkenMove.ToString();
                if (Zinkenstelle.transform.localPosition.x <= ZS_UPPER_LIMIT)
                {
                    Zinkenstelle.transform.Translate(MoveVel * Time.deltaTime, 0, 0);
                }
            }
        }
        if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft) || Input.GetKey("a"))
        {
            if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft) || Input.GetKey("j"))
            {
                IsZinkenMove = 1;
                ZinkenMoveIndex.text = IsZinkenMove.ToString();
                if (Zinkenstelle.transform.localPosition.x >= ZS_LOWER_LIMIT)
                {
                    Zinkenstelle.transform.Translate(-MoveVel * Time.deltaTime, 0, 0);
                }
            }
        }
    }
}
    