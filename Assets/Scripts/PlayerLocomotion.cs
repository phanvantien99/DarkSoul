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
        [SerializeField] Transform groundFlag;
        Vector3 moveDirection;
        [HideInInspector] Transform myTransform;
        [HideInInspector] AnimatorHandler animatorHandler;

        public Rigidbody rigidbodyPlayer;

        [Header("Movement Stats")]
        [SerializeField] float movementSpeed = 5;
        [SerializeField] float sprintSpeed = 10;
        [SerializeField] float rotationSpeed = 10;
        [SerializeField] float playerSpeed = 0;

        [Header("Falling")]
        [SerializeField] LayerMask groundLayers;
        [SerializeField] float inAirTimer;
        [SerializeField] float leapVelocity;
        [SerializeField] float fallingSpeed = 45;
        [SerializeField] float rayCastHeightOffSet = 0.21f;

        // [HideInInspector] public float 

        private void Start()
        {
            rigidbodyPlayer = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            playerManager = GetComponent<PlayerManager>();
            myTransform = transform;
            animatorHandler.Initialize();

            playerManager.isGrounded = true;

        }
        public void HandleAllExtraMovement()
        {
            playerSpeed = HandlePlayerSpeed();
            HandleRolling();
            HandleFallingFixed();
        }



        #region Movement
        Vector3 normalVector;
        Vector3 targetPosition = Vector3.zero;
        public void HandleMovement(bool isSprinting)
        {

            if (inputHandler.rollFlag) return;
            if (playerManager.isInteracting) return;

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

            if (inputHandler.sprintFlag && inputHandler.moveAmount > 0)
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


        void HandleFallingFixed()
        {

            Vector3 origin = groundFlag.position;
            origin.y += 0.1f;

            if (!playerManager.isGrounded)
            {
                if (!playerManager.isInteracting)
                {
                    animatorHandler.PlayTargetAnimation("Falling", true);
                }
                inAirTimer += Time.deltaTime;
                rigidbodyPlayer.AddForce(transform.forward * leapVelocity);
                rigidbodyPlayer.AddForce(Vector3.down * fallingSpeed * inAirTimer);
                playerManager.isInAir = true;
            }

            if (Physics.CheckSphere(origin, 0.11f, groundLayers))
            {
                if (!playerManager.isGrounded && playerManager.isInteracting)
                {
                    animatorHandler.PlayTargetAnimation("Landing", true);
                }
                playerManager.isGrounded = true;
                inAirTimer = 0;
                Debug.Log(true);
            }
            else
            {
                Debug.Log(false);
                playerManager.isGrounded = false;
            }

        }

        private void OnDrawGizmosSelected()
        {
            Vector3 temp = transform.position;
            temp.y += 0.1f;
            Gizmos.DrawWireSphere(temp, 0.101f);
        }

        bool DetectIsGrounded()
        {
            RaycastHit hit;
            targetPosition = transform.position;
            targetPosition.y += 0.2f;
            Debug.Log(targetPosition);
            if (Physics.Raycast(targetPosition, Vector3.down, out hit, 0.21f))
            {
                return true;
            }
            return false;
        }

    }
}
