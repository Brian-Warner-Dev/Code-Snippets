using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//uses post rendering to draw the results graph using gl.lines
//saved user actions appear as nodes on the graph

public class PostRender : MonoBehaviour {
    public SleepLab sl;
    public ParticleSystem ps;
    public Material mat;
    public float xmin = .14f;
    float xmax = .965f;
    float ymin = .451f;
    float ymax = .978f;
    float fw = 0;
    float fh = 0;
    public int segments;
    public float speed = 144;
    public SleepLab.Trial trial;
    public int[] sp = new int[0];
    public VertexActions[] vactlist = new VertexActions[0];
    public float[] qlist = new float[0];
    public Vector3[] vertices = new Vector3[0];
    public float overall;
    public float opercent;

    [System.Serializable]
    public class VertexActions
    {
        public SleepLab.Action[] actionlist;
        public Vector2 position;
        public void Add(SleepLab.Action action)
        {
            SleepLab.Action[] t = new SleepLab.Action[actionlist.Length + 1];
            for (int i = 0; i < actionlist.Length; i++)
            {
                t[i] = actionlist[i];
            }
            t[t.Length - 1] = action;
            actionlist = t;
        }
    }

    public class Gstate
    {
        public bool[] beanbags = new bool[2];
        public bool teddybear;
        public int bedcolor;
        public bool[] boats = new bool[2];
        public bool[] books = new bool[2];
        public bool[] cars = new bool[2];
        public bool chair;
        public bool drum;
        public bool[] elephants = new bool[2];
        public int fan;
        public bool giraffe;
        public bool guitar;
        public bool lamp1;
        public bool lamp2;
        public bool lamp3;
        public bool laptop;
        public bool[] makeup = new bool[2];
        public bool airplane;
        public bool plant;
        public bool stereo1;
        public bool stereo2;
        public bool tv;
        public int wallcolor;
        public bool poster;

        public void ApplyAction(SleepLab.Action sla)
        {
            if (sla.iname == "Bean Bag 1")
            {
                beanbags[0] = sla.istate == "Toggled on";
            }
            if (sla.iname == "Bean Bag 2")
            {
                beanbags[1] = sla.istate == "Toggled on";
            }
            if (sla.iname == "Teddy Bear")
            {
                teddybear = sla.istate == "Toggled on";
            }
            if (sla.iname == "Bed")
            {
                bedcolor = sla.cnumber;
            }
            if (sla.iname == "Boat 1")
            {
                boats[0] = sla.istate == "Toggled on";
            }
            if (sla.iname == "Boat 2")
            {
                boats[1] = sla.istate == "Toggled on";
            }
            if (sla.iname == "Books 1")
            {
                books[0] = sla.istate == "Toggled on"; 
            }
            if (sla.iname == "Books 2")
            {
                books[1] = sla.istate == "Toggled on";
            }
            if (sla.iname == "Car 1")
            {
                cars[0] = sla.istate == "Toggled on";
            }
            if (sla.iname == "Car 2")
            {
                cars[1] = sla.istate == "Toggled on";
            }
            if (sla.iname == "Chair")
            {
                chair = sla.istate == "Toggled on";
            }
            if (sla.iname == "Drum")
            {
                drum = sla.istate == "Toggled on";
            }
            if (sla.iname == "Elephant 1")
            {
                elephants[0] = sla.istate == "Toggled on";
            }
            if (sla.iname == "Elephant 2")
            {
                elephants[1] = sla.istate == "Toggled on";
            }
            if (sla.iname == "Fan")
            {
                fan = sla.cnumber;
            }
            if (sla.iname == "Giraffe")
            {
                giraffe = sla.istate == "Toggled on";
            }
            if (sla.iname == "Guitar")
            {
                guitar = sla.istate == "Toggled on";
            }
            if (sla.iname == "Lamp 1")
            {
                lamp1 = sla.istate == "Toggled on";
            }
            if (sla.iname == "Lamp 2")
            {
                lamp2 = sla.istate == "Toggled on";
            }
            if (sla.iname == "Lamp 3")
            {
                lamp3 = sla.istate == "Toggled on";
            }
            if (sla.iname == "Laptop")
            {
                laptop = sla.istate == "Toggled on";
            }
            if (sla.iname == "Makeup 1")
            {
                makeup[0] = sla.istate == "Toggled on";
            }
            if (sla.iname == "Makeup 2")
            {
                makeup[1] = sla.istate == "Toggled on";
            }
            if (sla.iname == "Airplane")
            {
                airplane = sla.istate == "Toggled on";
            }
            if (sla.iname == "Plant")
            {
                plant = sla.istate == "Toggled on";
            }
            if (sla.iname == "Stereo 1")
            {
                stereo1 = sla.istate == "Toggled on";
            }
            if (sla.iname == "Stereo 2")
            {
                stereo2 = sla.istate == "Toggled on";
            }
            if (sla.iname == "TV")
            {
                tv = sla.istate == "Toggled on";
            }
            if (sla.iname == "Wall")
            {
                wallcolor = sla.cnumber;
            }
            if (sla.iname == "Poster")
            {
                poster = sla.istate == "Toggled on";
            }
        }

