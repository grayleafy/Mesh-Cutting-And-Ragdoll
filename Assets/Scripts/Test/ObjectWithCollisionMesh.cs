using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectWithCollisionMesh : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject collisionGameObject = null;
    public Mesh collisionMesh = null;
    public Mesh mesh = null;
    void Start()
    {
        if (collisionGameObject != null)
        {
            collisionMesh = collisionGameObject.GetComponent<MeshFilter>().mesh;
            mesh = gameObject.GetComponent<MeshFilter>().mesh;
            gameObject.GetComponent<MeshFilter>().mesh = collisionMesh;
            gameObject.AddComponent<MeshCollider>().convex = true;
            gameObject.AddComponent<Rigidbody>();
            gameObject.GetComponent<MeshFilter>().mesh = mesh;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
