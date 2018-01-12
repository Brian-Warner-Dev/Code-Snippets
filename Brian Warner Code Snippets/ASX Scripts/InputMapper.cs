using UnityEngine;
using System.Collections;

//used to map buttons and keys by integer so button inputs can be customized.
//keyboard is limited to 2 players.

public class InputMapper : MonoBehaviour {
	MainInput mi;
	public int[] a = new int[4];
	public int[] x = new int[4];
	public int[] b = new int[4];
	public int[] y = new int[4];
	public int[] lb = new int[4];
	public int[] rb = new int[4];
	public int[] lt = new int[4];
	public int[] rt = new int[4];
	public int[] l3 = new int[4];
	public int[] r3 = new int[4];
	public int[] back = new int[4];
	public int[] start = new int[4];
	public int[] du = new int[4];
	public int[] dd = new int[4];
	public int[] dl = new int[4];
	public int[] dr = new int[4];

	public int[] ka = new int[2];
	public int[] kx = new int[2];
	public int[] kb = new int[2];
	public int[] ky = new int[2];
	public int[] klb = new int[2];
	public int[] krb = new int[2];
	public int[] klt = new int[2];
	public int[] krt = new int[2];
	public int[] kl3 = new int[2];
	public int[] kr3 = new int[2];
	public int[] kback = new int[2];
	public int[] kstart = new int[2];
	public int[] kdu = new int[2];
	public int[] kdd = new int[2];
	public int[] kdl = new int[2];
	public int[] kdr = new int[2];
	public int[] klu = new int[2];
	public int[] kld = new int[2];
	public int[] kll = new int[2];
	public int[] klr = new int[2];
	public int[] kru = new int[2];
	public int[] krd = new int[2];
	public int[] krl = new int[2];
	public int[] krr = new int[2];
	KeyCode[] kc = new KeyCode[101];