        public Gstate()
        {
            beanbags[0] = false; beanbags[1] = false;
            teddybear = false;
            bedcolor = 5;
            boats[0] = false; boats[1] = false;
            books[0] = false; books[1] = false;
            cars[0] = false; cars[1] = false;
            chair = false;
            drum = false;
            elephants[0] = false; elephants[1] = false;
            fan = 0;
            giraffe = false;
            guitar = false;
            lamp1 = false;
            lamp2 = false;
            lamp3 = false;
            laptop = false;
            makeup[0] = false; makeup[1] = false;
            airplane = false;
            plant = false;
            stereo1 = false;
            stereo2 = false;
            tv = false;
            wallcolor = 5;
            poster = false;
        }
    }

    private void Start()
    {
        fw = xmax - xmin;
        fh = ymax - ymin;
        vactlist = new VertexActions[0];
    }

    void Update() {
        opercent = Mathf.Round(overall*100 / 41589.14f);//
        if (sl.showCharts && sl.curchart < sl.trials.Length)
        {
            sl.trials[sl.curchart].opercent = opercent;
            sl.trials[sl.curchart].ehours = overall;
        }
        ps.enableEmission = GetComponent<Camera>().enabled;
    }

    Vector2 GetGoal(Gstate gs)
    {
        Vector2 result = new Vector2(100,100);

        if (gs.tv)
        {
            result.x -= 5;
            result.y -= 10;
        }
        if (gs.stereo1)
        {
            result.x -= 5;
            result.y -= 10;
        }
        if (gs.stereo2)
        {
            result.x -= 5;
            result.y -= 10;
        }
        if (gs.lamp1)
        {
            result.x -= 5;
            result.y -= 10;
        }
        if(gs.lamp2)
        {
            result.x -= 5;
            result.y -= 10;
        }
        if(gs.lamp3)
        {
            result.x -= 5;
            result.y -= 10;
        }
        if(gs.laptop)
        {
            result.x -= 5;
            result.y -= 10;
        }

        if(gs.fan == 3)
        {
            result.x -= 5;
            result.y -= 10;
        }
        if (gs.fan == 2)
        {
            result.x -= 2.5f;
            result.y -= 6;
        }
        if (gs.fan == 1)
        {
        }
        if (gs.fan == 0)
        {
            result.x -= 1;
            result.y -= 2;
        }

        if (gs.beanbags[0])
        {
            result.x -= 4.5f;
        }
        if (gs.beanbags[1])
        {
            result.x -= 4.5f;
        }
        if(gs.boats[0])
        {
            result.x -= 4.5f;
        }
        if(gs.cars[0])
        {
            result.x -= 4.5f;
        }
        if(gs.drum)
        {
            result.x -= 4.5f;
        }
        if(gs.elephants[1])
        {
            result.x -= 4.5f;
        }
        if(gs.giraffe)
        {
            result.x -= 4.5f;
        }
        if(gs.guitar)
        {
            result.x -= 4.5f;
        }
        if (gs.makeup[1])
        {
            result.x -= 4.5f;
        }
        if(gs.airplane)
        {
            result.x -= 4.5f;
        }

        if (gs.bedcolor == 0 || gs.bedcolor == 2 || gs.bedcolor == 10)
        {
        }
        else if (gs.bedcolor == 5)
        {
            result.x -= 6;
            result.y -= 8;
        }
        else if (gs.bedcolor == 1)
        {
            result.x -= 3;
            result.y -= 4;
        }
        else
        {
            result.x -= 1.5f;
            result.y -= 2;
        }

        if (gs.wallcolor == 0 || gs.wallcolor == 2 || gs.wallcolor == 10)
        {
        }
        else if (gs.wallcolor == 5)
        {
            result.x -= 9;
            result.y -= 12;
        }
        else if (gs.wallcolor == 1)
        {
            result.x -= 4.5f;
            result.y -= 6;
        }
        else
        {
            result.x -= 2.25f;
            result.y -= 3;
        }

        return result;
    }

