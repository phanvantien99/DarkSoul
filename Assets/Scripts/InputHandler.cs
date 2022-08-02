using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField] float horizontal;
    [SerializeField] float vertical;
    [SerializeField] float moveAmount;
    [SerializeField] float mouseX;
    [SerializeField] float mouseY;

    PlayerController inputActions;

    Vector2 movementInput;
    Vector2 cameraInput;

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

    void TickInput(float delta)
    {

    }

    void MoveInput(float delta)
    {
        horizontal = movementInput.x;
        vertical = movementInput.y;

        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        mouseX = cameraInput.x;
        mouseY = cameraInput.y;
    }
}
