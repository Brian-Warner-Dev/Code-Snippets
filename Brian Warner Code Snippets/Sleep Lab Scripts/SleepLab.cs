using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//main lab script
public class SleepLab : MonoBehaviour {
    public _EventController ec;
    public TestClick[] clickables; //includes ALL interactables. Used in touch/click raycast code, placed in this script for performance
    float pfade = 0; //print successful message fade
    public Vector3 treset = new Vector3(-1000,-1000,-1000); //reset value for touch/tap input when not in use
    public Vector3 tappos; //tap position, active the first frame a touch is detected (when touchpos was previously set to treset)
    public Vector3 touchpos; //touch position, always active when a touch is detected
    float jumptograph = 0; //delay before jumping into the graph for the current trial, used immediately after finishing a sleep cycle
    string[] hmessages; //help messages
    int hcounter = 0; //help page
    public bool showHelp = true; //whether or not to show help
    public string etext = "Dogs bark outside"; //event bubble text
    public float mfade = 1; //event message fade
    Vector3 mp; //mouse position
    public GridSprite gsprite; //reference to uninstantiated event icon sprite
    public Light nitelight; // light to show during the sleep cycle
    public Light daylight; // light to show during the day
    public float fade = 0; //variable used for flashing text, updated every frame
    int cvac = -1; //iterate through arrays of actions (used in chart screen), each array is a single point on the timeline where multiple actions in one point may occur if toggled quickly
    //the first array is always all the items that were turned on on sleep cycle start, plus the colors and fan speed
    int caction = 0; //iterate through the current array of actions
    public Action[] alist; //current array of actions
    public Color _gcolor; //background gradient color of chart screen
    public PostRender pr; //script used for displaying chart
    public int curchart; //current trial to display chart data for
    public bool showCharts; //whether or not we're on the chart screen
    public string[] scolor; //string names of all the wall/bed colors.
    public window _win; //window script that fades the window on the wall when entering sleep
    public Clock _clock; //bottom left clock script
    public bool sleeping; //on when in sleep cycle
    public Camera _chartcam; //camera for viewing the lines/particles drawn on the chart screen
    public Camera gcam; //the rotating graph camera, since the graph is actually a particle effect with trail render
    public Camera ccam; //clock camera
    public CRotate crot; //rotating graph camera's script
    public Texture white; //blank white texture for drawing
    AudioSource _as; //audio source on this object, which is the crickets sound during sleep
    public Wallcontroller bed; //bed color control
    public Wallcontroller wall; //wall color control
    public Color[] bcolors; //a list of all selectable colors for the bed/wall
    public Fan fan; //fan script
    public Lamp[] _lamps; //lamp scripts
    public Laptop lap; //laptop script
    public GameObject[] obj; //list of all toggle objects that don't have any special scripts
    public Stereo1 ster1; //stereo script
    public Stereo1 ster2; // the second stereo
    public TV _tv; //tv script
    public string[] inames; //names of all the icons
    public Texture[] tui; //quick and dirty array for storing any additional gui textures I need
    public Texture tpop; //popup texture for event messages
    public Texture[] arrows; //arrow textures
    public Camera[] cams; //the two main cameras
    int curCam = 1; //the current active camera
    public Texture[] ticons; //textures for all the icon buttons
    public Texture tsidebar; //sidebar texture
    float w; //screen width
    float h; //screen height
    public int showProperties = -1; //which object type is selected, -1 if nothing is selected
    public Trial[] trials = new Trial[0]; //trial data

    //script for saving the data for all trials to the copy to clipboard text box
    void SaveCharts()
    {
        //todo:save chart function
        string cstring = ""; //this is the main string
        for (int i = 0; i < 3; i++)
        {
            string endl = System.Environment.NewLine;
            if (i != 0)
                cstring += endl;
            if (i >= trials.Length)
                cstring += "Trial " + (i + 1).ToString() + " - No Trial Data";
            else
            {
                int electronics = 0;
                int clutter = 0;
                string estring = "Electronics - ";
                string clutterstring = "Clutter - ";
                string qstring = "Quality - ";
                string vent = "Events - ";
                string tos_string = "Never Slept";
                if (trials[i].timeofsleep.Length > 0)
                {
                    tos_string = GetTime(trials[i].timeofsleep[0]);
                }
                cstring += "Trial " + (i + 1).ToString() + " - Effective Sleep Hours: " + GetHours(trials[i].ehours) + " - Overall Quality: " + trials[i].opercent.ToString("F0") + "% - Time of Sleep: " + tos_string + endl;
                for (int h = 0; h < 12; h++)
                {
                    if (h < 4)
                    {
                        for (int a = 0; a < trials[i].actions.Length; a++)
                        {
                            if (trials[i].actions[a].timestamp < 72000 + (h + 1) * 3600 && trials[i].actions[a].timestamp >= 72000 + h * 3600)
                            {
                                int index = trials[i].actions[a].icon;

                                if (index == 21 || index == 20 || index == 19 || index == 15 || index == 14 || index == 13 || index == 12 || index == 9)
                                {

                                    if (trials[i].actions[a].istate.Contains("Toggled on"))
                                    {
                                        electronics++;
                                    }
                                    if (trials[i].actions[a].istate == "Toggled off")
                                    {
                                        if (electronics > 0)
                                            electronics--;
                                    }
                                    if (trials[i].actions[a].istate.Contains("Speed"))
                                    {
                                        if (trials[i].actions[a].istate.Contains("0"))
                                        {
                                            electronics--;
                                        }
                                        else
                                        {
                                            electronics++;
                                        }
                                    }
                                }
                                else if (index == 0 || (index == 3 && trials[i].actions[a].iname == "Boat 1") || (index == 5 && trials[i].actions[a].iname == "Car 1") || index == 7 || (index == 8 && trials[i].actions[a].iname == "Elephant 2") || index == 10 || index == 11 || index == 17 || (index == 16 && trials[i].actions[a].iname == "Makeup 2"))
                                {
                                    if (trials[i].actions[a].istate == "Toggled on")
                                    {
                                        clutter++;
                                    }
                                    else
                                    {
                                        clutter--;
                                    }
                                }
                            }
                            else if (trials[i].actions[a].timestamp < 72000)
                                break;
                        }
                        estring += (8 + h).ToString() + " PM: " + electronics.ToString() + " | ";
                        clutterstring += (8 + h).ToString() + " PM: " + clutter.ToString() + " | ";
                    }
                    else
                    {
                        for (int a = 0; a < trials[i].actions.Length; a++)
                        {
                            if (trials[i].actions[a].timestamp < 72000)
                            {
                                
                                if (trials[i].actions[a].timestamp < (h + 1 - 4) * 3600 && trials[i].actions[a].timestamp >= (h - 4) * 3600)
                                {
                                    int index = trials[i].actions[a].icon;
                                    
                                    if (index == 21 || index == 20 || index == 19 || index == 15 || index == 14 || index == 13 || index == 12 || index == 9)
                                    {
                                        
                                        if (trials[i].actions[a].istate.Contains("Toggled on"))
                                        {
                                            electronics++;
                                        }
                                        if (trials[i].actions[a].istate == "Toggled off")
                                        {
                                            if (electronics > 0)
                                                electronics--;
                                        }
                                        if (trials[i].actions[a].istate.Contains("Speed"))
                                        {
                                            if (trials[i].actions[a].istate.Contains("0"))
                                            {
                                                electronics--;
                                            }
                                            else
                                            {
                                                electronics++;
                                            }
                                        }
                                    }
                                    else if (index == 0 || (index == 3 && trials[i].actions[a].iname == "Boat 1") || (index == 5 && trials[i].actions[a].iname == "Car 1") || index == 7 || (index == 8 && trials[i].actions[a].iname == "Elephant 2") || index == 10 || index == 11 || index == 17 || (index == 16 && trials[i].actions[a].iname == "Makeup 2"))
                                    {
                                        if (trials[i].actions[a].istate == "Toggled on")
                                        {
                                            clutter++;
                                        }
                                        else
                                        {
                                            clutter--;
                                        }
                                    }
                                }
                            }
                        }
                        if (h == 4)
                        {
                            estring += "12 AM: " + electronics.ToString() + " | ";
                            clutterstring += "12 AM: " + clutter.ToString() + " | ";
                        }
                        else
                        {
                            estring += (h - 4).ToString() + " AM: " + electronics.ToString();
                            clutterstring += (h - 4).ToString() + " AM: " + clutter.ToString();
                            if (h < 11)
                            {
                                estring += " | ";
                                clutterstring += " | ";
                            }

                        }
                    }
                }
                cstring += estring + endl;
                cstring += clutterstring + endl;
                for (int q = 0; q < 12; q++)
                {
                    if (q < 4)
                    {
                        qstring += (8 + q).ToString() + " PM: " + trials[i].cquality[q].ToString("F0") + "% | ";
                    }
                    else if (q == 4)
                    {
                        qstring += "12 AM: " + trials[i].cquality[q].ToString("F0") + "% | ";
                    }
                    else
                    {
                        qstring += (q - 4).ToString() + " AM: " + trials[i].cquality[q].ToString("F0") + "%";
                        if (q < 11)
                        {
                            qstring += " | ";
                        }
                    }
                }
                
                cstring += qstring + endl;

                for (int a = 0; a < trials[i].actions.Length; a++)
                {
                    if (trials[i].actions[a].iname.Contains("Event"))
                    {
                        vent += GetTime(trials[i].actions[a].timestamp) + ": " + trials[i].actions[a].istate + " | ";
                    }
                }
                cstring += vent;
            }
        }
        Application.ExternalCall("SaveChart", cstring);
        //print(cstring); //print out a version of the string to console that respects \r \n, for testing
    }