	void Awake()
	{
        int temp = a[0];
         a = new int[4];
        for (int i = 0; i < 4; i++)
        {
            a[i] = temp;
        }
        temp = x[0];
         x = new int[4];
        for (int i = 0; i < 4; i++)
        {
            x[i] = temp;
        }
        temp = b[0];
         b = new int[4];
        for (int i = 0; i < 4; i++)
        {
            b[i] = temp;
        }

        temp = y[0];
        y = new int[4];
        for (int i = 0; i < 4; i++)
        {
            y[i] = temp;
        }
        temp = lb[0];
        lb = new int[4];
        for (int i = 0; i < 4; i++)
        {
            lb[i] = temp;
        }
        temp = rb[0];
        rb = new int[4];
        for (int i = 0; i < 4; i++)
        {
            rb[i] = temp;
        }
        temp = lt[0];
        lt = new int[4];
        for (int i = 0; i < 4; i++)
        {
            lt[i] = temp;
        }
        temp = rt[0];
        rt = new int[4];
        for (int i = 0; i < 4; i++)
        {
            rt[i] = temp;
        }
        temp = l3[0];
        l3 = new int[4];
        for (int i = 0; i < 4; i++)
        {
            l3[i] = temp;
        }
        temp = r3[0];
        r3 = new int[4];
        for (int i = 0; i < 4; i++)
        {
            r3[i] = temp;
        }
        temp = back[0];
        back = new int[4];
        for (int i = 0; i < 4; i++)
        {
            back[i] = temp;
        }
        temp = start[0];
        start = new int[4];
        for (int i = 0; i < 4; i++)
        {
            start[i] = temp;
        }
        temp = du[0];
        du = new int[4];
        for (int i = 0; i < 4; i++)
        {
            du[i] = temp;
        }
        temp = dd[0];
        dd = new int[4];
        for (int i = 0; i < 4; i++)
        {
            dd[i] = temp;
        }
        temp = dl[0];
        dl = new int[4];
        for (int i = 0; i < 4; i++)
        {
            dl[i] = temp;
        }
        temp = dr[0];
        dr = new int[4];
        for (int i = 0; i < 4; i++)
        {
            dr[i] = temp;
        }

        mi = GetComponent<MainInput>();

		kc[0] = KeyCode.A;
		kc[1] = KeyCode.Escape;
		kc[2] = KeyCode.F1;
		kc[3] = KeyCode.F2;
		kc[4] = KeyCode.F3;
		kc[5] = KeyCode.F4;
		kc[6] = KeyCode.F5;
		kc[7] = KeyCode.F6;
		kc[8] = KeyCode.F7;
		kc[9] = KeyCode.F8;
		kc[10] = KeyCode.F9;
		kc[11] = KeyCode.F10;
		kc[12] = KeyCode.F11;
		kc[13] = KeyCode.F12;
		kc[14] = KeyCode.Print;
		kc[15] = KeyCode.ScrollLock;
		kc[16] = KeyCode.Pause;
		kc[17] = KeyCode.BackQuote;
		kc[18] = KeyCode.Alpha1;
		kc[19] = KeyCode.Alpha2;
		kc[20] = KeyCode.Alpha3;
		kc[21] = KeyCode.Alpha4;
		kc[22] = KeyCode.Alpha5;
		kc[23] = KeyCode.Alpha6;
		kc[24] = KeyCode.Alpha7;
		kc[25] = KeyCode.Alpha8;
		kc[26] = KeyCode.Alpha9;
		kc[27] = KeyCode.Alpha0;
		kc[28] = KeyCode.Minus;
		kc[29] = KeyCode.Equals;
		kc[30] = KeyCode.Insert;
		kc[31] = KeyCode.Home;
		kc[32] = KeyCode.PageUp;
		kc[33] = KeyCode.Numlock;
		kc[34] = KeyCode.KeypadDivide;
		kc[35] = KeyCode.KeypadMultiply;
		kc[36] = KeyCode.KeypadMinus;
		kc[37] = KeyCode.Tab;
		kc[38] = KeyCode.Q;
		kc[39] = KeyCode.W;
		kc[40] = KeyCode.E;
		kc[41] = KeyCode.R;
		kc[42] = KeyCode.T;
		kc[43] = KeyCode.Y;
		kc[44] = KeyCode.U;
		kc[45] = KeyCode.I;
		kc[46] = KeyCode.O;
		kc[47] = KeyCode.P;
		kc[48] = KeyCode.LeftBracket;
		kc[49] = KeyCode.RightBracket;
		kc[50] = KeyCode.Backslash;
		kc[51] = KeyCode.Delete;
		kc[52] = KeyCode.End;
		kc[53] = KeyCode.PageDown;
		kc[54] = KeyCode.Keypad7;
		kc[55] = KeyCode.Keypad8;
		kc[56] = KeyCode.Keypad9;
		kc[57] = KeyCode.KeypadPlus;
		kc[58] = KeyCode.CapsLock;
		kc[59] = KeyCode.A;
		kc[60] = KeyCode.S;
		kc[61] = KeyCode.D;
		kc[62] = KeyCode.F;
		kc[63] = KeyCode.G;
		kc[64] = KeyCode.H;
		kc[65] = KeyCode.J;
		kc[66] = KeyCode.K;
		kc[67] = KeyCode.L;
		kc[68] = KeyCode.Semicolon;
		kc[69] = KeyCode.Quote;
		kc[70] = KeyCode.Return;
		kc[71] = KeyCode.Keypad4;
		kc[72] = KeyCode.Keypad5;
		kc[73] = KeyCode.Keypad6;
		kc[74] = KeyCode.LeftShift;
		kc[75] = KeyCode.Z;
		kc[76] = KeyCode.X;
		kc[77] = KeyCode.C;
		kc[78] = KeyCode.V;
		kc[79] = KeyCode.B;
		kc[80] = KeyCode.N;
		kc[81] = KeyCode.M;
		kc[82] = KeyCode.Comma;
		kc[83] = KeyCode.Period;
		kc[84] = KeyCode.Slash;
		kc[85] = KeyCode.RightShift;
		kc[86] = KeyCode.UpArrow;
		kc[87] = KeyCode.Keypad1;
		kc[88] = KeyCode.Keypad2;
		kc[89] = KeyCode.Keypad3;
		kc[90] = KeyCode.KeypadEnter;
		kc[91] = KeyCode.LeftControl;
		kc[92] = KeyCode.LeftAlt;
		kc[93] = KeyCode.Space;
		kc[94] = KeyCode.RightAlt;
		kc[95] = KeyCode.RightControl;
		kc[96] = KeyCode.LeftArrow;
		kc[97] = KeyCode.DownArrow;
		kc[98] = KeyCode.RightArrow;
		kc[99] = KeyCode.Keypad0;
		kc[100] = KeyCode.KeypadPeriod;
	}
    bool Button(int b)
    {
        return (Button(b, 0) || Button(b, 1) || Button(b, 2) || Button(b, 3));
    }
	bool Button(int b, int p)
	{
		switch(b)
		{
		case 0:
			return mi.a[p];
		case 1:
			return mi.x[p];
		case 2:
			return mi.b[p];
		case 3:
			return mi.y[p];
		case 4:
			return mi.lb[p];
		case 5:
			return mi.rb[p];
		case 6:
			return mi.lt[p];
		case 7:
			return mi.rt[p];
		case 8:
			return mi.l3[p];
		case 9:
			return mi.r3[p];
		case 10:
			return mi.back[p];
		case 11:
			return mi.start[p];
		case 12:
			return mi.du[p];
		case 13:
			return mi.dd[p];
		case 14:
			return mi.dl[p];
		case 15:
			return mi.dr[p];
		default:
			Debug.LogError(b.ToString() + " is out of range for Button(). Not valid button type.");
			break;
		}
		return false;
	}
    bool ButtonDown(int b)
    {
        return ButtonDown(b, 0) || ButtonDown(b, 1) || ButtonDown(b, 2) || ButtonDown(b, 3);
    }
	bool ButtonDown(int b, int p)
	{
		switch(b)
		{
		case 0:
			return mi.adown[p];
		case 1:
			return mi.xdown[p];
		case 2:
			return mi.bdown[p];
		case 3:
			return mi.ydown[p];
		case 4:
			return mi.lbdown[p];
		case 5:
			return mi.rbdown[p];
		case 6:
			return mi.ltdown[p];
		case 7:
			return mi.rtdown[p];
		case 8:
			return mi.l3down[p];
		case 9:
			return mi.r3down[p];
		case 10:
			return mi.backdown[p];
		case 11:
			return mi.startdown[p];
		case 12:
                
			return mi.dudown[p];
		case 13:
                return mi.dddown[p];
		case 14:
			return mi.dldown[p];
		case 15:
			return mi.drdown[p];
		case 16:
			return mi.updown[p];
		default:
			Debug.LogError(b.ToString() + " is out of range for ButtonDown()");
			break;
		}
		return false;
	}
    public bool GetButton(string command)
    {
        return GetButton(0, command) || GetButton(1, command) || GetButton(2, command) || GetButton(3, command);
    }
	public bool GetButton(int p, string command)
	{
		switch(command)
		{
		case "Attack1":
			return Button(a[p],p);
		case "Attack2":
			return Button(x[p],p);
		case "Attack3":
			return Button(b[p],p);
		case "Attack4":
			return Button(y[p],p);
		case "Charge":
			return Button(lb[p],p);
		case "Overdrive":
			return Button(rb[p],p);
		case "Block":
			return Button(lt[p],p);
		case "Run":
			return Button(rt[p],p);
		case "L3":
			return Button(l3[p],p);
		case "R3":
			return Button(r3[p],p);
		case "Back":
			return Button(back[p],p);
		case "Start":
			return Button(start[p],p);
		case "Dpad Up":
			return Button(du[p],p);
		case "Dpad Down":
			return Button(dd[p],p);
		case "Dpad Left":
			return Button(dl[p],p);
		case "Dpad Right":
			return Button(dr[p],p);
		default:
			Debug.LogError(command + " is not a valid command for GetButton()");
			break;
		}
		return false;
	}
    public bool GetButtonDown(string command)
    {
        return GetButtonDown(0, command) || GetButtonDown(1, command) || GetButtonDown(2, command) || GetButtonDown(3, command);
    }
	public bool GetButtonDown(int p, string command)
	{
        if (p >= 4)
            return false;
		switch(command)
		{
		case "Attack1":
			return ButtonDown(a[p],p);
		case "Attack2":
			return ButtonDown(x[p],p);
		case "Attack3":
			return ButtonDown(b[p],p);
		case "Attack4":
			return ButtonDown(y[p],p);
		case "Charge":
			return ButtonDown(lb[p],p);
		case "Overdrive":
			return ButtonDown(rb[p],p);
		case "Block":
			return ButtonDown(lt[p],p);
		case "Run":
			return ButtonDown(rt[p],p);
		case "L3":
			return ButtonDown(l3[p],p);
		case "R3":
			return ButtonDown(r3[p],p);
		case "Back":
			return ButtonDown(back[p],p);
		case "Start":
			return ButtonDown(start[p],p);
		case "Dpad Up":
			return ButtonDown(du[p],p);
		case "Dpad Down":
			return ButtonDown(dd[p],p);
		case "Dpad Left":
			return ButtonDown(dl[p],p);
		case "Dpad Right":
			return ButtonDown(dr[p],p);
		case "Jump":
			return ButtonDown(16,p);
		default:
			Debug.LogError(command + " is not a valid command for GetButtonDown()");
			break;
		}
		return false;
	}
    public float GetAxis(string command)
    {
        if (GetAxis(0, command) != 0)
            return GetAxis(0, command);
        if (GetAxis(1, command) != 0)
            return GetAxis(1, command);
        if (GetAxis(2, command) != 0)
            return GetAxis(2, command);
        return GetAxis(3, command);
    }
	public float GetAxis(int p, string command)
	{
		switch(command)
		{
		case "lx":
			if(mi.dx[p]!=0)
				return mi.dx[p];
			return mi.lx[p];
		case "ly":
			if(mi.dy[p]!=0)
				return mi.dy[p];
			return mi.ly[p];
		case "rx":
			return mi.rx[p];
		case "ry":
			return mi.ry[p];
		case "dx":
			return mi.dx[p];
		case "dy":
			return mi.dy[p];
		default:
			Debug.LogError(command + " is not a valid command for GetAxis()");
			break;
		}
		return 0;
	}

