//This scripts runs the main lab code for the Bird Bone Desnity lab. This script is placed in the main camera object, positioned in front of the desk and bird. 
//find ..# to navigate
//12 - display message
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

//main lab script

public class Lab_UI : MonoBehaviour 
{
    public int variant;
    public Texture woodNormal;
    string[] brk_variations = new string[3];
    public Transform[] modelstruts = new Transform[4];
    public Font arial;
	float h; //screen width
	float w; //screen height
	public Texture white; //flat texture for testing ui element positions
    GUIStyle gstyle;
    int dragmat = 0; //material being clicked/dragged
    Vector3 mp; //mouse position
    public Texture[] tmat;
    string[] mstring = new string[7]; //save material names
    string[] tstring = new string[3]; //saves thickness display text
    public int chosenMat = 6; //current bone material
    //public Texture blueprint; //
    bool showBP = true; //show blueprint options
    public Color sbcolor; //sidebar color
    public Color buttonColor; //dark blue button color
    public Texture sbButton; //standard button texture
    public GameObject[] models; //the static and animated versions of the bird
    public bool test; //is bird currently simulating?
    int result; //final result of success, break, or heavy depending on choices
    string[] animations = new string[3]; //animation names, used to play specific animations
    public float stopTimer; //timer for swapping models
    public float fade; //used for fading the screen to black
    public Transform[] lookPoints; //notable aim points for the camera
    public int lookTarget = 0; //what the camera is currently pointing at out of lookPoints
    public string buildNumber; //build version
    public Renderer[] wbones; //objects to change materials for visualizing the current bone material
    public Color[] bcolor; //bone mat colors for visualizing bone material
    public ParticleSystem smoke; //effect for successful flight
    public ParticleSystem crashSmoke; //effect for crash animation
    public Transform[] btCylinder; //cylinder objects used for visualizing bone thickness
    public int thickness = 1; // bird's bone thickness
    int pthickness; //previous thickness, used to check for changes to thickness, allows for realtime debugging in editor
    float[] bscales = new float[3]; //scale values for visualizing bone thickness
    public Transform focal; //dummy object for aiming the camera
    //bool slide; //if true, slide in new bird
    public bool fly; //causes animated bird model to move forward
    public bool useFade; //use fade transition or slide transition
    public int cursorType; //changes cursor betweeen normal, wrench, and screwdriver
    public Texture wrench;
    public Texture screwdriver;
    public bool dragEnabled;
    public int sfeed = 0; //chooses message to display to fading text feed
    public float sfadefeed; //feed timer and fade
    public Color feedColor = new Color(1, 1f, 1f, 1); //feed color 
    string[] feedMessages = new string[13]; //list of displayable feed messages
    int pfeed;
    string[] positiveFeedback = new string[3];
    public GameObject go_wrench; //used to enable and re-enable wrench and screwdriver during testing
    public GameObject go_screwdriver;
    float[] boxFades = new float[6]; //fade effect over materials when material is added
    string[,] rtable = new string[4,7]; //recorded results table
    string[,] ptable = new string[4, 7]; //recorded results table
    string[,] stable = new string[4, 7];
    string[] stinputs = new string[4];
    string[,] finaltable = new string[4, 7]; //goal results table 
    public bool showtable; //show or unshow table when button is clicked
    Rect rnbutton; //results notbebook rect
    public Texture t_questionCursor; //? cursor for mouse over descriptions
    public Texture t_questionButton; //? button for activating tip cursor
    public Texture t_questionBright; 
    bool showTooltip; //check for activating ? cursor tooltips
    Rect tooltipRect; //rect for ? button
    string[] s_tooltips = new string[6]; //material descriptions
    int tiptoshow; //which tip is being mouseovered
    public Font consolas; //consolas font which is better for text that requires alignment
    float[] descriptionHeights = new float[6]; //adjustable heights for each description box
    float recordtotable; //timer for autopopulating the results table after a delay
    float recordFade; //fade effect after the results box is autopopulated
    float tableFade; //fade out table after results are recorded
    public float tickmarkFade; //timer and movement control for the check or x mark that shows after testing an outcome.
    int markType; //which tick mark to show, x or checkmark
    public Texture[] t_marks; //display textures for tick marks
    bool cornerTooltip = true; //enable or disable corner tooltips
    bool hoverOverCorner; //check for if the mouse is hovering over a tooltip corner
    bool bookEnabled = true; //enable or disable book based mat descriptions
    public bool showBook; //show or hide book when book button is pressed
    int page; //which mat description is displaying in book
    public Texture t_book; //book button texture
    Rect bookRect; //book button rect
    public Texture t_left; //book left arrow texture
    public Texture t_right; //book right arrow texture
    //Rect leftBookR; //left book arrow rect
    //Rect rightBookR; //right book arrow rect
    public ParticleSystem[] smokecloud; //smoke effect after crash animation
    public bool b_smokecloud; //starts smoke effect
    public Texture highlightScrewdriver; //texture for mousing over bird with screwdriver
    public Texture highlightWrench; //texture for mousing over material with wrench
    bool hoveringMat; //is user mousing over a material with the wrench tool?
    public bool logResults; //pauses transition after fading to black for logging results
    bool startFade; //starts fade effect
    bool hoverTable; //are we hovering over a table box?
    int gx; //which box did we last hover over?
    int gy;
    int px = -1; //which box did we last change
    int py = -1;
    bool pendingTableUpdate;
    public Texture sliceEffect;
    float slicetimer;
    int previousTestThickness = -1;
    int previousTestchosenMat = -1;
    Rect prntRect;
    Rect fullTableRect;
    public Color[] colCustom;
    float bookClosed;
    string[] bookText = new string[28];

    public Texture t_chart;

