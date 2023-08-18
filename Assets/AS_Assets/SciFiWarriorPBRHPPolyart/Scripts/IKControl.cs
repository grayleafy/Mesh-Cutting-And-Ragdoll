using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class IKControl : MonoBehaviour
{

    protected Animator animator;

    public bool ikActive = false;
    public Transform rightHandObj = null;
    public Transform lookObj = null;
    public float alpha = 1.0f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // 用于计算 IK 的回调
    void OnAnimatorIK()
    {
        if (animator)
        {

            // 如果 IK 处于活动状态，请将位置和旋转直接设置为目标。
            if (ikActive)
            {

                // 设置观察目标位置（如果已分配）
                if (lookObj != null)
                {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(lookObj.position);
                }

                // 设置右手目标位置和旋转（如果已分配）
                if (rightHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, alpha);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, alpha);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                }

            }

            // 如果 IK 未处于活动状态，请将手和头部的位置和旋转设置回原始位置
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                animator.SetLookAtWeight(0);
            }
        }
    }
}
