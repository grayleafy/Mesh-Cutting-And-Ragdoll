using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST : MonoBehaviour
{

    public Mesh mesh1 = null;
    public Mesh mesh2 = null;

    // Start is called before the first frame update
    void Start()
    {
        if (mesh1 != null) Debug.Log("mesh1:");
        if (mesh1.HasVertexAttribute(UnityEngine.Rendering.VertexAttribute.Position))
        {
            Debug.Log("has position");
        }
        if (mesh1.HasVertexAttribute(UnityEngine.Rendering.VertexAttribute.BlendIndices))
        {
            Debug.Log("has blend indice");
        }
        if (mesh1.HasVertexAttribute(UnityEngine.Rendering.VertexAttribute.BlendWeight))
        {
            Debug.Log("has weight");
        }

        BoneWeight boneWeight = mesh1.boneWeights[0];
        Debug.Log("indice0 " + boneWeight.boneIndex0);
        Debug.Log("weight0 " + boneWeight.weight0);

        Debug.Log("indice1 " + boneWeight.boneIndex1);
        Debug.Log("weight1 " + boneWeight.weight1);

        Debug.Log("indice2 " + boneWeight.boneIndex2);
        Debug.Log("weight2 " + boneWeight.weight2);

        Debug.Log("indice3 " + boneWeight.boneIndex3);
        Debug.Log("weight3 " + boneWeight.weight3);

        //if (mesh2 != null) Debug.Log("mesh2:");
        //if (mesh2.HasVertexAttribute(UnityEngine.Rendering.VertexAttribute.BlendWeight))
        //{
        //    Debug.Log("has weight");
        //}
    }

    // Update is called once per frame
    void Update()
    {

    }
}