    void Start() //initialize
    {
        bookText[0] = "Material";
        bookText[1] = "Ceramic";
        bookText[2] = "Graphite";
        bookText[3] = "Wood";
        bookText[4] = "Titanium";
        bookText[5] = "Plastic";
        bookText[6] = "Steel";
        bookText[7] = "Strengths";
        bookText[8] = "Ceramic is a strong (20,000 psi) under direct pressure, especially compared to weight.  Mold-able until cured.";
        bookText[9] = "Thirty times stronger than steel in directional strength. It is a good choice for structural components.";
        bookText[10] = "Inexpensive, readily available, strong under compression in longitudinal direction.";
        bookText[11] = "Very malleable and several times stronger than steel (63,000 psi) with only a percentage of the weight.";
        bookText[12] = "Inexpensive, mold-able and non corrosive.  Relatively strong for it’s weight (12,400 psi).";
        bookText[13] = "Extremely strong (36,000 psi minimum) and malleable.";
        bookText[14] = "Weaknesses";
        bookText[15] = "Very brittle and does not have high structural strength for torsion pressures.";
        bookText[16] = "Very expensive. Strength only in very specific directs and under specific designs.";
        bookText[17] = "Very susceptible to environmental damage such as water and sunlight. Strength to weight ratio is low.";
        bookText[18] = "Extremely expensive and difficult to produce and machine.";
        bookText[19] = "Susceptible to heat and light damage. Disfigures easily under excessive loads.";
        bookText[20] = "Heavy and moderately expensive.";
        bookText[21] = "Density";
        bookText[22] = "Very light weight";
        bookText[23] = "Very light weight";
        bookText[24] = "Depends on the species. Strength directly proportional to density.";
        bookText[25] = "Extremely light";
        bookText[26] = "Relatively light";
        bookText[27] = "Extremely heavy.";

        descriptionHeights[0] = .68f;
        descriptionHeights[1] = .68f;
        descriptionHeights[2] = .45f;
        descriptionHeights[3] = .68f;
        descriptionHeights[4] = .75f;
        descriptionHeights[5] = .68f;

        white = (Texture)Resources.Load("white"); //load blank white texture placed in Assets/Resources folder, folder named 'Resources' required
        gstyle = new GUIStyle();
        mstring[0] = "Ceramic";
        mstring[1] = "Graphite";
        mstring[2] = "Wood";
        mstring[3] = "Titanium";
        mstring[4] = "Plastic";
        mstring[5] = "Steel";
        mstring[6] = ""; 

        tstring[0] = "Thin";
        tstring[1] = "Medium";
        tstring[2] = "Thick";

        animations[0] = "Fly";
        animations[1] = "Heavy";
        animations[2] = "Break";

        brk_variations[0] = "Snap";
        brk_variations[1] = "Break";
        brk_variations[2] = "Shatter";
        changeBones();

        bscales[0] = .5f;
        bscales[1] = 1f;
        bscales[2] = 1.25f;
        changeThickness();

        feedMessages[0] = "Cannot test flight while a material is not selected. Use the wrench to select a material.";
        feedMessages[1] = "Use the screwdriver to remove the current bone before attaching a new bone type.";
        feedMessages[2] = "You must add a bone before you can remove it. Try clicking a bone material with the wrench.";
        feedMessages[3] = "The bird takes flight!";
        feedMessages[4] = "The bird is too heavy to fly...";
        feedMessages[5] = "The bird's bones are too flimsy and break.";
        feedMessages[6] = "Log your results!";
        feedMessages[7] = "Changed the wrong entry!";
        feedMessages[8] = "Attempted to log the wrong result!";
        feedMessages[9] = "Chart saved. Try clicking 'Copy to Clipboard' above.";

        string pfsize = Mathf.RoundToInt(Screen.height * .07f).ToString();
        positiveFeedback[0] = "<color=#FFFFAAFF><b><size="+ pfsize + ">Well done!</size></b></color>";
        positiveFeedback[1] = "<color=#FFAAFFFF><b><size=" + pfsize + ">Good job!</size></b></color>";
        positiveFeedback[2] = "<color=#AAFFFFFF><b><size=" + pfsize + ">Nice work!</size></b></color>";

        //initialize table to avoid initialization errors
        for(int row = 0; row<4; row++)
        {
            for (int column = 0; column < 7; column++)
            {
                rtable[row, column] = "";
            }

        }
        rtable[0, 0] = "Size";
        rtable[1, 0] = "Thin";
        rtable[2, 0] = "Medium";
        rtable[3, 0] = "Thick";
        for(int i = 0; i<6; i++)
        {
            rtable[0, i+1] = mstring[i];
        }
        copyStringTable(rtable, ptable, 4, 7);

        s_tooltips[0] = "For a metal, the compressive strength is near that of the tensile strength, while for a ceramic, the compressive strength may be 10 times the tensile strength. Alumina, for example, has a tensile strength of 20,000 psi 1138 MPa), while the compressive strength is 350,000 psi (2400 MPa).";
        s_tooltips[1] = "Due to the strength of its 0.142 Nm-long carbon bonds, graphene is the strongest material ever discovered, with an ultimate tensile strength of 130,000,000,000 Pascals (or 130 gigapascals), compared to 400,000,000 for A36 structural steel, or 375,700,000 for Aramid (Kevlar).";
        s_tooltips[2] =
@"Compression and bending strengths of wood species typically used in beams:

Stress (psi)
Wood Species	Bending Compression  Perpendicular  Parallel      
                Horizontal Shear:    to Grain:      to Grain:
                Wet     Dry          Wet    Dry      Wet    Dry
Birch, Yellow   1417    1668         477    715      960    1200
Fir, Douglas    1417    1668         417    625     1360    1700
Larch, Western  1417    1668         417    625     1360    1700
Maple, Red      1271    1495         410    615      880    1100
Oak, Black      1369    1610         590    885      920    1150
Pine, White     1222    1438         223    335      960    1200
Redwood         1320    1553         433    650     1200    1500
1 psi (lb/in2) = 6,894.8 Pa (N/m2) = 6.895x10-3 N/mm2"; 
        s_tooltips[3] = "It is paramagnetic and has fairly low electrical and thermal conductivity. Commercial (99.2% pure) grades of titanium have ultimate tensile strength of about 434 MPa (63,000 psi), equal to that of common, low-grade steel alloys, but are less dense.";
        s_tooltips[4] = "Nylon - 12,400 PSI.";
        s_tooltips[5] = "A36 steel in plates, bars, and shapes with a thickness of less than 8 in (203 mm) has a minimum yield strength of 36,000 psi (250 MPa) and ultimate tensile strength of 58,000–80,000 psi (400–550 MPa). Plates thicker than 8 in have a 32,000 psi (220 MPa) yield strength and the same ultimate tensile strength.";
    }

    void calculateResult() //calculate final outcome of choices
    {
        //find outcome
        result = 0;
        if (chosenMat == 3 && thickness == 2)//titanium
            result = 1;
        else if (chosenMat == 4) //plastic
        {
            result = 2;
            if (thickness == 2)
                result = 1;
        }
        else if(chosenMat == 0 || chosenMat == 1) //ceramic or graphite
        {
            if (thickness == 0)
                result = 2;
            if (thickness == 2)
                result = 1;
        }
        else if(chosenMat == 2) //wood
        {
            result = 1;
            if (thickness == 0)
                result = 2;
        }
        else if(chosenMat==5) //stainless steel
        {
            result = 1;
            if (thickness == 0)
                result = 0;
        }
    }

