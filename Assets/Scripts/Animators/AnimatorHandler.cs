using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ME
{
    public class AnimatorHandler : MonoBehaviour
    {

        public Animator anim;
        [SerializeField] InputHandler inputHandler;
        [SerializeField] PlayerLocomotion playerLoco;
        PlayerManager playerManager;
        int vertical;
        int horizontal;
        public bool canRotate;


        public void Initialize()
        {
            anim = GetComponent<Animator>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
            inputHandler = GetComponentInParent<InputHandler>();
            playerLoco = GetComponentInParent<PlayerLocomotion>();
            playerManager = GetComponentInParent<PlayerManager>();
        }

        public void PlayTargetAnimation(string targetAnimate, bool isInteract)
        {
            anim.applyRootMotion = isInteract;
            anim.SetBool("isInteracting", isInteract);
            anim.CrossFade(targetAnimate, 0.2f);
        }
        public void UpdateAnimatorValues(float verticalMovement, bool isSprinting)
        {


            float v = SnappedAnimator(verticalMovement);

            if (isSprinting) v = 2;
            anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);

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

        private void OnAnimatorMove()
        {
            if (!playerManager.isInteracting) return;

            // float delta = Time.deltaTime;

            playerLoco.rigidbodyPlayer.drag = 0;
            Vector3 deltaPosition = anim.deltaPosition;
            deltaPosition.y = 0;
            // Vector3 velocity = deltaPosition;
            playerLoco.rigidbodyPlayer.velocity = deltaPosition;
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
