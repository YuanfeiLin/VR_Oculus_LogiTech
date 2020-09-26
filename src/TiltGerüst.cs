using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TiltGerüst : MonoBehaviour
{
    /******************************************
     * TiltGeruest.cs
     * 
     * Lin Yuanfei 23.11.2019, yuanfei.lin@tum.de
     * 
     * die Neigung der Gerüst kontrollieren 
     * 
     ******************************************/
    public Rigidbody Stapler;
    public Rigidbody TiltPart;
    public float TiltVel =5f;
    public AudioSource As;
    public Canvas cv;
    //Winkel zu dokumentieren
    public float angle;
    public Text ZinkenMoveIndex;
    public int IsZinkenMove;

    //Makros
    const float TILT_UPPER_LIMIT = 5f ;
    const float TILT_LOWER_LIMT = -8f;

    // Update is called once per frame
    void Update()
    {
        As.Pause();
        angle = TiltPart.transform.localEulerAngles.x;
        // in Unity sich der Winkel nicht von -180 bis 180 Grad verändert, sondern von 0 bis 360 Grad in Zyklus
        if (angle <=360f & angle>=180f)
        {
            angle = angle - 360f;
        }
        if (cv.enabled == false)
        {
            IsZinkenMove = int.Parse(ZinkenMoveIndex.text);
            if (angle <= TILT_UPPER_LIMIT && IsZinkenMove != 1)
            {
                //nach vorne neigen
                if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight) || Input.GetKey("j") )
                {
                    As.Play();
                    TiltPart.transform.Rotate(TiltVel * Time.deltaTime, 0,0);
                }
            }
            if (angle >= TILT_LOWER_LIMT && IsZinkenMove != 1)
            {
                //nach hinten neigen
                if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft) || Input.GetKey("l"))
                {
                    As.Play();
                    TiltPart.transform.Rotate(-TiltVel * Time.deltaTime, 0, 0);
                }
            }       
        }
    }
}
