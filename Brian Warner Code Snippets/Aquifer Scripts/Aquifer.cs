using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//main script for Aquifer lab

public class Aquifer : MonoBehaviour { 
    float fade; //print message fade value
    public SuperFill emptysf;//empty superfill to feed to Water Trickle script if there are no materials placed 
    public PourWater pw; //water trickle script
    public GameObject wt; //uninstantiated water trickle object to clone off of
    public Fill ef; //fill line to fill water up to, uses the simplified old fill script
    public Vector3 tappos = new Vector3(-1, -1, -1);  //tap position
    public Texture ring; //highlight ring that goes around icons
    public Texture tbleft; //left arrow for help menu
    public Texture tbright; // right arrow for help menu
    public int hpage; //current help page
    string[] htext; //text to display on help page
    public Texture tbutton; //blue box button texture
    public Texture tpop;  //help popup texture
    public Texture thelp; //help button
    string[] stepinstruct; //very brief instructions of current step
    public Material[] tmats; //material for top portion of material layers
    public Material[] fmats; //fill portion of material layers
    public SuperFill sf; //uninstantiated superfill cloned for material layers
    float sptimer= 2; //splash screen timer
    public Texture splash; //splash screen
    public Transform tbot; //bottom of water pull up effect
    public ShowResult SRE; //jar variations script
    float delay = 0; //delay for state 5 after all water effects are done. Used before saving trial data and moving to the final results screen
    public Camera[] cams; //All the cameras (jar camera, and tank camera)
    int c_cam = 0; //index of current camera to show
    int pstate; //previous state, mainly used to check the moment the state was changed, and the state changed from
    public Flow1 f1; //First script of water siphon effect chain
    public Flow5 f5; //Last script of siphon chain. Once started, the first effect is turned off which eventually chains to turning the jar pour particle effect off.
    //public Transform suction1; //
    public Fill water; //the water that fills up the tank
    public SpriteRenderer pump; //renderer of the water pump that goes on the tank
    Rect _raise; //gui rect of the raise and lower buttons, global to evaluate touch in the update function instead of ongui
    Rect _lower; //lower button rect
    int held = 0; //if button is held
    public Renderer sipR; //renderer of siphon tube
    public GameObject siphon; //pivot object of siphon tube
    public int state; //menu state
    public int spraytype = 0; //current material spray
    public bool spraying; //if spraying or not
    string[] stypes = new string[5]; //names of spray types
    public Texture[] tbuttons = new Texture[4]; //spray button textures
    public Texture flat; //white empty texture
    int w = Screen.width; 
    int h = Screen.height;
    Vector3 mp = Vector3.zero; //mouse position
    public Mlayer[] layers = new Mlayer[4]; //the layers of material, each containing a superfill
    float pollution = 7; //base pollution value of water
    float sandy = 0; //sandiness of water (from sand)
    float cloudy = 0; //cloudiness of water (from clay)
    float muddy = 0; //muddiness of water (from soil)
    float porous = 0; //waterflow that controls if jar is half full or full
    bool reaches = true; //does tube reach water level
    int tlayer = -1; //layer tube is in
    public string[] trialdata; //trial data to be printed to html
    int n = 0; //current trial

    [System.Serializable]
    public class Mlayer //material layer data
    {
        public int type;
        public Fill fill;
        public SuperFill sf;
        public float amount;
    }//end mlayer

