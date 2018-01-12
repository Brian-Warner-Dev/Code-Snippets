using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//advanced fill script with multiple sets of vertices. 
//Each segment adds 3 vertices, with the lower row being the bottom of the tank, +segments for the top of the fill portion, and +segments*2 for the back of the top of the material
public class SuperFill : MonoBehaviour {
    public SuperFill ps;
    public Aquifer aq;
    public Vector3[] newVertices;
    public Vector2[] newUV;
    //public int[] newTriangles;
    public int[] newTriangles1;
    public int[] newTriangles2;
    Mesh mesh;

    int segments = 40;
    public Camera mc;

    public float speed = .6f;

    float xd = .33f;

    float halfseg = 19.5f;

    bool NonzeroTriangle(Vector3[] nt1, int v1, int v2, int v3)//check if y values of the top of the mesh are greater than 0
    {
        if (nt1[v1].y >0 && nt1[v2].y > 0 && nt1[v3].y > 0)
            return true;
        else
            return false;
    }
    //get closest vertices x-wise to the mouse or touch position and raise it
    public void getClosestPoint()
    {
        Vector3 mp = Input.mousePosition;
        mp = mc.ScreenToWorldPoint(mp);
        float tp = mp.x;
        float xoff = transform.position.x;
        float dist = 20000;
        int choice = -1;
        for(int i = 0; i<segments; i++)
        {
            if(Mathf.Abs(tp-(xoff+newVertices[i].x)) < dist)
            {
                dist = Mathf.Abs(tp - (xoff + newVertices[i].x));
                choice = i;
            }
        }
        newVertices[choice + segments].y += 1 * speed * Time.deltaTime;
        
        if (newVertices[choice + segments].y > 4.2f)
        {
            newVertices[choice + segments].y = 4.2f;
        }

        newVertices[choice + segments].x = newVertices[choice].x + ((choice - halfseg) / halfseg) * (xd * newVertices[choice + segments].y / 4.2f);

        if (choice-1>0)
        {
            newVertices[choice +segments- 1].y += .85f * speed * Time.deltaTime;
            if (newVertices[choice + segments -1].y > 4.2f)
            {
                newVertices[choice + segments -1].y = 4.2f;
            }

            newVertices[choice + segments - 1].x = newVertices[choice-1].x + ((choice - 1 - halfseg) / halfseg) * (xd * newVertices[choice + segments - 1].y / 4.2f);

            if (choice-2>0)
            {
                newVertices[choice+segments - 2].y += .5f * speed * Time.deltaTime;
                if (newVertices[choice + segments - 2].y > 4.2f)
                {
                    newVertices[choice + segments - 2].y = 4.2f;
                }
                newVertices[choice + segments - 2].x = newVertices[choice - 2].x + ((choice - 2 - halfseg) / halfseg) * (xd * newVertices[choice + segments - 2].y / 4.2f);
            }
        }
        if (choice + segments + 1 < segments*2)
        {
            newVertices[choice +segments+ 1].y += .85f * speed * Time.deltaTime;
            if (newVertices[choice + segments + 1].y > 4.2f)
            {
                newVertices[choice + segments + 1].y = 4.2f;
            }
            newVertices[choice + segments + 1].x = newVertices[choice + 1].x + ((choice + 1 - halfseg) / halfseg) * (xd * newVertices[choice + segments + 1].y / 4.2f);
            if (choice + segments + 2 < segments * 2)
            {
                newVertices[choice + segments+ 2].y += .5f * speed * Time.deltaTime;
                if(newVertices[choice + segments + 2].y>4.2f)
                {
                    newVertices[choice + segments + 2].y = 4.2f;
                }
                newVertices[choice + segments + 2].x = newVertices[choice + 2].x + ((choice + 2 - halfseg) / halfseg) * (xd * newVertices[choice + segments + 2].y / 4.2f);
            }
        }

        for(int i = segments*2; i<newVertices.Length; i++)
        {
            newVertices[i].y = newVertices[i - segments].y + 2 - 2* newVertices[i - segments].y/5.58f;
            newVertices[i].x = (2.15f - .2f * newVertices[i - segments].y / 4.5f) + (8.44f + .2f * newVertices[i - segments].y / 4.5f)* (float)(i-segments*2)/(float)segments;
        }

        //draw top
        for (int i = 0; i < newTriangles2.Length; i += 6)
        {
            int t = 0;
            if (i > 0)
                t = i / 6;
            if (NonzeroTriangle(newVertices, t + segments, t + segments * 2, t + 1 + segments))
            {
                newTriangles2[i] = t + segments;
                newTriangles2[i + 1] = t + segments * 2;
                newTriangles2[i + 2] = t + 1 + segments;
                newTriangles2[i + 3] = t + segments * 2;
                newTriangles2[i + 4] = t + segments * 2 + 1;
                newTriangles2[i + 5] = t + 1 + segments;
            }
            else
            {
                newTriangles2[i] = 0;
                newTriangles2[i + 1] = 0;
                newTriangles2[i + 2] = 0;
                newTriangles2[i + 3] = 0;
                newTriangles2[i + 4] = 0;
                newTriangles2[i + 5] = 0;
            }
        }
    }

