using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform targetTransform;
    [SerializeField] Transform cameraPivot;
    [SerializeField] Transform cameraTransform;
    Vector3 cameraTransformPosition;
    Vector3 cameraFollowVelocity = Vector3.zero;
    LayerMask ignoreLayers;

    [HideInInspector]
    public static CameraHandler singleton;

    [Header("Adjustment")]
    [SerializeField] float lookSpeed = 2;
    [SerializeField] float followSpeed = 0.2f;
    [SerializeField] float pivotSpeed = 2;

    float defaultPosition;
    float lookAngle;
    float pivotAngle;

    [SerializeField] float minimumPivot = -35;
    [SerializeField] float maximumPivot = 35;


    float targetPosition;
    public float cameraSphereRadius = 0.2f;
    public float cameraCollisionOffSet = 0.2f;
    public float minimumCollisionOffSet = 0.2f;

    private void Awake()
    {
        singleton = this;
        defaultPosition = cameraTransform.localPosition.z;
        ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
    }

    public void FollowTarget()
    {
        // get direction for camera
        // Lerp return a vector3 from a(camera) position to b(target) position within define time;
        Vector3 target = Vector3.SmoothDamp
            (transform.position, targetTransform.position, ref cameraFollowVelocity, followSpeed);
        transform.position = target;

        HandleCameraCollision();
    }

    public void HandleCameraRotation(float mouseX, float mouseY)
    {
        lookAngle += (mouseX * lookSpeed);
        pivotAngle -= (mouseY * pivotSpeed);

        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

        Vector3 rotation = Vector3.zero;
        rotation.y = lookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;

        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;

    }

    void HandleCameraCollision()
    {
        targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();



        if (Physics.SphereCast(cameraPivot.position, cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition), ignoreLayers))
        {
            float dis = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition = -(dis - cameraCollisionOffSet);


        }
        if (Mathf.Abs(targetPosition) < minimumCollisionOffSet)
        {
            targetPosition = -minimumCollisionOffSet;
        }


        cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraTransformPosition;

    }

}
