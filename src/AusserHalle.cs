using UnityEngine;
using UnityEngine.SceneManagement;

public class AusserHalle : MonoBehaviour
{
    /*
     * Wenn der Benutzer die Wand verlässt, wird die aktuelle Szene neu gestartet.
     */
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Stapler" || other.tag == "Regal")
        {

            int index = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(index);
        }
    }
}
