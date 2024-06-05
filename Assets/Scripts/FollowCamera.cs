using UnityEngine;

public class FollowCamera : MonoBehaviour
/// <summary>
/// Initializes the camera position and rotation, and sets the initial field of view.
/// </summary>
{
    /// <summary>
    /// The camera modes for the FollowCamera script.
    /// </summary>
    public enum CameraMode
    {
        FOLLOWING,
        ZOOMING_IN
    };

    /// <summary>
    /// The target object that the camera will follow.
    /// </summary>
    public GameObject target;

    /// <summary>
    /// The UI image component used for warping effect.
    /// </summary>
    public UnityEngine.UI.Image warpImageComponent;

    /// <summary>
    /// The current camera mode.
    /// </summary>
    public CameraMode mode = CameraMode.FOLLOWING;

    /// <summary>
    /// The field of view value for zooming.
    /// </summary>
    public float zoomFOV;

    /// <summary>
    /// The duration of the zoom in transition.
    /// </summary>
    public float zoomInDuration;

    /// <summary>
    /// The duration of the zoom out transition.
    /// </summary>
    public float zoomOutDuration;

    // Following Camera State Variables
    private readonly float smoothSpeed = 2.5f;
     // Camera distance from the player
    private Vector3 cameraOffset = new(5.0f, 8f, 5.0f);
     // Distance from the ground to the player's head
    private Vector3 targetOffset = new(0.0f, 3.0f, 0.0f);

    private Camera mainCamera;
    private ParticleSystem myParticleSystem;
    private float initialFOV;
    private float currentTime = 0.0f;
    private float transitionTime = 0.0f;
    private readonly float rotationMultiplier = 90.0f;
    private readonly float zoomMultiplier = 0.995f;
    private MyPlayerMovement movementScript;

    void Awake()
    {
        ServiceLocator.Instance.RegisterFollowCamera(this);
    }
    
    /// <summary>
    /// Called before the first frame update.
    /// Initializes the camera position and rotation, stops the particle system, and sets the initial field of view.
    /// </summary>
    void Start()
    {
        movementScript = target.GetComponent<MyPlayerMovement>();

        // get the main camera object, set its position and rotation
        mainCamera = Camera.main;
        mainCamera.transform.position = target.transform.position + new Vector3(0.0f, 100.0f, 0.0f);
        Quaternion targetRotation = Quaternion.LookRotation(target.transform.position + targetOffset - mainCamera.transform.position);
        mainCamera.transform.rotation = Quaternion.RotateTowards(mainCamera.transform.rotation, targetRotation, smoothSpeed);

        myParticleSystem = target.GetComponent<ParticleSystem>();
        myParticleSystem.Stop();

        warpImageComponent.enabled = false;

        initialFOV = mainCamera.fieldOfView;
    }

    /// <summary>
    /// Updates the camera position and rotation based on the current camera mode.
    /// </summary>
    void LateUpdate()
    {
        switch (mode)
        {
            case CameraMode.FOLLOWING:
                FollowUpdate();
                break;
            case CameraMode.ZOOMING_IN:
                ZoomInUpdate();
                break;
        }
    }

    /// <summary>
    /// Sets the camera mode to zoom in.
    /// </summary>
    public void SetZoomInMode()
    {
        mode = CameraMode.ZOOMING_IN;
        currentTime = 0.0f;
        transitionTime = zoomInDuration;
        warpImageComponent.enabled = true;

        myParticleSystem.Play();

        // set the warp image to transparent
        Color warpImageColor = warpImageComponent.color;
        warpImageColor.a = 0.0f;
        warpImageComponent.color = warpImageColor;
    }

    /// <summary>
    /// Updates the camera position and rotation in following mode.
    /// </summary>
    void FollowUpdate()
    {
        // Smoothly move the camera towards the desired position
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, target.transform.position + cameraOffset, smoothSpeed);

        // Rotate the camera to look at the player
        Quaternion targetRotation = Quaternion.LookRotation(target.transform.position + targetOffset - mainCamera.transform.position);
        mainCamera.transform.rotation = Quaternion.RotateTowards(mainCamera.transform.rotation, targetRotation, smoothSpeed);
    }

    /// <summary>
    /// Updates the camera position, rotation, and field of view during zoom in transition.
    /// </summary>
    void ZoomInUpdate()
    {
        Vector3 targetPosition = target.transform.position + targetOffset;

        if (currentTime < transitionTime)
        {
            // get the new camera location based on rotating it's current position around the target
            Vector3 direction = mainCamera.transform.position - target.transform.position;
            direction = Quaternion.Euler(0, Time.deltaTime * rotationMultiplier, 0) * direction;
            direction *= zoomMultiplier;
            // also make sure it is facing the target
            mainCamera.transform.SetPositionAndRotation(target.transform.position + direction,
                Quaternion.LookRotation(targetPosition - mainCamera.transform.position));

            mainCamera.fieldOfView = Mathf.LerpUnclamped(initialFOV, zoomFOV, currentTime / transitionTime);

            // rotate the particle system to face the camera
            var shapeModule = myParticleSystem.shape;
            shapeModule.rotation = Quaternion.LookRotation(mainCamera.transform.position - targetPosition).eulerAngles;

            // rotate the warp image to give the illusion of warping
            warpImageComponent.transform.Rotate(0, 0, 5);
            // set warp image to fade in
            Color warpImageColor = warpImageComponent.color;
            warpImageColor.a = Mathf.Lerp(0.0f, 1.0f, currentTime / transitionTime);
            warpImageComponent.color = warpImageColor;

            currentTime += Time.fixedDeltaTime;
        }
        else
        {
            movementScript.shift = false;

            if (warpImageComponent != null)
            {
                warpImageComponent.enabled = false;
            }
            mode = CameraMode.FOLLOWING;
            if (myParticleSystem != null)
            {
                myParticleSystem.Stop();
            }
            mainCamera.fieldOfView = initialFOV;
        }
    }

}
// Path: Assets/Scripts/FollowCamera.cs