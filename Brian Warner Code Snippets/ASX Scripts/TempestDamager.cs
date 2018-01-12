using UnityEngine;
using System.Collections;

//checks animation state and time for when to activate specific hit boxes, and any on-hit information such as stun, damage, and knockback.

public class TempestDamager : Damager
{
	// Use this for initialization
	void Start () {
		//fx = Camera.main.GetComponent<Linker>().fx;
		for(int i = 0; i<this.hb.Length; i++)
		{
			hb[i].dms = dms;
		}
	}
	
	// Update is called once per frame
	public void Update () 
	{
		if(pc.h.hitfreeze<=0 &&pc.moveTimer>0)
			pc.moveTimer-=Time.deltaTime;
		ph = ch;
		ch = pc.cstate.fullPathHash;
		if(ch!=ph||pplaytime>pc.cplaytime)
		{
			dms.resetDamage();
			resetHB();
		}

        if (pc.cstate.IsName("High Kick"))
        {
            if (pc.cplaytime >= .05 && pc.cplaytime < .5f)
            {
                tc[0].on = true;
            }
            else
                resetTC();
            if (pc.cplaytime > .3f && pc.cplaytime < .5f)
            {
                hb[0].active = true;
                hb[0].type = 2;
                hb[0].effect = 0;
                hb[0].force = forces[2];
                hb[0].damage = damages[0];
                hb[0].stun = stun[1];
                hb[0].proration = proration[2];
                hb[0].hitfreeze = .05f;
            }
            else
            {

                resetHB();
                dms.resetDamage();
            }
        }
        else if (pc.cstate.IsName("HK2"))
        {
            if (pc.cplaytime >= .05 && pc.cplaytime < .5f)
            {
                tc[1].on = true;
            }
            else
                resetTC();
            if (pc.cplaytime > .3f && pc.cplaytime < .5f)
            {
                hb[13].active = true;
                hb[13].type = 0;
                hb[13].effect = 0;
                hb[13].force = forces[2];
                hb[13].damage = damages[0];
                hb[13].stun = stun[1];
                hb[13].proration = proration[2];
                hb[13].hitfreeze = .075f;
            }
            else
            {
                resetHB();
                dms.resetDamage();
            }
        }
        else if (pc.cstate.IsName("HK3"))
        {
            if (pc.cplaytime >= .05 && pc.cplaytime < .5f)
            {
                tc[0].on = true;
            }
            else
                resetTC();
            if (pc.cplaytime > .3f && pc.cplaytime < .5f)
            {
                hb[13].active = true;
                hb[13].type = 0;
                hb[13].effect = 0;
                hb[13].force = forces[2];
                hb[13].damage = damages[0];
                hb[13].stun = stun[1];
                hb[13].proration = proration[2];
                hb[13].hitfreeze = .05f;
            }
            else
            {
                resetHB();
                dms.resetDamage();
            }
        }
        else if (pc.cstate.IsName("HK4"))
        {
            if (pc.cplaytime >= .05 && pc.cplaytime < .5f)
            {
                tc[1].on = true;
            }
            else
                resetTC();
            if (pc.cplaytime > .3f && pc.cplaytime < .5f)
            {
                hb[14].active = true;
                hb[14].type = 2;
                hb[14].effect = 0;
                hb[14].force = forces[3];
                hb[14].damage = damages[0];
                hb[14].stun = stun[1];
                hb[14].proration = proration[2];
                hb[14].hitfreeze = .05f;
            }
            else
            {
                resetHB();
                dms.resetDamage();
            }
        }
        else if (pc.cstate.IsName("Split Kick"))
        {
            hb[12].active = true;
            hb[12].type = 2;
            hb[12].effect = 0;
            hb[12].force = forces[0];
            hb[12].damage = damages[0];
            hb[12].stun = stun[0];
            hb[12].proration = proration[2];
            hb[12].hitfreeze = .05f;
        }
        else if (pc.cstate.IsName("Low Kick"))
        {
            if (pc.cplaytime >= .3 && pc.cplaytime < .5f)
            {
                tc[1].on = true;
            }
            else
                resetTC();
            if (!pc.tchange && pc.cplaytime > .3f && pc.cplaytime < .6f)
            {
                hb[7].active = true;
                hb[7].type = 0;
                hb[7].effect = 0;
                hb[7].force = forces[0];
                hb[7].damage = damages[0];
                hb[7].stun = stun[0];
                hb[7].proration = proration[2];
                hb[7].hitfreeze = .05f;
            }
            else
            {

                resetHB();
                dms.resetDamage();
            }
        }
        else if (pc.cstate.IsName("Air Spin"))
        {
            //todo: air spin
            if (pc.cplaytime >= .1 && pc.cplaytime < .7f)
            {
                tc[0].on = true;
                tc[1].on = true;
            }
            else
                resetTC();
            if (!pc.tchange && pc.cplaytime > .3f && pc.cplaytime < .6f)
            {
                hb[7].active = true;
                hb[7].type = 0;
                hb[7].effect = 0;
                hb[7].force = forces[4] * 1.1f;
                hb[7].damage = damages[0];
                hb[7].stun = stun[0];
                hb[7].proration = proration[2];
                hb[7].hitfreeze = .07f;
                hb[7].antigravity = .5f * pc.gravity;
                hb[7].antigravityDuration = .1f;
            }
            else
            {

                resetHB();
                dms.resetDamage();
            }
        }
        else if (pc.cstate.IsName("Air Spin Heavy"))
        {
            if (pc.cplaytime >= .1 && pc.cplaytime < .7f)
            {
                tc[0].on = true;
                tc[1].on = true;
            }
            else
                resetTC();
            if ((!pc.tchange && pc.cplaytime > .23f && pc.cplaytime < .32f) || (pplaytime <= .23f && pc.cplaytime >= .32f))
            {
                hb[7].active = true;
                hb[7].type = 0;
                hb[7].effect = 0;
                hb[7].force = forces[4];
                hb[7].damage = damages[0];
                hb[7].stun = stun[0];
                hb[7].proration = proration[2];
                hb[7].hitfreeze = .07f;
                hb[7].antigravity = .125f * pc.gravity;
                hb[7].antigravityDuration = .1f;
            }
            else if ((!pc.tchange && pc.cplaytime > .4f && pc.cplaytime < .5f) || (pplaytime <= .4f && pc.cplaytime >= .5f))
            {
                hb[7].active = true;
                hb[7].type = 0;
                hb[7].effect = 0;
                hb[7].force = forces[7];
                hb[7].damage = damages[0];
                hb[7].stun = stun[0];
                hb[7].proration = proration[2];
                hb[7].hitfreeze = .07f;
                hb[7].antigravity = .125f * pc.gravity;
                hb[7].antigravityDuration = .1f;
            }
            else if (!pc.tchange && pc.cplaytime > .6f && pc.cplaytime < .8f)
            {
                hb[7].active = true;
                hb[7].type = 0;
                hb[7].effect = 0;
                hb[7].force = forces[7];
                hb[7].damage = damages[0];
                hb[7].stun = stun[0];
                hb[7].proration = proration[2];
                hb[7].hitfreeze = .07f;
                hb[7].antigravity = .125f * pc.gravity;
                hb[7].antigravityDuration = .1f;
            }
            else
            {

                resetHB();
                dms.resetDamage();
            }
        }
        else if (pc.cstate.IsName("Air Spin Enhanced"))
        {
            if (pc.cplaytime >= .1 && pc.cplaytime < .5f)
            {
                tc[0].on = true;
                tc[1].on = true;
            }
            else if (pc.cplaytime > .6)
            {
                tc[0].on = true;
            }
            else
                resetTC();
            if (!pc.tchange && pc.cplaytime > .29f && pc.cplaytime < .43f)
            {
                hb[7].active = true;
                hb[7].type = 0;
                hb[7].effect = 0;
                hb[7].force = forces[4] * 1.25f;
                hb[7].damage = damages[0];
                hb[7].stun = stun[0];
                hb[7].proration = proration[2];
                hb[7].hitfreeze = .07f;
                hb[7].antigravity = .5f * pc.gravity;
                hb[7].antigravityDuration = .1f;
            }
            else if (!pc.tchange && pc.cplaytime > .68f && pc.cplaytime < .9f)
            {
                hb[7].active = true;
                hb[7].type = 0;
                hb[7].effect = 0;
                hb[7].force = forces[6];
                hb[7].damage = damages[2];
                hb[7].stun = stun[0];
                hb[7].proration = proration[2];
                hb[7].hitfreeze = .07f;
                hb[7].antigravity = .375f * pc.gravity;
                hb[7].antigravityDuration = .4f;
                hb[7].launch = 12f * pc.gravity;
                hb[7].launchDuration = .7f;
            }
            else
            {

                resetHB();
                dms.resetDamage();
            }
        }
        else if (pc.cstate.IsName("High Punch"))
        {

            if (!pc.tchange && pc.cplaytime > .17f && pc.cplaytime < .40f)
            {
                hb[1].active = true;
                hb[1].type = 0;
                hb[1].effect = 0;
                hb[1].force = forces[0];
                hb[1].damage = damages[0];
                hb[1].stun = stun[0];
                hb[1].proration = proration[0];
                hb[1].hitfreeze = .05f;
            }
            else
            {

                resetHB();
                dms.resetDamage();
            }
        }
        else if (pc.cstate.IsName("Special 1"))
        {
            if (pc.cplaytime < .5f)
            {
                tc[0].on = true;
                tc[1].on = true;
            }
            else if (pc.cplaytime > .55 && pc.cplaytime < .68f)
            {
                tc[1].on = true;
            }
            else
                resetTC();
            float mpercent = 1 - (pc.moveTimer / pc.h.ms.GetTempestFD("Special 1"));
            if (mpercent > .22f && mpercent < .3f)
            {
                hb[11].active = true;
                hb[11].type = 0;
                hb[11].effect = 0;
                hb[11].force = forces[0];
                hb[11].damage = damages[0];
                hb[11].stun = stun[0];
                hb[11].proration = proration[2];
                hb[11].hitfreeze = .025f;
            }
            else if (mpercent > .41f && mpercent < .48f)
            {
                hb[11].active = true;
                hb[11].type = 0;
                hb[11].effect = 0;
                hb[11].force = forces[0];
                hb[11].damage = damages[0];
                hb[11].stun = stun[0];
                hb[11].proration = proration[2];
                hb[11].hitfreeze = .025f;
            }
            else if (mpercent > .6f && mpercent < .7f)
            {
                hb[11].active = true;
                hb[11].type = 2;
                hb[11].effect = 0;
                hb[11].force = forces[3];
                hb[11].damage = damages[0];
                hb[11].stun = stun[1];
                hb[11].proration = proration[2];
                //hb[11].knockdown = true;
                hb[11].hitfreeze = .025f;
            }
            else
            {
                resetHB();
                dms.resetDamage();
            }
        }
        else if (pc.cstate.IsName("Low Punch") || pc.cstate.IsName("Low Punch 2") || pc.cstate.IsName("Low Punch 3"))
        {

            if (!pc.tchange && pc.cplaytime > .4f && pc.cplaytime < .60f)
            {
                hb[3].active = true;
                hb[3].type = 0;
                hb[3].effect = 0;
                hb[3].force = forces[0];
                hb[3].damage = damages[0];
                hb[3].stun = stun[1];
                hb[3].proration = proration[2];
                hb[3].hitfreeze = .05f;
            }
            else
            {

                resetHB();
                dms.resetDamage();
            }
        }
        else if (pc.cstate.IsName("Low Punch 4"))
        {
            //todo:low punch 4
            if (!pc.tchange && pc.cplaytime > .4f && pc.cplaytime < .60f)
            {
                hb[3].active = true;
                hb[3].type = 0;
                hb[3].effect = 0;
                hb[3].force = forces[4] * 1.1f;
                hb[3].damage = damages[0];
                hb[3].stun = stun[0];
                hb[3].proration = proration[2];
                hb[3].hitfreeze = .07f;
                hb[3].antigravity = .5f * pc.gravity;
                hb[3].antigravityDuration = .1f;
            }
            else
            {

                resetHB();
                dms.resetDamage();
            }
        }
        else if (pc.cstate.IsName("Crouch Low Punch"))
        {

            if (!pc.tchange && pc.cplaytime > .4f && pc.cplaytime < .60f)
            {
                hb[4].active = true;
                hb[4].type = 1;
                hb[4].effect = 0;
                hb[4].force = forces[0];
                hb[4].damage = damages[0];
                hb[4].stun = stun[0];
                hb[4].proration = proration[2];
                hb[4].hitfreeze = .05f;
            }
            else
            {

                resetHB();
                dms.resetDamage();
            }
        }
        else if (pc.cstate.IsName("Crouch High Punch"))
        {

            if (!pc.tchange && pc.cplaytime > .2f && pc.cplaytime < .40f)
            {
                hb[5].active = true;
                hb[5].type = 2;
                hb[5].effect = 0;
                hb[5].force = forces[0];
                hb[5].damage = damages[0];
                hb[5].stun = stun[0];
                hb[5].proration = proration[2];
                hb[5].hitfreeze = .05f;
            }
            else
            {

                resetHB();
                dms.resetDamage();
            }
        }
        else if (pc.cstate.IsName("Crouch High Kick"))
        {

            if (!pc.tchange && pc.cplaytime > .52f && pc.cplaytime < .65f)
            {
                hb[9].active = true;
                hb[9].type = 0;
                hb[9].effect = 0;
                hb[9].force = forces[0];
                hb[9].damage = damages[0];
                hb[9].stun = stun[0];
                hb[9].proration = proration[2];
                hb[9].hitfreeze = .05f;
            }
            else
            {

                resetHB();
                dms.resetDamage();
            }
        }
        else if (pc.cstate.IsName("Air High Punch"))
        {

            if (!pc.tchange && pc.cplaytime > .35f && pc.cplaytime < .60f)
            {
                hb[8].active = true;
                hb[8].type = 2;
                hb[8].effect = 0;
                hb[8].force = forces[0];
                hb[8].damage = damages[0];
                hb[8].stun = stun[0];
                hb[8].proration = proration[2];
                hb[8].hitfreeze = .05f;
            }
            else
            {

                resetHB();
                dms.resetDamage();
            }
        }
        else if (pc.cstate.IsName("Air Low Punch"))
        {

            if (!pc.tchange && pc.cplaytime > .45f && pc.cplaytime < .75f)
            {
                hb[10].active = true;
                hb[10].type = 2;
                hb[10].effect = 0;
                hb[10].force = forces[0];
                hb[10].damage = damages[0];
                hb[10].stun = stun[0];
                hb[10].proration = proration[2];
                hb[10].hitfreeze = .05f;
            }
            else
            {

                resetHB();
                dms.resetDamage();
            }
        }
        else if (pc.cstate.IsName("Crouch Low Kick"))
        {

            if (!pc.tchange && pc.cplaytime > .2f && pc.cplaytime < .40f)
            {
                hb[6].active = true;
                hb[6].type = 1;
                hb[6].effect = 0;
                hb[6].force = forces[0];
                hb[6].damage = damages[1];
                hb[6].stun = stun[0];
                hb[6].proration = proration[2];
                hb[6].hitfreeze = .05f;
            }
            else
            {

                resetHB();
                dms.resetDamage();
            }
        }
        else if (pc.cstate.IsName("Slide"))
        {

            if (!pc.tchange && pc.cplaytime > .1f && pc.cplaytime < .95f)
            {
                hb[6].active = true;
                hb[6].type = 1;
                hb[6].effect = 0;
                hb[6].force = forces[0];
                hb[6].damage = damages[0];
                hb[6].stun = stun[0];
                hb[6].proration = proration[2];
                hb[6].hitfreeze = .05f;

                Controller2 pc2 = pc.GetComponent<Controller2>();
                if (pc2.b_poweredup == true)
                {
                    hb[6].damage = damages[0] * 4;
                    hb[6].knockdown = true;
                    hb[6].hitfreeze = .1f;
                }
            }
            else
            {

                resetHB();
                dms.resetDamage();
            }
        }
        else if (pc.cstate.IsName("Air Low Kick"))
        {

            if (pc.cplaytime >= .1f && pc.cplaytime < .6f)
            {
                hb[2].active = true;
                hb[2].type = 2;
                hb[2].effect = 0;
                hb[2].force = forces[0];
                hb[2].damage = damages[0];
                hb[2].stun = stun[2];
                hb[2].proration = proration[0];
                hb[2].hitfreeze = .05f;
            }
            else
            {
                resetHB();
                dms.resetDamage();
            }
        }
        else if (pc.cstate.IsName("Grab"))
        {
            if (pc.cplaytime >= .1f && pc.cplaytime < .6f)
            {
                hb[15].active = true;
                hb[15].type = 3;
                hb[15].damage = 0;
            }
            else
            {
                resetHB();
                dms.resetDamage();
            }
        }
        else
        {
            resetHB();
            resetTC();
        }
		pplaytime = pc.cplaytime;
	}

	void resetTC()
	{
		for(int i = 0; i<tc.Length; i++)
		{
			tc[i].on = false;
		}
	}
	void resetHB()
	{
		for(int i = 0; i<hb.Length; i++)
		{
			hb[i].active = false;
			hb[i].effect = -1;
			hb[i].force = Vector3.zero;
			hb[i].damage = 0;
			hb[i].knockdown = false;
			hb[i].stun = 0;
			hb[i].proration = 1;
			hb[i].stuncap = false;
			hb[i].minstun = 0;
			hb[i].antigravity = 0;
			hb[i].antigravityDuration=0;
			hb[i].launch = 0;
			hb[i].launchDuration=0;
		}
	}
	public void spinOpen()
	{
		hb[11].active = true;
		hb[11].type = 2;
		hb[11].effect = 0;
		hb[11].force = forces[1];
		hb[11].damage = damages[0];
		hb[11].stun = stun[0];
		hb[11].proration = proration[2];
		hb[11].hitfreeze = .05f;
	}
	public void damageClose()
	{
		resetHB();
		dms.resetDamage();
	}
	public void StartTimer(float t)
	{
		pc.moveTimer = t;
	}
}
