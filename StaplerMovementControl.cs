
using UnityEngine;
using UnityEngine.UI;

public class StaplerMovementControl : MonoBehaviour
{
    /*************************************************************************
     * StaplerMovementControl.cs
     * 
     * Lin Yuanfei 23.11.2019, yuanfei.lin@tum.de
     * 
     *  die Bewegung des Staplers kontrollieren 
     *  
     *  Geschwindigkeit von Stapler v=8km/h(222.22cm/s)
     *  
      *************************************************************************/
    //public heißt Interation von Unity per UI-Interface
    public Rigidbody stapler;
    // Variable von Geschwindigkeit vor- und nachwärts
    public float ForwardVel=0f;
    public float BackwardVel=0f;
    // Variable von Drehgeschwindigkeit links und rechts
    public float RotatVelRight=0f;
    public float RotateVelLeft=0f;
    // Speicher von derzeitiger Geschwindigkeit vor- und nachwärts
    float FwVelTemp = 0f;
    float BwVelTemp = 0f;
    // Speicher von derzeitiger Pitch von Audio Souce
    float AsPitchTempFw = 0f;
    float AsPitchTempBw = 0f;

    // Quelle von Schall(Audio) verbinden
    public AudioSource As;
    // Canvas-Element, sichbar oder unsichtbar zu machen
    public Canvas cv;
    // Index zur Detektion von der Bewegung von Zinkenstelle und  Annamezustand von Palette
    public Text ZinkenMoveIndex;
    public Text PaletteOnIndex;
    // Um die Text von Index zu lesen
    public int PaletteIsOn;
    public int IsZinkenMove;

    //Makro
    const float LOAD_DRIVE_ACC = 111.11f;
    const float UNLOAD_DRIVE_ACC = 126.98f;
    const float LOAD_ROTATE_ACC = 20f;
    const float UNLOAD_ROTATE_ACC = 22.8f;
    const float BRAKE_ACC = 222.22f;
    const float BRAKE_LIMIT = 150f;
    const float DRIVE_VEL_MAX = 222.22f;
    const float LOAD_ACC_TIME = 2f;
    const float UNLOAD_ACC_TIME = 1.75f;
    const float BASIC_PITCH = 0.6f;
    const float LOAD_PITCH_ACC = 0.2f;
    const float UNLOAD_PITCH_ACC = 0.22f;

    // TimeDiff*:  die Bedienzeit berechnen, um die Beschleunigung am Anfang der Bewegung oder die Bremse nach der Bewegung zu regeln 
    public float TimeDiffRight = Time.time;
    public float TimeDiffLeft = Time.time;
    public float TimeDiffBw = Time.time;
    public float TimeDiffFw = Time.time;
    public float TimeDiffBrakeBw = Time.time;
    public float TimeDiffBrakeFw = Time.time;

    //TimeInside und TimeOutside dient zu überprüfen, ob der Joystick gerade in Betrieb ist.
    float TimeOutsideBw = 0;
    float TimeInsideBw = 0;
    float TimeOutsideFw = 0;
    float TimeInsideFw = 0;
    float TimeOutsideRi = 0;
    float TimeInsideRi = 0;
    float TimeOutsideLe = 0;
    float TimeInsideLe = 0;
    // Geschwindigkeit bei der Bremse
    public float BrakeFwVel = 0;
    public float BrakeBwVel = 0;
    //Zustände von Joystick : 0 heißt nicht gedruckt, 1 heißt gedruckt  
    public int IsJoystickRi = 0;
    public int IsJoystickLe = 0;
    public int IsJoystickUp = 0;
    public int IsJoystickDo = 0;

