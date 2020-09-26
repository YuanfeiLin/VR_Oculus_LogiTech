using System.Collections;
using System.Collections.Generic;
using UnityEngine;

   /*************************************************************************
    * CameraMoving.cs
    * 
    * Lin Yuanfei 23.11.2019, yuanfei.lin@tum.de
    *
    * Sichtbereich anpassen
    ***********************************************************************/
public class CameraMoving : MonoBehaviour
{
    public OVRCameraRig OVRC;
    public float upforce = 30f;
    public float downforce = -20f;
    public float forwardforce = 40f;
    public float backforce = -30f;
    public Canvas cv;

    void FixedUpdate()
    {
        if (cv.enabled == true)
        {
            //vorne und aufwaerts anpassen
            if (OVRInput.Get(OVRInput.Button.Two) || Input.GetKey("z"))
            {
                OVRC.transform.Translate(0, forwardforce * Time.deltaTime, 0);
                OVRC.transform.Translate(0, 0, upforce * Time.deltaTime);
            }
            // hinten und abwaerts anpassen
            if (OVRInput.Get(OVRInput.Button.Four) || Input.GetKey("x"))
            {
                OVRC.transform.Translate(0, backforce * Time.deltaTime, 0);
                OVRC.transform.Translate(0, 0, downforce * Time.deltaTime);
            }
        }
    }
}
