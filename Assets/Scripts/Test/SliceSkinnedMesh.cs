using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceSkinnedMesh : MonoBehaviour
{

    public float a = 0, b = 1, c = 1;
    public float d = 0.5f;
    public Vector3 referencePoint;
    public Vector3 normal;

    // Start is called before the first frame update
    void Start()
    {
        StartSlice();
    }

    void StartSlice()
    {
        normal = transform.InverseTransformDirection(normal);
        referencePoint = transform.InverseTransformPoint(referencePoint);
        Slice.Plane plane = new Slice.Plane(referencePoint, normal);


        //referencePoint = transform.InverseTransformPoint(referencePoint);
        //normal = transform.InverseTransformDirection(normal);
        //Slice.Plane plane = new Slice.Plane(referencePoint, normal);




        System.Diagnostics.Stopwatch watch = new();
        watch.Start();
        List<Mesh> list = Slice.Slicer.Slice(GetComponent<SkinnedMeshRenderer>().sharedMesh, plane);
        GetComponent<SkinnedMeshRenderer>().sharedMesh = list[0];


        watch.Stop();
        Debug.Log(watch.Elapsed);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
