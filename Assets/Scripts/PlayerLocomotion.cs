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
        [SerializeField] float fallingSpeed = 45;

        [Header("Ground & Air Detection Stats")]
        [SerializeField] float groundDetectionStartPoint = 0.5f;
        [SerializeField] float minimumDistanceToBeginFall = 1f;
        [SerializeField] float groundDirectionRayDistance = 0.2f;
        LayerMask ignoreForGroundCheck;
        public float inAirTimer;


        private void Start()
        {
            rigidbodyPlayer = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            playerManager = GetComponent<PlayerManager>();
            myTransform = transform;
            animatorHandler.Initialize();

            playerManager.isGrounded = true;
            ignoreForGroundCheck = ~(1 << 8 | 1 << 11);
        }
        public void HandleAllExtraMovement()
        {
            // inputHandler.HandleInputs();
            playerSpeed = HandlePlayerSpeed();
            HandleRolling();
            // HandleGravity();
            // HandleFalling();
            HandleFallingFixed();
        }



        #region Movement
        Vector3 normalVector;
        Vector3 targetPosition;
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

        void HandleFalling()
        {
            // playerManager.isGrounded = false;
            RaycastHit hit;
            Vector3 origin = transform.position;
            origin.y += groundDetectionStartPoint;
            // if (Physics.Raycast(origin, transform.forward, out hit, 0.4f))
            // {
            //     moveDirection = Vector3.zero;
            // }
            // if (playerManager.isInAir)
            // {
            //     rigidbodyPlayer.AddForce(Vector3.down * fallingSpeed);
            //     rigidbodyPlayer.AddForce(moveDirection * fallingSpeed / 10f); // not get stuck on the edge -> TRY COMMENT THIS LINE
            // }
            Vector3 dir = moveDirection;
            dir.Normalize();
            origin += dir * groundDirectionRayDistance;
            targetPosition = transform.position;

            Debug.DrawRay(origin, Vector3.down * groundDetectionStartPoint, Color.cyan, 0.1f, false);

            if (Physics.Raycast(origin, Vector3.down, out hit, minimumDistanceToBeginFall, ignoreForGroundCheck)) // check is grounded == true?
            {
                normalVector = hit.normal;
                Vector3 tp = hit.point;
                playerManager.isGrounded = true;
                targetPosition.y = tp.y;
                if (playerManager.isInAir)
                {
                    if (inAirTimer > 0.5f)
                    {
                        Debug.Log("You are in the air for: " + inAirTimer);
                        animatorHandler.PlayTargetAnimation("Landing", true);
                        inAirTimer = 0;
                    }
                    else
                    {
                        animatorHandler.PlayTargetAnimation("Female Locomotion", false);
                        inAirTimer = 0;
                    }

                    playerManager.isInAir = false;
                }
            }
            else
            {
                if (playerManager.isGrounded)
                {
                    playerManager.isGrounded = false;
                }
                if (!playerManager.isInAir)
                {
                    if (playerManager.isInteracting == false)
                    {
                        animatorHandler.PlayTargetAnimation("Falling", true);
                    }
                    Vector3 velocity = rigidbodyPlayer.velocity;
                    velocity.Normalize();
                    rigidbodyPlayer.velocity = velocity * (movementSpeed / 2);
                }
                playerManager.isInAir = true;
            }

            if (playerManager.isGrounded)
            {
                if (playerManager.isInteracting || inputHandler.moveAmount > 0)
                {
                    transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
                }
                else
                {
                    transform.position = targetPosition;
                }
            }

        }
        #endregion


        void HandleFallingFixed()
        {
            RaycastHit hit;
            Vector3 origin = transform.position;
            origin.y += groundDetectionStartPoint;

            if (playerManager.isInAir)
            {
                rigidbodyPlayer.AddForce(Vector3.down * fallingSpeed);
                rigidbodyPlayer.AddForce(moveDirection * fallingSpeed / 10f);
            }
            Debug.DrawRay(origin, Vector3.down * minimumDistanceToBeginFall, Color.red, 0);
            if (Physics.Raycast(origin, Vector3.down, out hit, minimumDistanceToBeginFall, ignoreForGroundCheck))
            {

                targetPosition = transform.position;
                targetPosition.y = hit.point.y; // store the point that hit raycast position y

                transform.position = targetPosition; // -> THE ERROR make it's go SLOWLY


                playerManager.isGrounded = true;
                if (playerManager.isInAir)
                {
                    if (inAirTimer > 0.5f)
                    {
                        Debug.Log("You are in air for: " + inAirTimer);
                        animatorHandler.PlayTargetAnimation("Landing", true);
                        inAirTimer = 0;
                    }
                    else
                    {
                        animatorHandler.PlayTargetAnimation("Female Locomotion", false);
                        inAirTimer = 0;
                    }
                    playerManager.isInAir = false;
                }
            }
            else
            {
                if (playerManager.isGrounded) playerManager.isGrounded = false;
                if (!playerManager.isInAir)
                {
                    if (!playerManager.isInteracting) animatorHandler.PlayTargetAnimation("Falling", true);
                    rigidbodyPlayer.velocity *= (movementSpeed / 2);
                }
                playerManager.isInAir = true;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(63, 199, 12);
            Vector3 temp = transform.position;
            // temp.y += 0.5f;
            Gizmos.DrawCube(temp, new Vector3(0.1f, 0.1f, 0.1f));
        }

        void HandleGravity()
        {

            if (playerManager.isInAir)
            {
                rigidbodyPlayer.AddForce(Vector3.down * fallingSpeed, ForceMode.Impulse);
            }
        }
    }
}
