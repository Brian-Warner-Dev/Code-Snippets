using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//sleep graph script, which is just a rotating particle effect
//controls the height of the particle depending on sleep quality % 
//Clutter objects only affect sleep quality while awake
//subject will not fall asleep if the quality is at 0, otherwise they will fall asleep faster when the percentage is higher
public class CRotate : MonoBehaviour {
    public ParticleSystem _psleep;
    public float eq;
    public float eduration;
    public SleepLab sl;
    Camera cam;
    public Clock c;
    public Gradient grngrad;
    public Gradient yelgrad;
    public ParticleSystem ps;
    public TrailRenderer tr;
    public bool fellasleep;
    public float disruption;
    public float doze;
    public float drate;
    public float quality;
    public float current = 0;
    public float rate;
    public Transform point;
    float yori = 0;
    float maxoff = 2.6f;
    Vector3 pos;
    public float offspeed;
    public int test = 0;
    float idoze;
	// Use this for initialization
	void Start () {
        yori = point.localPosition.y;
        pos = point.transform.localPosition;
        cam = GetComponent<Camera>();
        drate = c.speed*2*c.multipliers[c.smod];
        idoze = doze;
	}

    public void HourChange(int c)
    {
        if (c == 12)
            return;
        if(fellasleep && c<sl.trials[sl.trials.Length-1].cquality.Length)
            sl.trials[sl.trials.Length - 1].cquality[c] = current;

    }

    public void Reset()
    {
        doze = idoze;
        fellasleep = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (eq > 0)
        {
            eduration -= c.speed * Time.deltaTime * c.multipliers[c.smod];
            if (eduration <= 0)
            {
                eq = 0;
            }
        }
        transform.Rotate(0, rate * Time.deltaTime, 0);
        cam.enabled = c.active;

            if (!fellasleep)
            {
                ParticleSystem.ColorOverLifetimeModule coltm = ps.colorOverLifetime;
                coltm.color = yelgrad;
            }
            else
            {
                ParticleSystem.ColorOverLifetimeModule coltm = ps.colorOverLifetime;
                coltm.color = grngrad;
            }
        if (quality > 100)
            quality = 100;
        if (disruption > 100)
            disruption = 100;
        drate = c.multipliers[c.smod] * c.speed * ((2 * 4f * disruption / 100f));
        if (fellasleep)
        {
            if (Mathf.Abs(quality - current) < 20)
                current = Mathf.MoveTowards(current, quality - eq, Time.deltaTime * offspeed * c.multipliers[c.smod]);
            else
                current = Mathf.Lerp(current, quality - eq, Time.deltaTime * offspeed * .05f * c.multipliers[c.smod]);
            if (current <= 0)
            {
                current = 0;
                fellasleep = false;
                doze = idoze;

                float[] tos = new float[sl.trials[sl.trials.Length - 1].timeofsleep.Length + 1];
                for (int i = 0; i < sl.trials[sl.trials.Length - 1].timeofsleep.Length; i++)
                {
                    tos[i] = sl.trials[sl.trials.Length - 1].timeofsleep[i];
                }
                tos[tos.Length - 1] = c.seconds;
                sl.trials[sl.trials.Length - 1].timeofsleep = tos;
            }
        }
        else
        {
            if (Mathf.Abs(disruption - current) < 20)
                current = Mathf.MoveTowards(current, disruption - eq, Time.deltaTime * offspeed*c.multipliers[c.smod]);
            else
                current = Mathf.Lerp(current, disruption - eq, Time.deltaTime * offspeed * .05f * c.multipliers[c.smod]);

            if (current <= 0)
            {
                current = 0;
            }

            doze -= drate * Time.deltaTime;
            if (doze <= 0)
            {
                fellasleep = true;
                float[] tos = new float[sl.trials[sl.trials.Length - 1].timeofsleep.Length + 1];
                for (int i = 0; i < sl.trials[sl.trials.Length - 1].timeofsleep.Length; i++)
                {
                    tos[i] = sl.trials[sl.trials.Length - 1].timeofsleep[i];
                }
                tos[tos.Length - 1] = c.seconds;
                sl.trials[sl.trials.Length - 1].timeofsleep = tos;
            }
        }
        if (current <= 51 && current > 25)
        {

            if (sl.fade <= 1)
            {
                ps.startColor = Color.Lerp(Color.white, new Color(1, .6f, 0), sl.fade);
            }
            else
            {
                ps.startColor = Color.Lerp(Color.white, new Color(1, .6f, 0), 2f - sl.fade);
            }
        }
        else if (current <= 25)
        {
            if (sl.mfade <= 1)
            {
                ps.startColor = Color.Lerp(Color.white, new Color(1, 0, 0), sl.fade);
            }
            else
            {
                ps.startColor = Color.Lerp(Color.white - new Color(0,.2f,.2f,0), new Color(1, 0, 0), 2f - sl.fade);
            }
        }
        else
        {
            ps.startColor = Color.white;
        }
        tr.startColor = ps.startColor;
        pos.y = yori + (maxoff * current / 100f);
        if(test == 0)
            point.transform.localPosition = pos;
        
        
	}
}