	// todo:Start
	void Start () {
        tappos = treset;
        touchpos = treset;
        mp = Input.mousePosition;
        _as = GetComponent<AudioSource>();
        
        //randomize room state
        for (int i = 0; i < obj.Length; i++) 
        {
            obj[i].SetActive(Random.Range(0,2) == 0);
        }
        _tv.on = Random.Range(0, 2) == 0;
        ster1.on = Random.Range(0, 2) == 0;
        ster2.on = Random.Range(0, 2) == 0;
        lap.on = Random.Range(0, 2) == 0;
        fan.speed = Random.Range(0, 4);
        _lamps[0].on = Random.Range(0, 2) == 0;
        _lamps[1].on = Random.Range(0, 2) == 0;
        _lamps[2].on = Random.Range(0, 2) == 0;
        wall.c = bcolors[Random.Range(0, bcolors.Length)];
        bed.c = bcolors[Random.Range(0, bcolors.Length)];

        SetQuality(); //give the graph script some initial values, lab might still work without this line

        //todo:help messages
        hmessages = new string[7];
        hmessages[0] = "Welcome to the Sleep Lab! The goal of this lab is to provide you with an easy way to change and test how various factors affect how well you sleep at night.";
        hmessages[1] = "Use the arrows at the bottom to switch between camera views of the room. The list of icons on the right sidebar can be used to choose an object you would like to modify. After activating an icon, use the properties window on the upper left to toggle it on and off, or change other properties like color or speed depending on the object.";
        hmessages[2] = "Set up the room however you wish, and press the sleep button to start simulating the sleep cycle. A graph will display at the top of how well your sleep quality is. You can continue to change object properties during sleep.";
        hmessages[3] = "Your sleep avatar will first be in the state of falling asleep before entering rest. Clutter within the room will affect how long it takes for you to fall asleep. Once you enter sleep, factors such as light, noise, or even colors will affect your sleep.";
        hmessages[4] = "Experiment to see how each object affects you while asleep or falling asleep. You can use the + or - speed settings to slow or speed up the sleep cycle.";
        hmessages[5] = "Once the night is finished, use the Charts button to see the final graph of your sleep cycle. You can mouse over or touch the graph to see the quality percentage at any point in the graph. Tap the circles or use the arrows to see what actions you made over night.";
        hmessages[6] = "Your last 3 trials will be recorded. Try to find the best and worst combinations for sleep quality. Once you are finished, use the Print button on the charts screen to save your data to the copy to clipboard box below.";
    }

