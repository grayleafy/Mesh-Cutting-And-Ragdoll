using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;


public class SliceTest : MonoBehaviour
{
    // Start is called before the first frame update
    public float a = 0, b = 1, c = 1;
    public float d = 0.5f;
    public Vector3 referencePoint;
    public Vector3 normal;

    void Start()
    {

    }

    public void StartSlice()
    {
        Slice.Plane plane = new Slice.Plane();
        plane.a = a;
        plane.b = b;
        plane.c = c;
        plane.d = d;

        //referencePoint = transform.InverseTransformPoint(referencePoint);
        //normal = transform.InverseTransformDirection(normal);
        //Slice.Plane plane = new Slice.Plane(referencePoint, normal);

        Matrix4x4 mat = transform.worldToLocalMatrix;


        float t1 = Time.time;
        List<Mesh> list = Slice.Slicer.Slice(GetComponent<MeshFilter>().mesh, plane);
        Destroy(gameObject.GetComponent<MeshRenderer>());

        GameObject up = new GameObject("up");
        up.transform.parent = transform;
        up.transform.localPosition = new Vector3(0.2f, 0.2f, 0.2f);
        up.AddComponent<MeshRenderer>();
        {
            Material[] materials = gameObject.GetComponent<MeshRenderer>().materials;
            Material[] materials2 = new Material[materials.Length + 1];
            for (int i = 0; i < materials.Length; i++) materials2[i] = materials[i];
            materials2[materials.Length] = gameObject.GetComponent<MeshRenderer>().material;
            up.GetComponent<MeshRenderer>().materials = materials2;
        }
        up.AddComponent<MeshFilter>().mesh = list[0];
        //up.SetActive(true);

        GameObject down = new GameObject("down");
        down.transform.parent = transform;
        down.transform.localPosition = new Vector3(-0.2f, -0.2f, -0.2f);
        down.AddComponent<MeshRenderer>();
        {
            Material[] materials = gameObject.GetComponent<MeshRenderer>().materials;
            Material[] materials2 = new Material[materials.Length + 1];
            for (int i = 0; i < materials.Length; i++) materials2[i] = materials[i];
            materials2[materials.Length] = gameObject.GetComponent<MeshRenderer>().material;
            down.GetComponent<MeshRenderer>().materials = materials2;
        }

        down.AddComponent<MeshFilter>().mesh = list[1];
        //down.SetActive(true);
        float t2 = Time.time;
        Debug.Log(t2 - t1);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
