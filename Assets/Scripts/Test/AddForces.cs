using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForces : MonoBehaviour
{
    // Start is called before the first frame update
    public new Rigidbody rigidbody = null;
    public Vector3 forceDirection = Vector3.up;
    [SerializeField, SetProperty("Force")]
    private float _force = 0;

    public float Force
    {
        set
        {
            _force = value;
            rigidbody.AddForce(Force * forceDirection);
            Debug.Log("add force");
        }
        get
        {
            return _force;
        }
    }

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
