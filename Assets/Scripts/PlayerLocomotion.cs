using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ME
{
    public class PlayerLocomotion : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] Transform cameraObject;
        InputHandler inputHandler;
        PlayerManager playerManager;
        Vector3 moveDirection;

        [HideInInspector] Transform myTransform;

        [HideInInspector] AnimatorHandler animatorHandler;

        public Rigidbody rigidbodyPlayer;

        [Header("Movement Stats")]
        [SerializeField] float movementSpeed = 5;
        [SerializeField] float sprintSpeed = 10;
        [SerializeField] float rotationSpeed = 10;
        [SerializeField] float playerSpeed = 0;

        // [SerializeField] bool isSprinting;


        private void Start()
        {
            rigidbodyPlayer = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            playerManager = GetComponent<PlayerManager>();
            myTransform = transform;
            animatorHandler.Initialize();

        }



        public void HandleAllExtraMovement()
        {
            // inputHandler.HandleInputs();
            playerSpeed = HandlePlayerSpeed();
            HandleRolling();
        }



        #region Movement

        public void HandleMovement(bool isSprinting)
        {

            if (inputHandler.rollFlag) return;

            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;

            moveDirection *= playerSpeed;

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbodyPlayer.velocity = projectedVelocity;
            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, isSprinting);
            if (animatorHandler.canRotate)
            {
                HandleRotation();
            }
        }

        float HandlePlayerSpeed()
        {

            if (inputHandler.sprintFlag)
            {
                playerManager.isSprinting = true; // active animation in HandleMovement (Code in PlayerManager)
                return sprintSpeed;
            }
            else // if !sprintFlag then de-active animation
            {
                playerManager.isSprinting = false;
                return movementSpeed;
            }

        }

        Vector3 normalVector;
        Vector3 targetPosition;
        void HandleRotation()
        {
            Vector3 targetDirection = Vector3.zero;
            // float moveOverride = inputHandler.moveAmount;

            targetDirection = cameraObject.transform.forward * inputHandler.vertical; // follow direction of camera => if player press vertical then it's will rotate
            targetDirection += cameraObject.right * inputHandler.horizontal;
            targetDirection.Normalize();
            targetDirection.y = 0;

            if (targetDirection == Vector3.zero)
            {
                targetDirection = myTransform.forward;
            }
            float rs = rotationSpeed;
            Quaternion tr = Quaternion.LookRotation(targetDirection);
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * Time.deltaTime);
            myTransform.rotation = targetRotation;
        }

        void HandleRolling()
        {
            if (animatorHandler.anim.GetBool("isInteracting")) return;
            if (inputHandler.rollFlag)
            {
                // moveDirection = cameraObject.forward * inputHandler.vertical;
                // moveDirection += cameraObject.right * inputHandler.horizontal;

                if (inputHandler.moveAmount > 0)
                {
                    animatorHandler.PlayTargetAnimation("Rolling 1", true);
                    // moveDirection.y = 0;
                    // Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                    // myTransform.rotation = rollRotation;
                }
            }
        }
        #endregion

    }
}
