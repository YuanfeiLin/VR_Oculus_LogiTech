using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveGabelZinken : MonoBehaviour
{
    /****************************************************
     * MoveGabelZinken.cs
     * 
     * Lin Yuanfei 23.11.2019, yuanfei.lin@tum.de
     * 
     * die Bewegung der Gabelzinken kontrollieren
     * (heben und senken)
     * 
     * Geschwindigkeit:
     * Heben: beladen 0,45 m/s, unbeladen 0,5 m/s
     * Senken: beladen 0,5 m/s, unbeladen 0,45 m/s 
     * 
     ****************************************************/
    public Canvas cv;
    public Rigidbody FirstMovePart;
    public Rigidbody SecondMovePart;
    // Bewegungsgeschwindigkeit für Gabelzinken
    public float MoveVelUp = 0f;
    public float MoveVelDw = 0f;
    // Zustand der Palette
    public Text PaletteOnIndex;
    public int PaletteIsOn =0;
    //TimeDiff*:  die Bedienzeit berechnen, um die Beschleunigung am Anfang der Bewegung oder die Bremse nach der Bewegung zu regeln 
    public float TimeDiffUp = Time.time;
    public float TimeDiffDw = Time.time;
    //TimeInside und TimeOutside dient zu überprüfen, ob der Joystick gerade in Betrieb ist.
    float TimeOutsideUp = 0;
    float TimeInsideUp = 0;
    float TimeOutsideDw = 0;
    float TimeInsideDw = 0;
    //Zustände von Joystick : 0 heißt nicht gedruckt, 1 heißt gedruckt  
    public int IsJoystickUp = 0;
    public int IsJoystickDw = 0;

    //Makro
    const float LOAD_UP_ACC = 25f;
    const float UNLOAD_UP_ACC = 25.7f;
    const float LOAD_DW_ACC = 22.5f;
    const float UNLOAD_DW_ACC = 28.6f;
    const float LOAD_ACC_TIME = 2f;
    const float UNLOAD_ACC_TIME = 1.75f;
    const float BASIC_PITCH = 0.6f;
    const float LOAD_PITCH_ACC = 0.2f;
    const float UNLOAD_PITCH_ACC = 0.22f;
    //Beschränkungen für die Bewegung
    const float FP_LOWER_LIMIT = 355f;//FP: First Part
    const float SP_LOWER_LIMIT = 0f;//SP: Second Part
    const float FP_UPPER_LIMIT = 1600f;
    const float SP_UPPER_LIMIT = 1700f;
    public AudioSource As;
    // Update is called once per frame
    void Update()
    {
        
        if (cv.enabled == false)
        {
            PaletteIsOn = int.Parse(PaletteOnIndex.text);
            As.Pause();
            TimeOutsideUp += Time.deltaTime;
            //Um zu überprüfen, ob der Joystick gerade in Betrieb ist.
            //Wenn nicht, wird der Zähler auf null gesetzt.
            if ((TimeOutsideUp - TimeInsideUp) > Time.deltaTime)
            {
                TimeDiffUp = Time.time;
                TimeOutsideUp = 0;
                TimeInsideUp = 0;
            }
            TimeOutsideDw += Time.deltaTime;
            if ((TimeOutsideDw - TimeInsideDw) > Time.deltaTime)
            {
                TimeDiffDw = Time.time;
                TimeOutsideDw = 0;
                TimeInsideDw = 0;
            }
            // Hierbei werden die Gabelzinken gehoben wenn man das Joystick nach hinten bewegt. Auch gilt für Senkung der Gabelzinken
            if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickDown) || Input.GetKey("k") )//Heben
            { 
                As.Play();
                TimeInsideUp += Time.deltaTime;
                IsJoystickUp = 1;
                IsJoystickDw = 0;
                if (PaletteIsOn == 1)
                {
                    // Heben, beladen 0,45 m/s
                    if ((Time.time - TimeDiffUp) < LOAD_ACC_TIME)
                    {
                        MoveVelUp = LOAD_DW_ACC * (Time.time - TimeDiffUp);
                        As.pitch = BASIC_PITCH + LOAD_PITCH_ACC * (Time.time - TimeDiffUp);
                    }
                }
                if (PaletteIsOn == 0)
                {
                    // Heben, unbeladen 0,5 m/s
                    if ((Time.time - TimeDiffUp) < UNLOAD_ACC_TIME)
                    {
                        MoveVelUp = UNLOAD_DW_ACC * (Time.time - TimeDiffUp);
                        As.pitch = BASIC_PITCH + UNLOAD_PITCH_ACC * (Time.time - TimeDiffUp);
                    }
                }
                if (FirstMovePart.transform.localPosition.z <= FP_UPPER_LIMIT)
                {
                    FirstMovePart.transform.Translate(0, 0, MoveVelUp * Time.deltaTime);
                }
                else if (SecondMovePart.transform.localPosition.z <= SP_UPPER_LIMIT)
                {
                    SecondMovePart.transform.Translate(0, 0, MoveVelUp * Time.deltaTime);
                }
            }
            // Wenn man die Bewegungsrichtung verändern, wird der Zähler noch auf null gesetzt.
            if (IsJoystickUp == 0)
            {
                TimeDiffUp = Time.time;
            }
            if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp) || Input.GetKey("i"))
            {
                As.Play();
                TimeInsideDw += Time.deltaTime;
                IsJoystickUp = 0;
                IsJoystickDw = 1;
                if (PaletteIsOn == 1)
                {
                    //Senken, beladen 0,5 m/s
                    if ((Time.time - TimeDiffDw) < LOAD_ACC_TIME)
                    {
                        MoveVelDw = LOAD_UP_ACC * (Time.time - TimeDiffDw);
                        As.pitch = BASIC_PITCH + LOAD_PITCH_ACC * (Time.time - TimeDiffDw);
                    }
                }
                if (PaletteIsOn == 0)
                {
                    //Senken, unbeladen 0,45 m/s
                    if ((Time.time - TimeDiffDw) < UNLOAD_ACC_TIME)  
                    {
                        MoveVelDw = UNLOAD_UP_ACC * (Time.time - TimeDiffDw);
                        As.pitch = BASIC_PITCH + UNLOAD_PITCH_ACC * (Time.time - TimeDiffDw);
                    }
                }       
                if (SecondMovePart.transform.localPosition.z >= SP_LOWER_LIMIT)
                {
                    SecondMovePart.transform.Translate(0, 0, -MoveVelDw * Time.deltaTime);
                }
                else if (FirstMovePart.transform.localPosition.z >= FP_LOWER_LIMIT)
                {
                    FirstMovePart.transform.Translate(0, 0, -MoveVelDw * Time.deltaTime);
                }
            }
            // Wenn man die Bewegungsrichtung verändern, wird der Zähler noch auf null gesetzt.
            if (IsJoystickDw == 0)
            {
                TimeDiffDw = Time.time;
            }
        }
    }
}