    void SaveData() //save trial data to string array
    {
        float fp = pollution;
        if (fp < 0)
            fp = 0;
        float sp = sandy;
        if (sp < 0)
            sp = 0;
        float cp = cloudy;
        if (cp < 0)
            cp = 0;
        float mp = muddy;
        if (mp < 0)
            mp = 0;
        float pcent = (((7f - (fp)) / 7f) * 100f);
        if (pcent > 100)
            pcent = 100;
        if (pcent < 0)
            pcent = 0;
        if (n >= 3)
        {
            n = 2;
            trialdata[0] = trialdata[1];
            trialdata[1] = trialdata[2];
        }
        
        trialdata[n] += "Trial #" + (n+1).ToString();
        trialdata[n] += "\n-----";
        trialdata[n] += "\nMaterials:";
        if (layers.Length > 0 && tlayer>=0)
        {
            for (int i = 0; i < layers.Length; i++)
            {
                trialdata[n] += "\n" + stypes[layers[i].type] + " - " + ((layers[i].amount / 4.2f) * 100).ToString("F0") + "%";
            }
            trialdata[n] += "\nTube Layer - " + stypes[layers[tlayer].type];
        }
        else
        {
            trialdata[n] += "\nNone";
            trialdata[n] += "\nTube Layer - None";
        }
            
        trialdata[n] += "\nResults:";
        if (!reaches)
        {
            trialdata[n] += "\nWell doesn't reach aquifer.";
            trialdata[n] += "\nWater is polluted.";
            if (!SRE.toggles[0])
            {
                trialdata[n] += "\nWell doesn't reach water.";
            }
            else
            {
                if (!SRE.waterfull)
                {
                    trialdata[n] += "\nThe waterflow is too weak to pump a full jar of water.";
                }
                else
                {
                    trialdata[n] += "\nThe jar is brimming with water!";
                }
            }
        }
        else
        {
            bool clean = false;
            //dstring = "The pump is in the " + stypes[layers[tlayer].type] + " layer.";
            if (porous > 0)
                trialdata[n] += "\nWaterflow: " + porous.ToString("F1");
            else
                trialdata[n] += "\nWaterflow: " + porous.ToString("F1");
            if (!SRE.waterfull)
            {
                trialdata[n] += "\nThe waterflow is too weak to pump a full jar of water.";
            }
            else
            {
                trialdata[n] += "\nThe jar is brimming with water!";
            }
            if (sandy > 0)
                trialdata[n] += "\nSandy: " + (sp * 100).ToString("F0") + " ppm";
            if (cloudy > 0)
                trialdata[n] += "\nCloudy: " + (cp*100).ToString("F0") + " ppm";
            if (muddy > 0)
                trialdata[n] += "\nMuddy: " + (mp*100).ToString("F0") + " ppm";
            if (pollution > 0)
                trialdata[n] += "\nInitial Pollutants: " + (fp*100).ToString("F0") + " ppm";

            trialdata[n] += "\nTDS is " + ((fp + cp + sp + mp) * 100).ToString("F0") + " ppm. Initial pollutants " + pcent.ToString("F0") + "% purified.";

            if (sandy <= 0 && cloudy <= 0 && muddy <= 0 && pollution <= 0)
            {
                trialdata[n] += "\n\nThe water is clean!";
                clean = true;
            }
            if (clean && SRE.waterfull)
            {
                trialdata[n] += "\n\nCongratulations, you have succeeded in pumping a full jar of clean water!";
            }
        }
        n++;
    }//end savedata

    void PrintData() //print the saved trial data
    {
        string ultimate = "=====";

        for (int i = 0; i < trialdata.Length; i++)
        {
            if (trialdata[i] == "")
                break;
            ultimate += "\n";
            ultimate += trialdata[i];
            ultimate += "\n=====";
        }
        Application.ExternalCall("SaveChart", ultimate);
    }//end printdata

    void Start () {
        for (int i = 0; i < 3; i++)
        {
            trialdata[i] = "";
        }
        //spray type names
        stypes[0] = "none";
        stypes[1] = "Soil";
        stypes[2] = "Gravel";
        stypes[3] = "Sand";
        stypes[4] = "Clay";

        //state instructions
        stepinstruct = new string[4];
        stepinstruct[0] = "To build your Aquifer, choose materials and hold over the tank to fill it.";
        stepinstruct[1] = "Set the height of your tube so it reaches the aquifer in the desired layer.";
        stepinstruct[2] = "Set the speed of your well pump.";
        stepinstruct[3] = "Set how high you wish to fill the tank with water.";

        //help menu text
        htext = new string[10];
        htext[0] = "Welcome to Build-A-Quifer! Your goal is to build the perfect aquifer and pump an abundant amount of clean drinking water from it. A small variety of natural materials will be available to you to help filter the polluted water that will be poured into the tank.";
        htext[1] = "TDS stands for Total Dissolved Solids, which can include anything from potassium and sodium, to toxic materials such as lead and arsenic. TDS is measured in Parts Per Million (PPM). The lower the PPM, the more likely it is safe to drink. Your water should reach a TDS of between 0-50 PPM, which is standard drinking water.";
        htext[2] = "To accomplish this, we begin by filling the tank with materials that will filter out pollutants from the water as it seeps down. Choose a material and hold click over the tank to spray. Note for each new material you add, the previous material cannot be added again until you clear the tank. What's most important is that the material is placed around the water tube area. Let's go over each material and their pros and cons.";
        htext[3] = "Sand is great for removing pollutants out of the water. However, it will fill the water with grainy sand the more you add in. Although not great for waterflow, it will choke the water less than clay or soil.";
        htext[4] = "Clay will filter out the sand and soil from your water, but will make the water cloudy with clay. Be careful with clay, as it will drastically reduce waterflow the more you add compared to other materials.";
        htext[5] = "Gravel is permeable and is a great material for improving waterflow. It will also help filter out cloudiness from clay.";
        htext[6] = "Soil is not the worst material for waterflow, but it will certainly not improve it. Soil will add muddiness to your water. However it is a common upper layer for aquifers near farms.";
        htext[7] = "Once you've added your materials, lower the water tube so it reaches the desired layer of the aquifer. Note that it will only grab water from the layers at and above the bottom of the tube.";
        htext[8] = "Next set the desired water level of your tank. The tank will be filled with water up to this point. Make sure the water level is higher than the bottom of the tube.";
        htext[9] = "The tank will then be filled with polluted water. Set it up right, and you will recieve a full jar of clean potable water. When you are successful, hit the Print button to create a log of your results in the Copy to Clipboard box beneath the web player. Good luck!";
    }//end start