    void Update()
    {
        if (!test)
        {
            lookTarget = 0;
            fly = false;
        }
        dragEnabled = cursorType == 1;
        if (chosenMat != 6 && cursorType == 1)
            cursorType = 0;
        if (bookClosed > 0)
            bookClosed -= Time.deltaTime;
        if(prntRect.Contains(mp) && !logResults && showtable && Input.GetMouseButtonDown(0))
        {
            sfeed = 9;
            sfadefeed = 4;
            //Screenshot ss = GetComponent<Screenshot>();
            //ss.MakeScreenshot(fullTableRect);
            string chartstring = "--------------------------------------------------------------------\n";
            chartstring += "| Size   | Ceramic | Graphite | Wood  | Titanium | Plastic | Steel |\n";
            chartstring += "--------------------------------------------------------------------\n";
            chartstring += "| Thin   | ";
            //Thin
            for(int i = 0; i<6; i++)
            {
                chartstring += rtable[1, i + 1];
                if (rtable[1, i + 1] == "Fly")
                    chartstring += "  ";
                else if (rtable[1, i + 1] == "")
                    chartstring += "     ";
                if (i == 0)
                    chartstring += "  ";
                else if (i == 1)
                    chartstring += "   ";
                else if (i == 2)
                    chartstring += "";
                else if (i == 3)
                    chartstring += "   ";
                else if (i == 4)
                    chartstring += "  ";
                else if (i == 5)
                    chartstring += "";
                if (i < 5)
                    chartstring += " | ";
                else
                    chartstring += " |\n";
            }
            //Medium
            chartstring += "| Medium | ";
            for (int i = 0; i < 6; i++)
            {
                chartstring += rtable[2, i + 1];
                if (rtable[2, i + 1] == "Fly")
                    chartstring += "  ";
                else if (rtable[2, i + 1] == "")
                    chartstring += "     ";
                if (i == 0)
                    chartstring += "  ";
                else if (i == 1)
                    chartstring += "   ";
                else if (i == 2)
                    chartstring += "";
                else if (i == 3)
                    chartstring += "   ";
                else if (i == 4)
                    chartstring += "  ";
                else if (i == 5)
                    chartstring += "";
                if (i < 5)
                    chartstring += " | ";
                else
                    chartstring += " |\n";
            }
            //Thick
            chartstring += "| Thick  | ";
            for (int i = 0; i < 6; i++)
            {
                chartstring += rtable[3, i + 1];
                if (rtable[3, i + 1] == "Fly")
                    chartstring += "  ";
                else if (rtable[3, i + 1] == "")
                    chartstring += "     ";
                if (i == 0)
                    chartstring += "  ";
                else if (i == 1)
                    chartstring += "   ";
                else if (i == 2)
                    chartstring += "";
                else if (i == 3)
                    chartstring += "   ";
                else if (i == 4)
                    chartstring += "  ";
                else if (i == 5)
                    chartstring += "";
                if (i < 5)
                    chartstring += " | ";
                else
                    chartstring += " |\n";
            }
            chartstring += "--------------------------------------------------------------------";
            Application.ExternalCall("SaveChart", chartstring);
        }
        if (slicetimer > 0)
            slicetimer -= Time.deltaTime*5;
        if(logResults)
        {
            if(hoverTable && Input.GetMouseButtonDown(0))
            {
                if(!pendingTableUpdate)
                {
                    pendingTableUpdate = true;
                    px = gx;
                    py = gy;
                }
                else if(gx!=px || gy!=py)
                {
                    copyStringTable(ptable, rtable, 4, 7);
                }
                if(rtable[gy,gx] == animations[0])
                {
                    rtable[gy, gx] = animations[1];
                    px = gx;
                    py = gy;
                }
                else if(rtable[gy,gx] == animations[1])
                {
                    rtable[gy, gx] = animations[2];
                    px = gx;
                    py = gy;
                }
                else if(rtable[gy,gx] == animations[2])
                {
                    rtable[gy, gx] = animations[0];
                    px = gx;
                    py = gy;
                }
                else
                {
                    rtable[gy, gx] = animations[0];
                    px = gx;
                    py = gy;
                }
                
            }
        }
        if (b_smokecloud)
        {
            if (!smokecloud[variant-2].isPlaying)
                smokecloud[variant-2].Play();
            if(!test)
            {
                b_smokecloud = false;
                if (smokecloud[variant-2].isPlaying)
                    smokecloud[variant-2].Stop();

                smokecloud[variant - 2].Clear();
            }
        }
        /*if(leftBookR.Contains(mp) && showBook && Input.GetMouseButtonDown(0))
        {
            page--;
            if (page < 0)
                page = 5;
        }
        else if (rightBookR.Contains(mp) && showBook && Input.GetMouseButtonDown(0))
        {
            page++;
            if (page > 5)
                page = 0;
        }*/
        if (bookRect.Contains(mp))
        {
            if(Input.GetMouseButtonDown(0) && cursorType == 0 && !test && !logResults)
            {
                showBook = !showBook;
            }
        }

        //check mark timer (obsolete)
        if (tickmarkFade > 0)
        {
            tickmarkFade -= Time.deltaTime*2;
            if(tickmarkFade<=0)
                recordtotable = .25f;
        }
        if (tableFade > 0)
            tableFade -= Time.deltaTime*1f;
        if(recordFade>0)
        {
            recordFade -= Time.deltaTime*2;
            if (recordFade <= 0)
                tableFade = 1;
        }
        if (recordtotable > 0)
        {
            recordtotable -= Time.deltaTime*2;
            if(recordtotable<=0)
            {
                rtable[thickness + 1, chosenMat + 1] = animations[result];
                recordFade = 1;
            }
        }

        //click event for tooltip button
        if(cursorType==3 && Input.GetMouseButtonDown(0))
        {
            cursorType = 0;
            showTooltip = false;
        }
        else if (tooltipRect.Contains(mp) && !test && !logResults && (cursorType == 0||cursorType==3))
        {
            if(Input.GetMouseButtonDown(0))
            {
                showTooltip = !showTooltip;
                if (showTooltip)
                    cursorType = 3;
                else
                    cursorType = 0;
            }
        }
        

        //update fade effect for selecting a material
        for(int i = 0; i<boxFades.Length; i++)
        {
            if (boxFades[i] > 0)
                boxFades[i] -= Time.deltaTime*3.5f;
        }

        //hide tools while testing
        go_wrench.SetActive(!test);
        go_screwdriver.SetActive(!test);

        //update feed fade
        if (sfadefeed > 0)
            sfadefeed -= Time.deltaTime;
        //show and hide mouse cursor
        if (cursorType != 0)
        {
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
        }

        //update thickness if changed
        if (pthickness != thickness)
        {
            changeThickness();
            pthickness = thickness;
        }
        //update camera aim position
        focal.position = Vector3.MoveTowards(focal.position, lookPoints[lookTarget].position,Time.deltaTime*2);
        transform.LookAt(focal);
        //if flying, move the bird forward
        if(fly)
        {
            models[1].transform.position -= Vector3.forward * Time.deltaTime * 1f;
            lookTarget = 2;
        }
        if (stopTimer > 0) //wait for delay 
        {
            stopTimer -= Time.deltaTime*.5f;
            fly = false;

            //fade = Mathf.MoveTowards(fade, 1, Time.deltaTime * 1.5f);
                
            if (stopTimer<=0)
            {
                startFade = true;
                if (rtable[thickness + 1, chosenMat + 1] == animations[0] || rtable[thickness + 1, chosenMat + 1] == animations[1] || rtable[thickness + 1, chosenMat + 1] == animations[2])
                {
                }
                else
                {
                    logResults = true;
                    feedColor = Color.white;
                    sfeed = 6;
                    sfadefeed = 4;
                    pendingTableUpdate = false;
                }
            }
        }
        else //showbp when not in simulation
        {
            //use fade transition if enabled
            if(startFade)
            {
                fade = Mathf.MoveTowards(fade, 1, Time.deltaTime * 1.5f);
                if (fade == 1 && !logResults)
                    startFade = false;
                if (test && fade==1)
                {
                    lookTarget = 0;
                    focal.position = Vector3.MoveTowards(focal.position, lookPoints[lookTarget].position, Time.deltaTime * 200);
                    test = false;
                    models[0].SetActive(true);
                    for(int i = 1; i<models.Length; i++)
                        models[i].SetActive(false);
                    Vector3 goalpos = models[1].transform.position;
                    goalpos.x = 0;
                    goalpos.z = models[0].transform.position.z;
                    models[1].transform.position = goalpos;
                }
            }
            else
                fade = Mathf.MoveTowards(fade, 0, Time.deltaTime * 1.5f);
            if (fade == 0 && !test && !logResults)
                showBP = true;
        }

        if (test) //start test simulation
        {
            if (models[0].activeSelf)
            {
                lookTarget = 1;
                calculateResult();
                models[0].SetActive(false);
                variant = Random.Range(2,5);
                if(result==2) //crash variations
                {
                    models[variant].SetActive(true);
                    lookTarget = variant+1;
                }
                else
                    models[1].SetActive(true); //fly or heavy
                
                if (result == 0)
                    markType = 1;
                else
                    markType = 0;
                sfeed = 3 + result;
                sfadefeed = 4;
                if (result == 0)
                {
                    slicetimer = 2;
                    feedColor = new Color(.7f, 1, 1, 1);
                }
                else if(result == 1)
                {
                    feedColor = new Color(1, 1, .7f, 1);
                }
                else if(result == 2)
                {
                    feedColor = new Color(1, .7f, .7f, 1);
                }
                if(result<2)
                    models[1].GetComponent<Animator>().Play(animations[result], 0, 0);
                else
                    models[variant].GetComponent<Animator>().Play(brk_variations[variant-2], 0, 0);
            }
        }
        if (rnbutton.Contains(mp) && !showBook)
        {
            if (Input.GetMouseButtonDown(0) && !test && tableFade<=0 && recordFade<=0 && !logResults)
            {
                showtable = !showtable;
            }
        }
    } //End Update()

