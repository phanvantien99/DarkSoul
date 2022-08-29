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
        [SerializeField] float rayCastHeightOffSet;

        [Header("Handle Stair")]
        [SerializeField] Transform stepRayUpper;
        [SerializeField] Transform stepRayLower;
        [SerializeField] float stepHeight = 0.3f;
        [SerializeField] float stepSmooth = 0.1f;

        // [HideInInspector] public float 
        Vector3[] directions = new Vector3[]{
            new Vector3(0f, 0f, 1f),
            new Vector3(1f, 0f, 1f),
            new Vector3(-1f, 0f, 1f)
        };

        private void Start()
        {
            rigidbodyPlayer = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            playerManager = GetComponent<PlayerManager>();
            myTransform = transform;
            animatorHandler.Initialize();
            stepRayUpper.position = new Vector3(stepRayUpper.position.x, stepHeight, stepRayUpper.position.z);

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
            HandleStairAndSlope();
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

            // Vector3 origin = groundFlag.position;
            RaycastHit hit;
            Vector3 origin = transform.position;
            origin.y += rayCastHeightOffSet;
            targetPosition = transform.position;

            if (!playerManager.isGrounded)
            {
                if (!playerManager.isInteracting)
                {
                    animatorHandler.PlayTargetAnimation("Falling", true);
                }
                inAirTimer += Time.deltaTime;
                rigidbodyPlayer.AddForce(transform.forward * leapVelocity);
                rigidbodyPlayer.AddForce(Vector3.down * fallingSpeed * inAirTimer);
            }

            if (Physics.SphereCast(origin, 0.05f, Vector3.down, out hit, 0.2f, groundLayers))
            {
                if (!playerManager.isGrounded && playerManager.isInteracting)
                {
                    if (inAirTimer >= 3f)
                        animatorHandler.PlayTargetAnimation("Landing", true);
                }
                playerManager.isGrounded = true;
                inAirTimer = 0;
                playerManager.isInteracting = false;
            }
            else
            {
                playerManager.isGrounded = false;
            }
        }



        void HandleStairAndSlope()
        {
            foreach (var direction in directions)
            {
                if (Physics.Raycast(stepRayLower.position, transform.TransformDirection(Vector3.forward), 0.15f))
                {
                    Debug.Log(true);
                    if (!Physics.Raycast(stepRayUpper.position, transform.TransformDirection(Vector3.forward), 0.2f))
                    {
                        rigidbodyPlayer.position -= new Vector3(0f, -stepSmooth, 0f);
                    }
                }
            }
        }


        private void OnDrawGizmosSelected()
        {
            Vector3 temp = transform.position;
            temp.y += rayCastHeightOffSet;
            Gizmos.DrawRay(temp, Vector3.down * 0.2f);
            Gizmos.DrawWireSphere(temp, 0.05f);
            Gizmos.DrawRay(stepRayLower.position, transform.TransformDirection(Vector3.forward) * 0.15f);
            Gizmos.DrawRay(stepRayUpper.position, transform.TransformDirection(Vector3.forward) * 0.2f);
        }


    }
}
