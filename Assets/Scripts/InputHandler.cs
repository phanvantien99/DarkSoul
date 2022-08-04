using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class InputHandler : MonoBehaviour
    {
        [SerializeField] public float horizontal;
        [SerializeField] public float vertical;
        [SerializeField] public float moveAmount;
        [SerializeField] float mouseX;
        [SerializeField] float mouseY;

        [HideInInspector]
        AnimatorHandler animatorHandler;

        PlayerController inputActions;
        CameraHandler cameraHandler;
        Vector2 movementInput;
        Vector2 cameraInput;

        private void Awake()
        {
            cameraHandler = CameraHandler.singleton;
        }



        private void Start()
        {
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
        }

        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;
            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, mouseX, mouseY);
            }
        }

        private void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerController();
                inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
            }

            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void HandleInputs(float delta)
        {
            MoveInput(delta);
        }

        void MoveInput(float delta)
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;

            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            animatorHandler.UpdateAnimatorValues(moveAmount, 0);
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }
    }
}
