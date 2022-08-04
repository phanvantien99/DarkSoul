using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform targetTransform;
    [SerializeField] Transform cameraPivot;
    [SerializeField] Transform cameraTransform;
    Transform myTransform;
    LayerMask ignoreLayers;

    [HideInInspector]
    public static CameraHandler singleton;

    [Header("Adjustment")]
    [SerializeField] float lookSpeed = 0.1f;
    [SerializeField] float followSpeed = 0.1f;
    [SerializeField] float pivotSpeed = 0.03f;

    float defaultPosition;
    float lookAngle;
    float pivotAngle;

    [SerializeField] float minimumPivot = -35;
    [SerializeField] float maximumPivot = 35;


    private void Awake()
    {
        singleton = this;
        myTransform = transform;
        defaultPosition = cameraTransform.localPosition.z;
        ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
    }

    public void FollowTarget(float delta)
    {
        // get direction for camera
        // Lerp return a vector3 from a(camera) position to b(target) position within define time;
        Vector3 target = Vector3.Lerp(myTransform.position, targetTransform.position, delta / followSpeed);
        myTransform.position = target;
    }

    public void HandleCameraRotation(float delta, float mouseX, float mouseY)
    {
        lookAngle += (mouseX * lookSpeed) / delta;
        pivotAngle -= (mouseY * lookSpeed) / delta;
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);


        Vector3 rotation = Vector3.zero;
        rotation.y = lookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        myTransform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;

        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;

    }

}
