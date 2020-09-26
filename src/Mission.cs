
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.IO;
using System.Text;

public class Mission : MonoBehaviour
{
    /************************************************************************
     * Mission.cs
     * 
     * Lin Yuanfei, 23.11.2019, yuanfei.lin@tum.de
     * 
     * Hauptcode:
     * 
     * Aufgabe stellen, Fertigkeit detektieren, Szenarien kontrollieren
     * 
     ************************************************************************/
    // Um die Anzahl vom Versuch zu zählen, machen wir hierbei Counter statisch, also es wird nicht durch Erneuerung wieder auf 1 gesetzt
    static int CounterChange=1;
    public Text missionText;
    public Text scoreText;
    public Text PaletteOnIndex;
    public int PaletteIsOn;
    public Canvas Cv;
    public Text missionTagText;
    public Text StepNumText;
    public Text SceneNameText;
    public Text TestNumText;
    public AudioSource As;
    public Rigidbody Stapler;
    public Rigidbody Gerüst;
    //Gabelstelle
    public Rigidbody FirstPart;
    public Rigidbody SecondPart;
    public Renderer image1;
    public Renderer image2;
    public Renderer image3;
    public Renderer image4;
    public int score;
    //Anfangswert für die Position des Staplers -- Um die Distanz zu messen
    float PosStaplerZ0;
    float PosStaplerX0;
    float PosStaplerY0;
    float PosGabelstZ;
    float AngleStaplerY;
    //Zähler von Erfüllung der Aufgabe
    public float TimeCountAch = 0f;
    public float TimeCountStep = 0f;
    public float totalTime = 0f;
    // Renderer dient zur Regelung von der Sichbarkeit, Rigidbody dient zur Regelung der Bewegung von Pfeile
    public Renderer pfeileRd1;
    public Rigidbody pfeile1;
    public Renderer pfeileRd2;
    public Rigidbody pfeile2;
    public Renderer pfeileRd3;
    public Rigidbody pfeile3;
    public Renderer pfeileRd4;
    public Rigidbody pfeile4;
    public Renderer pfeileRd5;
    public Rigidbody pfeile5;
    public Renderer pfeileRd6;
    public Rigidbody pfeile6;
    public Renderer pfeileRd7;
    public Rigidbody pfeile7;
    public Renderer pfeileRd8;
    public Rigidbody pfeile8;
    public Renderer pfeileRd9;
    public Rigidbody pfeile9;
    //-- Markierung --
    //Ziel/Start position
    // WE:Wareneingang WA:Warenausgang L1:Lage1 L2:Lage2
    public Renderer ZielStart;
    public Renderer WE;
    public Renderer WA;
    public Renderer L1;
    public Renderer L2;
    //Messung von Distanz
    double Distance;
    int SceneIndex;
    //Path für ExportierungsDoku
    string PathWithD;
    string PathWtouD;
    //Umwandlung von Betriebszeit, runde Zahl und Dezimale(nach dem Komma)
    int SecIntStep;
    int SecDeciStep;
    int SecIntTotal;
    int SecDeciTotal;
    // Testanzahl einstellen
    const int TestNumIndex = 11;
    const float AchieveTime = 1f;
    const float InvokeTimeTutorial = 0.5f;
    const float InvokeTimeMisson = 1f;

