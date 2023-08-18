using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FPMouseLook : MonoBehaviour
{
    private Transform cameraTransform;
    [SerializeField] private Transform characterTransform;
    private Vector3 cameraRotation;
    public float MouseSensitivity;
    public Vector2 MaxminAngle;
    private void Start()
    {
        //获取组件
        cameraTransform = transform;
    }
    void Update()
    {
        MouseSensitivity = 5;
        //鼠标控制
        var tmp_MouseX = Input.GetAxis("Mouse X");
        var tmp_MouseY = Input.GetAxis("Mouse Y");
        //鼠标灵敏度
        cameraRotation.x -= tmp_MouseY * MouseSensitivity;
        cameraRotation.y += tmp_MouseX * MouseSensitivity;
        //限制上下看的范围
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, -65, 65);
        //在世界空间中变换的旋转
        cameraTransform.rotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0);
        characterTransform.rotation = Quaternion.Euler(0, cameraRotation.y, 0);

    }
}
