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
        Vector3 moveDirection;

        [HideInInspector]
        public Transform myTransform;
        [HideInInspector]
        public AnimatorHandler animatorHandler;

        public GameObject normalCamera;
        public Rigidbody rigidbodyPlayer;

        [Header("Status")]
        [SerializeField] float movementSpeed = 5;
        [SerializeField] float rotationSpeed = 10;

        public bool isSprinting;


        private void Start()
        {
            rigidbodyPlayer = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            myTransform = transform;
            animatorHandler.Initialize();
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            inputHandler.HandleInputs();

            HandleRollingAndSprinting();

        }

        private void FixedUpdate()
        {
            HandleMovement();
        }

        public void HandleMovement()
        {
            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;

            moveDirection *= movementSpeed;
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbodyPlayer.velocity = projectedVelocity;
            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, isSprinting);
            if (animatorHandler.canRotate)
            {
                HandleRotation();
            }
        }



        #region Movement
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

        void HandleRollingAndSprinting()
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