    //Bewegung von Pfeilen
    private void FixedUpdate()
    {
        pfeile1.transform.Translate(0, 20 * Mathf.Sin(Time.time * 15f), 0);
        pfeile2.transform.Translate(0, 20 * Mathf.Sin(Time.time * 15f), 0);
        pfeile3.transform.Translate(0, 20 * Mathf.Sin(Time.time * 15f), 0);
        pfeile4.transform.Translate(0, 20 * Mathf.Sin(Time.time * 15f), 0);
        pfeile5.transform.Translate(0, 10 * Mathf.Sin(Time.time * 15f), 0);
        pfeile6.transform.Translate(0, 10 * Mathf.Sin(Time.time * 15f), 0);
        pfeile7.transform.Translate(0, 10 * Mathf.Sin(Time.time * 15f), 0);
        pfeile8.transform.Translate(0, 10 * Mathf.Sin(Time.time * 15f), 0);
        pfeile9.transform.Translate(0, 10 * Mathf.Sin(Time.time * 15f), 0);
    }
    //Szenarien erneuern, wenn man alle Aufgaben fertig hat.
    void MissonEnd()
    {
        if (CounterChange == TestNumIndex+1)
        {
            Application.Quit();
        }
        SceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(SceneIndex);
    }
    //Format von Zeit in xy.xy verändern
    string ChangeTimeForm()
    {
        if (StepNumText.text != "Schritt 9")
        {
            SecIntStep = (int)TimeCountStep;
            SecDeciStep = (int)((TimeCountStep - (int)TimeCountStep) * 100);
            string runtime = string.Format("{0:D2}.{1:D2}", SecIntStep, SecDeciStep) + " s" + " \r\n";
            return runtime;
        }
        else
        {
            SecIntStep = (int)TimeCountStep;
            SecDeciStep = (int)((TimeCountStep - (int)TimeCountStep) * 100);
            SecIntTotal = (int)totalTime;
            SecDeciTotal = (int)((totalTime - (int)totalTime) * 100);
            string runtime = string.Format("{0:D2}.{1:D2}", SecIntStep, SecDeciStep) + " s \r\n" + "Totaltime: "+string.Format("{0:D2}.{1:D2}", SecIntTotal, SecDeciTotal) + " s \r\n";
            return runtime;
        }
    }
    //wenn aktuelle Aufgabe fertig ist, wird die neue Leistung gezeigt
    void GetScore()
    {
        As.Play();
        scoreText.text = score.ToString();
    }
    //Daten exportieren, Parameter erneuern
    void RenewOutput(int IsAppendDis)
    {
        string runTime = ChangeTimeForm();
        GetScore();
        //Runtime exportieren
        File.AppendAllText(PathWithD, StepNumText.text + "-" + missionText.text + ":" + runTime);
        File.AppendAllText(PathWtouD, StepNumText.text + "-" + missionText.text + ":" + runTime);
        TimeCountAch = 0f;
        TimeCountStep = 0f;
        if (IsAppendDis == 1)
        {
            //Distanz exportieren
            File.AppendAllText(PathWithD, StepNumText.text + "-" + "Distance" + ":" + ((int)(GetDistanceManh(PosStaplerX0, PosStaplerY0, PosStaplerZ0) / 100f)).ToString() + "m\r\n");
        }
    }
    // Euklidischer Abstand berechnen
    double GetDistanceEu(double x0, double y0, double z0)
    {
        double deltax = Stapler.transform.localPosition.x - x0;
        double deltay = Stapler.transform.localPosition.y - y0;
        double deltaz = Stapler.transform.localPosition.z - z0;
        double distanz = System.Math.Sqrt(deltax * deltax + deltay * deltay + deltaz * deltaz);
        return distanz;
    }
    //Manhattaner Abstand berechnen
    double GetDistanceManh(double x0, double y0, double z0)
    {
        double deltax = Stapler.transform.localPosition.x - x0;
        double deltay = Stapler.transform.localPosition.y - y0;
        double deltaz = Stapler.transform.localPosition.z - z0;
        double distanz = System.Math.Abs(deltax) + System.Math.Abs(deltay) + System.Math.Abs(deltaz);
        return distanz;
    }
    void Start()
    {
        // Adresse von .txt erstellen, die aktuelle Uhrzeit notieren
        PathWithD = Application.dataPath + "/RT(mit Distanz).txt";
        PathWtouD = Application.dataPath + "/RT(onhe Distanz).txt";
        if (!File.Exists(PathWithD))
        {
            File.WriteAllText(PathWithD, TestNumText.text +" -" + System.DateTime.Now.ToString()+ " \r\n");
        }
        if (!File.Exists(PathWtouD))
        {
            File.WriteAllText(PathWtouD, TestNumText.text + " -" + System.DateTime.Now.ToString() + " \r\n");
        }
        //Renderer - Am Anfang unsichtbar machen
        Cv.enabled = false;
        pfeileRd1.enabled = false;
        pfeileRd2.enabled = false;
        pfeileRd3.enabled = false;
        pfeileRd4.enabled = false;
        pfeileRd5.enabled = false;
        pfeileRd6.enabled = false;
        pfeileRd7.enabled = false;
        pfeileRd8.enabled = false;
        pfeileRd9.enabled = false;
        ZielStart.enabled = false;
        WE.enabled = false;
        WA.enabled = false;
        L1.enabled = false;
        L2.enabled = false;
        //Position dokumentieren, damit man ihn im nächsten Schritt verwenden kann
        PosStaplerZ0 = Stapler.transform.localPosition.z;
        PosStaplerX0 = Stapler.transform.localPosition.x;
        PosStaplerY0 = Stapler.transform.localPosition.y;
        PosGabelstZ = 0;
        //Szenarien erstellen (0: Tutorial, 1: Aufgabe)
        SceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (SceneIndex == 0)
        {
            TestNumText.text = " ";
            SceneNameText.text = "Tutorial";
            missionTagText.text = "Staplerkontrolle";
            missionText.text = "Fahre 3m vorwärts";
            scoreText.text = 0.ToString();
        }
        if(SceneIndex == 1)
        {
            TestNumText.text = CounterChange.ToString();
            image1.enabled = false;
            image2.enabled = false;
            image3.enabled = false;
            image4.enabled = false;
            score = 1000;
            GetScore();
            ChangeMission10();// ChangeMission10();
            TimeCountAch = 0f;
            totalTime = 0f;
            TimeCountStep = 0f;
            pfeileRd1.enabled = true;
            WE.enabled = true;
            //nicht ganz senkrecht, um die Palette einfacher zu erheben
            Gerüst.transform.localEulerAngles = new Vector3(1f, 0, 0);
        }
    }
    // Aufgabe und entsprende Anzeige in Unity verändern
    void ChangeMission1()
    {   missionText.text = "Fahre 3m rückwärts";}
    void ChangeMission2()
    {   missionText.text = "90° nach links";}
    void ChangeMission3()
    {   missionText.text = "90° nach rechts";}
    void ChangeMission4()
    {   missionTagText.text = "Hubgerüstkontrolle";
        missionText.text = "Hebe das Hubgerüst um 1m";}
    void ChangeMission5()
    {   missionText.text = "Senke das Hubgerüst um 1m";}
    void ChangeMission6()
    {   missionText.text = "Hubgerüst nach vorne neigen";}
    void ChangeMission7()
    {   missionText.text = "Hubgerüst nach hinten neigen";}
    void ChangeMission8()
    {   missionTagText.text = "Gabelzinkenkontrolle";
        missionText.text = "Verschiebe die Gabelzinken \n nach links";}
    void ChangeMission9()
    {   missionText.text = "Verschiebe die Gabelzinken \n nach rechts";}
    void ChangeMission10()
    {   SceneNameText.text = "Aufgabe";
        StepNumText.text = "Schritt 1";
        missionTagText.text = "Fahrt zu Palette in WE";
        missionText.text = "Gabel bodenfrei anheben";
        Gerüst.transform.localEulerAngles = new Vector3(353f, 0, 0);
        Stapler.transform.localEulerAngles = new Vector3(270f, 0, 0);}
    void ChangeMission11()
    {   missionText.text = "zur markierten Palette fahren";}
    void ChangeMission12()
    {   StepNumText.text = "Schritt 2";
        missionTagText.text = "Palette aufnehmen";
        missionText.text = "Gabelzinken senken"; }
    void ChangeMission13()
    {   missionText.text = "Gabel unter Palette fahren";}
    void ChangeMission14()
    {   missionText.text = "Palette bodenfrei anheben";}
    void ChangeMission15()
    {   missionText.text = "Hubgerüst zurückneigen";}
    void ChangeMission16()
    {   missionText.text = "Palette aus Lagerplatz fahren";}
    void ChangeMission17()
    {   StepNumText.text = "Schritt 3";
        missionTagText.text = "Transport zum Regal";
        missionText.text = "Zur markierten Abgabestelle fahren";}
    void ChangeMission18()
    {   StepNumText.text = "Schritt 4";
        missionTagText.text = "Platzieren im Regal";
        missionText.text = "90° zum Regal ausrichten \n im Abstand von 0,3 m";}
    void ChangeMission19()
    {   missionText.text = "Hubgerüst senkrecht stellen";}
    void ChangeMission20()
    {   missionText.text = "Palette anheben";}
    void ChangeMission21()
    {   missionText.text = "Palette in Regal fahren";}
    void ChangeMission22()
    {   missionText.text = "Palette absetzen";}
    void ChangeMission23()
    {   missionText.text = "Gabel aus Palette ziehen";}
    void ChangeMission24()
    {   missionText.text = "Gabel bodenfrei senken";}
    void ChangeMission25()
    {   StepNumText.text = "Schritt 5";
        missionTagText.text = "Fahrt zur zweiten Palatte";
        missionText.text = "Zum markierten Lageplatz fahren";}
    void ChangeMission26()
    {   StepNumText.text = "Schritt 6";
        missionTagText.text = "Palette aufnehmen";
        missionText.text = "90° zum Regal ausrichten \n im Abstand von 0,3 m";}
    void ChangeMission27()
    {   missionText.text = "Gabel heben"; }
    void ChangeMission28()
    {   missionText.text = "Gabel unter Palette fahren";}
    void ChangeMission29()
    {   missionText.text = "Palette anheben";}
    void ChangeMission30()
    {   missionText.text = "Palette aus Regalplatz fahren";}
    void ChangeMission31()
    {   missionText.text = "Palette senken";}
    void ChangeMission32()
    {   missionText.text = "Hubgerüst zurückneigen";}
    void ChangeMission33()
    {   StepNumText.text = "Schritt 7";
        missionTagText.text = "Transport zum WA";
        missionText.text = "Zum markierten Abgabeort fahren";}
    void ChangeMission34()
    {   StepNumText.text = "Schritt 8";
        missionTagText.text = "Platzieren im WA";
        missionText.text = "Palette auf Lagerplatz fahren";}
    void ChangeMission35()
    {   missionText.text = "Hubgerüst senkrecht stellen";}
    void ChangeMission36()
    {   missionText.text = "Palette absetzen";}
    void ChangeMission37()
    {   missionText.text = "Gabel aus Palette ziehen";}
    void ChangeMission38()
    {   missionText.text = "Gabel bodenfrei anheben";}
    void ChangeMission39()
    {   StepNumText.text = "Schritt 9";
        missionTagText.text = "Fahrt zu Start/Ziel";
        missionText.text = "Zum Startpunkt zurückfahren";}