    void OnGUI() //Start GUI
    {
        hoveringMat = false;
        hoverTable = false;
        h = Screen.height;
        w = Screen.width;
        GUI.skin.button.alignment = TextAnchor.MiddleCenter;
        gstyle.fontSize = Mathf.RoundToInt(h * .0225f);
        gstyle.wordWrap = false;
        gstyle.normal.textColor = Color.white;
        //GUI.skin.button.fontSize = gstyle.fontSize; //set font size for buttons
        mp = Input.mousePosition; //mouse position
        mp.y = h - mp.y; //flip y origin for use with .contains()
        hoverOverCorner = false;

        //draw fade-to-black effect
        if (useFade)
        {
            GUI.color = new Color(0, 0, 0, fade);
            GUI.DrawTexture(new Rect(0, 0, w, h), white);
            reset();
        }

        //draw sidebar
        GUI.color = sbcolor;
        Rect sbR = new Rect(w * .77f, 0, w * .23f, h);
        GUI.DrawTexture(sbR, white);
        reset();

        //draw sidebar buttons
        //Test
        Rect button1 = new Rect(w * .77f, h * .025f, w * .23f, h * .15f);
        GUI.DrawTexture(button1, sbButton);
        GUIStyle buttonStyle = new GUIStyle(gstyle);
        buttonStyle.fontSize = Mathf.RoundToInt(.05f * h);
        GUI.Label(button1, "Test", buttonStyle);
        if (button1.Contains(mp) && !showBook)
        {
            GUI.color = Color.yellow;
            GUI.Label(button1, "Test", buttonStyle);
            reset();
            if (Input.GetMouseButtonDown(0) && !test)
            {
                if (chosenMat == 6)
                {
                    sfeed = 0;
                    sfadefeed = 4f;

                }
                else
                {
                    showBP = false;
                    showtable = false;
                    showTooltip = false;
                    cursorType = 0;
                    test = true;
                }
            }
        }

        //Help
        Rect button2 = new Rect(button1);
        button2.y += button2.height;
        GUI.DrawTexture(button2, sbButton);
        GUI.Label(button2, "Help", buttonStyle);
        if (button2.Contains(mp) && !showBook && bookClosed<=0)
        {
            GUI.color = Color.yellow;
            GUI.Label(button2, "Help", buttonStyle);
            reset();
            if (Input.GetMouseButtonDown(0) && !test && !logResults && fade==0)
            { 
                GetComponent<Instructions_UI>().enabled = true;
                enabled = false;
            }
        }

        //Exit
        Rect button3 = new Rect(button2);
        button3.y += button3.height;
        GUI.DrawTexture(button3, sbButton);
        GUI.Label(button3, "Exit", buttonStyle);
        if (button3.Contains(mp)&&!showBook)
        {
            GUI.color = Color.yellow;
            GUI.Label(button3, "Exit", buttonStyle);
            reset();
            if (Input.GetMouseButtonDown(0) && !test)
            {
                Application.Quit();
            }
        }

        //Results Notebook button
        rnbutton = new Rect(button3);
        rnbutton.y = h * .7f;
        rnbutton.x = w * .79f;
        rnbutton.xMax = w * .975f;
        rnbutton.height = rnbutton.width*.8f;
        GUI.DrawTexture(rnbutton, white);
        rnbutton.xMin += h * .015f;
        rnbutton.xMax -= h * .015f;
        rnbutton.yMax -= h * .015f;
        rnbutton.yMin += h * .015f;
        GUI.color = buttonColor;
        
        GUI.DrawTexture(rnbutton, white);
        if (recordFade>0)
        {
            GUI.color = new Color(1, 1, 1, recordFade);
            GUI.DrawTexture(rnbutton, white);
        }
        reset();
        GUIStyle rnstyle = new GUIStyle(gstyle);
        rnstyle.wordWrap = true;
        rnstyle.fontSize = Mathf.RoundToInt(.05f * h);
        if(rnbutton.Contains(mp) && !showBook)
        {
            GUI.color = Color.yellow;
        }
        GUI.Label(rnbutton, "Results Notebook", rnstyle);
        reset();

        //sb info
        gstyle.alignment = TextAnchor.MiddleLeft;
        Rect buildRect = new Rect(button3);
        buildRect.x += h * .025f;
        buildRect.y += h * .115f;
        GUI.Label(buildRect, "Current build: v" + buildNumber,gstyle);

        Rect lwingRect = new Rect(buildRect);
        lwingRect.y += h * .06f;
        GUI.Label(lwingRect, "Wing Material: " + mstring[chosenMat], gstyle);

        Rect btRect = new Rect(lwingRect);
        btRect.y += h * .06f;
        GUI.Label(btRect, "Bone Thickness: " + tstring[thickness], gstyle);

        

        gstyle.alignment = TextAnchor.MiddleCenter;
        if (showBP || logResults || fade != 0)
        {
            //draw mat backdrop
            Rect mbackR = new Rect(w * .05f, h * .835f, w * .555f, h * .15f);
            if (cursorType == 1)
                GUI.color = colCustom[0];
            GUI.DrawTexture(mbackR, white);
            reset();

            Rect mbackinR = new Rect(mbackR);
            mbackinR.width *= .98f;
            mbackinR.height -= mbackR.width - mbackinR.width;
            mbackinR.x = mbackR.center.x - mbackinR.width * .5f;
            mbackinR.y = mbackR.center.y - mbackinR.height * .5f;
            GUI.color = buttonColor;
            if (cursorType == 1)
                GUI.color = Color.Lerp(buttonColor, Color.white, .125f);
            GUI.DrawTexture(mbackinR, white);
            reset();

            //draw bone mat buttons
            Rect[] matR = new Rect[6];
            tiptoshow = 0;
            for (int i = 0; i < 6; i++)
            {
                matR[i] = new Rect(w * .065f, h * .855f, h * .115f, h * .075f);
                if (i < 6) //offsets for row 1
                {
                    matR[i].x += w * .088f * i;

                }
                else //offsets for row 2
                {
                    matR[i].x += w * .088f * (i - 3);
                    matR[i].y += h * .125f;
                }
                Rect srect = matR[i];
                srect.x += srect.width * .5f;
                srect.y += srect.height * 1.3f;
                srect.width = 0;
                srect.height = 0;
                gstyle.alignment = TextAnchor.MiddleCenter;
                GUI.Label(srect, mstring[i], gstyle);
                //when not dragging a material, set to drag hovered material when user clicks
                if (dragmat == 0)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (dragEnabled)
                        {
                            if (matR[i].Contains(mp))
                            {
                                dragmat = i + 1;
                            }
                        }
                        else
                        {
                            if (matR[i].Contains(mp))
                            {
                                if (cursorType == 1)
                                {
                                    chosenMat = i;
                                    changeBones();
                                    cursorType = 0;
                                    boxFades[i] = 1;
                                }
                            }
                        }
                    }
                }
                else if (dragmat == i + 1) //if material is being dragged, set position to mouse position.
                {
                    matR[i].x = mp.x - matR[i].width * .5f;
                    matR[i].y = mp.y - matR[i].height * .5f;
                }
                if (chosenMat != i && chosenMat != 6)
                    setAlpha(.1f);
                GUI.DrawTexture(matR[i], tmat[i]);
                reset();

                //tooltip box at corner of materials
               /* Rect tcornerRect = new Rect(matR[i]);
                tcornerRect.y -= tcornerRect.height * .1f;
                tcornerRect.height = tcornerRect.height * .5f;
                tcornerRect.xMin = tcornerRect.xMax -tcornerRect.height*.5f;
                tcornerRect.xMax = tcornerRect.x + tcornerRect.height;
                tcornerRect.x = matR[i].x;
                GUI.DrawTexture(tcornerRect, t_questionBright);
                if (tcornerRect.Contains(mp) && cornerTooltip)
                {
                    tiptoshow = i + 1;
                    hoverOverCorner = true;
                }
                tcornerRect = shrinkRect(tcornerRect, tcornerRect.height * .2f);
                GUI.color = buttonColor;*/
                //GUI.DrawTexture(tcornerRect, white);
                

                reset();
                if (matR[i].Contains(mp))
                {
                    tiptoshow = i + 1;
                    hoveringMat = true;
                }
                //draw box fade effect when material is selected
                if (boxFades[i] > 0)
                {
                    GUI.color = new Color(1, 1, 1, boxFades[i]);
                    matR[i].x -= matR[i].height * .125f;
                    matR[i].y -= matR[i].height * .125f;
                    matR[i].width += matR[i].height * .25f;
                    matR[i].height += matR[i].height * .65f;

                    GUI.DrawTexture(matR[i], white);
                    reset();
                }
                reset();
            }

            