    void init()
    {
        if(vertices.Length != segments)
            vertices = new Vector3[segments];
        qlist = new float[segments];
        for (int i = 0; i < segments; i++)
        {
            if (i == 0)
            {
                vertices[i] = new Vector3(xmin, ymin, 0);
            }
            else if (i == segments - 1)
            {
                vertices[i] = new Vector3(xmax, ymax, 0);
            }
            else
            {
                vertices[i] = vertices[0];
                vertices[i].x = xmin + fw * (i) / (segments-1);
            }
        }
        float quality = 0;//current quality
        float goalquality = 100; //goal quality
        float goaldisruption = 100; //goal disruption
        int act = 0; // action index
        float t = 72000; //current time
        float interval = 43200f / (segments - 1f);
        bool pm = true;
        Gstate gamestate = new Gstate();
        bool asleep = false;
        vactlist = new VertexActions[0];
        qlist[0] = 0;
        float eq = 0;
        float eduration = 0;
        overall = 0;
        sp = new int[0];
        int tos_iterator = 0;

        for (int i = 1; i < vertices.Length; i++)
        {
            t += interval;
            
            if (eduration > 0)
            {
                eduration -= interval;
                if (eduration <= 0)
                {
                    eq = 0;
                }
            }
            if (t >= 86400)
            {
                t -= 86400;
                pm = false;
            }
            bool added = false;
            while (true)
            {
                if (act >= trial.actions.Length)
                    break;
                if (pm)
                {
                    if (trial.actions[act].timestamp <= t && trial.actions[act].timestamp >= 72000 || (t+interval >= 86400 && trial.actions[act].timestamp < 86400 && trial.actions[act].timestamp>72000))
                    {
                        if (!added)
                        {
                            added = true;
                            VertexActions[] temp = new VertexActions[vactlist.Length + 1];
                            for (int j = 0; j < vactlist.Length; j++)
                            {
                                temp[j] = vactlist[j];
                            }
                            temp[temp.Length - 1] = new VertexActions();
                            temp[temp.Length - 1].actionlist = new SleepLab.Action[1];
                            temp[temp.Length - 1].actionlist[0] = trial.actions[act];
                            vactlist = temp;
                        }
                        else
                        {
                            vactlist[vactlist.Length - 1].Add(trial.actions[act]);
                        }
                        gamestate.ApplyAction(trial.actions[act]);
                        if (trial.actions[act].iname == "Event")
                        {
                            eq = trial.actions[act].eq;
                            eduration = trial.actions[act].eduration;
                        }
                        act++;
                    }
                    else
                        break;
                }
                else
                {
                    if ((trial.actions[act].timestamp <= t) || (t + interval >= 28800 && t <72000))
                    {
                        if (!added)
                        {
                            added = true;
                            VertexActions[] temp = new VertexActions[vactlist.Length + 1];
                            for (int j = 0; j < vactlist.Length; j++)
                            {
                                temp[j] = vactlist[j];
                            }
                            temp[temp.Length - 1] = new VertexActions();
                            temp[temp.Length - 1].actionlist = new SleepLab.Action[1];
                            temp[temp.Length - 1].actionlist[0] = trial.actions[act];
                            vactlist = temp;
                        }
                        else
                        {
                            vactlist[vactlist.Length - 1].Add(trial.actions[act]);
                        }
                        gamestate.ApplyAction(trial.actions[act]);
                        if (trial.actions[act].iname == "Event")
                        {
                            eq = trial.actions[act].eq;
                            eduration = trial.actions[act].eduration;
                        }
                        act++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            Vector2 rvalue = GetGoal(gamestate);
            goaldisruption = rvalue.x - eq;
            goalquality = rvalue.y - eq;
            if (goalquality < 0)
                goalquality = 0;
            if (goaldisruption < 0)
                goaldisruption = 0;

            if (asleep)
            {
                quality = Mathf.MoveTowards(quality, goalquality, interval * speed);
            }
            else
            {
                quality = 0;
            }
            if (tos_iterator < trial.timeofsleep.Length)
            {
                if (t >= trial.timeofsleep[tos_iterator] && trial.timeofsleep[tos_iterator] >= 72000)
                {
                    float ctdelta = t;
                    if (t > 72000)
                    {
                        ctdelta -= 72000;
                    }
                    else if (t < 72000)
                    {
                        ctdelta += 14400;
                    }

                    float tsdelta = trial.timeofsleep[tos_iterator];
                    if (tsdelta > 72000)
                    {
                        tsdelta -= 72000;
                    }
                    else if (tsdelta < 72000)
                    {
                        tsdelta += 14400;
                    }

                    float idelta = Mathf.Abs(ctdelta - tsdelta);
                    if (!asleep)
                    {
                        idelta = Mathf.Abs(interval - idelta);
                    }
                    quality = Mathf.MoveTowards(quality, goalquality, idelta * speed);
                    asleep = !asleep;
                    tos_iterator++;

                    int[] _sp = new int[sp.Length + 1];
                    for (int b = 0; b < sp.Length; b++)
                    {
                        _sp[b] = sp[b];
                    }
                    _sp[_sp.Length - 1] = i;
                    sp = _sp;
                }
                else if (t >= trial.timeofsleep[tos_iterator] && t < 72000 && trial.timeofsleep[tos_iterator] < 72000)
                {
                    float ctdelta = t;
                    if (t > 72000)
                    {
                        ctdelta -= 72000;
                    }
                    else if (t < 72000)
                    {
                        ctdelta += 14400;
                    }

                    float tsdelta = trial.timeofsleep[tos_iterator];
                    if (tsdelta > 72000)
                    {
                        tsdelta -= 72000;
                    }
                    else if (tsdelta < 72000)
                    {
                        tsdelta += 14400;
                    }

                    float idelta = Mathf.Abs(ctdelta - tsdelta);
                    if (!asleep)
                    {
                        idelta = Mathf.Abs(interval - idelta);
                    }
                    quality = Mathf.MoveTowards(quality, goalquality, idelta * speed);
                    asleep = !asleep;
                    tos_iterator++;

                    int[] _sp = new int[sp.Length + 1];
                    for (int b = 0; b < sp.Length; b++)
                    {
                        _sp[b] = sp[b];
                    }
                    _sp[_sp.Length - 1] = i;
                    sp = _sp;
                }
            }

            vertices[i].y = ymin + fh * (quality / 100f);
            if (added)
            {
                vactlist[vactlist.Length - 1].position = new Vector2(vertices[i].x, vertices[i].y);
            }
            qlist[i] = Mathf.Round(quality);
            if (asleep)
            {
                
                overall += interval * quality/100f;
            }
            
        }
    }

    

    void OnPostRender() {
        if (trial == null)
        {
            vactlist = new VertexActions[0];
            return;
        }
        if (!trial.active)
        {
            vactlist = new VertexActions[0];
            return;
        }
        if (segments <= 0)
        {
            segments = 2;
        }
        init();

        GL.PushMatrix();
        mat.SetPass(0);
        GL.LoadOrtho();
        GL.Begin(GL.LINES);
        GL.Color(Color.yellow);
        bool makeyellow = true;
        bool pmakeyellow = true;
        int iterator = 0;
        for (int i = 0; i < segments-1; i++)
        {
            if (iterator < sp.Length)
            {
                if (i + 1 == sp[iterator])
                {
                    makeyellow = !makeyellow;
                    int test = sp[iterator];
                    while (true)
                    {
                        if (iterator == sp.Length)
                            break;
                        if (sp[iterator] == test)
                            iterator++;
                        else
                            break;
                    }
                }
            }
            if (pmakeyellow != makeyellow)
            {
                if (makeyellow)
                {
                    GL.Color(Color.yellow);
                }
                else
                {
                    GL.Color(Color.cyan);
                }
                pmakeyellow = makeyellow;
            }
            GL.Vertex(vertices[i]);
            GL.Vertex(vertices[i+1]);
        }
        GL.End();
        GL.PopMatrix();
    }
}
