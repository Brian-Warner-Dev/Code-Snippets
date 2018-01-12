using UnityEngine;
using System.Collections;
using XInputDotNetPure;

//contains functions for checking controller and keyboard input, or both for a specific player or any player.

public class MainInput : MonoBehaviour 
{
	public float deadzoneLX;
	public float deadzoneLY;
	public float deadzoneRX;
	public float deadzoneRY;
	public float sensitivityLX = 1;
	public float sensitivityLY = 1;
	public float sensitivityRX = 1;
	public float sensitivityRY = 1;

	public bool[] playerIndexSet = new bool[4];
	PlayerIndex[] playerIndex = new PlayerIndex[4];
	GamePadState[] state = new GamePadState[4];
	GamePadState[] prevState = new GamePadState[4];
	int p1 = -1;
	int p2 = -1;
    int p3 = -1;
    int p4 = -1;

	//held buttons
	public bool[] a = new bool[4];
	public bool[] x = new bool[4];
	public bool[] b = new bool[4];
	public bool[] y = new bool[4];
	public bool[] lb = new bool[4];
	public bool[] rb = new bool[4];
	public bool[] lt = new bool[4];
	public bool[] rt = new bool[4];
	public bool[] l3 = new bool[4];
	public bool[] r3 = new bool[4];
	public bool[] back = new bool[4];
	public bool[] start = new bool[4];
	public bool[] du = new bool[4];
	public bool[] dd = new bool[4];
	public bool[] dl = new bool[4];
	public bool[] dr = new bool[4];

	//pressed buttons
	public bool[] adown = new bool[4];
	public bool[] xdown = new bool[4];
	public bool[] bdown = new bool[4];
	public bool[] ydown = new bool[4];
	public bool[] lbdown = new bool[4];
	public bool[] rbdown = new bool[4];
	public bool[] ltdown = new bool[4];
	public bool[] rtdown = new bool[4];
	public bool[] l3down = new bool[4];
	public bool[] r3down = new bool[4];
	public bool[] backdown = new bool[4];
	public bool[] startdown = new bool[4];
	public bool[] dudown = new bool[4];
	public bool[] dddown = new bool[4];
	public bool[] dldown = new bool[4];
	public bool[] drdown = new bool[4];
	public bool[] updown = new bool[4];

	//axes
	public float[] lx = new float[4];
	public float[] ly = new float[4];
	public float[] rx = new float[4];
	public float[] ry = new float[4];
	public float[] dx = new float[4];
	public float[] dy = new float[4];

    private void Start()
    {
        lx = new float[4];
        ly = new float[4];
        rx = new float[4];
        ry = new float[4];
        dx = new float[4];
        dy = new float[4];
        a = new bool[4];
        x = new bool[4];
        b = new bool[4];
        y = new bool[4];
        lb = new bool[4];
        rb = new bool[4];
        lt = new bool[4];
        rt = new bool[4];
        l3 = new bool[4];
        r3 = new bool[4];
        back = new bool[4];
        start = new bool[4];
        du = new bool[4];
        dd = new bool[4];
        dl = new bool[4];
        dr = new bool[4];

        adown = new bool[4];
        xdown = new bool[4];
        bdown = new bool[4];
        ydown = new bool[4];
        lbdown = new bool[4];
        rbdown = new bool[4];
        ltdown = new bool[4];
        rtdown = new bool[4];
        l3down = new bool[4];
        r3down = new bool[4];
        backdown = new bool[4];
        startdown = new bool[4];
        dudown = new bool[4];
        dddown = new bool[4];
        dldown = new bool[4];
        drdown = new bool[4];
        updown = new bool[4];

        playerIndexSet = new bool[4];
    }