            //draw thickness buttons
            gstyle.fontSize = Mathf.RoundToInt(h * .0375f);

            //thin
            Rect thinRect = new Rect(w * .605f, h * .68f, w * .16f, h * .12f);
            thinRect = shrinkRect(thinRect, h * .02f);
            if (thickness == 0)
                GUI.color = colCustom[0];
            GUI.DrawTexture(thinRect, white);
            Rect rshrink = new Rect(thinRect);
            rshrink = shrinkRect(rshrink, h * .0075f);
            GUI.color = buttonColor;
            if (thickness == 0)
                GUI.color = colCustom[1];
            GUI.DrawTexture(rshrink, white);
            reset();
            if (thinRect.Contains(mp) && !showBook)
            {
                GUI.color = Color.yellow;
                if (Input.GetMouseButtonDown(0) && !test && !logResults)
                    thickness = 0;
            }
            GUI.Label(thinRect, "Thin", gstyle);
            reset();

            //medium
            Rect mediumRect = new Rect(w * .605f, h * .78f, w * .16f, h * .12f);
            mediumRect = shrinkRect(mediumRect, h * .02f);
            if (thickness == 1)
                GUI.color = colCustom[0];
            GUI.DrawTexture(mediumRect, white);
            rshrink = new Rect(mediumRect);
            rshrink = shrinkRect(rshrink, h * .0075f);
            GUI.color = buttonColor;
            if (thickness == 1)
                GUI.color = colCustom[1];
            GUI.DrawTexture(rshrink, white);
            reset();
            if (mediumRect.Contains(mp) && !showBook)
            {
                GUI.color = Color.yellow;
                if (Input.GetMouseButtonDown(0) && !test && !logResults)
                    thickness = 1;
            }
            GUI.Label(mediumRect, "Medium", gstyle);
            reset();

            //thick
            Rect thickRect = new Rect(w * .605f, h * .88f, w * .16f, h * .12f);
            thickRect = shrinkRect(thickRect, h * .02f);
            if (thickness == 2)
                GUI.color = colCustom[0];
            GUI.DrawTexture(thickRect, white);
            rshrink = new Rect(thickRect);
            rshrink = shrinkRect(rshrink, h * .0075f);
            GUI.color = buttonColor;
            if (thickness == 2)
                GUI.color = colCustom[1];
            GUI.DrawTexture(rshrink, white);
            reset();
            if (thickRect.Contains(mp) && !showBook)
            {
                GUI.color = Color.yellow;
                if (Input.GetMouseButtonDown(0) && !test && !logResults)
                    thickness = 2;
            }
            GUI.Label(thickRect, "Thick", gstyle);
            reset();

