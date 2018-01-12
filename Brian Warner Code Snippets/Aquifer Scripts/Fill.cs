using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//old fill script
//used with a water tank sprite asset that appears to be 3D with perspective using an orthographic camera.
//draws a mesh to appear as the front of the tank, and a second mesh to appear as the top of the fill material
//the "lid" shrinks down as the tank fills up to maintain the illusion of perspective
public class Fill : MonoBehaviour {
    public int type;
    public Vector3[] newVertices;
    public Vector2[] newUV;
    public int[] newTriangles;
    Mesh mesh;

    public float linetop = 5.58f;
    public float linebot = 0f;


    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;
    }

    private void Update()
    {
        if (linebot < 0)
            linebot = 0;
        if (linebot > 4.5f)
            linebot = 4.5f;
        if (linetop > 4.5f)
            linetop = 4.5f;
        if (linetop < 0)
            linetop = 0;
        if (linebot > linetop)
            linebot = linetop;
        if (linetop < linebot)
            linetop = linebot;

        float bx = 0;
        float bxx = 0;
        

        if (linebot == 0)
        {
            bx = 0;
            bxx = 12;
        }
        else
        {
            bx = -.38f * (linebot / 5.58f);
            bxx = 12 + .38f * (linebot / 5.58f);
        }

        newVertices[0] = new Vector3(bx, linebot, 0);
        newVertices[1] = new Vector3(bxx, linebot, 0);

        float b2x = 0;
        float b2xx = 0;

        b2x = -.38f * (linetop / 5.58f);
        b2xx = 12 + .3f * (linetop / 5.58f);

        newVertices[2] = new Vector3(b2x, linetop, 0);
        newVertices[3] = new Vector3(b2xx, linetop, 0);
        

        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;

        if(type == 2)
        {
            GetComponent<Renderer>().material.mainTextureScale = new Vector2(2, linetop/4.5f);
        }
    }
}
