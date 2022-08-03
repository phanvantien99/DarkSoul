using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SG
{
    public class AnimatorHandler : MonoBehaviour
    {

        public Animator anim;
        int vertical;
        int horizontal;

        public bool canRotate;
        public void Initialize()
        {
            anim = GetComponent<Animator>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement)
        {

            float v = SnappedAnimator(verticalMovement);
            float h = SnappedAnimator(horizontalMovement);
            Debug.Log(v);
            anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
            anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
        }


        float SnappedAnimator(float movement)
        {
            if (movement > 0 && movement < 0.55f)
                return 0.5f;
            else if (movement > 0.55f)
                return 1;
            else if (movement < 0 && movement > -0.55f)
                return -0.5f;
            else if (movement < -0.55f)
                return -1;
            return 0;
        }

        public void CanRotate()
        {
            canRotate = true;

        }

        public void StopRotate()
        {
            canRotate = false;

        }

    }

}