	public bool GetKey(int p, string command)
	{
		if(p>1 || p<0)
		{
			return false;
		}
		switch(command)
		{
		case "lu":
			return Key(klu[p]);
		case "ld":
			return Key(kld[p]);
		case "ll":
			return Key(kll[p]);
		case "lr":
			return Key(klr[p]);
		case "ru":
			return Key(kru[p]);
		case "rd":
			return Key(krd[p]);
		case "rl":
			return Key(krl[p]);
		case "rr":
			return Key(krr[p]);
		case "Attack1":
			return Key(ka[p]);
		case "Attack2":
			return Key(kx[p]);
		case "Attack3":
			return Key(kb[p]);
		case "Attack4":
			return Key(ky[p]);
		case "Charge":
			return Key(klb[p]);
		case "Overdrive":
			return Key(krb[p]);
		case "Block":
			return Key(klt[p]);
		case "Run":
			return Key(krt[p]);
		case "L3":
			return Key(kl3[p]);
		case "R3":
			return Key(kr3[p]);
		case "Back":
			return Key(kback[p]);
		case "Start":
			return Key(kstart[p]);
		case "Dpad Up":
			return Key(kdu[p]);
		case "Dpad Down":
			return Key(kdd[p]);
		case "Dpad Left":
			return Key(kdl[p]);
		case "Dpad Right":
			return Key(kdr[p]);
		default:
			Debug.LogError(command + " is not a valid command for GetKey()");
			break;
		}
		return false;
	}
	bool Key(int b)
	{
		if(b<=0||b>=kc.Length)
			return false;
		return Input.GetKey(kc[b]);
	}
	public bool GetKeyDown(int p, string command)
	{
		if(p>1 || p<0)
		{
			return false;
		}
		switch(command)
		{
		case "lu":
			return KeyDown(klu[p]);
		case "ld":
			return KeyDown(kld[p]);
		case "ll":
			return KeyDown(kll[p]);
		case "lr":
			return KeyDown(klr[p]);
		case "ru":
			return KeyDown(kru[p]);
		case "rd":
			return KeyDown(krd[p]);
		case "rl":
			return KeyDown(krl[p]);
		case "rr":
			return KeyDown(krr[p]);
		case "Attack1":
			return KeyDown(ka[p]);
		case "Attack2":
			return KeyDown(kx[p]);
		case "Attack3":
			return KeyDown(kb[p]);
		case "Attack4":
			return KeyDown(ky[p]);
		case "Charge":
			return KeyDown(klb[p]);
		case "Overdrive":
			return KeyDown(krb[p]);
		case "Block":
			return KeyDown(klt[p]);
		case "Run":
			return KeyDown(krt[p]);
		case "L3":
			return KeyDown(kl3[p]);
		case "R3":
			return KeyDown(kr3[p]);
		case "Back":
			return KeyDown(kback[p]);
		case "Start":
			return KeyDown(kstart[p]);
		case "Dpad Up":
			return KeyDown(kdu[p]);
		case "Dpad Down":
			return KeyDown(kdd[p]);
		case "Dpad Left":
			return KeyDown(kdl[p]);
		case "Dpad Right":
			return KeyDown(kdr[p]);
		case "Jump":
			return KeyDown(klu[p]);
		default:
			Debug.LogError(command + " is not a valid command for GetKeyDown()");
			break;
		}
		return false;
	}
	bool KeyDown(int b)
	{
		if(b<=0||b>=kc.Length)
			return false;
		return Input.GetKeyDown(kc[b]);
	}
    public float GetKeyAxis(string command)
    {
        float r = 0;
        r = GetKeyAxis(0, command);
        if (r != 0)
            return r;
        return GetKeyAxis(1, command);
    }
	public float GetKeyAxis(int p, string command)
	{
		float r = 0;
		if(p>1 || p<0)
		{
			return 0;
		}
		switch(command)
		{
		case "lx":
			if(GetKey(p,"lr"))
				r = 1;
			else if(GetKey(p,"ll"))
				r = -1;
			return r;
		case "ly":
			if(GetKey(p,"lu"))
				r = 1;
			else if(GetKey(p,"ld"))
				r = -1;
			return r;
		case "rx":
			if(GetKey(p,"rr"))
				r = 1;
			else if(GetKey(p,"rl"))
				r = -1;
			return r;
		case "ry":
			if(GetKey(p,"ru"))
				r = 1;
			else if(GetKey(p,"rd"))
				r = -1;
			return r;
		case "dx":
			if(GetKey(p,"Dpad Right"))
				r = 1;
			else if(GetKey(p,"Dpad Left"))
				r = -1;
			return r;
		case "dy":
			if(GetKey(p,"Dpad Up"))
				r = 1;
			else if(GetKey(p,"Dpad Down"))
				r = -1;
			return r;
		default:
			Debug.LogError(command + " is not a valid command for GetKeyAxis()");
			break;
		}
		return 0;
	}