            Rect modelwing = new Rect(w * .15f, h * .3f, w * .475f, h * .25f); //release area for dropping and changing bone materials
            //GUI.DrawTexture(modelwing,white);
            if (!dragEnabled)
            {
                if (cursorType == 2 && modelwing.Contains(mp))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        chosenMat = 6;
                        changeBones();
                        cursorType = 0;
                    }
                }
            }
            //if currently dragging a material and mouse is released over drop area, change material and reset drag status
            //GUI.color = new Color(1, 1, 1, .4f);
            //GUI.DrawTexture(modelwing,white);
            reset();

            if (dragmat > 0 && Input.GetMouseButtonUp(0) && modelwing.Contains(mp))
            {
                //change wing mat
                chosenMat = dragmat - 1;
                dragmat = 0;
                changeBones();
            }
            else if (!Input.GetMouseButton(0)) //reset drag status even if they release the mouse outside of the drop area
                dragmat = 0;

            //drawTooltipButton();
        } //end of showbp
        else
            showTooltip = false;

        //draw table
        if(showtable || recordtotable>0 ||tableFade>0 || recordFade>0 ||(logResults && fade==1))
            drawtable();

        //..12 - display message feed
        if(sfadefeed>0)
        {
            GUIStyle feedStyle = new GUIStyle(gstyle);
            feedStyle.fontSize = Mathf.RoundToInt(.035f * h);
            feedStyle.alignment = TextAnchor.UpperLeft;
            feedStyle.font = arial;
            if(sfeed == result+3)
            {
                feedStyle.alignment = TextAnchor.UpperCenter;
                feedStyle.fontSize = Mathf.RoundToInt(.05f * h);
            }
            feedStyle.wordWrap = true;
            GUI.color = new Color(feedColor.r, feedColor.g, feedColor.b, sfadefeed);
            Rect frect = new Rect(w*.025f,h*.05f,.75f*w,.6f*h);
            if (sfeed > -1)
                GUI.Label(frect, feedMessages[sfeed], feedStyle);
            else
            {
                feedStyle.richText = true;
                GUI.Label(frect, positiveFeedback[pfeed], feedStyle);
            }
            reset();
            if (slicetimer > 0)
            {
                if (slicetimer > 1f)
                {
                    GUI.color = new Color(.75f,1,1f,1);
                    frect.height = h * .06f;
                    frect.width -= h * .01f;
                    setAlpha((slicetimer - 1f) * .5f);
                    GUI.DrawTexture(frect, white);
                    reset();
                }
                else
                {
                    setAlpha(slicetimer*2);
                    frect.height = h * .05f;
                    frect.y += h * .025f;
                    frect.width = frect.height * 10;
                    frect.height = h * .0125f*slicetimer;
                    
                    frect.x += w * .5f - w * .5f * slicetimer;
                    GUI.DrawTexture(frect, sliceEffect);
                    reset();
                }
            }
        }

        //if (showTooltip || hoverOverCorner)
            //drawTip();

        //draw wrench or screwdriver cursor if in-use
        

        if (tickmarkFade > 0)
        {
            float subfade = tickmarkFade;
            if (subfade > 1)
                subfade = 1;
            Rect tmarkRect = new Rect(Mathf.LerpUnclamped(rnbutton.center.x,w * .4f,subfade), Mathf.LerpUnclamped(rnbutton.center.y, h * .5f, subfade), 0, 0);
            tmarkRect = shrinkRect(tmarkRect, Mathf.LerpUnclamped(0,-h * .05f, subfade));
            
            GUI.DrawTexture(tmarkRect, t_marks[markType]);
        }

        //draw description book content if enabled
        if (bookEnabled)
        {
            drawBookButton();
            drawBook();
        }

        //draw results update button;
        if (logResults && fade==1)
        {
            Rect upRect = new Rect(w * .3f, h * .55f, w * .18f, h * .07f);
            GUI.DrawTexture(upRect, white);
            GUI.color = buttonColor;
            Rect supRect = shrinkRect(upRect, h * .005f);
            GUI.DrawTexture(supRect, white);
            reset();
            if (upRect.Contains(mp))
            {
                GUI.color = Color.yellow;
                if(Input.GetMouseButtonDown(0))
                {
                    logResults = false;
                    bool checkResult = false;
                    if(px>=0)
                    {
                        if (rtable[py, px] != animations[result])
                            checkResult = true;
                    }

                    if ((py != thickness + 1 || px != chosenMat + 1) && px >= 0)
                    {
                        sfeed = 7;
                        sfadefeed = 4;
                        copyStringTable(ptable, rtable, 4, 7);
                        previousTestThickness = thickness;
                        previousTestchosenMat = chosenMat;
                        logResults = true;
                    }
                    else if (checkResult)
                    {
                        sfeed = 8;
                        sfadefeed = 4;
                        copyStringTable(ptable, rtable, 4, 7);
                        previousTestThickness = thickness;
                        previousTestchosenMat = chosenMat;
                        logResults = true;
                    }
                    else
                    {
                        previousTestThickness = thickness;
                        previousTestchosenMat = chosenMat;
                        if (px > 0)
                        {
                            if (rtable[py, px] != ptable[py, px])
                            {
                                sfeed = -1;
                                pfeed = Random.Range(0, 3);
                                sfadefeed = 4;
                            }
                        }
                        else
                            logResults = true;
                        copyStringTable(rtable, ptable, 4, 7);
                        if (!logResults)
                        {
                            py = -1;
                            px = -1;
                            previousTestThickness = -1;
                            previousTestchosenMat = -1;
                        }
                    }
                    
                }
            }
            GUI.Label(upRect, "Update Results", gstyle);
            reset();
        }

        //draw special cursors, draw all other content above this line
        if (cursorType != 0)
        {
            Rect crect = new Rect(mp.x, mp.y, h * .08f, h * .08f);
            if (cursorType == 1)
            {
                if (hoveringMat)
                    GUI.DrawTexture(crect, highlightWrench);
                else
                    GUI.DrawTexture(crect, wrench);
            }
            else if (cursorType == 2)
            {
                Rect modelwing = new Rect(w * .15f, h * .3f, w * .475f, h * .25f);
                if (modelwing.Contains(mp))
                {
                    GUI.DrawTexture(crect, highlightScrewdriver);
                }
                else
                    GUI.DrawTexture(crect, screwdriver);

            }
            else if (cursorType == 3)
                GUI.DrawTexture(crect, t_questionCursor);
        }
    } //end OnGUI()

    void reset() //reset gui colors
    {
        GUI.color = Color.white;
        GUI.backgroundColor = Color.white;
    }

    void changeBones() //updates bone material color
    {
        for (int i = 28; i < wbones.Length; i++)
        {
            wbones[i].material.color = bcolor[chosenMat];
            wbones[i].material.SetColor("_EmissionColor", bcolor[chosenMat]);
            if(chosenMat==2)
            {
                if(!wbones[i].material.IsKeywordEnabled("_NORMALMAP"))
                    wbones[i].material.EnableKeyword("_NORMALMAP");
                wbones[i].material.SetTexture("_BumpMap",woodNormal);
            }
            else
            {
                wbones[i].material.SetTexture("_BumpMap", null);
                
            }
            if (chosenMat == 6)
            {
                wbones[i].enabled = false;
            }
            else
                wbones[i].enabled = true;
        }
    }
    void changeThickness() //updates bone thickness
    {
        //for(int i = 0; i<btCylinder.Length; i++)
        //{
         //   btCylinder[i].localScale = new Vector3(bscales[thickness], btCylinder[i].localScale.y, bscales[thickness]);
        //}
        for(int i = 0; i<10; i++)
        {
            modelstruts[i].localScale = new Vector3(modelstruts[i].localScale.x, bscales[thickness], bscales[thickness]);
        }
    }

    void drawtable() //draw results table
    {
        Rect trect = new Rect(w*.035f,h*.2f,w*.1f,h*.05f);
        float y = trect.y;
        gstyle.fontSize = Mathf.RoundToInt(h * .03f);
        Rect titlerect = new Rect(trect);
        titlerect.y -= titlerect.height;
        titlerect.width *= 7;

        Rect xrect = new Rect(titlerect);

        if (tableFade > 0)
            setAlpha(tableFade * 2);
        fullTableRect.x = titlerect.x;
        fullTableRect.y = titlerect.y;
        fullTableRect.width = titlerect.width;
        GUI.DrawTexture(titlerect, white);
        titlerect = shrinkRect(titlerect, trect.height * .05f);
        GUI.color = new Color(90f/255f,112f/255f,161f/255f);
        GUI.color = buttonColor;
        if (tableFade > 0)
            setAlpha(tableFade * 2);
        GUI.DrawTexture(titlerect, white);
        reset();
        GUI.Label(titlerect, "Results Table", gstyle);
        xrect.xMin = xrect.xMax - xrect.height;
        if (tableFade > 0)
            setAlpha(tableFade * 2);
        GUI.DrawTexture(xrect, white);
        xrect = shrinkRect(xrect, trect.height * .05f);
        GUI.color = new Color(.05f,.14f,.34f);
        GUI.color = buttonColor;
        if (xrect.Contains(mp) && showtable)
        {
            GUI.color = Color.red;
            if(Input.GetMouseButtonDown(0))
            {
                showtable = false;
            }
        }
        if (tableFade > 0)
            setAlpha(tableFade * 2);
        GUI.DrawTexture(xrect, white);
        reset();
        
        GUI.Label(xrect, "X", gstyle);

        //loop that draws the grid boxes
        for (int i = 0; i<7; i++)
        {
            //label
            trect.y = y;
            if (tableFade > 0)
                setAlpha(tableFade * 2);
            GUI.DrawTexture(trect, white);
            Rect boxRect = shrinkRect(trect, trect.height * .05f);
            GUI.color = buttonColor;
            if(i == 1 || i == 3 || i == 5)
            {
                GUI.color = new Color(23f/255f,128f/255f,153f/255f);
            }
            GUI.color = buttonColor;
            GUI.color = new Color(90f / 255f, 112f / 255f, 161f / 255f);
            if (tableFade > 0)
                setAlpha(tableFade * 2);
            GUI.DrawTexture(boxRect, white);
            reset();
            GUI.Label(boxRect, rtable[0, i], gstyle);

            //row 1
            trect.y += trect.height;
            if (tableFade > 0)
                setAlpha(tableFade * 2);
            GUI.DrawTexture(trect, white);
            boxRect = shrinkRect(trect, trect.height * .05f);
            GUI.color = buttonColor;
            if (i == 1 || i == 3 || i == 5)
            {
                GUI.color = new Color(23f / 255f, 128f / 255f, 153f / 255f);
            }
            if(i==0)
                GUI.color = new Color(90f / 255f, 112f / 255f, 161f / 255f);
            if (rtable[1, i] == "Fly")
                GUI.color = new Color(0f, .5f, 1f, 1);
            else if (rtable[1, i] == "Break")
                GUI.color = new Color(1f, .25f, 0f, 1);
            else if (rtable[1, i] == "Heavy")
                GUI.color = new Color(.85f, .7f, 0f, 1);

            if (recordFade > 0 && i-1 == chosenMat && thickness == 0)
                GUI.color = Color.Lerp(buttonColor, Color.white, recordFade * 1);
            if (tableFade > 0)
                setAlpha(tableFade * 2);
            GUI.DrawTexture(boxRect, white);
            if(previousTestThickness+1 == 1 && previousTestchosenMat+1 == i && logResults && thickness == previousTestThickness && chosenMat == previousTestchosenMat)
            {
                reset();
                setAlpha(.5f);
                GUI.DrawTexture(boxRect, white);
            }
            reset();
            GUI.Label(boxRect, rtable[1, i], gstyle);
            if(boxRect.Contains(mp))
            {
                hoverTable = true;
                gy = 1;
                gx = i;
            }

            //row 2
            trect.y += trect.height;
            if (tableFade > 0)
                setAlpha(tableFade * 2);
            GUI.DrawTexture(trect, white);
            boxRect = shrinkRect(trect, trect.height * .05f);
            GUI.color = buttonColor;
            if (i == 1 || i == 3 || i == 5)
            {
                GUI.color = new Color(23f / 255f, 128f / 255f, 153f / 255f);
            }
            if (i == 0)
                GUI.color = new Color(90f / 255f, 112f / 255f, 161f / 255f);
            if (rtable[2, i] == "Fly")
                GUI.color = new Color(0f, .5f, 1f, 1);
            else if (rtable[2, i] == "Break")
                GUI.color = new Color(1f, .25f, 0f, 1);
            else if (rtable[2, i] == "Heavy")
                GUI.color = new Color(.85f, .7f, 0f, 1);

            if (recordFade > 0 && i-1 == chosenMat && thickness == 1)
                GUI.color = Color.Lerp(buttonColor, Color.white, recordFade * 1);
            if (tableFade > 0)
                setAlpha(tableFade * 2);
            GUI.DrawTexture(boxRect, white);
            if (previousTestThickness + 1 == 2 && previousTestchosenMat+1 == i && logResults && thickness == previousTestThickness && chosenMat == previousTestchosenMat)
            {
                reset();
                setAlpha(.5f);
                GUI.DrawTexture(boxRect, white);
            }
            reset();
            GUI.Label(boxRect, rtable[2, i], gstyle);
            if (boxRect.Contains(mp))
            {
                hoverTable = true;
                gy = 2;
                gx = i;
            }

            //row 3
            trect.y += trect.height;
            if (tableFade > 0)
                setAlpha(tableFade * 2);
            GUI.DrawTexture(trect, white);
            boxRect = shrinkRect(trect, trect.height * .05f);
            GUI.color = buttonColor;
            if (i == 1 || i == 3 || i == 5)
            {
                GUI.color = new Color(23f / 255f, 128f / 255f, 153f / 255f);
            }
            if (i == 0)
                GUI.color = new Color(90f / 255f, 112f / 255f, 161f / 255f);
            if (rtable[3, i] == "Fly")
                GUI.color = new Color(0f, .5f, 1f, 1);
            else if(rtable[3,i] == "Break")
                GUI.color = new Color(1f, .25f, 0f, 1);
            else if (rtable[3, i] == "Heavy")
                GUI.color = new Color(.85f, .7f, 0f, 1);

            if (recordFade > 0 && i-1 == chosenMat && thickness == 2)
                GUI.color = Color.Lerp(buttonColor, Color.white, recordFade * 1);
            if (tableFade > 0)
                setAlpha(tableFade * 2);
            GUI.DrawTexture(boxRect, white);
            if (previousTestThickness + 1 == 3 && previousTestchosenMat+1 == i && logResults && thickness==previousTestThickness && chosenMat==previousTestchosenMat)
            {
                reset();
                setAlpha(.5f);
                GUI.DrawTexture(boxRect, white);
            }
            reset();
            GUI.Label(boxRect, rtable[3, i], gstyle);
            if (boxRect.Contains(mp))
            {
                hoverTable = true;
                gy = 3;
                gx= i;
            }

            trect.x += trect.width;
        } //end for loop
        fullTableRect.yMax = trect.yMax;
        //..23 draw print results button 
        if(!logResults)
        {
            prntRect = new Rect(w * .3f, h * .55f, w * .18f, h * .07f);
            GUI.DrawTexture(prntRect, white);
            GUI.color = buttonColor;
            Rect sprntRect = shrinkRect(prntRect, h * .005f);
            GUI.DrawTexture(sprntRect, white);
            reset();
            if(prntRect.Contains(mp))
            {
                GUI.color = Color.yellow;
            }
            GUI.Label(prntRect, "Save Chart", gstyle);
            reset();
        }
    } //end draw results table

    void drawTooltipButton() //draw ? button
    {
        if (fade != 0)
            return;
        if (showTooltip)
            GUI.color = new Color(1, 1, 1, .4f);

        tooltipRect = new Rect(w * .005f, h * .9f, .07f * h, .07f * h);
        GUI.DrawTexture(tooltipRect, t_questionButton);
        reset();
    }
    void drawTip() //draws tip when hovered over with ? cursor
    {
        if (tiptoshow > 0)
        {
            float tipy = descriptionHeights[tiptoshow-1];
            Rect tipRect = new Rect(w * .05f, h * tipy, w * .556f, h *(.8f-tipy));
            GUI.DrawTexture(tipRect, white);
            GUI.color = buttonColor;
            tipRect = shrinkRect(tipRect, .005f * h);
            GUI.DrawTexture(tipRect, white);
            reset();
            tipRect = shrinkRect(tipRect, .01f * h);
            GUIStyle tipstyle = new GUIStyle();
            tipstyle.alignment = TextAnchor.UpperLeft;
            tipstyle.wordWrap = true;
            tipstyle.font = consolas;
            tipstyle.fontSize = Mathf.RoundToInt(Mathf.Min(h,w)/60);
            tipstyle.normal.textColor = Color.white;
            GUI.Label(tipRect, s_tooltips[tiptoshow-1], tipstyle);
        }
    }

    void drawBookButton() //draw book icon
    {
        if (logResults || test ||fade>0)
            return;
        bookRect = new Rect(w * .005f, h * .875f, .07f * h, .07f * h);
        GUI.DrawTexture(bookRect, t_book);
    }
    
    void drawBook()
    {
        if (test)
            showBook = false;
        if(showBook)
        {
            showtable = false;
            //draw backdrop and inner blue portion
            Rect backdrop = new Rect(w * .5f-w*.725f*.658f, h * .5f - h*.65f*.5f, w * .725f, h * .65f);
            GUI.DrawTexture(backdrop, white);
            GUI.color = buttonColor;
            Rect innerDrop = shrinkRect(backdrop, backdrop.height * .0125f);
            GUI.DrawTexture(innerDrop, white);
            reset();

            //draw book exit button
            Rect xrect = new Rect(backdrop);
            xrect.yMax -= xrect.height * .9f;
            xrect.xMin = xrect.xMax - xrect.height;
            GUI.DrawTexture(xrect, white);
            xrect = shrinkRect(xrect, backdrop.height * .0125f);
            GUI.color = buttonColor;
            if(xrect.Contains(mp)) //exit book when clicked
            {
                GUI.color = Color.red;
                if (Input.GetMouseButtonDown(0))
                {
                    showBook = false;
                    bookClosed = .1f;
                }
            }
            GUI.DrawTexture(xrect, white);
            reset();
            GUI.Label(xrect, "X", gstyle);

            GUIStyle centeredStyle = new GUIStyle(gstyle);
            centeredStyle.alignment = TextAnchor.MiddleCenter;
            Rect dataRect = new Rect(w * .375f, h * .22f, 0, 0);
            GUI.Label(dataRect, "Material Data", centeredStyle);

            Rect bchart = new Rect(w*.04f, h*.27f, w*.69f, h*.53f);
            

            GUI.DrawTexture(bchart, t_chart);

            Rect[] gridR = new Rect[28];
            gridR[0] = new Rect(bchart.x, bchart.y, w * .092f, h * .027f);
            for (int i = 1; i < 7 ; i++)
            {
                if(i==1)
                    gridR[i] = new Rect(bchart.x+w*.092f*i, bchart.y, w * .1f, h * .027f);
                else
                    gridR[i] = new Rect(bchart.x - w*.01f + w * .1f * i, bchart.y, w * .1f, h * .027f);
            }
            gridR[7] = new Rect(bchart.x, bchart.y+gridR[0].height, w * .092f, h * .203f);
            for (int i = 8; i < 14; i++)
            {
                if (i == 8)
                    gridR[i] = new Rect(bchart.x + w * .092f * (i-7), gridR[7].y, w * .1f, gridR[7].height);
                else
                    gridR[i] = new Rect(bchart.x - w * .01f + w * .0999f * (i-7), gridR[7].y, w * .1f, gridR[7].height);
                if (i == 11)
                    gridR[i].x -= h * .0025f;
                if(i == 12)
                    gridR[i].x -= h * .0015f;
            }
            gridR[14] = new Rect(bchart.x, bchart.y + gridR[0].height + gridR[7].height, w * .092f, h * .182f);
            for(int i = 15; i < 21; i++)
            {
                if (i == 15)
                    gridR[i] = new Rect(bchart.x + w * .092f * (i - 14), gridR[14].y, w * .1f, gridR[14].height);
                else
                    gridR[i] = new Rect(bchart.x - w * .01f + w * .0997f * (i -14), gridR[14].y, w * .1f, gridR[14].height);

                if(i==16)
                    gridR[i].x += h * .0025f;
                if (i == 19)
                    gridR[i].x -= h * .0025f;
            }
            gridR[21] = new Rect(bchart.x, bchart.y + gridR[0].height + gridR[7].height + gridR[14].height, w * .092f, h * .114f);
            for(int i = 22; i < 28; i++)
            {
                if (i == 22)
                    gridR[i] = new Rect(bchart.x + w * .092f * (i - 21), gridR[21].y, w * .1f, gridR[21].height);
                else
                    gridR[i] = new Rect(bchart.x - w * .01f + w * .1f * (i - 21), gridR[21].y, w * .1f, gridR[21].height);
            }
            GUIStyle bookStyle = new GUIStyle(gstyle);
            bookStyle.wordWrap = true;
            bookStyle.fontSize = Mathf.RoundToInt(h * .019f);
            bookStyle.alignment = TextAnchor.MiddleCenter;
            for (int i = 0; i<gridR.Length;i++)
            {
                //GUI.DrawTexture(gridR[i], white);
                Rect btr = new Rect(gridR[i]);
                btr.width -= w * .005f;
                btr.y += h * .0025f;
                btr.x += h * .003f;
                GUI.Label(btr, bookText[i], bookStyle);
            }


            /*
            //draw current material 
            Rect matR = new Rect(innerDrop);
            matR.xMax -= innerDrop.width * .9f;
            matR.height = matR.width *.6f;
            matR.x += w * .2f;
            matR.y += h * .03f;
            GUI.DrawTexture(matR,tmat[page]);

            //draw mat name
            Rect nameR = new Rect(matR);
            nameR.y = nameR.center.y;
            nameR.x += nameR.width * 2f;
            nameR.width = 0;
            nameR.height = 0;
            GUI.Label(nameR, mstring[page], gstyle);

            //draw description
            Rect descR = new Rect(innerDrop);
            descR.xMin += innerDrop.width * .1f;
            descR.xMax -= innerDrop.width * .1f;
            descR.y += h * .13f;
            descR.height -= h * .15f;
            GUIStyle descStyle = new GUIStyle(gstyle);
            descStyle.wordWrap = true;
            descStyle.alignment = TextAnchor.UpperLeft;
            descStyle.font = consolas;
            descStyle.fontSize = Mathf.RoundToInt(h *.025f);
            GUI.Label(descR, this.s_tooltips[page], descStyle);
            */
            /*
            //draw arrows
            leftBookR = new Rect(innerDrop);
            leftBookR.y += innerDrop.height * .45f;
            leftBookR.x += innerDrop.width * .025f;
            leftBookR.width = innerDrop.height * .05f;
            leftBookR.height = leftBookR.width * 2;
            if(leftBookR.Contains(mp))
            {
                GUI.color = Color.red;
            }
            GUI.DrawTexture(leftBookR, t_left);
            reset();

            rightBookR = new Rect(innerDrop);
            rightBookR.y += innerDrop.height * .45f;
            rightBookR.x = innerDrop.xMax - innerDrop.width * .025f;
            rightBookR.width = innerDrop.height * .05f;
            rightBookR.x -= rightBookR.width;
            rightBookR.height = rightBookR.width * 2;
            if (rightBookR.Contains(mp))
            {
                GUI.color = Color.red;
            }
            GUI.DrawTexture(rightBookR, t_right);
            */
            reset();

        }
    }

    Rect shrinkRect(Rect r, float s)
    {
        r.xMin += s;
        r.xMax -= s;
        r.yMin += s;
        r.yMax -= s;
        return r;
    }

    void setAlpha(float f)
    {
        Color c = GUI.color;
        c.a = f;
        GUI.color = c;
    }
    void copyStringTable(string[,] from,string[,] to, int x, int y)
    {
        for(int i = 0; i<x; i++)
        {
            for(int j = 0; j<y; j++)
            {
                //nested loop for copying grid
                to[i, j] = from[i, j];
            }
        }
    }
}
