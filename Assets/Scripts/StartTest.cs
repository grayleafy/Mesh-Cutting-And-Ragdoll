using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("start");
    }

    private void OnEnable()
    {
        Debug.Log("enable");
    }

    private void Awake()
    {
        Debug.Log("awake");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