    public bool GetHybrid(string command)
    {
        return GetHybrid(0, command) || GetHybrid(1, command) || GetHybrid(2, command) || GetHybrid(3, command);
    }
	public bool GetHybrid(int p, string command)
	{
		bool k = false;
		k = GetKey(p,command);
		if(k)
			return k;
		k = GetButton(p,command);
		return k;
	}
	public bool GetHybridDown(int p, string command)
	{
		bool k = false;
		k = GetKeyDown(p,command);
        if (k)
        {
            return k;
        }
        /*if (GetButtonDown(p, command))
        {
            print(p.ToString() + " " + command + " = " + GetButtonDown(p,command).ToString());
        }*/
		return GetButtonDown(p,command);
	}
    public bool GetHybridDown(string command)
    {
        return GetHybridDown(0, command) || GetHybridDown(1, command) || GetHybridDown(2, command) || GetHybridDown(3, command);
    }
	public float GetHybridAxis(int p, string command)
	{
		if(p>1 || p<0)
		{
			Debug.LogError((p+1).ToString() + " is not a valid player# for GetHybridAxis()");
			return 0;
		}
		float k = 0;
		k = GetKeyAxis(p,command);
		if(Mathf.Abs(k)>.5f)
			return k;
		k = GetAxis(p,command);
		return k;
	}
    public float GetHybridAxis(string command)
    {
        float k = 0;
        k = GetKeyAxis(command);
        if (Mathf.Abs(k) > .5f)
            return k;
        k = GetAxis(command);
        return k;
    }
}