    //removes the last material placed
    void Undo()
    {
        if (layers == null)
            return;
        if (layers.Length <= 0)
        {
            return;
        }

        Destroy(layers[layers.Length - 1].sf.gameObject);
        Mlayer[] temp = new Mlayer[layers.Length - 1];
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = layers[i];
        }
        layers = temp;
    } //end undo

    //builds results based on tube placement, water level, and most importantly material amounts
    void Evaluate()
    {
        for (int i = 0; i<SRE.toggles.Length; i++)
        {
            SRE.toggles[i] = false;
            SRE.waterfull = true;
        }
        float[] ytops = new float[layers.Length];
        for(int i = 0; i<layers.Length; i++)
        {
            ytops[i] = layers[i].sf.newVertices[69].y + layers[i].sf.transform.position.y;
        }
        float test = tbot.transform.position.y;
        tlayer = -1;
        for(int i = layers.Length-1; i>=0; i--)
        {
            if (test < ytops[i])
            {
                tlayer = i;
            }
        }

        if(tlayer == -1)
        {
            reaches = false;
            SRE.waterfull = true;
            if (test <= -0.26f)
            {
                SRE.toggles[0] = true;
                SRE.toggles[1] = true;
            }
            return;
        }
        else
        {
            reaches = true;

            pollution = 7;
            sandy = 0;
            cloudy = 0;
            muddy = 0;
            porous = 0;
            

            for(int i = layers.Length-1; i>=tlayer; i--)
            {
                
                //Sand
                if (layers[i].type == 3)
                {
                    float amt = layers[i].amount;
                    pollution -= 10 * amt;
                    muddy -= 10 * amt;
                    sandy += 1 * amt*10;
                    porous -= 10*amt;
                }
                //Clay
                if(layers[i].type == 4)
                {
                    float amt = layers[i].amount;
                    while (amt > 0)
                    {
                        amt -= .1f;
                        if (amt > 0)
                        {
                            muddy--;
                            sandy-=1.25f;
                            cloudy += 1 ;
                            porous -= 4;
                        }
                    }
                }
                //Gravel
                if(layers[i].type == 2)
                {
                    float amt = layers[i].amount;
                    while (amt > 0)
                    {
                        amt -= .1f;
                        if (amt > 0)
                        {
                            cloudy--;
                            porous += 7.25f;
                        }
                    }
                }
                //Soil
                if (layers[i].type == 1)
                {
                    float amt = layers[i].amount;
                    while (amt > 0)
                    {
                        amt -= .1f;
                        if (amt > 0)
                        {
                            muddy += 1;
                            porous -= 3;
                        }
                    }
                }
            }

            SRE.waterfull = porous >= 0;

            if(pollution>0.5f)
            {
                SRE.toggles[1] = true;
            }
            if (sandy > 0)
                SRE.toggles[2] = true;
            if (cloudy > 0)
                SRE.toggles[3] = true;
            if (muddy > 0)
                SRE.toggles[4] = true;

            SRE.toggles[0] = true;
        }
    }//end evaluate

    void Update() {
        if (fade > 0)
        {
            fade -= Time.deltaTime*.75f;
        } //fade print successful message
        tappos = new Vector3(-1, -1, -1);//initialize tap position
        Vector3 tp = new Vector3(0, 0, 0);//initialize touch position
        if (Input.touchCount>0)//check for touch
        {
            
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (Input.touches[i].phase == TouchPhase.Ended)
                {
                    tappos = Input.touches[i].position;
                }
                tp = Input.touches[i].position;
            }
        }

        if (state == 6)
        {
            if (pw != null) //remove trickle effect upon entering results screen
            {
                Destroy(pw.tri.gameObject);
                Destroy(pw.gameObject);
            }
        }

        spraying = false; //reinitialize spraying bool
        Rect areatank = new Rect(w * .015f, h * .3f, w * .725f, h * .675f); //tank clickable area

        
        if(state!= pstate && state == 0)
        {
            f5.empty = true;
        }//reinitialize last flow effect upon entering first state

        if (state == -1)//splash state
        {
            if (sptimer > 0)
            {
                sptimer -= Time.deltaTime;
            }
            else
                state = 0;

        }

        if (state == 5)//delay before evaluating trial data and moving to final results screen
        {
            
            delay -= Time.deltaTime;
            if (delay <= 0)
            {
                Evaluate();
                SaveData();
                state = 6;
            }
        }
        if (state == 6) //go to jar camera while on results state
        {
            c_cam = 1;
        }
        else
            c_cam = 0;

        if(state == 3) //fill tank with water
        {
            if (pw == null)
            {
                GameObject go = (GameObject)Instantiate(wt, wt.transform.position, wt.transform.rotation);
                go.SetActive(true);
                pw = go.GetComponent<PourWater>();
                if (layers.Length >= 1)
                {
                    pw.sf = layers[layers.Length - 1].sf;
                }
                else
                {
                    pw.sf = emptysf;
                }
            }
            if(water.type!= 5)
                water.type = 5;
            if (pw.tri != null)
            {
                if (pw.tri.stop)
                {
                    if (water.linetop < ef.linetop)
                    {
                        water.linetop += Time.deltaTime * 1.25f;
                        if (water.linetop > ef.linetop)
                            water.linetop = ef.linetop;
                    }
                }
            }
        }

        if (state == 7) //raise/lower water level
        {
            if (Input.GetMouseButton(0))
            {
                if (_lower.Contains(mp))
                {
                    held = 3;
                }
                if (_raise.Contains(mp))
                {
                    held = 4;
                }
            }
            if (tp != Vector3.zero)
            {
                if (_lower.Contains(tp))
                {
                    held = 3;
                }
                if (_raise.Contains(tp))
                {
                    held = 4;
                }
            }
        }

        if (state == 1)//fade pump while raising/lowering tube
        {

            pump.color = new Color(1, 1, 1, .25f);
            if (Input.GetMouseButtonDown(0))
            {
                if (_lower.Contains(mp))
                {
                    held = 1;
                }
                if (_raise.Contains(mp))
                {
                    held = 2;
                }
            }
            if (tp != Vector3.zero)
            {
                if (_lower.Contains(tp))
                {
                    held = 1;
                }
                if (_raise.Contains(tp))
                {
                    held = 2;
                }
            }
            if (!sipR.enabled)
                sipR.enabled = true;
        }
        else
        {
            pump.color = Color.white;
        }

        if(held == 1)//raise/lower pump
        {
            if (!Input.GetMouseButton(0)&&tp == Vector3.zero)
                held = 0;
            else
            {

                Vector3 scale = siphon.transform.localScale;
                scale.y += Time.deltaTime*1.5f;
                if (scale.y > 8f)
                    scale.y = 8f;
                siphon.transform.localScale = scale;
            }
        }
        if(held == 2)
        {
            if (!Input.GetMouseButton(0)&&tp==Vector3.zero)
                held = 0;
            else
            {
                Vector3 scale = siphon.transform.localScale;
                scale.y -= Time.deltaTime * 1.5f;
                if (scale.y < 0)
                    scale.y = 0;
                siphon.transform.localScale = scale;
            }
        }
        if (held == 3)//raise/lower fill level
        {
            if (!Input.GetMouseButton(0) && tp == Vector3.zero)
                held = 0;
            else
            {
                ef.linetop -= Time.deltaTime;
                held = 0;
            }
        }
        if (held == 4)
        {
            if (!Input.GetMouseButton(0) && tp == Vector3.zero)
                held = 0;
            else
            {
                ef.linetop += Time.deltaTime;
                held = 0;
            }
        }
        if ((Input.GetMouseButton(0)||tp!=Vector3.zero) && state == 0)//spray material if current material either hasn't been placed or is the most recently placed
        {
            if((areatank.Contains(mp)||areatank.Contains(tp)) && spraytype > 0)
            {
                //USE SUPER FILL
                spraying = true;
                if(layers.Length == 0)
                {
                    layers = new Mlayer[1];
                    layers[0] = new Mlayer();
                    layers[0].type = spraytype;
                    GameObject go = (GameObject)Instantiate(sf.gameObject,sf.transform.position,sf.transform.rotation);
                    go.SetActive(true);
                    layers[0].sf = go.GetComponent<SuperFill>();
                    layers[0].sf.aq = this;
                    Material[] mt = new Material[2];
                    mt[0] = fmats[spraytype - 1];
                    mt[1] = tmats[spraytype - 1];
                    layers[0].sf.GetComponent<Renderer>().materials = mt;
                    layers[0].sf.name = 0.ToString();
                }
                else
                {
                    bool has = false;
                    for (int i = 0; i < layers.Length; i++)
                    {
                        if (layers[i].type == spraytype)
                        {
                            has = true;
                        }
                    }
                    if (has)
                    {
                        if (layers[layers.Length - 1].type == spraytype)
                        {
                            layers[layers.Length - 1].sf.getClosestPoint();
                            
                            if(layers.Length>1)
                            {
                                layers[layers.Length - 1].amount = layers[layers.Length - 1].sf.newVertices[69].y - layers[layers.Length - 2].sf.newVertices[69].y;
                            }
                            else
                            {
                                layers[layers.Length - 1].amount = layers[layers.Length - 1].sf.newVertices[69].y;
                            }
                        }
                    }
                    else
                    {
                        Mlayer[] temp = new Mlayer[layers.Length + 1];
                        for (int i = 0; i < layers.Length; i++)
                        {
                            temp[i] = layers[i];
                        }
                        temp[temp.Length - 1] = new Mlayer();
                        temp[temp.Length - 1].type = spraytype;
                        GameObject go = (GameObject)Instantiate(sf.gameObject, sf.transform.position, sf.transform.rotation);
                        go.SetActive(true);
                        temp[temp.Length - 1].sf = go.GetComponent<SuperFill>();
                        temp[temp.Length - 1].sf.aq = this;
                        Material[] mt = new Material[2];
                        mt[0] = fmats[spraytype - 1];
                        mt[1] = tmats[spraytype - 1];
                        temp[temp.Length - 1].sf.GetComponent<Renderer>().materials = mt;
                        temp[temp.Length - 1].sf.newVertices = temp[temp.Length - 2].sf.newVertices;
                        temp[temp.Length - 1].sf.ps = temp[temp.Length - 2].sf;
                        temp[temp.Length - 1].sf.name = (temp.Length - 1).ToString();
                        temp[temp.Length - 1].sf.transform.position = new Vector3(temp[temp.Length - 1].sf.transform.position.x, temp[temp.Length - 1].sf.transform.position.y, temp[temp.Length - 1].sf.ps.transform.position.z+.0001f);
                        layers = temp;

                    }
                }
            }
        }
        for(int i = 0; i<cams.Length; i++)//set active camera
        {
            if (i == c_cam)
                cams[i].enabled = true;
            else
                cams[i].enabled = false;
        }

        pstate = state;//reset pstate
	}

    private void OnGUI()
    {
        w = Screen.width;
        h = Screen.height;
        Vector3 tp = tappos;
        tp.y = h - tp.y;
        mp = Input.mousePosition;
        mp.y = h - mp.y;
        GUIStyle gstyle = new GUIStyle();
        gstyle.fontSize = Mathf.RoundToInt(h * .035f);
        gstyle.normal.textColor = Color.white;

        GUI.skin.button.fontSize = gstyle.fontSize;
        GUI.skin.box.fontSize = gstyle.fontSize;
        GUI.skin.box.alignment = TextAnchor.UpperLeft;

        GUIStyle cstyle = new GUIStyle(gstyle);
        cstyle.alignment = TextAnchor.MiddleCenter;

        if (state == -2)
        {
            //help state
            Rect rpop = new Rect(w * .2f, h * .2f, w * .6f, h * .6f);
            GUI.DrawTexture(rpop, tpop);

            GUIStyle hstyle = new GUIStyle(gstyle);
            hstyle.wordWrap = true;
            hstyle.normal.textColor = Color.black;

            Rect rtext = new Rect(rpop.x + rpop.width * .05f, rpop.y + rpop.height * .1f, rpop.width - rpop.width * .1f, rpop.height);
            GUI.Label(rtext, htext[hpage], hstyle);

            Rect rbleft = new Rect(rpop.x + rpop.width * .05f, rpop.y + rpop.height * .75f, h * .1f, h * .1f);
            if (hpage > 0)
            {
                if (GUI.Button(rbleft, tbleft, gstyle) || rbleft.Contains(tp))
                {
                    hpage--;
                    if (hpage < 0)
                        hpage = 0;

                    tp = Vector3.zero;
                    tappos = Vector3.zero;
                }
            }

            Rect rbright = new Rect(rpop.x + rpop.width - rpop.width * .05f - h * .1f, rpop.y + rpop.height * .75f, h * .1f, h * .1f);
            if (hpage < htext.Length - 1)
            {
                if (GUI.Button(rbright, tbright, gstyle) || rbright.Contains(tp))
                {
                    hpage++;
                    if (hpage >= htext.Length)
                        hpage = htext.Length - 1;

                    tp = Vector3.zero;
                    tappos = Vector3.zero;
                }
            }

            Rect tclose = new Rect(rpop.x + rpop.width * .3f, rpop.y + rpop.height * .75f, rpop.width * .4f, h * .1f);
            GUI.DrawTexture(tclose, tbutton);

            cstyle.normal.textColor = Color.black;
            if (GUI.Button(tclose, "Close", cstyle) || tclose.Contains(tp))
            {
                state = 0;
                tp = Vector3.zero;
                tappos = Vector3.zero;
            }
            cstyle.normal.textColor = Color.white;
        }
        else if (state == -1) 
        {
            //splash state
            GUI.color = new Color(1, 1, 1, sptimer * 2);
            Rect full = new Rect(0, 0, w, h);
            GUI.DrawTexture(full, splash);
        }
        else if (state == 0)
        {
            //spray state
            Rect mbutton = new Rect(w * .8f, h * .25f, w * .08f, w * .08f);
            GUI.DrawTexture(mbutton, tbuttons[0], ScaleMode.ScaleToFit);
            if (spraytype == 1)
            {
                GUI.DrawTexture(mbutton, ring, ScaleMode.ScaleToFit);
            }
            if (GUI.Button(mbutton, "", gstyle) || mbutton.Contains(tp))
            {
                spraytype = 1;
                tp = Vector3.zero;
                tappos = Vector3.zero;
            }
            mbutton = new Rect(w * .9f, h * .25f, w * .08f, w * .08f);
            GUI.DrawTexture(mbutton, tbuttons[1], ScaleMode.ScaleToFit);
            if (GUI.Button(mbutton, "", gstyle) || mbutton.Contains(tp))
            {
                spraytype = 2;
                tp = Vector3.zero;
                tappos = Vector3.zero;
            }
            if (spraytype == 2)
            {
                GUI.DrawTexture(mbutton, ring, ScaleMode.ScaleToFit);
            }
            mbutton = new Rect(w * .8f, h * .45f, w * .08f, w * .08f);
            GUI.DrawTexture(mbutton, tbuttons[2], ScaleMode.ScaleToFit);
            if (GUI.Button(mbutton, "", gstyle) || mbutton.Contains(tp))
            {
                spraytype = 3;
                tp = Vector3.zero;
                tappos = Vector3.zero;
            }
            if (spraytype == 3)
            {
                GUI.DrawTexture(mbutton, ring, ScaleMode.ScaleToFit);
            }
            mbutton = new Rect(w * .9f, h * .45f, w * .08f, w * .08f);
            GUI.DrawTexture(mbutton, tbuttons[3], ScaleMode.ScaleToFit);
            if (GUI.Button(mbutton, "", gstyle) || mbutton.Contains(tp))
            {
                spraytype = 4;
                tp = Vector3.zero;
                tappos = Vector3.zero;
            }
            if (spraytype == 4)
            {
                GUI.DrawTexture(mbutton, ring, ScaleMode.ScaleToFit);
            }

            Rect rspray = new Rect(h * .01f, h * .01f, w * .2f, h * .1f);
            string amt = "";
            if (layers.Length > 0)
            {
                for (int i = 0; i < layers.Length; i++)
                {
                    if (layers[i].type == spraytype)
                        amt = "Amount: " + ((layers[i].amount * 1)*100f).ToString("F0") + " ppm";
                }
            }
            GUI.Box(rspray, "Spray Type: " + stypes[spraytype] + "\n" + amt);

            Rect rclear = new Rect(w * .825f, h * .65F, w * .14F, w * .04F);
            if (GUI.Button(rclear, "Clear") || rclear.Contains(tp))
            {
                Clear();
                tp = Vector3.zero;
                tappos = Vector3.zero;
            }

            rclear.y += rclear.height * 1.25f;
            if (GUI.Button(rclear, "Continue") || rclear.Contains(tp))
            {
                state = 1;
                tp = Vector3.zero;
                tappos = Vector3.zero;
            }
            rclear.y += rclear.height * 1.25f;
            if (GUI.Button(rclear, "Undo") || rclear.Contains(tp))
            {
                tp = Vector3.zero;
                tappos = Vector3.zero;
                Undo();
            }


            Rect ristruct = new Rect(w * .5f, h * .95f, 0, 0);
            Rect rpop = new Rect(w * .2f, h * .915f, w * .6f, h * .07f);
            GUI.color = new Color(1, 1, 1, .5f);
            GUI.DrawTexture(rpop, tpop);
            GUI.color = Color.black;
            GUI.Label(ristruct, stepinstruct[0], cstyle);
            GUI.color = Color.white;

            Rect rhelp = new Rect(w * .9f, h * .05f, w * .08f, w * .08f);
            if (GUI.Button(rhelp, thelp, gstyle) || rhelp.Contains(tp))
            {
                state = -2;
                tp = Vector3.zero;
                tappos = Vector3.zero;
            }
        }
        else if (state == 1)
        {
            //tube raise/lower state
            Rect rclear = new Rect(w * .825f, h * .65F, w * .14F, w * .04F);
            _lower = new Rect(rclear);
            if (GUI.Button(rclear, "Lower"))
            {
            }
            Rect rup = new Rect(rclear);
            rup.y -= rclear.height * 1.25f;
            _raise = new Rect(rup);
            if (GUI.Button(rup, "Raise"))
            {
            }
            rclear.y += rclear.height * 1.25f;
            if (GUI.Button(rclear, "Continue") || rclear.Contains(tp))
            {
                state = 7;
                tp = Vector3.zero;
                tappos = Vector3.zero;
            }
            rclear.y += rclear.height * 1.25f;
            if (GUI.Button(rclear, "Go Back") || rclear.Contains(tp))
            {
                state = 0;
                tp = Vector3.zero;
                tappos = Vector3.zero;
            }

            Rect ristruct = new Rect(w * .5f, h * .95f, 0, 0);
            Rect rpop = new Rect(w * .19f, h * .915f, w * .62f, h * .07f);
            GUI.color = new Color(1, 1, 1, .5f);
            GUI.DrawTexture(rpop, tpop);
            GUI.color = Color.black;
            GUI.Label(ristruct, stepinstruct[1], cstyle);
            GUI.color = Color.white;
        }
        //state 2 was removed (pump speed)
        else if (state == 7)
        {
            //water level state
            Rect rclear = new Rect(w * .825f, h * .65F, w * .14F, w * .04F);
            _lower = new Rect(rclear);
            if (GUI.Button(rclear, "Lower"))
            {
            }
            Rect rup = new Rect(rclear);
            rup.y -= rclear.height * 1.25f;
            _raise = new Rect(rup);
            if (GUI.Button(rup, "Raise"))
            {
            }

            Rect rcontinue = new Rect(w * .825f, h * .75F, w * .14F, w * .04F);
            rcontinue.y = _lower.y + _lower.height + rcontinue.height * .2f;
            if (GUI.Button(rcontinue, "Start") || rcontinue.Contains(tp))
            {
                tp = Vector3.zero;
                tappos = Vector3.zero;
                state = 3;
            }

            rcontinue.y += rcontinue.height * 1.25f;
            if (GUI.Button(rcontinue, "Go Back") || rcontinue.Contains(tp))
            {
                tp = Vector3.zero;
                tappos = Vector3.zero;
                state = 1;
            }
            Rect ristruct = new Rect(w * .5f, h * .95f, 0, 0);
            Rect rpop = new Rect(w * .285f, h * .915f, w * .43f, h * .07f);
            GUI.color = new Color(1, 1, 1, .5f);
            GUI.DrawTexture(rpop, tpop);
            GUI.color = Color.black;
            GUI.Label(ristruct, stepinstruct[3], cstyle);
            GUI.color = Color.white;
        }
        else if (state == 3)
        {
            //water filling state
            if (water.linetop >= ef.linetop)
            {
                Rect ract = new Rect(w * .8f, h * .5f, w * .15f, h * .15f);
                if (GUI.Button(ract, "Activate Water\nPump") || ract.Contains(tp))
                {
                    tp = Vector3.zero;
                    tappos = Vector3.zero;
                    state = 4;
                    f1.flow = true;
                }
            }
        }
        else if (state == 4)
        {
            //siphon state
            if (f5.full)
            {
                state = 5;
                delay = 1;
            }
        }
        else if (state == 5)
        {
            //end of water effect delay state
        }
        else if (state == 6)
        {
            //final results state
            GUIStyle mstyle = new GUIStyle(gstyle);
            mstyle.alignment = TextAnchor.LowerLeft;
            Rect rdesc = new Rect(w * .1f, h * .85f, 0, 0);
            string dstring = "";

            Rect rbdrop = new Rect(w*.08f,h*.1f,w*.6f,h*.76f);
            GUI.color = new Color(0, 0f, 0f, .5f);
            GUI.DrawTexture(rbdrop, flat);
            GUI.color = Color.white;
            if (!reaches)
            {
                dstring = "<color=#ff0000ff>Well doesn't reach aquifer.</color>";
                dstring += "\n\n<color=#44aa00ff>Water is polluted.</color>";
                if (!SRE.toggles[0])
                {
                    dstring = "<color=#ff0000ff>Well doesn't reach water.</color>";
                }
                else
                {
                    if (!SRE.waterfull)
                    {
                        dstring += "\n\n<color=#88CCCCff>The waterflow is too weak to pump a full jar of water.</color>";
                    }
                    else
                    {
                        dstring += "\n\n<color=#00ffffff>The jar is brimming with water!</color>";
                    }
                }
            }
            else //Tube is beneath water and inside a layer of material
            {
                // IMPORTANT CODE HERE //
                bool clean = false;
                //dstring = "The pump is in the " + stypes[layers[tlayer].type] + " layer.";
                if (porous > 0)
                    dstring = "<color=#00ffffff>Waterflow: " + porous.ToString("F1") + "</color>";
                else
                    dstring = "<color=#0088ffff>Waterflow: " + porous.ToString("F1") + "</color>";
                if (!SRE.waterfull)
                {
                    dstring += "\n\n<color=#88CCCCff>The waterflow is too weak to pump a full jar of water.</color>";
                }
                else
                {
                    dstring += "\n\n<color=#00ffffff>The jar is brimming with water!</color>";
                }
                if (sandy > 0)
                    dstring += "\n\n<color=#FCF5BEff>Sandy: " + (sandy*100).ToString("F0") + " ppm</color>";
                if (cloudy > 0)
                    dstring += "\n\n<color=#ff5200ff>Cloudy: " + (cloudy * 100).ToString("F0") + " ppm</color>";
                if (muddy > 0)
                    dstring += "\n\n<color=#654A01ff>Muddy: " + (muddy * 100).ToString("F0") + " ppm</color>";
                float fp = pollution;
                if (fp < 0)
                    fp = 0;
                float sp = sandy;
                if (sp < 0)
                    sp = 0;
                float cp = cloudy;
                if (cp < 0)
                    cp = 0;
                float mp = muddy;
                if (mp < 0)
                    mp = 0;
                float pcent = (((7f - (fp)) / 7f) * 100f);
                if (pcent > 100)
                    pcent = 100;
                if (pcent < 0)
                    pcent = 0;
                dstring += "\n\n<color=#44ff00ff>Initial Pollutants: " + (fp * 100f).ToString("F0") + " ppm</color>";
                dstring += "\n\n<color=#ffDDDDff>TDS is " + ((fp+sp+cp+mp)*100f).ToString("F0") + " ppm. Initial pollutants " + pcent.ToString("F0") + "% purified.</color>";
                if (sandy <= 0 && cloudy <= 0 && muddy <= 0 && pollution <= .5f)
                {
                    dstring += "\n\nThe water is clean! (0-50 PPM)";
                    clean = true;
                }
                if (clean && SRE.waterfull)
                {
                    dstring += "\n\nCongratulations, you have succeeded in pumping a full jar of clean water!";
                }
            }
            mstyle.richText = true;
            GUI.Label(rdesc, dstring, mstyle);

            Rect rstart = new Rect(w * .5f - w * .125f * .5f, h * .875f, w * .125f, h * .1f);
            rstart.x = w*.5f - rstart.width*1.1f;
            if (GUI.Button(rstart, "Reset") || rstart.Contains(tp))
            {
                tp = Vector3.zero;
                tappos = Vector3.zero;
                state = 0;
                water.linetop = 0;
                water.type = 0;
            }

            rstart.x = w * .5f + rstart.width * .1f;

            if (GUI.Button(rstart, "Print") || rstart.Contains(tp))
            {
                tp = Vector3.zero;
                tappos = Vector3.zero;
                PrintData();
                fade = 1;
            }

            rstart.x = rstart.xMax + rstart.width * .1f;
            GUI.color = new Color(1, 1, 1, fade);
            GUI.Label(rstart, "Print Successful", gstyle);
            GUI.color = Color.white;
        }
    }

    //clear all placed materials
    void Clear()
    {
        for (int i = 0; i < layers.Length; i++)
        {
            if(layers[i].fill!=null)
                Destroy(layers[i].fill.gameObject);
            if(layers[i].sf!=null)
                Destroy(layers[i].sf.gameObject);
        }
        layers = new Mlayer[0];
    }
}
