using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/************************************************************************
  * ColliderDetection.cs
  * 
  * Lin Yuanfei, 23.11.2019, yuanfei.lin@tum.de
  * 
  * Wenn die Palette erkannt wird, bewegt sich die Palette mit der Zinkenstelle
  * 
  ***********************************************************************/

public class ColliderDetection : MonoBehaviour
{
    public Rigidbody Stapler;
    public Text PaletteOnIndex;
    int PaletteIsOn = 0;
   
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Palette")
        {
            
            other.transform.parent = Stapler.transform;
            PaletteIsOn = 1;
            PaletteOnIndex.text = PaletteIsOn.ToString();
           
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Palette")
        {
            PaletteIsOn = 0;
            PaletteOnIndex.text = PaletteIsOn.ToString();
            other.transform.parent = null;
        }
    }
}
