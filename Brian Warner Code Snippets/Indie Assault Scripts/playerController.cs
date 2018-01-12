using UnityEngine;
using System.Collections;
//master character controller containing all the common functions and variables shared between different characters.
public class playerController : MonoBehaviour {

	public Transform character; //transform of fbx with animator
	public int pnum; //player num
	public string spnum; //string form of player num
	public Animator anim; //animator of character
	public float haxis; //left horizontal
	public float lbhor; //back facing horizontal based on haxis and face
	public float lhor; //absolute value horizontal
	public float vaxis; //left vertical
	public bool adown; //A
	public bool a; //A held
	public bool bdown; //B
	public bool b; //B held
	public bool r1; //R1 held
	public bool x; //x held
	public bool xdown; // X
	public bool r1down; // R1
	public bool ftap; //forward tap
	public bool utap; // up tap
	public bool dtap; // down tap
	public bool airborne; //is airborne
	public bool sjumped; //second jump available
	public float[] hstate = new float[6]; //for checking for taps
	public float[] vstate = new float[6]; //for checking for taps
	public int stimer; //used for smash attacks
	public int dtimer; //used for jabs
	public AnimatorStateInfo cstate; //current state
	public AnimatorStateInfo nstate; //state being transitioned into
	public AnimatorTransitionInfo tstate; //transition
	public float cplaytime; //current animation frame time
	public float nplaytime; //next animation frame time
	public bool tchange; //is in transition
	public int face = 1; //direction
	public bool froll; //for rolling
	public bool bthrow; //for switching face once when back throwing
	public float walkSpeed; //walk speed
	public float dashTime; //initial frames of dash
	public int jpressure; //jump pressure counter which increases while in jump prep
	public int sdelay; //delay at beginning to prevent land animation from happening due to starting physics calculation
	public bool attacking; //is attacking or not
	public int rprep; //for jumping while running
	public int uTimer = -1; //timer before up tap activates an xdown
	public int xTimer; //timer to keep x held after up tap triggered xdown. 
	public bool fastfall; //speed falling or not
	public bool smashable; //can use final smash
	public bool upbed; //can use up B/air dodge
	public int ffTimer; //for going through on way floors
	public Vector3 rot; //current rotation
	public Transform shield; //transform of shield
	public bool gravOn; //gravity switch
	public int fwTimer; //for flashing
	public int diztimer; //for dizzy flashing
	public bool shieldbreak; //shield break trigger
	public float shieldHealth; //shield health
	public Transform smoke; //transform of smoke effect
	public float sdrag; //base drag
	public float kdrag; //modified drag while being launched
	public int ktimer; //timer for maintaining kdrag and pain animation
	public bool spawning; //is spawning
	public bool spawnMobile; //spawn platform still moving into position
	public int damage; //current damage
	public Transform splat;//transform of spawn platform
	public bool inputable = true; //can input
	public bool isInvincible; //is currently invincible
	public bool grabbable;//is player grabbable
	public bool isGrabbing; //is player grabbing
	public bool isHolding; //is holding a player
	public bool held; //is player held
	public Transform grabbedPlayer; //link to the player that has been grabbed. Reset null while not grabbing.
	public Vector3 grabPos; //position to put grabbed player
	public int grabTimer; //timer for canceling grab
	public ledgeBox tledge; //ledgebox script of ledge being held onto
	public bool isLedging; //is holding ledge
	public int ledgeTimer; //timer allowed to hold ledge
	public int htimer; //initial timer when struck that forces the pain animation to be maintained for at least a little while
	public int downTimer = -1; //timer allowed to remain floored
	public bool ukemied; //has ukemied or not
	public bool l2down; //taunt button/L2
	public float hitlag; //hitstun and hitlag. While greater than 0, player is immobile, animation freezes, and force additions are saved until hitlag ends
	public bool wasSleep; //for use with hitlag
	public Vector3 savVel; //saved up velocity from before hitlag
	public Vector3 savForce; //saved up forces during hitlag to be added later
	
	//int comboTimer;
	public float proration = 1;//proration for stale moves
	
	public int epoint = 0; //facing of platform edge being touched

	string staleMove; //the last move hit by
	int staleCombo; //counter that goes up whenever a stale move connects