    void Update()
	{
        
        if (!playerIndexSet[0] || !state[0].IsConnected) //find player 1
		{
            
            p1 = -1;
			playerIndexSet[0] = false;
			for (int i = 0; i < 4; ++i)
			{
                
                if (i == p2)
                {
                    
                    continue;
                }
                
				PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                

                if (testState.IsConnected)
				{
					Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
					playerIndex[0] = testPlayerIndex;
					playerIndexSet[0] = true;
					p1 = i;
				}
			}
		}
		if(!playerIndexSet[1] || !state[1].IsConnected || playerIndex[1]==playerIndex[0]) //find player 2
		{
			p2 = -1;
			playerIndexSet[1] = false;
			for (int i = 0; i < 4; ++i)
			{
				PlayerIndex testPlayerIndex = (PlayerIndex)i;
				GamePadState testState = GamePad.GetState(testPlayerIndex);
				if(i==p1)
					continue;
				if (testState.IsConnected)
				{
					Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
					playerIndex[1] = testPlayerIndex;
					playerIndexSet[1] = true;
					p2 = i;
				}
			}
		}
        if (!playerIndexSet[2] || !state[2].IsConnected || playerIndex[2] == playerIndex[0]) //find player 2
        {
            p3 = -1;
            playerIndexSet[2] = false;
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (i == p1)
                    continue;
                if (testState.IsConnected)
                {
                    Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                    playerIndex[2] = testPlayerIndex;
                    playerIndexSet[2] = true;
                    p3 = i;
                }
            }
        }
        if (!playerIndexSet[3] || !state[3].IsConnected || playerIndex[3] == playerIndex[0]) //find player 2
        {
            p4 = -1;
            playerIndexSet[3] = false;
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (i == p1)
                    continue;
                if (testState.IsConnected)
                {
                    Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                    playerIndex[3] = testPlayerIndex;
                    playerIndexSet[3] = true;
                    p4 = i;
                }
            }
        }

        for (int i = 0; i<4; i++) 
		{
			//Update States
			prevState[i] = state[i];
			state[i] = GamePad.GetState(playerIndex[i]);
			if(!playerIndexSet[i])
			{
				a[i] = false;
				x[i] = false;
				b[i] = false;
				y[i] = false;
				lb[i] = false;
				rb[i] = false;
				lt[i] = false;
				rt[i] = false;
				l3[i] = false;
				r3[i] = false;
				start[i] = false;
				back[i] = false;
				du[i] = false;
				dd[i] = false;
				dl[i] = false;
				dr[i] = false;

				adown[i] = false;
				xdown[i] = false;
				bdown[i] = false;
				ydown[i] = false;
				lbdown[i] = false;
				rbdown[i] = false;
				ltdown[i] = false;
				rtdown[i] = false;
				l3down[i] = false;
				r3down[i] = false;
				startdown[i] = false;
				backdown[i] = false;
				dudown[i] = false;
				dddown[i] = false;
				dldown[i] = false;
				drdown[i] = false;
				updown[i] = false;

				lx[i] = 0;
				ly[i] = 0;
				rx[i] = 0;
				ry[i] = 0;
				dx[i] = 0;
				dy[i] = 0;
                continue;
			}
            
			//Update Button Helds
			// a
			if(state[i].Buttons.A == ButtonState.Pressed)
				a[i] = true;
			else
				a[i] = false;
			// x
			if(state[i].Buttons.X == ButtonState.Pressed)
				x[i] = true;
			else
				x[i] = false;
			// b
			if(state[i].Buttons.B == ButtonState.Pressed)
				b[i] = true;
			else
				b[i] = false;
			// y
			if(state[i].Buttons.Y == ButtonState.Pressed)
				y[i] = true;
			else
				y[i] = false;
			// lb
			if(state[i].Buttons.LeftShoulder == ButtonState.Pressed)
				lb[i] = true;
			else
				lb[i] = false;
			// rb
			if(state[i].Buttons.RightShoulder == ButtonState.Pressed)
				rb[i] = true;
			else
				rb[i] = false;
			// lt
			if(state[i].Triggers.Left>.5f)
				lt[i] = true;
			else
				lt[i] = false;
			// rt
			if(state[i].Triggers.Right>.5f)
				rt[i] = true;
			else
				rt[i] = false;
			// l3
			if(state[i].Buttons.LeftStick == ButtonState.Pressed)
				l3[i] = true;
			else
				l3[i] = false;
			// r3
			if(state[i].Buttons.RightStick == ButtonState.Pressed)
				r3[i] = true;
			else
				r3[i] = false;
			// start
			if(state[i].Buttons.Start == ButtonState.Pressed)
				start[i] = true;
			else
				start[i] = false;
			// back
			if(state[i].Buttons.Back == ButtonState.Pressed)
				back[i] = true;
			else
				back[i] = false;
			// dpad up
			if(state[i].DPad.Up == ButtonState.Pressed && playerIndexSet[i])
				du[i] = true;
			else
				du[i] = false;
			// dpad down
			if(state[i].DPad.Down == ButtonState.Pressed && playerIndexSet[i])
				dd[i] = true;
			else
				dd[i] = false;
			// dpad left
			if(state[i].DPad.Left == ButtonState.Pressed && playerIndexSet[i])
				dl[i] = true;
			else
				dl[i] = false;
			// dpad right
			if(state[i].DPad.Right == ButtonState.Pressed && playerIndexSet[i])
				dr[i] = true;
			else
				dr[i] = false;

			//Update Button Helds
			// a
			if(prevState[i].Buttons.A == ButtonState.Released && state[i].Buttons.A == ButtonState.Pressed)
				adown[i] = true;
			else
				adown[i] = false;
			// x
			if(prevState[i].Buttons.X == ButtonState.Released && state[i].Buttons.X == ButtonState.Pressed)
				xdown[i] = true;
			else
				xdown[i] = false;
			// b
			if(prevState[i].Buttons.B == ButtonState.Released && state[i].Buttons.B == ButtonState.Pressed)
				bdown[i] = true;
			else
				bdown[i] = false;
			// y
			if(prevState[i].Buttons.Y == ButtonState.Released && state[i].Buttons.Y == ButtonState.Pressed)
				ydown[i] = true;
			else
				ydown[i] = false;
			// lb
			if(prevState[i].Buttons.LeftShoulder == ButtonState.Released && state[i].Buttons.LeftShoulder == ButtonState.Pressed)
				lbdown[i] = true;
			else
				lbdown[i] = false;
			// rb
			if(prevState[i].Buttons.RightShoulder == ButtonState.Released && state[i].Buttons.RightShoulder == ButtonState.Pressed)
				rbdown[i] = true;
			else
				rbdown[i] = false;
			// lt
			if(prevState[i].Triggers.Left<=.5f && state[i].Triggers.Left>.5f)
				ltdown[i] = true;
			else
				ltdown[i] = false;
			// rt
			if(prevState[i].Triggers.Right<=.5f && state[i].Triggers.Right>.5f)
				rtdown[i] = true;
			else
				rtdown[i] = false;
			// l3
			if(prevState[i].Buttons.LeftStick == ButtonState.Released && state[i].Buttons.LeftStick == ButtonState.Pressed)
				l3down[i] = true;
			else
				l3down[i] = false;
			// r3
			if(prevState[i].Buttons.RightStick == ButtonState.Released && state[i].Buttons.RightStick == ButtonState.Pressed)
				r3down[i] = true;
			else
				r3down[i] = false;
			// start
			if(prevState[i].Buttons.Start == ButtonState.Released && state[i].Buttons.Start == ButtonState.Pressed)
				startdown[i] = true;
			else
				startdown[i] = false;
			// back
			if(prevState[i].Buttons.Back == ButtonState.Released && state[i].Buttons.Back == ButtonState.Pressed)
				backdown[i] = true;
			else
				backdown[i] = false;
			// dpad up
			if(prevState[i].DPad.Up == ButtonState.Released && state[i].DPad.Up == ButtonState.Pressed)
				dudown[i] = true;
			else
				dudown[i] = false;
			// dpad down
			if(prevState[i].DPad.Down == ButtonState.Released && state[i].DPad.Down == ButtonState.Pressed)
				dddown[i] = true;
			else
				dddown[i] = false;
			// dpad left
			if(prevState[i].DPad.Left == ButtonState.Released && state[i].DPad.Left == ButtonState.Pressed)
				dldown[i] = true;
			else
				dldown[i] = false;
			// dpad right
			if(prevState[i].DPad.Right == ButtonState.Released && state[i].DPad.Right == ButtonState.Pressed)
				drdown[i] = true;
			else
				drdown[i] = false;

			//Update Axes
			// left stick
			float normLX = state[i].ThumbSticks.Left.X;
			float normLY = state[i].ThumbSticks.Left.Y;
			if(Mathf.Abs(normLX) < deadzoneLX)
			   lx[i] = 0;
			else
				lx[i] = (abs(normLX) - deadzoneLX) * (normLX / abs(normLX));
			if (deadzoneLX > 0) 
				lx[i] /= 1 - deadzoneLX;
			lx[i]=lx[i]*sensitivityLX;
			if(lx[i]>1) 
				lx[i] = 1;
			else if(lx[i]<-1)
				lx[i] = -1;

			if(Mathf.Abs(normLY) < deadzoneLY)
				ly[i] = 0;
			else
				ly[i] = (abs(normLY) - deadzoneLY) * (normLY / abs (normLY));
			if (deadzoneLY > 0) 
				ly[i] /= 1 - deadzoneLY;
			ly[i]=ly[i]*sensitivityLY;
			if(ly[i]>1) 
				ly[i] = 1;
			else if(ly[i]<-1)
				ly[i] = -1;

			// right stick
			float normRX = state[i].ThumbSticks.Right.X;
			float normRY = state[i].ThumbSticks.Right.Y;
			if(Mathf.Abs(normRX) < deadzoneRX)
				rx[i] = 0;
			else
				rx[i] = (abs (normRX) - deadzoneRX) * (normRX / abs (normRX));
			if (deadzoneRX > 0) 
				rx[i] /= 1 - deadzoneRX;
			rx[i]=rx[i]*sensitivityRX;
			if(rx[i]>1) 
				rx[i] = 1;
			else if(rx[i]<-1)
				rx[i] = -1;

			if(Mathf.Abs(normRY) < deadzoneRY)
				ry[i] = 0;
			else
				ry[i] = (abs (normRY) - deadzoneRY) * (normRY / abs (normRY));
			if (deadzoneRY > 0) 
				ry[i] /= 1 - deadzoneRY;
			ry[i]=ry[i]*sensitivityRY;
			if(ry[i]>1) 
				ry[i] = 1;
			else if(ry[i]<-1)
				ry[i] = -1;
			// dpad
			dx[i] = 0;
			dy[i] = 0;
			if(du[i])
				dy[i] = 1;
			if(dd[i])
				dy[i] = -1;
			if(dr[i])
				dx[i] = 1;
			if(dl[i])
				dx[i] = -1;

			// up
			float pnormLY = prevState[i].ThumbSticks.Left.Y;
			float ply = 0; 
			if(Mathf.Abs(pnormLY) < deadzoneLY)
				ply = 0;
			else
				ply = (abs(pnormLY) - deadzoneLY) * (pnormLY / abs(pnormLY));
			if (deadzoneLY > 0) 
				ply /= 1 - deadzoneLY;
			ply=ply*sensitivityLY;
			if(ply>1) 
				ply = 1;
			else if(ply<-1)
				ply = -1;
			if(ply<=.5f && ly[i]>.5f)
				updown[i] = true;
			else
				updown[i] = false;
		}
	}

	float abs(float n)
	{
		return Mathf.Abs(n);
	}
}