    // immer durchgeführt
    void Update()
    {
        // Kein Geräusch am Anfang
        As.Pause();
        if (cv.enabled == true) // man kann nur nach der Einleitung den Stapler steuern
        {
            As.Play(); // Geräusch spielen
            IsJoystickUp = 0;
            IsJoystickDo = 0;

            IsZinkenMove = int.Parse(ZinkenMoveIndex.text);//die Zinkenstelle und der Stapler können nicht gleichzeitig bewegen
            PaletteIsOn = int.Parse(PaletteOnIndex.text); // überprüfen, ob sich die Palette im Stapler befindet

            //Um zu überprüfen, ob der Joystick gerade in Betrieb ist.
            TimeOutsideRi += Time.deltaTime;
            if ((TimeOutsideRi - TimeInsideRi) > Time.deltaTime)
            {
                TimeDiffRight = Time.time;
                TimeOutsideRi = 0;
                TimeInsideRi = 0;
            }
            TimeOutsideLe += Time.deltaTime;
            if ((TimeOutsideLe - TimeInsideLe) > Time.deltaTime)
            {
                TimeDiffLeft = Time.time;
                TimeOutsideLe = 0;
                TimeInsideLe = 0;
            }
            TimeOutsideBw += Time.deltaTime;
            if ((TimeOutsideBw - TimeInsideBw) > Time.deltaTime)
            {
                TimeDiffBw = Time.time;
                TimeOutsideBw = 0;
                TimeInsideBw = 0;
                //Simulation der Trägheit nach dem Bremsen
                if ((Time.time - TimeDiffBrakeBw) <= 1f && BwVelTemp>= BRAKE_LIMIT)
                {
                    BrakeBwVel= FwVelTemp - BRAKE_ACC * (Time.time - TimeDiffBrakeBw);
                }         
                else 
                {
                    BrakeBwVel = 0;
                }
                stapler.transform.Translate(0,BrakeBwVel*Time.deltaTime, 0);
            }
            TimeOutsideFw += Time.deltaTime;
            if ((TimeOutsideFw - TimeInsideFw) > Time.deltaTime)
            {
                TimeDiffFw = Time.time;
                TimeOutsideFw = 0;
                TimeInsideFw = 0;
                if ((Time.time - TimeDiffBrakeFw) <= 1f && FwVelTemp >= BRAKE_LIMIT)
                {
                    BrakeFwVel = FwVelTemp - BRAKE_ACC * (Time.time - TimeDiffBrakeFw);
                }
                else 
                {
                    BrakeFwVel = 0;
                }
                stapler.transform.Translate(0, -BrakeFwVel*Time.deltaTime, 0);
            }

            //Esc  -- Verlasse die VR-Programm
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Home))
            {
                Application.Quit();
            }
            //  Oculus Joystick || Tastatur || Logitech Joystick
            if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown) || Input.GetKey("s") )// Input.GetKey("s")
            {
                // immer Reset von Zeitzähler von Trägheit während Bedienung
                TimeDiffBrakeBw = Time.time;
                TimeInsideBw += Time.deltaTime;
                // Zuständen aktualisieren
                IsJoystickRi = 0;
                IsJoystickDo = 1;
                IsJoystickUp = 0;
                IsJoystickLe = 0;
                if (PaletteIsOn == 1 && IsZinkenMove!=1)
                {
                    if ((Time.time - TimeDiffBw) <= LOAD_ACC_TIME)
                    {
                        ForwardVel = LOAD_DRIVE_ACC * (Time.time - TimeDiffBw);
                        As.pitch = BASIC_PITCH + LOAD_PITCH_ACC * (Time.time - TimeDiffBw);
                    }
                    else
                    {
                        ForwardVel = DRIVE_VEL_MAX;
                        As.pitch = 1f;
                    }
                }
                if (PaletteIsOn == 0 && IsZinkenMove!=1)
                {
                    if ((Time.time - TimeDiffBw) <= UNLOAD_ACC_TIME)
                    {
                        ForwardVel = UNLOAD_DRIVE_ACC * (Time.time - TimeDiffBw);
                        As.pitch = BASIC_PITCH + UNLOAD_PITCH_ACC * (Time.time - TimeDiffBw);
                    }
                    else
                    {
                        ForwardVel = DRIVE_VEL_MAX;
                        As.pitch = 1f;
                    }
                }
                stapler.transform.Translate(0, ForwardVel * Time.deltaTime, 0);
                FwVelTemp = ForwardVel;
                AsPitchTempFw = As.pitch;
            }
            
            // Reset von Zeitzähler
            if (IsJoystickDo == 0 )
            {
                TimeDiffBw = Time.time;
            }
            // während der Fahrt vorwärts oder nachwärts, kommt die Drehbeschleunigung auf Fahrgeschwindigkeit an.
            if (IsJoystickDo == 1 || IsJoystickUp == 1)
            {
                if (IsZinkenMove != 1)
                {
                    if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft) || Input.GetKey("a"))
                    {
                        if (IsJoystickDo == 1)
                        {
                            stapler.transform.RotateAround(stapler.position, new Vector3(0, stapler.position.y, 0), ForwardVel * Time.deltaTime / 30);
                        }
                        if (IsJoystickUp == 1)
                        {
                            stapler.transform.RotateAround(stapler.position, new Vector3(0, -stapler.position.y, 0), BackwardVel * Time.deltaTime / 30);
                        }
                    }
                    if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight) || Input.GetKey("d") )
                    {
                        if (IsJoystickDo == 1)
                        {
                            stapler.transform.RotateAround(stapler.position, new Vector3(0, -stapler.position.y, 0), ForwardVel * Time.deltaTime / 30);
                        }
                        if (IsJoystickUp == 1)
                        {
                            stapler.transform.RotateAround(stapler.position, new Vector3(0, stapler.position.y, 0), BackwardVel * Time.deltaTime / 30);
                        }
                    }
                }
            }
            // wenn es keine Bewegung nach vorne oder hinten gibt, dreht der Stapler sich immer noch von 0 bis max. 
            if (IsJoystickDo == 0 && IsJoystickUp == 0)
            {
 
                if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight) || Input.GetKey("d") )
                {
                    TimeInsideRi += Time.deltaTime;
                    if (IsZinkenMove != 1)
                    {
                        //die Zustände anderen Befehle detektieren, um die Beschleunigung zu zurücksetzen
                        IsJoystickRi = 1;
                        IsJoystickDo = 0;
                        IsJoystickUp = 0;
                        IsJoystickLe = 0;
                        if (PaletteIsOn == 1)
                        {
                            if ((Time.time - TimeDiffRight) < LOAD_ACC_TIME)// nur innerhalb 2 Sekunde gültig
                            {
                                RotatVelRight = LOAD_ROTATE_ACC * (Time.time - TimeDiffRight);
                                As.pitch = BASIC_PITCH + LOAD_PITCH_ACC * (Time.time - TimeDiffRight); // Ton des Geräusches
                            }
                        }
                        if (PaletteIsOn == 0)
                        {
                            if ((Time.time - TimeDiffRight) < UNLOAD_ACC_TIME)// nur innerhalb 3.5 Sekunde gültig
                            {
                                RotatVelRight = UNLOAD_ROTATE_ACC * (Time.time - TimeDiffRight);
                                As.pitch = BASIC_PITCH + UNLOAD_PITCH_ACC * (Time.time - TimeDiffRight);
                            }
                        }
                        stapler.transform.RotateAround(stapler.position, new Vector3(0, stapler.position.y, 0), RotatVelRight * Time.deltaTime);
                    }
                }

                //nach links rotieren
                if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft) || Input.GetKey("a"))
                {
                    TimeInsideLe += Time.deltaTime;
                    if (IsZinkenMove != 1)
                    {
                        IsJoystickRi = 0;
                        IsJoystickDo = 0;
                        IsJoystickUp = 0;
                        IsJoystickLe = 1;
                        if (PaletteIsOn == 1)
                        {
                            if ((Time.time - TimeDiffLeft) < LOAD_ACC_TIME)
                            {
                                RotateVelLeft = LOAD_ROTATE_ACC * (Time.time - TimeDiffLeft);
                                As.pitch = BASIC_PITCH + LOAD_PITCH_ACC * (Time.time - TimeDiffLeft);
                            }

                        }
                        if (PaletteIsOn == 0)
                        {
                            if ((Time.time - TimeDiffLeft) < UNLOAD_ACC_TIME)
                            {
                                RotateVelLeft = UNLOAD_ROTATE_ACC * (Time.time - TimeDiffLeft);
                                As.pitch = BASIC_PITCH + UNLOAD_PITCH_ACC * (Time.time - TimeDiffLeft);
                            }
                        }
                        stapler.transform.RotateAround(stapler.position, new Vector3(0, -stapler.position.y, 0), RotateVelLeft * Time.deltaTime);
                    }
                }
            }
            if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp) || Input.GetKey("w"))
            {
                TimeDiffBrakeFw = Time.time;
                TimeInsideFw += Time.deltaTime;
                IsJoystickRi = 0;
                IsJoystickDo = 0;
                IsJoystickUp = 1;
                IsJoystickLe = 0;
                if (PaletteIsOn == 1 && IsZinkenMove!=1)
                {
                    if ((Time.time - TimeDiffFw) < LOAD_ACC_TIME)
                    {
                        BackwardVel = LOAD_DRIVE_ACC * (Time.time - TimeDiffFw);
                        As.pitch = BASIC_PITCH + LOAD_PITCH_ACC * (Time.time - TimeDiffFw);
                    }
                    else
                    {
                        BackwardVel = DRIVE_VEL_MAX;
                        As.pitch = 1f;
                    }
                }
                if (PaletteIsOn == 0 && IsZinkenMove!=1)
                {
                    if ((Time.time - TimeDiffFw) < UNLOAD_ACC_TIME)
                    {
                        BackwardVel = UNLOAD_DRIVE_ACC * (Time.time - TimeDiffFw);
                        As.pitch = BASIC_PITCH + UNLOAD_PITCH_ACC * (Time.time - TimeDiffFw);
                    }
                    else
                    {
                        BackwardVel = DRIVE_VEL_MAX;
                        As.pitch = 1f;
                    }
                }
                stapler.transform.Translate(0, -BackwardVel * Time.deltaTime, 0);
                BwVelTemp = BackwardVel;
                AsPitchTempBw = As.pitch;
            }
            if (IsJoystickUp == 0 )
            {
                TimeDiffFw = Time.time;
            }
        }
    }
}
