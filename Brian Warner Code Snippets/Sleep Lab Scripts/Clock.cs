using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//controls the clock in the bottom left. Clock speed depends on speed multiplier setting and whether or not the subject is currently asleep.
public class Clock : MonoBehaviour {
    public int smod = 1;
    public float[] multipliers = new float[4];
    public CRotate crot;
    public Tfade tf;
    public bool active;
    public float seconds = 0;
    float smax = 86400;
    float shalf = 0;
    float mmax = 3600;
    public Transform h;
    public Transform m;
    Quaternion zero;
    public float speed = 1;
    public float sleepspeed = 1;
    public float notasleepspeed = 1;

    public int hour;
    public int minute;

    public float nextHour = 72000;

    public string[] _type = new string[2];
    public int t = 0;
    public string period = "A.M.";
    int counter = 0;

    public void SetTime(int _h, int _m, int _s)
    {
        _s += _m * 60;
        _s += _h * 3600;
        seconds = _s;
    }

	// Use this for initialization
	void Start () {
        _type[0] = "A.M.";
        _type[1] = "P.M.";
        shalf = smax / 2f;
        zero = Quaternion.Euler(new Vector3(0, 0, 0));
        multipliers = new float[5];
        multipliers[0] = .25f;
        multipliers[1] = .5f;
        multipliers[2] = 1f;
        multipliers[3] = 2f;
        multipliers[4] = 3f;
        smod = 2;
    }
	
	// Update is called once per frame
	void Update () {
        if (!crot.fellasleep)
        {
            speed = notasleepspeed;
        }
        else
        {
            speed = sleepspeed;
        }
        if (active)
        {
            seconds += Time.deltaTime * speed * multipliers[smod];
            if (seconds >= nextHour && Mathf.Abs(nextHour - seconds) < 3600)
            {
                float delta = seconds;
                if (delta >= shalf)
                {
                    delta -= shalf;
                }
                tf.tm.text = Mathf.FloorToInt(delta / 3600f).ToString() + ":00 " + period;
                if (Mathf.FloorToInt(delta / 3600f) == 0)
                    tf.tm.text = "12:00 " + period;
                tf.lifetime = tf.maxlife;
                nextHour += 3600;
                if (nextHour >= smax)
                {
                    nextHour -= smax;
                }
                crot.HourChange(counter);
                counter++;
            }
            if (seconds >= 28800)
            {
                if (seconds < 72000)
                {
                    seconds = 28800;
                    active = false;
                }
            }
        }
        else
        {
            seconds = 72000;
            nextHour = 72000;
            counter = 0;
        }
        if (seconds >= smax)
        {
            seconds -= smax;
        }
        float sdelta = seconds;
        if (sdelta >= shalf)
        {
            sdelta -= shalf;
        }
        h.transform.rotation = zero;
        h.transform.Rotate(new Vector3(0, 0, sdelta/shalf * -360f));

        float mdelta = sdelta % mmax;
        m.transform.rotation = zero;
        m.transform.Rotate(new Vector3(0, 0, mdelta / mmax * -360f));
        minute = Mathf.FloorToInt(mdelta / 60);
        hour = Mathf.FloorToInt(sdelta / 3600);
        if (hour == 0)
            hour = 12;

        t = 0;
        if (seconds > shalf)
        {
            t = 1;
        }
        period = _type[t];
	}
}