    void Start()
    {
        halfseg = (segments-1) / 2f;
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        if (newVertices.Length == 0)
        {
            newVertices = new Vector3[segments * 3];

            for (int i = 0; i < segments; i++)
            {
                if (i == 0)
                    newVertices[i] = Vector3.zero;
                else
                    newVertices[i] = new Vector3((12f / (segments - 1)) * (i), 0, 0);
            }
            for (int i = segments; i < segments * 2; i++)
            {
                newVertices[i] = newVertices[i - segments];
                //newVertices[i].y = 1;
            }
            for (int i = segments * 2; i < segments * 3; i++)
            {
                newVertices[i] = newVertices[i - segments];
                newVertices[i].z = 1;
            }
        }
        else
        {
            Vector3[] temp = new Vector3[newVertices.Length];
            for(int i = 0; i<newVertices.Length; i++)
            {
                temp[i] = newVertices[i];
            }
            newVertices = temp;
        }

        //newTriangles = new int[(segments * 12)-6];
        newTriangles1 = new int[(segments * 6) - 6];
        newTriangles2 = new int[(segments * 6) - 6];

        //i<(segments*6)-6
        for(int i = 0; i<newTriangles1.Length; i+=6)
        {
            //if (i == 234)
              //  continue;
            int t = 0;
            if (i > 0)
                t = i / 6;
            newTriangles1[i] = t;
            newTriangles1[i + 1] = t + segments;
            newTriangles1[i + 2] = t + 1;
            newTriangles1[i + 3] = t + segments;
            newTriangles1[i + 4] = t + segments + 1;
            newTriangles1[i + 5] = t + 1;
        }

        //draw top
        for(int i = 0; i<newTriangles2.Length; i+=6)
        {
            int t = 0;
            if (i > 0)
                t = i / 6;
            newTriangles2[i] = t+segments;
            newTriangles2[i + 1] = t + segments*2;
            newTriangles2[i + 2] = t + 1 + segments;
            newTriangles2[i + 3] = t + segments*2;
            newTriangles2[i + 4] = t + segments*2 + 1;
            newTriangles2[i + 5] = t + 1 + segments;
        }

        mesh.subMeshCount = 2;//divide top and bottom portion of mesh to use a different material for the top

        newUV = new Vector2[newVertices.Length];

        for(int i = 0; i<newUV.Length; i++)
        {
            if(i<segments)
            {
                newUV[i].x = i / 40f;
                newUV[i].y = 0;
            }
            else if(i<segments*2)
            {
                newUV[i].x = (i-segments) / 40f;
                newUV[i].y = .5f;
            }
            else
            {
                newUV[i].x = (i - segments*2) / 40f;
                newUV[i].y = 1;
            }
        }

        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.SetTriangles(newTriangles1, 0);
        mesh.SetTriangles(newTriangles2, 1);
    }

    private void Update()
    {
        for(int i = 0; i<aq.layers.Length; i++)
        {
            if(aq.layers[i].sf == this)
            {
                aq.layers[i].amount = newVertices[69].y;
                if(ps!=null)
                {
                    aq.layers[i].amount = newVertices[69].y - ps.newVertices[69].y;
                }
            }
        }
        
        //update mesh
        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.SetTriangles(newTriangles1, 0);
        mesh.SetTriangles(newTriangles2, 1);
    }
}
