using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MeshTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;
        

        //Vector3[] vertices = mesh.vertices;

        //for (int i = 0; i < vertices.Length; i++)
        //{
        //    vertices[i].Set(vertices[i].x * 2.0f, vertices[i].y, vertices[i].z * 0.5f);
        //}

        //mesh.vertices = vertices;


        List<Vector3> vertices = new List<Vector3>();
        mesh.GetVertices(vertices);

        for (int i = 0; i < vertices.Count; i++)
        {
            var v = vertices[i];
            v.Set(vertices[i].x * 2.0f, vertices[i].y, vertices[i].z * 0.5f);
            vertices[i] = v;
        }

        //foreach (Vector3 v in vertices)
        //{
        //    v.Set(v.x * 2.0f, v.y, v.z * 0.5f);
        //}

       

        mesh.SetVertices(vertices);
        Debug.Log("set vertices");


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