	void OnTriggerEnter(Collider other)
	{
		if(other.transform.CompareTag("Respawn")) //ignore collisions with spawner that doesn't belong to player
		{
			spPlatform sp = other.GetComponent<spPlatform>();
			if(sp.spCharacter!=transform.root)
			{
				Physics.IgnoreCollision(transform.collider,other.transform.collider);
			}
		}
		if(other.transform.CompareTag("Ceiling")&&ktimer>0) //if launched while inside ceiling, destroy and spawn backfall
		{
			GameObject.Destroy(gameObject);
			Transform controller = GameObject.FindGameObjectWithTag("GameController").transform;
			if(controller!=null)
			{
				controller.SendMessage("BackfallTitus",pnum,SendMessageOptions.DontRequireReceiver);
			}
		}
		else if(other.transform.CompareTag("BlastWall")) //if hitting blastwall, destroy
		{
			Transform controller = GameObject.FindGameObjectWithTag("GameController").transform;
			if(controller!=null)
			{
				controller.SendMessage("KOed",pnum,SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	void OnTriggerStay(Collider other) //if touching platform edge and rolling/final fmashing, stop horizontal movement in that direction
	{
		if(other.CompareTag("Edge"))
		{
			epoint = other.GetComponent<pEdge>().face;
			if(cstate.IsName("Back Roll")||cstate.IsName("Front Roll")||cstate.IsName("Final Charge")||cstate.IsName("Final Smash"))
			{
				if(epoint == Mathf.Sign(rigidbody.velocity.x))
				{
					Vector3 temp = rigidbody.velocity;
					temp.x = 0;
					rigidbody.velocity = temp;
					Vector3 pos = transform.position;
					pos.x = other.transform.position.x;
					transform.position = pos;
				}
			}
		}
	}
	
	void Start () {
		shield = transform.Find("shield"); //set shield transform
		spnum = pnum.ToString(); //set string version of player number
		anim = character.GetComponent<Animator>(); //set animator
		upInfo(); //update animation states
		resetAxisStates(); //reset axis states
		upInput(); //update input
		sdelay = 5; //set start delay
		uTimer = -1; 
		xTimer = 0;
		ffTimer = -1;
		rot = transform.localRotation.eulerAngles;
		gravOn = true;
		shieldHealth = 100;
		diztimer = 0;
		sdrag = rigidbody.drag;
	}

	void Update()
	{
		upInput(); //failsafe update
	}

	void LateUpdate()
	{
		upInput(); //failsafe update
		epoint = 0; //reset edge point facing
	}

	//updates input variables
	public void upInput()
	{
		if(inputable)
		{
			if(!spawning)
			{
				haxis = Input.GetAxis("lhor"+spnum);
				lhor = Mathf.Abs(haxis);
				if(Mathf.Sign(haxis) == face)
					lbhor = 0;
				else
					lbhor = -Mathf.Abs(haxis);
				vaxis = Input.GetAxis("lver"+spnum);
				if(attacking)
					xTimer = 0;
				x = (Input.GetButton("x"+spnum)||Input.GetButton("y"+spnum)||xTimer>0);
				a = Input.GetButton("a"+spnum);
				
				r1 = (Input.GetButton("r1"+spnum)||Input.GetButton("l1"+spnum)||Input.GetButton("r2"+spnum));
				
				b = Input.GetButton("b"+spnum);

				if(Input.GetButtonDown("l2"+spnum))
					l2down = true;
				else if(tchange)
					l2down = false;
				if(Input.GetButtonDown("b"+spnum))
					bdown = true;
				else if(tchange)
					bdown = false;
				if (Input.GetButtonDown("x"+spnum)||Input.GetButtonDown("y"+spnum)||uTimer==0)
					xdown = true;
				else if(tchange)
					xdown = false;
				if (Input.GetButtonDown("a"+spnum)||Input.GetButtonDown("r2"+spnum))
					adown = true;
				else if(tchange)
					adown = false;
				if(Input.GetButtonDown("r1"+spnum)||Input.GetButtonDown("l1"+spnum)||Input.GetButtonDown("r2"+spnum))
					r1down = true;
				else if(tchange)
					r1down = false;
				
				if(airborne&&upbed)
				{
					a = false;
					adown = false;
				}
			}
			else if(spawnMobile)
			{
				
				haxis = Input.GetAxis("lhor"+spnum);
				lhor = Mathf.Abs(haxis);
				if(Mathf.Sign(haxis) == face)
					lbhor = 0;
				else
					lbhor = -Mathf.Abs(haxis);
				vaxis = Input.GetAxis("lver"+spnum);
				x = (Input.GetButton("x"+spnum)||Input.GetButton("y"+spnum)||xTimer>0);
				if (Input.GetButtonDown("x"+spnum)||Input.GetButtonDown("y"+spnum)||uTimer==0)
					xdown = true;
				else if(tchange)
					xdown = false;
				
				if(lhor>.2f||Mathf.Abs(vaxis)>.2f||xdown)
				{
					if(splat!=null)
					{
						GameObject sw = GameObject.Instantiate(Resources.Load("Shockwave 3")) as GameObject;
						sw.transform.position = splat.transform.position+new Vector3(0,.5f,0);
						GameObject.Destroy(splat.gameObject);
					}
					spawning = false;
					spawnMobile = false;
				}
			}
			else
			{
				haxis = 0;
				vaxis = 0;
				lhor = 0;
				lbhor = 0;
				xdown = false;
				x = false;
				adown = false;
				a = false;
				bdown = false;
				b = false;
				r1 = false;
				r1down = false;
				face = 1;
			}
		}
		else
		{
			haxis = 0;
			vaxis = 0;
			lhor = 0;
			lbhor = 0;
			xdown = false;
			x = false;
			adown = false;
			a = false;
			bdown = false;
			b = false;
			r1 = false;
			r1down = false;
			//face = 1;
		}
	}

	//updates timer variables
	public void upTimer()
	{
		/*if(comboTimer>0)
			comboTimer--;
		else
			proration = 1;*/
		if(hitlag>0)
			hitlag -= Time.fixedDeltaTime;
		if(stimer>0)
			stimer--;
		if(grabTimer>0)
			grabTimer--;
		if(ktimer>0)
		{
			rigidbody.drag = kdrag;
			if(cstate.IsName("Tumble")||cstate.IsName("Pain"))
			{
				if(rigidbody.velocity.y<0 && htimer==0)
				{
					rigidbody.drag = sdrag;
				}
			}
			if(hitlag<=0)
				ktimer--;
		}
		else
			rigidbody.drag = sdrag;
		if(uTimer>-1)
			uTimer--;
		if(xTimer>0)
			xTimer--;
		if(sdelay>0)
			sdelay--;
		if(rprep>0)
			rprep--;
		if(dashTime>0)
			dashTime--;
		if(ledgeTimer>0)
			ledgeTimer--;
		if(fwTimer>-1)
			fwTimer--;
		if(ffTimer>-1)
			ffTimer--;
		if(downTimer>-1)
			downTimer--;
		if(tstate.IsUserName("tofloor"))
			downTimer = 300;
		if(htimer>0 && hitlag<=0)
		{
			htimer--;
			if(!cstate.IsName("Pain"))
				anim.Play("Pain",0,0);
		}
		if(dtimer>0)
			dtimer--;
		if(diztimer>0)
			diztimer--;
	}

	//updates animation states
	public void upInfo()
	{
		cstate = anim.GetCurrentAnimatorStateInfo(0);
		nstate = anim.GetNextAnimatorStateInfo(0);
		tstate = anim.GetAnimatorTransitionInfo(0);
		cplaytime = cstate.normalizedTime%1;
		nplaytime = nstate.normalizedTime%1;
		//nplaytime = nstate.normalizedTime%1;
		//tplaytime = tstate.normalizedTime%1;
		tchange = anim.IsInTransition(0);
	}

	//resets axis states
	public void resetAxisStates()
	{
		for(int i = 0; i<hstate.Length;i++)
		{
			hstate[i] = haxis;
			vstate[i] = vaxis;
		}
	}

	//updates ukemi
	public void upUkemi()
	{
		if(cstate.IsName("Ukemi"))
		{
			if(!ukemied)
			{
				GameObject sw = GameObject.Instantiate(Resources.Load("small wave")) as GameObject;
				sw.transform.position = transform.position;
				GameObject.Instantiate(Resources.Load("landing1 sound"));
			}
			ukemied = true;
		}
		else
			ukemied = false;
	}

	//updates spawning
	public void upSpawn()
	{
		if(splat==null)
		{
			spawning = false;
			spawnMobile = false;
		}
		if((spawning) ||
		   cstate.IsName("Sidestep") ||
		   cstate.IsName("Front Roll") ||
		   cstate.IsName("Back Roll") ||
		   cstate.IsName("Ledge Roll") ||
		   cstate.IsName("Final Smash") ||
		   cstate.IsName("Final Charge") ||
		   cstate.IsName("Air Dodge") ||
		   cstate.IsName("Shield") ||
		   cstate.IsName("Ukemi") ||
		   isLedging)
		{
			transform.BroadcastMessage("setInvincible",true,SendMessageOptions.DontRequireReceiver);
			isInvincible = true;
		}
		else
		{
			transform.BroadcastMessage("setInvincible",false,SendMessageOptions.DontRequireReceiver);
			isInvincible = false;
		}
	}

	//was pummeled, reset held animation
	public void Pummeled()
	{
		if(held)
		{
			anim.Play("Held",0,0);
		}
	}

	//update taps
	public void upTap()
	{
		ftap = false;
		utap = false;
		dtap = false;
		float lowPoint = .2f;
		//forward tap
		if(hstate[0]>=.95f)
		{
			float lowest = 1f;
			for(int i=0; i<hstate.Length; i++)
			{
				if(hstate[i]<=lowest){lowest = hstate[i];}
				else{lowest = 1; break;}
			}
			if(lowest<lowPoint)
			{
				ftap = true;
				resetAxisStates();
			}
		}
		else if(hstate[0]<=-.95f)
		{
			float lowest = -1f;
			for(int i=0; i<hstate.Length; i++)
			{
				if(hstate[i]>=lowest){lowest = hstate[i];}
				else{lowest = -1; break;}
			}
			if(lowest>-lowPoint)
			{
				ftap = true;
				resetAxisStates();
			}
		}
		
		//Up tap
		if(vstate[0]>=.95f)
		{
			float lowest = 1f;
			for(int i=0; i<vstate.Length; i++)
			{
				if(vstate[i]<=lowest){lowest = vstate[i];}
				else{lowest = 1; break;}
			}
			if(lowest<lowPoint)
			{
				utap = true;
				resetAxisStates();
			}
		}
		if(vstate[0]<=-.95f)
		{
			float lowest = -1f;
			for(int i=0; i<vstate.Length; i++)
			{
				if(vstate[i]>=lowest){lowest = vstate[i];}
				else{lowest = -1; break;}
			}
			if(lowest>-lowPoint)
			{
				dtap = true;
				resetAxisStates();
			}
		}
	}

	//update tap checking axis states
	public void upStates()
	{
		for(int i = 0; i<hstate.Length-1;i++)
		{
			hstate[hstate.Length-1-i] = hstate[hstate.Length-1-i-1];
			vstate[vstate.Length-1-i] = vstate[vstate.Length-1-i-1];
		}
		hstate[0] = haxis;
		vstate[0] = vaxis;
	}

	//update effects
	public void upEffects()
	{
		if((dashTime>0||(cstate.IsName("Jump")&&cplaytime<.2f))||(ktimer>20&&airborne))
		{
			smoke.particleEmitter.emit = true;
		}
		else
		{
			smoke.particleEmitter.emit = false;
		}
		
		if(cstate.IsName("Run")||cstate.IsName("Run2"))
		{
			BroadcastMessage("switchOn");
		}
		else 
		{
			BroadcastMessage("switchOff");
		}
		if(cstate.IsName("Forward Charge")||cstate.IsName("Up Charge")||cstate.IsName("Down Charge"))
		{
			BroadcastMessage("chargeUp",true);
		}
		else
			BroadcastMessage("chargeUp",false);
	}

	public void runFriction()
	{
		//if((tstate.IsUserName("tolo")||(cstate.IsName("Locomotion")&&lhor==0))&&ktimer==0)
		if(tstate.IsUserName("tolo")&&lhor==0&&ktimer==0)
		{
			if(tstate.IsUserName("tolo"))
			{
				rigidbody.AddForce(-rigidbody.velocity*5);
			}
			else if(cstate.IsName("Locomotion"))
			{
				rigidbody.AddForce(-rigidbody.velocity*2.5f);
			}
		}
	}

	//update shield
	public void upShield()
	{
		Vector3 placement = new Vector3(haxis/2,vaxis/2,0);
		shield.SendMessage("reposition",placement);
		if(cstate.IsName("Shield"))
		{
			
			shieldHealth = Mathf.MoveTowards(shieldHealth,0,Time.fixedDeltaTime*10);
			if(shieldHealth<50)
			{
				if(!shieldbreak)
				{
					shieldbreak = true;
					GameObject.Instantiate(Resources.Load("Zap Electric Sound"));
				}
				shield.SendMessage("shieldUp",false);
				BroadcastMessage("shieldbreak");
				BroadcastMessage("flashDizzy",true);
				diztimer = 300;
			}
			else
			{
				shield.SendMessage("shieldUp",true);
			}
		}
		else
		{
			shield.SendMessage("shieldUp",false);
			if(shieldHealth<100)
			{
				shieldHealth = Mathf.MoveTowards(shieldHealth,100,Time.fixedDeltaTime*20);
			}
			if(diztimer==0)
			{
				shieldbreak = false;
				BroadcastMessage("flashDizzy",false);
			}
			else
				BroadcastMessage("flashDizzy",true);
		}
		shield.SendMessage("resize",shieldHealth);
	}	

	//update hitlag
	public void upLag()
	{
		if(hitlag>0)
		{
			if(!wasSleep)
			{
				savVel = rigidbody.velocity;
			}
			rigidbody.velocity = Vector3.zero;
			rigidbody.useGravity = false;
			if(rigidbody.constantForce!=null)
				rigidbody.constantForce.enabled = false;
			anim.speed = 0;
			wasSleep = true;
		}
		else if(hitlag<=0 && wasSleep)
		{
			rigidbody.velocity = savVel;
			rigidbody.useGravity = true;
			if(rigidbody.constantForce!=null)
				rigidbody.constantForce.enabled = true;
			wasSleep = false;
			anim.speed = 1;
			rigidbody.AddForce(savForce,ForceMode.Impulse);
			savForce = Vector3.zero;
		}
	}

	//reciever set floored or not
	void floor(bool fcheck)
	{
		airborne = true;
		if(fcheck&&rigidbody.velocity.y<10f)
			airborne = false;
		if(sdelay>0)
			airborne = false;
	}

	//is attacking
	public void upAttack()
	{
		if(cstate.IsName("Jab 1")||
		   cstate.IsName("Jab 2")||
		   cstate.IsName("Jab 3")||
		   cstate.IsName("Final Charge")||
		   cstate.IsName("Final Smash")||
		   cstate.IsName("Grab")||
		   cstate.IsName("Dash Grab")||
		   cstate.IsName("Up Tilt")||
		   cstate.IsName("Down Tilt")||
		   cstate.IsName("Forward Tilt")||
		   cstate.IsName("Forward Charge")||
		   cstate.IsName("Forward Smash")||
		   cstate.IsName("Down Charge")||
		   cstate.IsName("Down Smash")||
		   cstate.IsName("Up Charge")||
		   cstate.IsName("Up Smash")||
		   cstate.IsName("Back Throw")||
		   cstate.IsName("Forward Throw")||
		   cstate.IsName("Down Throw")||
		   cstate.IsName("Up Throw")||
		   cstate.IsName("Dash Attack")||
		   cstate.IsName("Ledge Attack")||
		   cstate.IsName("Hold")||
		   cstate.IsName("Pummel")||
		   cstate.IsName("dAir")||
		   cstate.IsName("uAir")||
		   cstate.IsName("fAir")||
		   cstate.IsName("bAir")||
		   cstate.IsName("Up B")||
		   nstate.IsName("Up B")||
		   cstate.IsName("nAir")||
		   cstate.IsName("Getup Up Attack"))
			attacking = true;
		else
			attacking = false;
	}

	//update fastfall and gravity/constant force
	public void upGrav()
	{
		if(airborne&&gravOn)
		{
			if(fastfall)
			{
				rigidbody.AddForce(0,-9.8f*10,0,ForceMode.Acceleration);
			}
		}
		if(isLedging || spawning || held || cstate.IsName("Air Dodge"))
		{
			if(constantForce!=null)
				constantForce.enabled = false;
			rigidbody.useGravity = false;
			if(!cstate.IsName("Air Dodge"))
				rigidbody.Sleep();
		}
		else
		{
			if(constantForce!=null)
				constantForce.enabled = true;
			rigidbody.useGravity = true;
		}
	}

	//reset bionic smash guage
	public void resetSmash()
	{
		GameObject gc = GameObject.FindGameObjectWithTag("GameController");
		if(gc!=null)
		{
			gc.transform.BroadcastMessage("resetSmash",pnum);
		}
	}

	//ignore oneway floor
	void oneFloor(Collider fcollider)
	{
		if(ffTimer==0&&cstate.IsName("Crouch")&&!tchange)
		{
			Physics.IgnoreCollision(transform.collider,fcollider);
		}
	}

	//fall through oneway if holding down and fast falling
	void fastfallCheck(Collider fcollider)
	{
		if(fastfall&&vaxis==-1)
		{
			Physics.IgnoreCollision(transform.collider,fcollider);
		}
	}

	//set player num
	void setPlayer(int p)
	{
		pnum = p;
		spnum = pnum.ToString();
	}

	//set ktimer
	void setKnock(int k)
	{
		ktimer = k;
	}

	//set spawning data
	void isSpawning(Transform platform)
	{
		spPlatform sp = platform.GetComponent<spPlatform>();
		spawning = true;
		if(sp.moveTimer==0)
			spawnMobile = true;
		else
			spawnMobile = false;
		splat = platform;
	}

	//cancel spawning state
	void stopSpawning()
	{
		splat = null;
		spawnMobile = false;
		spawning = false;
	}

	//set bionic smashableness
	void setSmash(int sm)
	{
		if(sm >= 100)
		{
			smashable = true;
		}
		else
		{
			smashable = false;
		}
	}

	//set inputable variable
	void canInput(bool can)
	{
		inputable = can;
	}

	//check if grabbable and respond if so
	void checkGrabbable(Transform other)
	{
		if(grabbable)
		{
			other.BroadcastMessage("grabVictim",transform.root);
			grabbable = false;
			held = true;
		}
	}
	
	void grabVictim(Transform other)
	{
		//reference grabbed player if grabbable
		grabbedPlayer = other;
		isHolding = true;
		other.BroadcastMessage("setFace",-face);
		GameObject.Instantiate(Resources.Load("grab sound"));
		GameObject sw = GameObject.Instantiate(Resources.Load("small wave")) as GameObject;
		sw.transform.position = transform.position + new Vector3(face,1.4f,0);
		//set grabbed player to held state
	}

	//set held variable
	void setHeld(bool set)
	{
		held = set;
	}

	//force set facing
	void setFace(int set)
	{
		face = set;
	}

	//react to damage
	void struck(Vector3 dforce)
	{
		if(!held)
		{
			/*if(comboTimer==0)
				proration = 1;*/
			htimer = 10;
			anim.Play("Pain",0,0);
			rigidbody.velocity = Vector3.zero;
			rigidbody.AddForce(dforce,ForceMode.Impulse);
			if(hitlag>0)
				savForce+=dforce*proration;
			/*comboTimer = 20;*/
			/*if(wasSleep)
			{
				//rigidbody.velocity = savVel*1f;
				rigidbody.useGravity = true;
				wasSleep = false;
				anim.speed = 1;
				rigidbody.AddForce(savForce,ForceMode.Impulse);
				savForce = Vector3.zero;
			}
			hitlag = 0;*/
		}
	}

	//inflict shield damage
	void damageShield(float amount)
	{
		shieldHealth -= amount;
	}

	//set hitstun/hitlag
	void setLag(float hl)
	{
		hitlag += hl;
	}

	//update stale moves
	void setStale(string identifier)
	{
		if(identifier == staleMove && identifier!="")
		{
			staleCombo++;
		}
		else
		{
			staleCombo = 0;
		}
		staleMove = identifier;
		proration = 1f - staleCombo/10f;
		if(proration<3f/10f)
			proration=3f/10f;
	}
}