    // Update is called once per frame
    void Update()
    {
        PaletteIsOn = int.Parse(PaletteOnIndex.text);
        //Restart
        if (SceneNameText.text == "Tutorial")
        {
            if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) || Input.GetKey("m") || Input.GetButton("Restart"))
            {
                
                File.AppendAllText(PathWithD, TestNumText.text + "."+ System.DateTime.Now.ToString() + " \r\n");
                File.AppendAllText(PathWtouD, TestNumText.text + "." + System.DateTime.Now.ToString() + " \r\n");
                SceneManager.LoadScene(SceneIndex);
            }
        }
        if (GetDistanceEu(PosStaplerX0, PosStaplerY0, PosStaplerZ0) > 280 && missionText.text == "Fahre 3m vorwärts")
        {
            score = 100;
            GetScore();
            Invoke("ChangeMission1", InvokeTimeTutorial);
            PosStaplerZ0 = Stapler.transform.localPosition.z;
            PosStaplerX0 = Stapler.transform.localPosition.x;
            PosStaplerY0 = Stapler.transform.localPosition.y;
        }
        if (GetDistanceEu(PosStaplerX0, PosStaplerY0, PosStaplerZ0) > 280 && (Stapler.transform.localPosition.z - PosStaplerZ0) < 0 && missionText.text == "Fahre 3m rückwärts")
        {
            score = 200;
            GetScore();
            Invoke("ChangeMission2", InvokeTimeTutorial);
            AngleStaplerY = Stapler.transform.localEulerAngles.y;
        }
        float AngleStaplerYNew1 = Stapler.transform.localEulerAngles.y;
        if (((AngleStaplerYNew1 - AngleStaplerY) < 270 && (AngleStaplerYNew1 - AngleStaplerY) > 260 || (AngleStaplerYNew1 - AngleStaplerY) < -80 && (AngleStaplerYNew1 - AngleStaplerY) > -90) && missionText.text == "90° nach links")
        {
            score = 300;
            GetScore();
            Invoke("ChangeMission3", InvokeTimeTutorial);
            AngleStaplerY = Stapler.transform.localEulerAngles.y;
        }
        if ((AngleStaplerYNew1 - AngleStaplerY) > 80 && (AngleStaplerYNew1 - AngleStaplerY) < 90 && missionText.text == "90° nach rechts")
        {
            score = 400;
            GetScore();
            Invoke("ChangeMission4", InvokeTimeTutorial);
            // dann detektieren die Position, nicht am Anfang
            PosGabelstZ = FirstPart.transform.localPosition.z;
        }
        if ((FirstPart.transform.localPosition.z - PosGabelstZ) > 125 && missionText.text == "Hebe das Hubgerüst um 1m")
        {
            score = 500;
            GetScore();
            Invoke("ChangeMission5", InvokeTimeTutorial);
        }
        if ((FirstPart.transform.localPosition.z - PosGabelstZ) < 5 && missionText.text == "Senke das Hubgerüst um 1m")
        {
            score = 600;
            GetScore();
            Invoke("ChangeMission6", InvokeTimeTutorial);
        }
        if ((OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight) || Input.GetKey("j") || Input.GetAxis("Horizontal2")>= 0.7) && missionText.text == "Hubgerüst nach vorne neigen")
        {
            score = 700;
            GetScore();
            Invoke("ChangeMission7", InvokeTimeTutorial);
        }
        if ((OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft) || Input.GetKey("l") || Input.GetAxis("Horizontal2") <= -0.7) && missionText.text == "Hubgerüst nach hinten neigen")
        {
            score = 800;
            GetScore();
            Invoke("ChangeMission8", InvokeTimeTutorial);
        }
        if (missionText.text == "Verschiebe die Gabelzinken \n nach links")
        {
            if ((OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft) && OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft)) || (Input.GetAxis("Horizontal2") <= -0.7 && Input.GetAxis("Horizontal1") <= -0.7) || Input.GetKey("o"))
            {
                score = 900;
                GetScore();
                Invoke("ChangeMission9", InvokeTimeTutorial);
            }
        }
        if (missionText.text == "Verschiebe die Gabelzinken \n nach rechts")
        {
            if ((OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight) && OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight)) || (Input.GetAxis("Horizontal2") >= 0.7 && Input.GetAxis("Horizontal1") >= 0.7) || Input.GetKey("p"))
            {
                score = 1000;
                GetScore();
                Invoke("ChangeMission10", InvokeTimeTutorial+0.5f);
                //Parameter auf 0 setzen
                TimeCountAch = 0f;
                totalTime = 0f;
                TimeCountStep = 0f;
                pfeileRd1.enabled = true;
                WE.enabled = true;
                SceneManager.LoadScene(SceneIndex + 1);
            }
        }
        if (missionText.text == "Gabel bodenfrei anheben" && StepNumText.text == "Schritt 1")
        {
            //countable ist ein Parameter, mit dem Benutzer das Timing nach der ersten Operation starten können
            int countable = 0;
            if (Input.GetKey("w") || Input.GetAxis("Vertical1") != 0 || Input.GetAxis("Vertical2") != 0 || Input.GetAxis("Horizontal2") !=0 || Input.GetAxis("Horizontal1") !=0 ||OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight) || OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft) || OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp) || OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown) || OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight) || OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft) || OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp) || OVRInput.Get(OVRInput.Button.SecondaryThumbstickDown))
            {
                countable = 1;
            }
            if (countable == 1)
            {
                TimeCountStep += Time.deltaTime;
                totalTime = totalTime + Time.deltaTime;
            }
            if (FirstPart.transform.localPosition.z < 550f && FirstPart.transform.localPosition.z > 270f)
            {
                score = 1000;
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    RenewOutput(0);
                    PosStaplerZ0 = Stapler.transform.localPosition.z;
                    PosStaplerX0 = Stapler.transform.localPosition.x;
                    PosStaplerY0 = Stapler.transform.localPosition.y;
                    ChangeMission11();
                }
            }
            else
            {
                // die aktuelle Aufgabe ist schon abgeschlossen und das Parameter wird für das nächste Mal auf null gesetzt.
                TimeCountAch = 0f;
            }
        }
        float AngleStaplerYNew2 = Stapler.transform.localEulerAngles.y;
        if (AngleStaplerYNew2 <= 360f & AngleStaplerYNew2 >= 180f)
        {
            AngleStaplerYNew2 = AngleStaplerYNew2 - 360f;
        }
        if (missionText.text == "zur markierten Palette fahren" && StepNumText.text == "Schritt 1")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            if (Stapler.transform.position.z > -300f && AngleStaplerYNew2 > -2f && AngleStaplerYNew2 < 2f)
            {
                TimeCountAch += Time.deltaTime;
                score = 1500;
                if (TimeCountAch >= AchieveTime)
                {
                    RenewOutput(1);
                    ChangeMission12();
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        float AngleGerüstX = Gerüst.transform.localEulerAngles.x;
        if (AngleGerüstX <= 360f & AngleGerüstX >= 180f)
        {
            AngleGerüstX = AngleGerüstX - 360f;
        }
        if (missionText.text == "Gabelzinken senken" && StepNumText.text == "Schritt 2")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            if (FirstPart.transform.localPosition.z <= 300f)
            {
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    RenewOutput(0);
                    ChangeMission13();
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        if (missionText.text == "Gabel unter Palette fahren" && StepNumText.text == "Schritt 2")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            if (Stapler.transform.position.z >= -113f)
            {
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    RenewOutput(0);
                    ChangeMission14();
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        
        if (missionText.text == "Palette bodenfrei anheben" && StepNumText.text == "Schritt 2")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            Debug.Log(FirstPart.transform.localPosition.z);
            if (FirstPart.transform.localPosition.z < 550f && FirstPart.transform.localPosition.z > 270f && PaletteIsOn==1)
            {
                pfeileRd1.enabled = false;
                WE.enabled = false;
                pfeileRd2.enabled = true;
                L1.enabled = true;
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    RenewOutput(0);
                    ChangeMission15();
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        if (missionText.text == "Hubgerüst zurückneigen" && StepNumText.text == "Schritt 2")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            if (AngleGerüstX < -5f && PaletteIsOn == 1)
            {
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    PosStaplerZ0 = Stapler.transform.localPosition.z;
                    PosStaplerX0 = Stapler.transform.localPosition.x;
                    PosStaplerY0 = Stapler.transform.localPosition.y;
                    RenewOutput(0);
                    ChangeMission16();
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        if (missionText.text == "Palette aus Lagerplatz fahren" && StepNumText.text == "Schritt 2")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            if (GetDistanceEu(PosStaplerX0,PosStaplerY0,PosStaplerZ0)> 250 && PaletteIsOn == 1)
            {
                score = 2000;
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    RenewOutput(0);
                    ChangeMission17();
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        if (missionText.text == "Zur markierten Abgabestelle fahren" && StepNumText.text == "Schritt 3")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            pfeileRd5.enabled = true;
            pfeileRd6.enabled = true;
            pfeileRd7.enabled = true;
            if (Stapler.transform.localPosition.x > 3100)
            { pfeileRd5.enabled = false; }
            if (Stapler.transform.localPosition.x > 3800)
            { pfeileRd6.enabled = false; }
            if (Stapler.transform.localPosition.x > 4200)
            { pfeileRd7.enabled = false; }
            if (Stapler.transform.localPosition.x<4600f &&Stapler.transform.localPosition.x > 4200f && Stapler.transform.localPosition.z <-2000f && PaletteIsOn == 1)
            {
                score = 2500;
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    RenewOutput(1);
                    ChangeMission18();
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        if (missionText.text == "90° zum Regal ausrichten \n im Abstand von 0,3 m" && StepNumText.text == "Schritt 4")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            if ((AngleStaplerYNew2 > -91f && AngleStaplerYNew2 < -89f || AngleStaplerYNew2 > 89f && AngleStaplerYNew2 < 91f) && Stapler.transform.localPosition.x<4436f && Stapler.transform.localPosition.x>4355f)
            {
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    RenewOutput(0);
                    ChangeMission19();
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        if (missionText.text == "Hubgerüst senkrecht stellen" && StepNumText.text == "Schritt 4")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            if (AngleGerüstX > -1f && AngleGerüstX < 1f && PaletteIsOn == 1)
            {
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    RenewOutput(0);
                    ChangeMission20();
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        if (missionText.text == "Palette anheben" && StepNumText.text == "Schritt 4")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            if (SecondPart.transform.localPosition.z >= 1450f)
            {
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    RenewOutput(0);
                    ChangeMission21();
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        if (missionText.text == "Palette in Regal fahren" && StepNumText.text == "Schritt 4")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            if (Stapler.transform.position.x <= 4270f)
            {
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    RenewOutput(0);
                    ChangeMission22();
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        if (missionText.text == "Palette absetzen" && StepNumText.text == "Schritt 4")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            if (PaletteIsOn!=1)
            {
                pfeileRd2.enabled = false;
                L1.enabled = false;
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    RenewOutput(0);
                    ChangeMission23();
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        if (missionText.text == "Gabel aus Palette ziehen" && StepNumText.text == "Schritt 4")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            if (Stapler.transform.position.x >= 4351f)
            {
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    RenewOutput(0);
                    ChangeMission24();
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        if (missionText.text == "Gabel bodenfrei senken" && StepNumText.text == "Schritt 4")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            if (FirstPart.transform.localPosition.z < 550f && FirstPart.transform.localPosition.z > 270f)
            {
                pfeileRd3.enabled = true;
                
                L2.enabled = true;
                score = 3000;
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    PosStaplerZ0 = Stapler.transform.localPosition.z;
                    PosStaplerX0 = Stapler.transform.localPosition.x;
                    PosStaplerY0 = Stapler.transform.localPosition.y;
                    RenewOutput(0);
                    ChangeMission25();
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        if (missionText.text == "Zum markierten Lageplatz fahren" && StepNumText.text == "Schritt 5")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            if (Stapler.transform.localPosition.z>-1750)
            {
                score = 3500;
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    RenewOutput(1);
                    ChangeMission26();
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        if (missionText.text == "90° zum Regal ausrichten \n im Abstand von 0,3 m" && StepNumText.text == "Schritt 6")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            if ((AngleStaplerYNew2 > -91f && AngleStaplerYNew2 < -89f || AngleStaplerYNew2 > 89f && AngleStaplerYNew2 < 91f) && Stapler.transform.localPosition.x>4375f &&Stapler.transform.localPosition.x<4454f )
            {
                WA.enabled = true;
                pfeileRd3.enabled = false;
                L2.enabled = false;
                pfeileRd4.enabled = true;
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    RenewOutput(0);
                    ChangeMission27();
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        if (missionText.text == "Gabel heben" && StepNumText.text == "Schritt 6")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            if (SecondPart.transform.localPosition.z > 0 && SecondPart.transform.localPosition.z < 180f)
            {
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    RenewOutput(0);
                    ChangeMission28();
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        if (missionText.text == "Gabel unter Palette fahren" && StepNumText.text == "Schritt 6")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            if (Stapler.transform.localPosition.x >4559f)
            {
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    RenewOutput(0);
                    ChangeMission29();
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        if (missionText.text == "Palette anheben" && StepNumText.text == "Schritt 6")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            if (SecondPart.transform.localPosition.z >240 && PaletteIsOn == 1)
            {
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    PosStaplerZ0 = Stapler.transform.localPosition.z;
                    PosStaplerX0 = Stapler.transform.localPosition.x;
                    PosStaplerY0 = Stapler.transform.localPosition.y;
                    RenewOutput(0);
                    ChangeMission30();
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        if (missionText.text == "Palette aus Regalplatz fahren" && StepNumText.text == "Schritt 6")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            if(Stapler.transform.localPosition.x<4470f && PaletteIsOn == 1)
            {
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    RenewOutput(0);
                    ChangeMission31();
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        if (missionText.text == "Palette senken" && StepNumText.text == "Schritt 6")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            if (FirstPart.transform.localPosition.z <= 500f && PaletteIsOn == 1)
            {
                score = 4000;
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    RenewOutput(0);
                    ChangeMission32();
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        if(Input.GetKey("b"))
        {
            StepNumText.text = "Schritt 6";
            ChangeMission33();
        }
        if (missionText.text == "Hubgerüst zurückneigen" && StepNumText.text == "Schritt 6")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            //Stapler.transform.localPosition.x < -800
            if (AngleGerüstX < -5f /*&& PaletteIstOn == 1*/)
            {
                score = 4500;
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    PosStaplerZ0 = Stapler.transform.localPosition.z;
                    PosStaplerX0 = Stapler.transform.localPosition.x;
                    PosStaplerY0 = Stapler.transform.localPosition.y;
                    RenewOutput(0);
                    ChangeMission33();
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        if (missionText.text == "Zum markierten Abgabeort fahren" && StepNumText.text == "Schritt 7")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            pfeileRd8.enabled = true;
            pfeileRd9.enabled = true;
            if (Stapler.transform.localPosition.x < 4300)
            { pfeileRd8.enabled = false; }
            if (Stapler.transform.localPosition.x < 4100)
            {
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    RenewOutput(0);
                    PosStaplerZ0 = Stapler.transform.localPosition.z;
                    PosStaplerX0 = Stapler.transform.localPosition.x;
                    PosStaplerY0 = Stapler.transform.localPosition.y;
                    ChangeMission34();
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        if (missionText.text == "Palette auf Lagerplatz fahren" && StepNumText.text == "Schritt 8")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;

            if (Stapler.transform.localPosition.x < 3600)
            { pfeileRd9.enabled = false; }
            if (Stapler.transform.localPosition.x < 3112 && Stapler.transform.localPosition.x > 3003 && Stapler.transform.localPosition.z < -2066f && Stapler.transform.localPosition.z > -2187f)
            {
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    RenewOutput(1);
                    pfeileRd4.enabled = false;
                    ChangeMission35();
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        if (missionText.text == "Hubgerüst senkrecht stellen" && StepNumText.text == "Schritt 8")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            if (AngleGerüstX > 0f && AngleGerüstX < 2f && PaletteIsOn == 1)
            {
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    RenewOutput(0);
                    ChangeMission36();
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        if (missionText.text == "Palette absetzen" && StepNumText.text == "Schritt 8")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            if (PaletteIsOn != 1)
            {
                WE.enabled = false;
                pfeileRd4.enabled = false;
                ZielStart.enabled = true;
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    RenewOutput(0);
                    ChangeMission37();
                    PosStaplerZ0 = Stapler.transform.localPosition.z;
                    PosStaplerX0 = Stapler.transform.localPosition.x;
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        if (missionText.text == "Gabel aus Palette ziehen" && StepNumText.text == "Schritt 8")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            if (Mathf.Abs(Stapler.transform.localPosition.z - PosStaplerZ0) > 100 || Mathf.Abs(Stapler.transform.localPosition.x - PosStaplerX0) > 100)
            {
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    RenewOutput(0);
                    ChangeMission38();
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        if (missionText.text == "Gabel bodenfrei anheben" && StepNumText.text == "Schritt 8")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            if (FirstPart.transform.localPosition.z < 550f && FirstPart.transform.localPosition.z > 270f)
            {
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    RenewOutput(0);
                    PosStaplerZ0 = Stapler.transform.localPosition.z;
                    PosStaplerX0 = Stapler.transform.localPosition.x;
                    PosStaplerY0 = Stapler.transform.localPosition.y;
                    ChangeMission39();
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
        if (missionText.text == "Zum Startpunkt zurückfahren" && StepNumText.text == "Schritt 9")
        {
            TimeCountStep += Time.deltaTime;
            totalTime = totalTime + Time.deltaTime;
            if (Stapler.transform.localPosition.z < -1457f && Stapler.transform.localPosition.z > -1521f && Stapler.transform.localPosition.x < 2510f && Stapler.transform.localPosition.x > 2355)
            {
                score = 5000;
                TimeCountAch += Time.deltaTime;
                if (TimeCountAch >= AchieveTime)
                {
                    RenewOutput(1);
                    CounterChange =CounterChange+1;
                    SceneNameText.text = " ";
                    missionText.text = " ";
                    if (CounterChange != TestNumIndex+1)
                    {
                        missionTagText.text = CounterChange-1 + ".Versuch FERTIG!";
                        File.AppendAllText(PathWithD, "\r\n" + CounterChange + ". -" + System.DateTime.Now.ToString() + " \r\n");
                        File.AppendAllText(PathWtouD, "\r\n" + CounterChange + ". -" + System.DateTime.Now.ToString() + " \r\n");
                    }
                    else
                    {
                        missionTagText.text = "Herzlichen Dank für Ihre Teilname";
                    }
                    Invoke("MissonEnd", InvokeTimeMisson);
                }
            }
            else
            {
                TimeCountAch = 0f;
            }
        }
    }
}