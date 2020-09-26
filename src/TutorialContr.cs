using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class TutorialContr : MonoBehaviour
{
    /************************************************************************
    * TutorialContr.cs
    * 
    * Lin Yuanfei, 23.11.2019, yuanfei.lin@tum.de
    * 
    * die Sichbarkeit von Tutorial kontrollieren
    * 
    ************************************************************************/
    // Start is called before the first frame update
    public Renderer imagegesamt;
    public Renderer image1;
    public Renderer image2;
    public Renderer image3;
    public Renderer image4;
    public Canvas cv;
    int index;
    float counter = 0;
    private void Start()
    {
        // das aktuelle Szenario kennen
        index = SceneManager.GetActiveScene().buildIndex;
        if (index == 0)
        {
            image1.enabled = true;
            image2.enabled = false;
            image3.enabled = false;
            image4.enabled = false;
            imagegesamt.enabled = false;
            cv.enabled = true;
        }
        //Aufgabe-Szenario : Gibt es keine Tutorial am Anfang
        if (index == 1)
        {
            image1.enabled = false;
            image2.enabled = false;
            image3.enabled = false;
            image4.enabled = false;
            imagegesamt.enabled = false;
            cv.enabled = true;
        }
    }
    void FixedUpdate()
    {
        if (image1.enabled == true && (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight) || Input.GetKey("q") || Input.GetButton("Tutorial-next")))
        {
            //die Reaktionszeit von Joystick ist zu kurz, deshalb machen wir jetzt hierbei einen Zähler.
            if (counter <= 0.15f)
            {
                counter += Time.deltaTime;
            }
            else
            {
                image1.enabled = false;
                image2.enabled = true;
                counter = 0;
            }
        }
        if (image2.enabled == true && (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight) || Input.GetKey("q") || Input.GetButton("Tutorial-next")))
        {
            if (counter <= 0.15f)
            {
                counter += Time.deltaTime;
            }
            else
            {
                image2.enabled = false;
                image3.enabled = true;
                counter = 0;
            }
        }
        if (image2.enabled == true && (OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft) || Input.GetKey("e") || Input.GetButton("Tutorial-last")))
        {
            if (counter <= 0.15f)
            {
                counter += Time.deltaTime;
            }
            else
            {
                image2.enabled = false;
                image1.enabled = true;
                counter = 0;
            }
        }
        if (image3.enabled == true && (OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight) || OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight) || Input.GetKey("q") || Input.GetButton("Tutorial-next")))
        {
            if (counter <= 0.15f)
            {
                counter += Time.deltaTime;
            }
            else
            {
                image3.enabled = false;
                image4.enabled = true;
                counter = 0;
            }
        }
        if (image3.enabled == true && (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft) || OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft) || Input.GetKey("e") || Input.GetButton("Tutorial-last")))
        {
            if (counter <= 0.15f)
            {

                counter += Time.deltaTime;
            }
            else
            {
                image3.enabled = false;
                image2.enabled = true;
                counter = 0;
            }
        }
        if (image4.enabled == true && (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight) || OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight) || Input.GetKey("q") || Input.GetButton("Tutorial-next")))
        {

            if (counter <= 0.15f)
            {

                counter += Time.deltaTime;
            }
            else
            {
                image4.enabled = false;
                cv.enabled = false;
                counter = 0;
            }
        }
        if (image4.enabled == true && (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft) || OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft) || Input.GetKey("e") || Input.GetButton("Tutorial-last")))
        {

            if (counter <= 0.15f)
            {
                counter += Time.deltaTime;
            }
            else
            {
                image4.enabled = false;
                image3.enabled = true;
                counter = 0;
            }
        }

        if (cv.enabled == false)
        {
            imagegesamt.enabled = false;
            if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) || Input.GetKey("n") || Input.GetButton("Trigger1") || Input.GetButton("Trigger2"))
            {
                imagegesamt.enabled = true;
            }
        }
    }
}
