using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ME
{
    public class PlayerManager : MonoBehaviour
    {

        InputHandler inputHandler;
        Animator anime;
        public bool isInteracting;
        CameraHandler cameraHandler;
        PlayerLocomotion playerLocomotion;

        [Header("Player Flags")]
        public bool isSprinting;
        public bool isGrounded;

        private void Awake()
        {
            cameraHandler = CameraHandler.singleton;
        }

        void Start()
        {
            inputHandler = GetComponent<InputHandler>();
            anime = GetComponentInChildren<Animator>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
            Cursor.lockState = CursorLockMode.Locked;
        }

        // Update is called once per frame
        void Update()
        {
            inputHandler.HandleInputs();
            playerLocomotion.HandleAllExtraMovement();

        }


        private void FixedUpdate()
        {
            playerLocomotion.HandleMovement(isSprinting);
        }


        private void LateUpdate()
        {
            inputHandler.rollFlag = false;
            inputHandler.sprintFlag = false;
            isInteracting = anime.GetBool("isInteracting");

            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget();
                cameraHandler.HandleCameraRotation(inputHandler.mouseX, inputHandler.mouseY);
            }

        }



    }
}
