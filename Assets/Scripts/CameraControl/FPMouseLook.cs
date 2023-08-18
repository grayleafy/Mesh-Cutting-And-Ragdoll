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
        //��ȡ���
        cameraTransform = transform;
    }
    void Update()
    {
        MouseSensitivity = 5;
        //������
        var tmp_MouseX = Input.GetAxis("Mouse X");
        var tmp_MouseY = Input.GetAxis("Mouse Y");
        //���������
        cameraRotation.x -= tmp_MouseY * MouseSensitivity;
        cameraRotation.y += tmp_MouseX * MouseSensitivity;
        //�������¿��ķ�Χ
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, -65, 65);
        //������ռ��б任����ת
        cameraTransform.rotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0);
        characterTransform.rotation = Quaternion.Euler(0, cameraRotation.y, 0);

    }
}
