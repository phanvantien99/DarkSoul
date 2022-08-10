using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ME
{
    public class InputHandler : MonoBehaviour
    {
        [Header("Props")]
        [SerializeField] public float horizontal;
        [SerializeField] public float vertical;
        [SerializeField] public float moveAmount;
        [SerializeField] float mouseX;
        [SerializeField] float mouseY;
        bool _rollInput;
        bool _sprintInput;
        public bool isInteracting;
        [Header("Flag")]
        [HideInInspector] public bool rollFlag;
        [HideInInspector] public bool sprintFlag;
        [HideInInspector] AnimatorHandler animatorHandler;

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

        private void LateUpdate()
        {
            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget();
                cameraHandler.HandleCameraRotation(mouseX, mouseY);
            }
        }

        private void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerController();
                inputActions.PlayerMovement.Movement.performed += inputActions =>
                    movementInput = inputActions.ReadValue<Vector2>();

                inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
            }

            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }


        public void HandleInputs()
        {
            MoveInput();
            HandleRollInput();
            HandleSprint();
        }

        void MoveInput()
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;

            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }

        void HandleRollInput()
        {
            // b_Input = inputActions.PlayerActions.Roll.triggered;
            _rollInput = inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
            if (_rollInput)
            {
                rollFlag = true;
            }
        }

        void HandleSprint()
        {
            _sprintInput = inputActions.PlayerActions.Sprint.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
            if (_sprintInput)
            {

                sprintFlag = true;
            }
        }


    }
}