    //todo:get touch input
    public void UpTouch()
    {
        tappos = treset;
        bool wastouching = false; //false when previous frame touchpos was set to treset. When true, don't set tap position, when false set tap position if there is new touch detected
        if (touchpos != treset)
            wastouching = true;
        touchpos = treset;
        Vector3 opos = treset; //non y inverted version of touch
        for (int i = 0; i < Input.touchCount; i++)
        {
            touchpos = Input.GetTouch(i).position;
            opos = touchpos;
            touchpos.y = h - touchpos.y;
            if (!wastouching)
            {
                tappos = touchpos;
            }
        }

        if ((Input.GetMouseButtonDown(0) || tappos != treset) && !showHelp && !showCharts) //on click or tap, when not in help or chart menu
        {
            //todo:mouse/touch raycast to select objects
            float y = h * .73f;
            //the ui areas of the screen not to check for object selection
            Rect[] t = new Rect[9];
            t[0] = new Rect(0, y, w * .14f, h - y);
            t[1] = new Rect(w * .86f, 0, w * .14f, h);
            t[2] = new Rect(w * .14f, h * .85f, w * .049f, h * .15f);
            if (showProperties >= 0)
            {
                t[3] = new Rect(0, 0, w * .24f, h * .33f);
                t[4] = new Rect(w * .05f, h * .33f, w * .11f, h * .1f);
            }
            else
            {
                t[3] = new Rect(-1, -1, 0, 0);
                t[4] = new Rect(-1, -1, 0, 0);
            }
            t[5] = new Rect(w * .45f, h * .9f, w * .14f, h * .1f);
            t[6] = new Rect(w * .75f, h * .02f, w * .1f, h * .06f);
            t[7] = new Rect(w * .8f, h * .9f, h * .08f, h * .08f);
            t[8] = new Rect(w * .02f, h * .65f, w * .112f, h * .07f);

            Vector3 ori_cp = Input.mousePosition;
            if (tappos != treset)
            {
                ori_cp = opos;
            }
            Vector3 cp = ori_cp;
            cp.y = h - cp.y;

            bool clear = true;
            for (int i = 0; i < t.Length; i++)
            {
                if (t[i].Contains(cp))
                {
                    clear = false;
                    break;
                }
            }

            if (clear)
            {

                Ray ray = Camera.main.ScreenPointToRay(ori_cp);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100f))
                {
                    for (int i = 0; i < clickables.Length; i++)
                    {
                        
                        if (hit.transform == clickables[i].transform)
                        {
                            
                            showProperties = clickables[i].property;
                            break;
                        }
                    }
                }
                
            }
        }
    }

    //todo:create icon on the sleep graph of an action that was made during sleep
    public void MakeSprite(Action a)
    {
        bool brighten = !(a.istate == "Toggled off");
        
        GameObject go = (GameObject)Instantiate(gsprite.gameObject, gsprite.transform.position, gsprite.transform.rotation, gsprite.transform.parent);
        Rect srect = new Rect(0, 0, ticons[a.icon].width, ticons[a.icon].height);
        float ppu = 100f * ticons[a.icon].width / ticons[0].width;
        if (100f * ticons[a.icon].height / ticons[0].height > ppu && ppu < 70)
        {
            ppu = 100f * ticons[a.icon].height / ticons[0].height;
        }

        go.GetComponent<GridSprite>().sr.sprite = Sprite.Create(ticons[a.icon] as Texture2D, srect, new Vector2(.5f, .5f),ppu);
        Vector3 tm = go.transform.position;
        tm.y = crot.ps.transform.position.y;
        go.transform.position = tm;
        if (!brighten)
        {
            go.GetComponent<GridSprite>().sr.color = Color.gray;
        }
        if (a.istate.Contains("Color"))
        {
            go.GetComponent<GridSprite>().sr.color = bcolors[a.cnumber];
        }
        go.SetActive(true);
    }

    //return time string hh:mm AM/PM from a given value in seconds.
    string GetTime(float s)
    {
        //todo:GetTime()
        string result = "";
        int scopy = Mathf.FloorToInt(s);
        if (scopy >= 43200)
            scopy -= 43200;
        int _hour = Mathf.FloorToInt(scopy / 3600);
        scopy -= _hour * 3600;
        if (_hour == 0)
            _hour = 12;

        int _minute = Mathf.FloorToInt(scopy / 60);
        result = _hour.ToString() + ":";
        if (_minute < 10)
            result += "0";
        result += _minute.ToString();
        if (s >= 43200)
            result += " PM";
        else
            result += " AM";
        return result;
    }

    string GetHours(float s)
    {
        //todo:GetHours()
        string result = "";
        result = (s / 3600f).ToString("F1");
        return result;
    }

    //todo:action class
    //class used for a single action's data
    [System.Serializable]
    public class Action
    {
        public float eq; //event quality
        public float eduration; //event duration
        public string iname = ""; //name of action
        public string istate = ""; //description of action
        public float timestamp = 72000; //when action happened
        public int cnumber = 0; //additional int data, such as fan speed or color index
        public int icon = 0; //which object is this
        public Action(string n, string s, float t) //initialize using the new keyword
        {
            iname = n;
            istate = s;
            timestamp = t;
        }
        public void init(string n, string s, float t) //reinitialize existing instance
        {
            iname = n;
            istate = s;
            timestamp = t;
        }
        public Action() //generic 
        {
        }
        public Action(Action a) //copy existing
        {
            iname = a.iname;
            istate = a.istate;
            timestamp = a.timestamp;
            cnumber = a.cnumber;
            icon = a.icon;
            eq = a.eq;
            eduration = a.eduration;
        }
    }

    //saves trial data
    //todo:class trial
    [System.Serializable]
    public class Trial
    {
        public bool active = false; //whether or not we can display chart data for this trial
        public Action[] gamestate = new Action[0]; //initial state of the trial
        public Action[] actions = new Action[0]; //all actions that happened during the trial
        public SleepLab sl; //give this subclass access to this script
        public float[] timeofsleep = new float[0]; //time the avatar fell asleep
        public float opercent = -1; //overall sleep quality percentage
        public float ehours = 0;
        public float[] cquality = new float[12]; //current quality for each hour

        public Trial() //initialization
        {
            cquality = new float[12];
            for (int i = 0; i < cquality.Length; i++)
            {
                if(cquality[i] == null)
                    cquality[i] = 0;
            }
        }

        public void Add(ref Action[] a, Action additive) //Add new action to existing list
        {
            Action[] temp = new Action[a.Length+1];
            for (int i = 0; i < a.Length; i++)
            {
                temp[i] = a[i];
            }

            temp[temp.Length - 1] = new Action(additive);
            a = temp;
            actions = temp;
        }

        public void Add(Action additive) //add new action to action list
        {
            Action[] temp = new Action[actions.Length + 1];
            for (int i = 0; i < actions.Length; i++)
            {
                temp[i] = actions[i];
            }

            temp[temp.Length - 1] = new Action(additive);
            actions = temp;
            
        }

        public void Initialize() //get current gamestate
        {
            Action wildcard = new Action();
            //beanbag
            if (sl.obj[16].activeSelf)
            {
                wildcard.init("Bean Bag 1", "Toggled on", 72000);
                wildcard.icon = 0;
                Add(ref gamestate, wildcard);
            }
            //Beanbag
            if (sl.obj[17].activeSelf)
            {
                wildcard.init("Bean Bag 2", "Toggled on", 72000);
                wildcard.icon = 0;
                this.Add(ref gamestate, wildcard);
            }
            //teddy
            if (sl.obj[18].activeSelf)
            {
                wildcard.init("Teddy Bear", "Toggled on", 72000);
                wildcard.icon = 1;
                this.Add(ref gamestate, wildcard);
            }
            //bed
            for (int i = 0; i < sl.bcolors.Length; i++)
            {
                if (sl.bed.c == sl.bcolors[i])
                {
                    wildcard.init("Bed", "Color set to " + sl.scolor[i], 72000);
                    wildcard.cnumber = i;
                    wildcard.icon = 2;
                    this.Add(ref gamestate, wildcard);
                    break;
                }
            }
            //boat
            if (sl.obj[14].activeSelf)
            {
                wildcard.init("Boat 1", "Toggled on", 72000);
                wildcard.icon = 3;
                this.Add(ref gamestate, wildcard);
            }
            //boat
            if (sl.obj[15].activeSelf)
            {
                wildcard.init("Boat 2", "Toggled on", 72000);
                wildcard.icon = 3;
                this.Add(ref gamestate, wildcard);
            }
            //book
            if (sl.obj[12].activeSelf)
            {
                wildcard.init("Books 1", "Toggled on", 72000);
                wildcard.icon = 4;
                this.Add(ref gamestate, wildcard);
            }
            //book
            if (sl.obj[13].activeSelf)
            {
                wildcard.init("Books 2", "Toggled on", 72000);
                wildcard.icon = 4;
                this.Add(ref gamestate, wildcard);
            }
            //car
            if (sl.obj[10].activeSelf)
            {
                wildcard.init("Car 1", "Toggled on", 72000);
                wildcard.icon = 5;
                this.Add(ref gamestate, wildcard);
            }
            //car
            if (sl.obj[11].activeSelf)
            {
                wildcard.init("Car 2", "Toggled on", 72000);
                wildcard.icon = 5;
                this.Add(ref gamestate, wildcard);
            }
            //chair
            if (sl.obj[9].activeSelf)
            {
                wildcard.init("Chair", "Toggled on", 72000);
                wildcard.icon = 6;
                this.Add(ref gamestate, wildcard);
            }
            //drum
            if (sl.obj[8].activeSelf)
            {
                wildcard.init("Drum", "Toggled on", 72000);
                wildcard.icon = 7;
                this.Add(ref gamestate, wildcard);
            }
            //elephant
            if (sl.obj[6].activeSelf)
            {
                wildcard.init("Elephant 1", "Toggled on", 72000);
                wildcard.icon = 8;
                this.Add(ref gamestate, wildcard);
            }
            //elephant
            if (sl.obj[7].activeSelf)
            {
                wildcard.init("Elephant 2", "Toggled on", 72000);
                wildcard.icon = 8;
                this.Add(ref gamestate, wildcard);
            }
            //fan
            if (sl.fan.speed > 0)
            {
                wildcard.init("Fan", "Speed set to " + sl.fan.speed.ToString(), 72000);
                wildcard.cnumber = sl.fan.speed;
                wildcard.icon = 9;
                this.Add(ref gamestate, wildcard);
            }
            //giraffe
            if (sl.obj[5].activeSelf)
            {
                wildcard.init("Giraffe", "Toggled on", 72000);
                wildcard.icon = 10;
                this.Add(ref gamestate, wildcard);
            }
            //Guitar
            if (sl.obj[4].activeSelf)
            {
                wildcard.init("Guitar", "Toggled on", 72000);
                wildcard.icon = 11;
                this.Add(ref gamestate, wildcard);
            }
            //light
            if (sl._lamps[0].on)
            {
                wildcard.init("Lamp 2", "Toggled on", 72000);
                wildcard.icon = 12;
                this.Add(ref gamestate, wildcard);
            }
            //light
            if (sl._lamps[1].on)
            {
                wildcard.init("Lamp 1", "Toggled on", 72000);
                wildcard.icon = 13;
                this.Add(ref gamestate, wildcard);
            }
            //light
            if (sl._lamps[2].on)
            {
                wildcard.init("Lamp 3", "Toggled on", 72000);
                wildcard.icon = 14;
                this.Add(ref gamestate, wildcard);
            }
            //lap
            if (sl.lap.on)
            {
                wildcard.init("Laptop", "Toggled on", 72000);
                wildcard.icon = 15;
                this.Add(ref gamestate, wildcard);
            }
            //makeup
            if (sl.obj[2].activeSelf)
            {
                wildcard.init("Makeup 1", "Toggled on", 72000);
                wildcard.icon = 16;
                this.Add(ref gamestate, wildcard);
            }
            //makeup
            if (sl.obj[3].activeSelf)
            {
                wildcard.init("Makeup 2", "Toggled on", 72000);
                wildcard.icon = 16;
                this.Add(ref gamestate, wildcard);
            }
            //airplane
            if (sl.obj[1].activeSelf)
            {
                wildcard.init("Airplane", "Toggled on", 72000);
                wildcard.icon = 17;
                this.Add(ref gamestate, wildcard);
            }
            //potplant
            if (sl.obj[0].activeSelf)
            {
                wildcard.init("Plant", "Toggled on", 72000);
                wildcard.icon = 18;
                this.Add(ref gamestate, wildcard);
            }
            //stereo
            if (sl.ster1.on)
            {
                wildcard.init("Stereo 1", "Toggled on", 72000);
                wildcard.icon = 19;
                this.Add(ref gamestate, wildcard);
            }
            //stereo
            if (sl.ster2.on)
            {
                wildcard.init("Stereo 2", "Toggled on", 72000);
                wildcard.icon = 20;
                this.Add(ref gamestate, wildcard);
            }
            //tv
            if (sl._tv.on)
            {
                wildcard.init("TV", "Toggled on", 72000);
                wildcard.icon = 21;
                this.Add(ref gamestate, wildcard);
            }
            //wall
            for (int i = 0; i < sl.bcolors.Length; i++)
            {
                if (sl.wall.c == sl.bcolors[i])
                {
                    wildcard.init("Wall", "Color set to " + sl.scolor[i], 72000);
                    wildcard.icon = 22;
                    wildcard.cnumber = i;
                    this.Add(ref gamestate, wildcard);
                    break;
                }
            }
            //poster
            if (sl.obj[19].activeSelf)
            {
                wildcard.init("Poster", "Toggled on", 72000);
                wildcard.icon = 23;
                this.Add(ref gamestate, wildcard);
            }
        }
    }

    //todo:SetQuality()
    //get quality or disruption % of current state of the room
    //disruption is relevant when not asleep, quality is relevant while asleep
    void SetQuality()
    {
        float disruption = 0;
        float quality = 100;

        //electronics
        if (_tv.on)
        {
            disruption += 5f;
            quality -= 10f;
        }
        if (ster1.on)
        {
            disruption += 5f;
            quality -= 10f;
        }
        if (ster2.on)
        {
            disruption += 5f;
            quality -= 10f;
        }
        for (int i = 0; i < _lamps.Length; i++)
        {
            if (_lamps[i].on)
            {
                disruption += 5f;
                quality -= 10f;
            }
        }
        if (lap.on)
        {
            disruption += 5f;
            quality -= 10f;
        }

        if (fan.speed == 0)
        {
            disruption += 1f;
            quality -= 2f;
        }
        if (fan.speed == 3)
        {
            disruption += 5f;
            quality -= 10f;
        }
        if (fan.speed == 2)
        {
            disruption += 2.5f;
            quality -= 6f;
        }
        if (fan.speed == 1)
        {

        }
        //colors
        if (bed.c == bcolors[5])
        {
            quality -= 8;
            disruption += 6;
        }
        if (wall.c == bcolors[5])
        {
            quality -= 12;
            disruption += 9;
        }
        if (bed.c == bcolors[1])
        {
            quality -= 4;
            disruption += 3;
        }
        if (wall.c == bcolors[1])
        {
            quality -= 6;
            disruption += 4.5f;
        }
        if (bed.c != bcolors[0] && bed.c != bcolors[2] && bed.c != bcolors[10] && bed.c != bcolors[5] && bed.c != bcolors[1])
        {
            quality -= 2;
            disruption += 1.5f;
        }
        if (wall.c != bcolors[0] && wall.c != bcolors[2] && wall.c != bcolors[10] && wall.c != bcolors[5] && wall.c != bcolors[1])
        {
            quality -= 3;
            disruption += 2.25f;
        }
        //other
        if (obj[16].activeSelf) //bean bag 1
        {
            disruption += 4.5f;
        }
        if (obj[17].activeSelf) //bean bag 2
        {
            disruption += 4.5f;
        }
        if (obj[14].activeSelf) //boat 1
        {
            disruption += 4.5f;
        }
        if (obj[10].activeSelf) //car 1
        {
            disruption += 4.5f;
        }
        if (obj[8].activeSelf) //drum
        {
            disruption += 4.5f;
        }
        if (obj[7].activeSelf) //elephant 2
        {
            disruption += 4.5f;
        }
        if (obj[5].activeSelf) //giraffe
        {
            disruption += 4.5f;
        }
        if (obj[4].activeSelf) //guitar
        {
            disruption += 4.5f;
        }
        if (obj[3].activeSelf) //makeup 2
        {
            disruption += 4.5f;
        }
        if (obj[1].activeSelf) //plane
        {
            disruption += 4.5f;
        }

        crot.disruption = 100 - disruption;
        crot.quality = quality;
    }

	// Update is called once per frame
    //todo:Update()
	void Update () {
        if (pfade > 0) //fade print successful message
        {
            pfade -= Time.deltaTime;
        }
        if (crot.fellasleep && sleeping) //if during a night cycle, the avatar falls asleep, play ZZZ particles
        {
            if (!crot._psleep.isPlaying)
                crot._psleep.Play();
        }
        else
        {
            if (crot._psleep.isPlaying)
            {
                crot._psleep.Stop();
            }
        }

        UpTouch(); //update touch inside here, and object clicking

        if (jumptograph > 0) // at end of sleep cycle, after a delay show the chart screen for the current trial
        {
            jumptograph -= Time.deltaTime;
            if (jumptograph <= 0)
            {
                curchart = trials.Length - 1;
                showCharts = true;
                cvac = -1;
                pr.trial = trials[trials.Length - 1];
                pr.trial.active = true;
            }
        }

        if (mfade > 0) //update message fade
            mfade -= Time.deltaTime;

        mp = Input.mousePosition; //update mouse position
        mp.y = h - mp.y;
        if (touchpos != treset)
        {
            mp = touchpos;
        }

        crot.enabled = sleeping; //show sleep graph only while sleeping
        gcam.enabled = crot.enabled; 
        ccam.enabled = !showCharts; //show clock when not on charts screen
        _chartcam.enabled = showCharts; 
        _win.sleep = sleeping; //fade the white window when sleeping to show the night background
        fade -= Time.deltaTime*1.75f;
        if (fade < 0)
            fade = 2;
        if (!sleeping)
        {
            crot.Reset(); //reset the graph's quality values when not simulating
            daylight.intensity = Mathf.MoveTowards(daylight.intensity, 1, Time.deltaTime * .5f); //switch lights gradually when entering or leaving sleep cycle
            nitelight.intensity = Mathf.MoveTowards(nitelight.intensity, 0, Time.deltaTime * .5f);
        }
        else
        {
            daylight.intensity = Mathf.MoveTowards(daylight.intensity, 0, Time.deltaTime * .5f);
            nitelight.intensity = Mathf.MoveTowards(nitelight.intensity, .5f, Time.deltaTime * .5f);
        }
        for (int i = 0; i < cams.Length; i++) //enable current main camera
        {
            if (i == curCam && !cams[i].enabled)
            {
                cams[i].enabled = true;
            }
            else if(cams[i].enabled && i!=curCam)
            {
                cams[i].enabled = false;
            }
        }
        if (!pr.enabled && Time.fixedUnscaledTime>.5f) //enable post render/ trial charts script slightly after application start to avoid heap overflow on game start.
            pr.enabled = true;
	}

    //Begin OnGUI
    //todo:OnGUI()
    private void OnGUI()
    {
        h = Screen.height;
        w = Screen.width;
        GUIStyle gstyle = new GUIStyle(); //mainly used to draw buttons without the default button texture. Also used as a base for other GUiStyle 
        gstyle.normal.textColor = Color.white;

        if (showHelp)
        {
            //display help menu
            GUIStyle hstyle = new GUIStyle(gstyle);
            hstyle.normal.textColor = Color.black;
            hstyle.fontSize = Mathf.RoundToInt(h * .035f);
            hstyle.wordWrap = true;

            Rect rhbg = new Rect(0,0,w*.7f,h*.8f);
            rhbg.center = new Vector2(w * .5f, h * .5f);
            GUI.DrawTexture(rhbg, tpop); //draw backdrop

            Rect rhtext = new Rect(rhbg.x + w*.07f,rhbg.y + w*.04f,0,0);
            rhtext.xMax = rhbg.xMax - w * .07f;
            rhtext.yMax = rhbg.yMax;
            GUI.Label(rhtext, hmessages[hcounter], hstyle); //draw help text

            Rect rnext = new Rect(0,0,h*.08f,h*.08f);
            rnext.center = new Vector2(w * .5f + w * .12f, h * .78f);
            if (GUI.Button(rnext, arrows[1], gstyle) || rnext.Contains(tappos)) //arrow for moving forward a page
            {
                tappos = treset;
                if(hcounter < hmessages.Length - 1)
                    hcounter++;
            }

            Rect rprev = new Rect(0, 0, h * .08f, h * .08f);
            rprev.center = new Vector2(w * .5f - w * .12f, h * .78f);
            if (GUI.Button(rprev, arrows[0], gstyle) || rprev.Contains(tappos)) //arrow for moving back a page
            {
                tappos = treset;
                if(hcounter>0)
                    hcounter--;
            }

            Rect rclose = new Rect(0,0,w*.15f,h*.08f);
            rclose.center = new Vector2(w * .5f, h * .78f);
            if (GUI.Button(rclose, tui[24], gstyle) || rclose.Contains(tappos)) //button that closes the help menu
            {
                tappos = treset;
                showHelp = false;
            }
        }
        else //if not showing help menu
        {
            if (!showCharts)
            {
                GUIStyle wstyle = new GUIStyle(gstyle);
                wstyle.normal.textColor = Color.yellow;
                wstyle.fontSize = Mathf.RoundToInt(h * .0325f);
                if (sleeping && !crot.fellasleep) //if awake during sleep cycle, show flashing yellow text
                {
                    Rect rnosleep = new Rect(w * .3f, h * .06f, 0, 0);
                    
                    if (fade <= 1)
                    {
                        GUI.color = new Color(1, 1, 1, fade);

                    }
                    else
                    {
                        GUI.color = new Color(1, 1, 1, Mathf.Abs(fade - 2));
                    }
                    GUI.Label(rnosleep, "Falling asleep...", wstyle);

                    GUI.color = Color.white;
                }

                if (sleeping && crot.eduration > 0) //during an event, display a warning symbol
                {
                    Rect rnosleep = new Rect(w * .265f, h * .06f, 0, 0);
                    if (fade <= 1)
                    {
                        GUI.color = new Color(1, 0, 0, fade);
                    }
                    else
                    {
                        GUI.color = new Color(1, 0, 0, Mathf.Abs(fade - 2));
                    }
                    GUI.Label(rnosleep, "!", wstyle);
                }

                Rect rsidebar = new Rect(w * .86f, 0, 0, h); //draw sidebar
                rsidebar.xMax = w;
                GUI.color = new Color(1, 1, 1, .8f);
                GUI.DrawTexture(rsidebar, tsidebar);
                GUI.color = Color.white;

                Rect ricon = new Rect(w * .9f, h * .015f, h * .06f, h * .06f); //used to draw the icon buttons on the right side
                float xoffset = ricon.width + w * .02f;
                float yoffset = ricon.height + h * .02f;

                for (int i = 0; i < 24; i += 2) // i+=2 instead of i++, in order to handle the left and right icon buttons per iteration going downward
                {
                    bool darken = false; //darken icons where at least one of the objects are inactive
                    if (i == 0 && (!obj[16].activeSelf || !obj[17].activeSelf)) {darken = true;}
                    if (i == 4 && (!obj[12].activeSelf || !obj[13].activeSelf)) { darken = true; }
                    if (i == 6 && !obj[9].activeSelf) { darken = true; }
                    if (i == 8 && (!obj[6].activeSelf || !obj[7].activeSelf)) { darken = true; }
                    if (i == 10 && !obj[5].activeSelf) { darken = true; }
                    if (i == 12 && !_lamps[1].on) { darken = true; }
                    if (i == 14 && !_lamps[2].on) { darken = true; }
                    if (i == 16 && (!obj[2].activeSelf || !obj[3].activeSelf)) { darken = true; }
                    if (i == 18 && !obj[0].activeSelf) { darken = true; }
                    if (i == 20 && !ster2.on) { darken = true; }

                    if (darken)
                    {
                        GUI.color = new Color(.5f, .5f, .5f, 1);
                        darken = false;
                    }
                    GUI.DrawTexture(ricon, ticons[i], ScaleMode.ScaleToFit);
                    GUI.color = Color.white;

                    if (showProperties == i) //draw highlight around current icon if selected
                    {
                        GUI.color = new Color(1, 1, 0, .5f);
                        GUI.DrawTexture(ricon, tui[26]);
                        GUI.color = Color.white;
                    }

                    Rect temp = new Rect(ricon);
                    temp.x += xoffset;
                    if (i + 1 != 24) //repeat for the right side icon
                    {
                        if (i+1 == 1 && !obj[18].activeSelf) { darken = true; }
                        if (i + 1 == 3 && (!obj[14].activeSelf||!obj[15].activeSelf)) { darken = true; }
                        if (i + 1 == 5 && (!obj[10].activeSelf || !obj[11].activeSelf)) { darken = true; }
                        if (i + 1 == 7 && !obj[8].activeSelf) { darken = true; }
                        if (i + 1 == 11 && !obj[4].activeSelf) { darken = true; }
                        if (i + 1 == 13 && !_lamps[0].on) { darken = true; }
                        if (i + 1 == 15 && !lap.on) { darken = true; }
                        if (i + 1 == 17 && !obj[1].activeSelf) { darken = true; }
                        if (i + 1 == 19 && !ster1.on) { darken = true; }
                        if (i + 1 == 21 && !_tv.on) { darken = true; }
                        if (i + 1 == 23 && !obj[19].activeSelf) { darken = true; }

                        if (darken)
                        {
                            GUI.color = new Color(.5f, .5f, .5f, 1);
                            darken = false;
                        }

                        GUI.DrawTexture(temp, ticons[i + 1], ScaleMode.ScaleToFit);
                        GUI.color = Color.white;
                        if (showProperties == i + 1)
                        {
                            GUI.color = new Color(1, 1, 0, .5f);
                            GUI.DrawTexture(temp, tui[26]);
                            GUI.color = Color.white;
                        }
                    }
                    ricon.y += yoffset; //update y draw position for the button icons
                }

                ricon = new Rect(w * .9f, h * .015f, h * .06f, h * .06f);
                for (int i = 0; i < 24; i += 2) //select icon when clicked or tapped, deselect if already selected
                {
                    if (GUI.Button(ricon, "", gstyle) || ricon.Contains(tappos))
                    {
                        tappos = treset;
                        if (showProperties != i)
                            showProperties = i;
                        else
                            showProperties = -1;
                    }
                    Rect temp = new Rect(ricon);
                    temp.x += xoffset;
                    if (i + 1 != 24)
                    {
                        if (GUI.Button(temp, "", gstyle) || temp.Contains(tappos))
                        {
                            tappos = treset;
                            if (showProperties != i + 1)
                                showProperties = i + 1;
                            else
                                showProperties = -1;

                        }
                    }
                    ricon.y += yoffset;
                }

                //draw the arrows for changing camera
                Rect rvswitch = new Rect(w * .45f, h * .9f, h * .08f, h * .08f);
                if (curCam == 0)
                {
                    GUI.color = new Color(1, 1, 1, .25f);
                }
                if (GUI.Button(rvswitch, arrows[0], new GUIStyle()) || rvswitch.Contains(tappos))
                {
                    tappos = treset;
                    curCam = 0;
                }
                GUI.color = Color.white;
                if (curCam == 1)
                {
                    GUI.color = new Color(1, 1, 1, .25f);
                }
                rvswitch.x = w * .55f;
                if (GUI.Button(rvswitch, arrows[1], new GUIStyle()) || rvswitch.Contains(tappos))
                {
                    tappos = treset;
                    curCam = 1;
                }
                GUI.color = Color.white;

                rvswitch.center = new Vector2(w * .52f, rvswitch.center.y);
                rvswitch.y += h * .004f;
                rvswitch.height *= .8f;
                GUI.DrawTexture(rvswitch, tui[25],ScaleMode.ScaleToFit);

                //if an item is selected, draw object properties
                //todo:draw properties
                if (showProperties >= 0)
                {
                    Rect rpop = new Rect(-h * .1f, -h * .1f, w * .3f, h * .43f);
                    GUI.color = new Color(1, 1, 1, .8f);
                    GUI.DrawTexture(rpop, tpop); //properties backdrop
                    
                    //draw arrows for changing to next or previous item
                    Rect r_left = new Rect(w * .05f, rpop.yMax + h * .02f, h * .07f, h * .07f);
                    if (GUI.Button(r_left, arrows[0], gstyle) || r_left.Contains(tappos))
                    {
                        tappos = treset;
                        if (showProperties > 0)
                            showProperties--;
                    }
                    Rect r_right = new Rect(r_left);
                    r_right.x += r_left.width + w * .03f;
                    if (GUI.Button(r_right, arrows[1], gstyle) || r_right.Contains(tappos))
                    {
                        tappos = treset;
                        if (showProperties < 23)
                            showProperties++;
                    }

                    //draw the icon for the selected item
                    Rect ri = new Rect(h * .02f, h * .02f, h * .05f, h * .05f);
                    GUI.DrawTexture(ri, ticons[showProperties], ScaleMode.ScaleToFit);

                    GUIStyle pstyle = new GUIStyle(gstyle);
                    pstyle.normal.textColor = Color.black;
                    pstyle.fontSize = Mathf.RoundToInt(h * .038f);

                    //draw name of item
                    Rect rname = new Rect(h * .09f, h * .025f, 0, 0);
                    GUI.Label(rname, inames[showProperties], pstyle);

                    //draw close button of properties window
                    Rect rclose = new Rect(w * .185f, h * .02f, h * .045f, h * .045f);
                    if (GUI.Button(rclose, tui[3], gstyle)||rclose.Contains(tappos))
                    {
                        tappos = treset;
                        showProperties = -1;
                    }

                    //depending on the item selected, draw the properties
                    //note in retrospect this portion could potentially be optimized by placing the properties on their own script(s) on property controller objects, and drawing the properties window from the property controller instead
                    // then enable/set active only the currently selected property controller script
                    // touch will still need to be handled using this script's variables using if(GUI.Button(x,x,x) || rect.contains(sl.tappos)) {sl.tappos = sl.treset; //code}
                    if (showProperties == 0) //bean bags
                    {
                        Rect rptext = new Rect(h * .02f, h * .1f, 0, 0);
                        GUI.Label(rptext, "Toggle", pstyle);

                        Rect rcbox = new Rect(pstyle.CalcSize(new GUIContent("Toggle")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);
                        if (!obj[16].activeSelf)
                        {
                            if (GUI.Button(rcbox, tui[0], gstyle) || rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[16].SetActive(true);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Bean Bag 1", "Toggled on", _clock.seconds);
                                    taction.icon = 0;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(rcbox, tui[1], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[16].SetActive(false);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Bean Bag 1", "Toggled off", _clock.seconds);
                                    taction.icon = 0;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }

                        rptext = new Rect(h * .02f, h * .175f, 0, 0);
                        GUI.Label(rptext, "Toggle", pstyle);

                        rcbox = new Rect(pstyle.CalcSize(new GUIContent("Toggle")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);
                        if (!obj[17].activeSelf)
                        {
                            if (GUI.Button(rcbox, tui[0], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[17].SetActive(true);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Bean Bag 2", "Toggled on", _clock.seconds);
                                    taction.icon = 0;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(rcbox, tui[1], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[17].SetActive(false);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Bean Bag 2", "Toggled off", _clock.seconds);
                                    taction.icon = 0;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                    }
                    if (showProperties == 1) //teddy bear
                    {
                        Rect rptext = new Rect(h * .02f, h * .1f, 0, 0);
                        GUI.Label(rptext, "Toggle", pstyle);

                        Rect rcbox = new Rect(pstyle.CalcSize(new GUIContent("Toggle")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);
                        if (!obj[18].activeSelf)
                        {
                            if (GUI.Button(rcbox, tui[0], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[18].SetActive(true);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Teddy Bear", "Toggled on", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(rcbox, tui[1], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[18].SetActive(false);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Teddy Bear", "Toggled off", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                    }
                    if (showProperties == 2) //bed
                    {
                        Rect rptext = new Rect(h * .02f, h * .08f, 0, 0);
                        GUI.Label(rptext, "Color:", pstyle);

                        Rect rcbox = new Rect(w * .01f, h * .13f, h * .04f, h * .04f);
                        float xoff = rcbox.width + w * .01f;
                        float yoff = rcbox.height + w * .01f;
                        float orix = rcbox.x;
                        for (int i = 0; i < bcolors.Length; i++)
                        {
                            GUI.color = bcolors[i];

                            if (GUI.Button(rcbox, tui[2], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                if (bed.c != bcolors[i])
                                {//
                                    if (sleeping)
                                    {
                                        Action taction = new Action("Bed", "Color set to " + scolor[i], _clock.seconds);
                                        taction.icon = showProperties;
                                        taction.cnumber = i;
                                        trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                    }
                                }
                                bed.c = bcolors[i];
                                SetQuality();
                            }
                            GUI.color = Color.white;
                            if (bed.c == bcolors[i])
                            {
                                GUI.DrawTexture(rcbox, tui[4]);
                            }
                            rcbox.x += xoff;
                            if ((i + 1) % 6 == 0)
                            {
                                rcbox.y += yoff;
                                rcbox.x = orix;
                            }
                        }
                    }
                    if (showProperties == 3) //boat
                    {
                        Rect rptext = new Rect(h * .02f, h * .1f, 0, 0);
                        GUI.Label(rptext, "Toggle", pstyle);

                        Rect rcbox = new Rect(pstyle.CalcSize(new GUIContent("Toggle")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);
                        if (!obj[14].activeSelf)
                        {
                            if (GUI.Button(rcbox, tui[0], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[14].SetActive(true);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Boat 1", "Toggled on", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(rcbox, tui[1], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[14].SetActive(false);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Boat 1", "Toggled off", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }

                        rptext = new Rect(h * .02f, h * .175f, 0, 0);
                        GUI.Label(rptext, "Toggle", pstyle);

                        rcbox = new Rect(pstyle.CalcSize(new GUIContent("Toggle")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);
                        if (!obj[15].activeSelf)
                        {
                            if (GUI.Button(rcbox, tui[0], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[15].SetActive(true);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Boat 2", "Toggled on", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(rcbox, tui[1], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[15].SetActive(false);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Boat 2", "Toggled off", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                    }
                    if (showProperties == 4) //books
                    {
                        Rect rptext = new Rect(h * .02f, h * .1f, 0, 0);
                        GUI.Label(rptext, "Toggle", pstyle);

                        Rect rcbox = new Rect(pstyle.CalcSize(new GUIContent("Toggle")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);
                        if (!obj[12].activeSelf)
                        {
                            if (GUI.Button(rcbox, tui[0], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[12].SetActive(true);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Books 1", "Toggled on", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(rcbox, tui[1], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[12].SetActive(false);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Books 1", "Toggled off", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }

                        rptext = new Rect(h * .02f, h * .175f, 0, 0);
                        GUI.Label(rptext, "Toggle", pstyle);

                        rcbox = new Rect(pstyle.CalcSize(new GUIContent("Toggle")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);
                        if (!obj[13].activeSelf)
                        {
                            if (GUI.Button(rcbox, tui[0], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[13].SetActive(true);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Books 2", "Toggled on", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(rcbox, tui[1], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[13].SetActive(false);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Books 2", "Toggled off", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                    }
                    if (showProperties == 5) //cars
                    {
                        Rect rptext = new Rect(h * .02f, h * .1f, 0, 0);
                        GUI.Label(rptext, "Toggle", pstyle);

                        Rect rcbox = new Rect(pstyle.CalcSize(new GUIContent("Toggle")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);
                        if (!obj[10].activeSelf)
                        {
                            if (GUI.Button(rcbox, tui[0], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[10].SetActive(true);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Car 1", "Toggled on", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(rcbox, tui[1], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[10].SetActive(false);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Car 1", "Toggled off", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }

                        rptext = new Rect(h * .02f, h * .175f, 0, 0);
                        GUI.Label(rptext, "Toggle", pstyle);

                        rcbox = new Rect(pstyle.CalcSize(new GUIContent("Toggle")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);
                        if (!obj[11].activeSelf)
                        {
                            if (GUI.Button(rcbox, tui[0], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[11].SetActive(true);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Car 2", "Toggled on", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(rcbox, tui[1], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[11].SetActive(false);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Car 2", "Toggled off", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                    }
                    if (showProperties == 6) //chair
                    {
                        Rect rptext = new Rect(h * .02f, h * .1f, 0, 0);
                        GUI.Label(rptext, "Toggle", pstyle);

                        Rect rcbox = new Rect(pstyle.CalcSize(new GUIContent("Toggle")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);
                        if (!obj[9].activeSelf)
                        {
                            if (GUI.Button(rcbox, tui[0], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[9].SetActive(true);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Chair", "Toggled on", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(rcbox, tui[1], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[9].SetActive(false);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Chair", "Toggled off", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                    }
                    if (showProperties == 7) //drum
                    {
                        Rect rptext = new Rect(h * .02f, h * .1f, 0, 0);
                        GUI.Label(rptext, "Toggle", pstyle);

                        Rect rcbox = new Rect(pstyle.CalcSize(new GUIContent("Toggle")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);
                        if (!obj[8].activeSelf)
                        {
                            if (GUI.Button(rcbox, tui[0], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[8].SetActive(true);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Drum", "Toggled on", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(rcbox, tui[1], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[8].SetActive(false);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Drum", "Toggled off", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                    }
                    if (showProperties == 8) //elephant
                    {
                        Rect rptext = new Rect(h * .02f, h * .1f, 0, 0);
                        GUI.Label(rptext, "Toggle", pstyle);

                        Rect rcbox = new Rect(pstyle.CalcSize(new GUIContent("Toggle")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);
                        if (!obj[6].activeSelf)
                        {
                            if (GUI.Button(rcbox, tui[0], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[6].SetActive(true);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Elephant 1", "Toggled on", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(rcbox, tui[1], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[6].SetActive(false);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Elephant 1", "Toggled off", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }

                        rptext = new Rect(h * .02f, h * .175f, 0, 0);
                        GUI.Label(rptext, "Toggle", pstyle);

                        rcbox = new Rect(pstyle.CalcSize(new GUIContent("Toggle")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);
                        if (!obj[7].activeSelf)
                        {
                            if (GUI.Button(rcbox, tui[0], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[7].SetActive(true);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Elephant 2", "Toggled on", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(rcbox, tui[1], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[7].SetActive(false);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Elephant 2", "Toggled off", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                    }
                    if (showProperties == 9) //fan
                    {
                        Rect rptext = new Rect(h * .02f, h * .1f, 0, 0);
                        GUI.Label(rptext, "0", pstyle);

                        Rect rcbox = new Rect(pstyle.CalcSize(new GUIContent("0")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);

                        if ((GUI.Button(rcbox, tui[0], gstyle) && fan.speed != 0) || (rcbox.Contains(tappos) && fan.speed != 0))
                        {
                            tappos = treset;
                            fan.speed = 0;
                            SetQuality();
                            if (sleeping)
                            {
                                Action taction = new Action("Fan", "Toggled off", _clock.seconds);
                                taction.icon = showProperties;
                                taction.cnumber = 0;
                                trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                            }
                        }
                        if (fan.speed == 0)
                        {
                            GUI.DrawTexture(rcbox, tui[1]);
                        }

                        for (int i = 3; i > 0; i--)
                        {
                            rcbox.y += rcbox.height * 1.25f;
                            rptext.y = rcbox.y;
                            GUI.Label(rptext, i.ToString(), pstyle);
                            if ((GUI.Button(rcbox, tui[0], gstyle) && fan.speed != i) || (rcbox.Contains(tappos) && fan.speed != i))
                            {
                                fan.speed = i;
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Fan", "Speed set to " + i.ToString(), _clock.seconds);
                                    taction.icon = showProperties;
                                    taction.cnumber = i;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                            if (fan.speed == i)
                            {
                                GUI.DrawTexture(rcbox, tui[1]);
                            }
                        }
                    }

                    if (showProperties == 10) //giraffe
                    {
                        Rect rptext = new Rect(h * .02f, h * .1f, 0, 0);
                        GUI.Label(rptext, "Toggle", pstyle);

                        Rect rcbox = new Rect(pstyle.CalcSize(new GUIContent("Toggle")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);
                        if (!obj[5].activeSelf)
                        {
                            if (GUI.Button(rcbox, tui[0], gstyle) || rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[5].SetActive(true);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Giraffe", "Toggled on", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(rcbox, tui[1], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[5].SetActive(false);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Giraffe", "Toggled off", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                    }
                    if (showProperties == 11) //guitar
                    {
                        Rect rptext = new Rect(h * .02f, h * .1f, 0, 0);
                        GUI.Label(rptext, "Toggle", pstyle);

                        Rect rcbox = new Rect(pstyle.CalcSize(new GUIContent("Toggle")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);
                        if (!obj[4].activeSelf)
                        {
                            if (GUI.Button(rcbox, tui[0], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[4].SetActive(true);
                                SetQuality();
                                obj[4].GetComponent<AudioSource>().Play();
                                if (sleeping)
                                {
                                    Action taction = new Action("Guitar", "Toggled on", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(rcbox, tui[1], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[4].SetActive(false);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Guitar", "Toggled off", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                    }
                    if (showProperties == 12) //lamp 1
                    {

                        Rect rptext = new Rect(h * .02f, h * .1f, 0, 0);
                        GUI.Label(rptext, "On", pstyle);

                        Rect rcbox = new Rect(pstyle.CalcSize(new GUIContent("On")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);
                        if (!_lamps[1].on)
                        {
                            if (GUI.Button(rcbox, tui[0], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                _lamps[1].on = true;
                                SetQuality();
                                _as.Play();
                                if (sleeping)
                                {
                                    Action taction = new Action("Lamp 1", "Toggled on", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(rcbox, tui[1], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                _lamps[1].on = false;
                                SetQuality();
                                _as.Play();
                                if (sleeping)
                                {
                                    Action taction = new Action("Lamp 1", "Toggled off", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                    }
                    if (showProperties == 13) //lamp 2
                    {
                        Rect rptext = new Rect(h * .02f, h * .1f, 0, 0);
                        GUI.Label(rptext, "On", pstyle);

                        Rect rcbox = new Rect(pstyle.CalcSize(new GUIContent("On")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);
                        if (!_lamps[0].on)
                        {
                            if (GUI.Button(rcbox, tui[0], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                _lamps[0].on = true;
                                SetQuality();
                                _as.Play();
                                if (sleeping)
                                {
                                    Action taction = new Action("Lamp 2", "Toggled on", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(rcbox, tui[1], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                _lamps[0].on = false;
                                SetQuality();
                                _as.Play();
                                if (sleeping)
                                {
                                    Action taction = new Action("Lamp 2", "Toggled off", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                    }
                    if (showProperties == 14) //lamp 3
                    {
                        Rect rptext = new Rect(h * .02f, h * .1f, 0, 0);
                        GUI.Label(rptext, "On", pstyle);

                        Rect rcbox = new Rect(pstyle.CalcSize(new GUIContent("On")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);
                        if (!_lamps[2].on)
                        {
                            if (GUI.Button(rcbox, tui[0], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                _lamps[2].on = true;
                                SetQuality();
                                _as.Play();
                                if (sleeping)
                                {
                                    Action taction = new Action("Lamp 3", "Toggled on", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(rcbox, tui[1], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                _lamps[2].on = false;
                                SetQuality();
                                _as.Play();
                                if (sleeping)
                                {
                                    Action taction = new Action("Lamp 3", "Toggled off", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                    }
                    if (showProperties == 15) //laptop
                    {
                        Rect rptext = new Rect(h * .02f, h * .1f, 0, 0);
                        GUI.Label(rptext, "On", pstyle);

                        Rect rcbox = new Rect(pstyle.CalcSize(new GUIContent("On")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);
                        if (!lap.on)
                        {
                            if (GUI.Button(rcbox, tui[0], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                lap.on = true;
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Laptop", "Toggled on", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(rcbox, tui[1], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                lap.on = false;
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Laptop", "Toggled off", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                    }
                    if (showProperties == 16) //makeup
                    {
                        Rect rptext = new Rect(h * .02f, h * .1f, 0, 0);
                        GUI.Label(rptext, "Toggle", pstyle);

                        Rect rcbox = new Rect(pstyle.CalcSize(new GUIContent("Toggle")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);
                        if (!obj[2].activeSelf)
                        {
                            if (GUI.Button(rcbox, tui[0], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[2].SetActive(true);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Makeup 1", "Toggled on", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(rcbox, tui[1], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;

                                obj[2].SetActive(false);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Makeup 1", "Toggled off", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }

                        rptext = new Rect(h * .02f, h * .175f, 0, 0);
                        GUI.Label(rptext, "Toggle", pstyle);

                        rcbox = new Rect(pstyle.CalcSize(new GUIContent("Toggle")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);
                        if (!obj[3].activeSelf)
                        {
                            if (GUI.Button(rcbox, tui[0], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[3].SetActive(true);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Makeup 2", "Toggled on", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(rcbox, tui[1], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[3].SetActive(false);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Makeup 2", "Toggled off", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                    }
                    if (showProperties == 17) //airplane
                    {
                        Rect rptext = new Rect(h * .02f, h * .1f, 0, 0);
                        GUI.Label(rptext, "Toggle", pstyle);

                        Rect rcbox = new Rect(pstyle.CalcSize(new GUIContent("Toggle")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);
                        if (!obj[1].activeSelf)
                        {
                            if (GUI.Button(rcbox, tui[0], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[1].SetActive(true);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Airplane", "Toggled on", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(rcbox, tui[1], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[1].SetActive(false);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Airplane", "Toggled off", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                    }
                    if (showProperties == 18) //potted plant
                    {
                        Rect rptext = new Rect(h * .02f, h * .1f, 0, 0);
                        GUI.Label(rptext, "Toggle", pstyle);

                        Rect rcbox = new Rect(pstyle.CalcSize(new GUIContent("Toggle")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);
                        if (!obj[0].activeSelf)
                        {
                            if (GUI.Button(rcbox, tui[0], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[0].SetActive(true);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Potted Plant", "Toggled on", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(rcbox, tui[1], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[0].SetActive(false);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Potted Plant", "Toggled off", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                    }
                    if (showProperties == 19) //stereo 1
                    {
                        Rect rptext = new Rect(h * .02f, h * .1f, 0, 0);
                        GUI.Label(rptext, "On", pstyle);

                        Rect rcbox = new Rect(pstyle.CalcSize(new GUIContent("On")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);
                        if (!ster1.on)
                        {
                            if (GUI.Button(rcbox, tui[0], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                ster1.on = true;
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Stereo 1", "Toggled on", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(rcbox, tui[1], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                ster1.on = false;
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Stereo 1", "Toggled off", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                    }
                    if (showProperties == 20) //stereo 2
                    {
                        Rect rptext = new Rect(h * .02f, h * .1f, 0, 0);
                        GUI.Label(rptext, "On", pstyle);

                        Rect rcbox = new Rect(pstyle.CalcSize(new GUIContent("On")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);
                        if (!ster2.on)
                        {
                            if (GUI.Button(rcbox, tui[0], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                ster2.on = true;
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Stereo 2", "Toggled on", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(rcbox, tui[1], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                ster2.on = false;
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Stereo 2", "Toggled off", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                    }
                    if (showProperties == 21) //tv
                    {
                        Rect rptext = new Rect(h * .02f, h * .1f, 0, 0);
                        GUI.Label(rptext, "On", pstyle);

                        Rect rcbox = new Rect(pstyle.CalcSize(new GUIContent("On")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);
                        if (!_tv.on)
                        {
                            if (GUI.Button(rcbox, tui[0], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                _tv.on = true;
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("TV", "Toggled on", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(rcbox, tui[1], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                if (sleeping)
                                {
                                    Action taction = new Action("TV", "Toggled off", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                                _tv.on = false;
                                SetQuality();
                            }
                        }
                    }
                    if (showProperties == 22) //wall
                    {
                        Rect rptext = new Rect(h * .02f, h * .08f, 0, 0);
                        GUI.Label(rptext, "Color:", pstyle);

                        Rect rcbox = new Rect(w * .01f, h * .13f, h * .04f, h * .04f);
                        float xoff = rcbox.width + w * .01f;
                        float yoff = rcbox.height + w * .01f;
                        float orix = rcbox.x;
                        for (int i = 0; i < bcolors.Length; i++)
                        {
                            GUI.color = bcolors[i];
                            if (GUI.Button(rcbox, tui[2], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                if (wall.c != bcolors[i])
                                {
                                    if (sleeping)
                                    {
                                        Action taction = new Action("Wall", "Color set to " + scolor[i], _clock.seconds);
                                        taction.icon = showProperties;
                                        taction.cnumber = i;
                                        trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                    }
                                }
                                wall.c = bcolors[i];
                                SetQuality();
                            }
                            GUI.color = Color.white;
                            if (wall.c == bcolors[i])
                            {
                                GUI.DrawTexture(rcbox, tui[4]);
                            }
                            rcbox.x += xoff;
                            if ((i + 1) % 6 == 0)
                            {
                                rcbox.y += yoff;
                                rcbox.x = orix;
                            }
                        }
                    }
                    if (showProperties == 23) //poster
                    {
                        Rect rptext = new Rect(h * .02f, h * .1f, 0, 0);
                        GUI.Label(rptext, "Toggle", pstyle);

                        Rect rcbox = new Rect(pstyle.CalcSize(new GUIContent("Toggle")).x + rptext.x + h * .02f, rptext.y, h * .04f, h * .04f);
                        if (!obj[19].activeSelf)
                        {
                            if (GUI.Button(rcbox, tui[0], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[19].SetActive(true);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Poster", "Toggled on", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                        else
                        {
                            if (GUI.Button(rcbox, tui[1], gstyle)||rcbox.Contains(tappos))
                            {
                                tappos = treset;
                                obj[19].SetActive(false);
                                SetQuality();
                                if (sleeping)
                                {
                                    Action taction = new Action("Poster", "Toggled off", _clock.seconds);
                                    taction.icon = showProperties;
                                    trials[trials.Length - 1].Add(taction); MakeSprite(taction);
                                }
                            }
                        }
                    }
                } //end showProperties
                if (gcam.enabled) //if graph camera is enabled, draw graph UI
                {
                    GUI.color = new Color(0, 1, 0, 1);
                    Rect rline = new Rect(w * .25f, h * .01f, h * .002f, h * .265f);
                    GUI.DrawTexture(rline, white);
                    rline = new Rect(w * .25f, h * .275f, w * .59f, h * .002f);
                    GUI.DrawTexture(rline, white);
                    GUI.color = Color.white;

                    GUIStyle sstyle = new GUIStyle(gstyle);
                    sstyle.fontSize = Mathf.RoundToInt(h * .04f);
                    sstyle.normal.textColor = new Color(0, .9f, 0, .6f);
                    sstyle.richText = true;
                    Rect rqtext = new Rect(w * .26f, h * .01f, 0, 0);
                    GUI.Label(rqtext, "Sleep Quality: " + crot.current.ToString("F0") + "%", sstyle);
                }

                Rect rsleep = new Rect(w * .02f, h * .94f, w * .112f, h * .07f);
                if (sleeping) //draw restart button
                {
                    Rect r_restart = new Rect(rsleep);
                    r_restart.y = h * .65f;
                    if (GUI.Button(r_restart, tui[28], gstyle) || r_restart.Contains(tappos))
                    {
                        //todo:restart button code
                        tappos = treset;
                        _clock.seconds = 72000;
                        _clock.nextHour = 72000;
                        crot.Reset();
                        crot.current = 0;
                        trials[trials.Length - 1] = new Trial();
                        trials[trials.Length - 1].sl = this;
                        trials[trials.Length - 1].Initialize();
                        _clock.tf.Restart();
                        crot.eduration = 0;
                        crot.eq = 0;
                        ec.e = 0;

                        crot.fellasleep = false;
                    }
                }

                if (!sleeping)
                {
                    //todo: show help button
                    Rect rhelp = new Rect(w * .8f, h * .9f, h * .08f, h * .08f);
                    if (GUI.Button(rhelp, tui[23], gstyle)||rhelp.Contains(tappos)) //help button
                    {
                        tappos = treset;
                        showHelp = true;
                    }
                    Rect rchart = new Rect(w * .75f, h * .02f, w * .1f, h * .07f);
                    if ((GUI.Button(rchart, tui[6], gstyle) && jumptograph<=0)||(rchart.Contains(tappos) && jumptograph<=0)) //draw charts button
                    {
                        tappos = treset;
                        //
                        curchart = 0;
                        if (trials.Length >= 1)
                        {
                            pr.trial = trials[0];
                        }
                        cvac = -1;
                        showCharts = true;
                    }
                    if (GUI.Button(rsleep, tui[5], gstyle) || rsleep.Contains(tappos)) //draw sleep button
                    {
                        //start sleep cycle
                        tappos = treset;
                        _clock.seconds = 72000;
                        crot.current = 0;
                        sleeping = true;
                        _clock.active = true;
                        if (trials.Length < 3)
                        {
                            Trial[] temp = new Trial[trials.Length + 1];
                            for (int i = 0; i < trials.Length; i++)
                            {
                                temp[i] = trials[i];
                            }
                            temp[temp.Length - 1] = new Trial();
                            trials = temp;
                            trials[trials.Length - 1].sl = this;
                            trials[trials.Length - 1].Initialize();

                        }
                        else
                        {
                            trials[0] = trials[1];
                            trials[1] = trials[2];
                            trials[2] = new Trial();
                            trials[2].sl = this;
                            trials[2].Initialize();

                        }
                    }
                }
                else
                {
                    //todo: display popup messages
                    float subfade = mfade * .7f;
                    if (subfade > .7f)
                        subfade = .7f;
                    GUI.color = new Color(1, 1, 1, subfade);
                    Rect rbubble = new Rect(0, 0, w * .25f, h * .3f);
                    rbubble.center = new Vector2(w * .5f, h * .55f);
                    GUI.DrawTexture(rbubble, tpop);
                    Rect rmessage = new Rect(w * .5f, h * .55f, 0, 0);
                    GUIStyle estyle = new GUIStyle(gstyle);
                    estyle.alignment = TextAnchor.MiddleCenter;
                    estyle.fontSize = Mathf.RoundToInt(h * .035f);
                    estyle.normal.textColor = Color.black;
                    GUI.color = new Color(.8f, .8f, 1, mfade);
                    GUI.Label(rmessage, etext, estyle);
                    GUI.color = Color.white;

                    GUIStyle timestyle = new GUIStyle(gstyle);
                    timestyle.fontSize = Mathf.RoundToInt(h * .04f);
                    timestyle.normal.textColor = new Color(0f, 1f, .05f, 1);
                    Rect rbtn = new Rect(rsleep);
                    rbtn.width = w * .12f;
                    rbtn.height = h * .045f;
                    rbtn.x -= w * .005f;
                    GUI.DrawTexture(rbtn, tui[22]);
                    //rsleep.x = w * .025f;
                    string hr = "";
                    if (crot.c.hour < 10)
                    {
                        hr = "0";
                    }
                    string m = "";
                    if (crot.c.minute < 10)
                    {
                        m = "0";
                    }
                    GUI.Label(rsleep, hr + crot.c.hour.ToString() + ":" + m + crot.c.minute.ToString() + " " + crot.c._type[crot.c.t], timestyle);

                    //draw speed up and slowdown buttons
                    Rect rplus = new Rect(w * .15f, h * .855f, h * .06f, h * .06f);
                    Rect rminus = new Rect(w * .15f, rplus.y + rplus.height + h * .01f, h * .06f, h * .06f);
                    if (GUI.Button(rplus, tui[20], gstyle) || rplus.Contains(tappos))
                    {
                        tappos = treset;
                        if (_clock.smod < _clock.multipliers.Length - 1)
                        {
                            _clock.smod++;
                        }
                    }
                    if (GUI.Button(rminus, tui[21], gstyle)||rminus.Contains(tappos))
                    {
                        tappos = treset;
                        if (_clock.smod >= 1)
                        {
                            _clock.smod--;
                        }
                    }

                    //draw current speed multiplier
                    Rect rmod = new Rect(w * .19f, h * .88f, 0, 0);
                    GUIStyle mstyle = new GUIStyle(gstyle);
                    mstyle.fontSize = Mathf.RoundToInt(h * .07f);
                    mstyle.normal.textColor = Color.yellow;
                    GUI.Label(rmod, "x" + _clock.multipliers[_clock.smod].ToString(), mstyle);
                    if (!_clock.active  && sleeping)
                    {
                        sleeping = false;
                        jumptograph = .2f;
                    }
                }
            }
            else
            {
                //show charts
                GUIStyle lstyle = new GUIStyle(gstyle);
                lstyle.fontSize = Mathf.RoundToInt(h * .025f);
                lstyle.alignment = TextAnchor.UpperCenter;
                float twidth = w * .826f;
                Rect rtlabel = new Rect(w * .14f, h * .56f, 0, 0);
                Rect rtline = new Rect(w * .14f, h * .55f, 1, h * .01f);
                for (int i = 0; i < 13; i++)
                {
                    GUI.DrawTexture(rtline, white);
                    if (i <= 3)
                    {
                        GUI.Label(rtlabel, (8 + i).ToString() + " PM", lstyle);
                    }
                    else if (i == 4)
                    {
                        GUI.Label(rtlabel, "12 AM", lstyle);
                    }
                    else
                    {
                        GUI.Label(rtlabel, (-4 + i).ToString() + " AM", lstyle);
                    }
                    rtlabel.x += twidth / 12f;
                    rtline.x = rtlabel.x;
                }
                if (caction >= alist.Length)
                {
                    caction = 0;
                }
                if (cvac == -1 && alist.Length > 0)
                {
                    alist = new Action[0];
                }
                //show charts code
                Rect rfull = new Rect(0, 0, w, h);
                GUI.color = _gcolor;
                //GUI.DrawTexture(rfull, white);
                GUI.DrawTexture(rfull, tui[16]);
                GUI.color = Color.white;

                Rect rtrial = new Rect(0, 0, w * .095f, h / 3f); // trial buttons
                for (int i = 0; i < 3; i++)
                {
                    if (curchart != i)
                    {
                        GUI.color = new Color(1, 1, 1, .5f);
                    }
                    if (GUI.Button(rtrial, tui[7 + i], gstyle)||rtrial.Contains(tappos))
                    {
                        tappos = treset;
                        if (i < trials.Length)
                        {
                            pr.trial = trials[i];
                            pr.trial.active = true;
                        }
                        else
                        {
                            pr.trial = null;
                        }
                        if (curchart != i)
                            cvac = -1;
                        curchart = i;
                    }
                    rtrial.y += rtrial.height;
                    GUI.color = Color.white;
                }
                Rect rlabel = new Rect(w * .105f, h * .35f, h * .07f, w * .1f); // quality and time labels
                GUI.DrawTexture(rlabel, tui[13]);
                rlabel = new Rect(w * .13f, h * .55f, w * .1f, h * .07f);
                //GUI.DrawTexture(rlabel, tui[14]);

                GUI.color = Color.green;
                float lw = h * .001f;
                if (lw < 1)
                    lw = 1;
                Rect rline = new Rect(w * .14f, h * .55f, w * .825f, lw); // grid line
                GUI.color = new Color(0, 1, 0, .25f);
                GUI.DrawTexture(rline, white);
                rline.y = h * .02f;

                GUI.DrawTexture(rline, white);
                GUI.color = Color.green;
                rline = new Rect(w * .14f, h * .02f, lw, h * .53f);
                GUI.DrawTexture(rline, white);
                rline.x = w * .965f;
                GUI.DrawTexture(rline, white);
                GUI.color = Color.white;

                Rect rgoback = new Rect(w * .85f, h * .9f, w * .125f, h * .1f); // go back button
                if (GUI.Button(rgoback, tui[10], gstyle)||rgoback.Contains(tappos))
                {
                    tappos = treset;
                    showCharts = false;
                }

                Rect rprint = new Rect(rgoback);
                rprint.height = h * .055f;
                rprint.y -= rprint.height * 2 + h * .03f * 2;
                GUI.DrawTexture(rprint, tui[27]);
                if (GUI.Button(rprint, "", gstyle) || rprint.Contains(tappos))
                {
                    tappos = treset;
                    SaveCharts();
                    pfade = 1;
                }

                Rect plabel = new Rect(rprint);
                plabel.y -= plabel.height;
                GUI.color = new Color(1, 1, 1, pfade*1.5f);
                GUIStyle ntstyle = new GUIStyle(gstyle);
                ntstyle.fontSize = Mathf.RoundToInt(h * .033f);
                ntstyle.alignment = TextAnchor.UpperLeft;
                GUI.Label(plabel, "Print Successful", ntstyle);
                GUI.color = Color.white;

                Rect rbbox = new Rect(0, 0, w, h);
                GUI.color = new Color(1, 1, 1, .6f);
                GUI.DrawTexture(rbbox, tui[15]);
                GUI.color = Color.white;

                //todo: action description
                Rect rdescription = new Rect(w * .2f, h * .75f, 0, 0); // name, timestamp, description
                GUIStyle dstyle = new GUIStyle(gstyle);
                dstyle.fontSize = Mathf.RoundToInt(h * .028f);
                if (caction < alist.Length)
                {
                    GUI.Label(rdescription, "Object: " + alist[caction].iname + "\n\nTime: " + GetTime(alist[caction].timestamp) + "\n\nAction: " + alist[caction].istate, dstyle);
                    Rect racon = new Rect(w * .15f, h * .69f, h * .065f, h * .065f);
                    GUI.DrawTexture(racon, ticons[alist[caction].icon], ScaleMode.ScaleToFit);
                }

                if (alist.Length > 0)
                {
                    Rect ruparrow = new Rect(w * .775f, h * .745f, h * .08f, h * .08f); // ^
                    if (GUI.Button(ruparrow, tui[11], gstyle)||ruparrow.Contains(tappos))
                    {
                        tappos = treset;
                        caction--;
                        if (caction < 0)
                        {
                            caction = 0;
                        }
                        if (alist.Length == 0)
                            caction = 0;
                    }
                    Rect rdnarrow = new Rect(ruparrow.x, ruparrow.y + h * .1f, ruparrow.width, ruparrow.height); // v
                    if (GUI.Button(rdnarrow, tui[12], gstyle)||rdnarrow.Contains(tappos))
                    {
                        tappos = treset;
                        caction++;
                        if (caction >= alist.Length)
                        {
                            caction = alist.Length - 1;
                        }
                    }
                }

                //arrows for changing between action points
                Rect rltarrow = new Rect(rgoback.x, h * .8f, h * .08f, h * .08f);
                Rect rrtarrow = new Rect(w * .85f + h * .1f, rltarrow.y, rltarrow.width, rltarrow.height);
                rrtarrow.x = rgoback.xMax - rrtarrow.width;
                if (GUI.Button(rltarrow, tui[18], gstyle)||rltarrow.Contains(tappos))
                {
                    tappos = treset;
                    if (cvac > 0)
                    {
                        cvac--;
                        caction = 0;
                        alist = pr.vactlist[cvac].actionlist;
                    }
                }
                if (GUI.Button(rrtarrow, tui[19], gstyle)||rrtarrow.Contains(tappos))
                {
                    tappos = treset;
                    if (cvac < pr.vactlist.Length - 1)
                    {
                        cvac++;
                        caction = 0;
                        alist = pr.vactlist[cvac].actionlist;
                    }
                }

                Rect rxofx = new Rect(w * .786f, h * .7f, 0, 0); // x/x
                if (alist.Length > 0)
                {
                    GUI.Label(rxofx, (caction + 1).ToString() + "/" + alist.Length.ToString(), dstyle);
                }

                if (trials.Length <= curchart)
                {
                    GUIStyle nstyle = new GUIStyle(gstyle);
                    nstyle.fontSize = Mathf.RoundToInt(h * .06f);
                    nstyle.normal.textColor = new Color(0, 1, 0, 1);
                    nstyle.alignment = TextAnchor.MiddleCenter;
                    Rect rnodata = new Rect(w * .54f, h * .3f, 0, 0);
                    GUI.Label(rnodata, "No Data", nstyle);
                }
                for (int i = 0; i < pr.vactlist.Length; i++)
                {
                    Rect rbullet = new Rect(0, 0, h * .03f, h * .03f);
                    rbullet.center = new Vector2(pr.vactlist[i].position.x * w, h - pr.vactlist[i].position.y * h);
                    GUI.color = new Color(1, 1, 1, .8f);
                    if (cvac == i)
                    {
                        GUI.color = Color.yellow;
                    }
                    GUI.DrawTexture(rbullet, tui[17]);
                    GUI.color = Color.white;
                    if (GUI.Button(rbullet, "", gstyle)||rbullet.Contains(tappos))
                    {
                        tappos = treset;
                        cvac = i;
                        alist = pr.vactlist[i].actionlist;
                    }
                }
                Rect rgraph = new Rect(w * .141f, h * .02f, w * .85f, h * .53f);
                rgraph.xMin -= w * .025f;
                GUIStyle pstyle = new GUIStyle(gstyle);
                pstyle.fontSize = Mathf.RoundToInt(h * .033f);
                if (rgraph.Contains(mp))
                {
                    for (int i = 0; i < pr.vertices.Length - 1; i++)
                    {
                        if (Mathf.Abs(mp.x - pr.vertices[i].x * w) < Mathf.Abs(mp.x - pr.vertices[i + 1].x * w))
                        {
                            Rect gline = new Rect(pr.vertices[i].x * w, h * .02f, 1, h * .53f);
                            GUI.DrawTexture(gline, white);

                            Rect rpercent = new Rect(mp.x + h * .03f, mp.y + h * .03f, 0, 0);
                            GUI.Label(rpercent, pr.qlist[i].ToString() + "%", pstyle);
                            break;
                        }
                        else if (i == 98)
                        {
                            Rect gline = new Rect(pr.vertices[i + 1].x * w, h * .02f, 1, h * .53f);
                            Rect rpercent = new Rect(mp.x - h * .03f - pstyle.fontSize, mp.y + h * .03f, 0, 0);
                            GUI.Label(rpercent, pr.qlist[i].ToString() + "%", pstyle);
                            GUI.DrawTexture(gline, white);
                        }
                    }
                }
                if (curchart < trials.Length)
                {
                    Rect rdetail = new Rect(w * .141f, h * .6f, 0, 0);
                    GUI.Label(rdetail, "Overall Sleep Quality: " + pr.opercent.ToString() + "%", pstyle);
                    if (trials[curchart].opercent == -1)
                    {
                        trials[curchart].opercent = pr.opercent;
                    }
                    rdetail.x = w * .4f;
                    string tos_string = "Never Slept";
                    if (trials[curchart].timeofsleep.Length > 0)
                    {
                        tos_string = GetTime(trials[curchart].timeofsleep[0]);
                    }
                    GUI.Label(rdetail, "Fell asleep at: " + tos_string, pstyle);

                    rdetail.x = w * .63f;

                    GUI.Label(rdetail, "Effective Sleep Hours: " + GetHours(trials[curchart].ehours) + " Hours", pstyle);
                }
            }
        }
    }//end ongui
}
