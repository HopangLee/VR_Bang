using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    #region Private Fields

    /*
    [Tooltip("The distance in the local x-z plane to the target")]
    [SerializeField]
    private float distance = 7.0f;


    [Tooltip("The height we want the camera to be above the target")]
    [SerializeField]
    private float height = 3.0f;


    [Tooltip("The Smooth time lag for the height of the camera.")]
    [SerializeField]
    private float heightSmoothLag = 0.3f;


    [Tooltip("Allow the camera to be offseted vertically from the target, for example giving more view of the sceneray and less ground.")]
    [SerializeField]
    private Vector3 centerOffset = Vector3.zero;
    */

    [Tooltip("Set this as false if a component of a prefab being instanciated by Photon Network, and manually call OnStartFollowing() when and if needed.")]
    [SerializeField]
    private bool followOnStart = false;


    // cached transform of the target
    Transform cameraTransform;


    // maintain a flag internally to reconnect if target is lost or camera is switched
    bool isFollowing;

    private Vector3 offset = new Vector3(0f, 1.8f, 0f);

    /*
    // Represents the current velocity, this value is modified by SmoothDamp() every time you call it.
    private float heightVelocity;


    // Represents the position we are trying to reach using SmoothDamp()
    private float targetHeight = 100000.0f;
    */

    #endregion

    #region Public Field
    //public Transform head;
    public float mouseSensitivity = 100f;
    public float xRotation = 0f;
    public float yRotation = 0f;
    #endregion

    #region MonoBehaviour CallBacks


    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization phase
    /// </summary>
    void Start()
    {
        // Start following the target if wanted.
        if (followOnStart)
        {
            
            OnStartFollowing();
        }
    }


    /// <summary>
    /// MonoBehaviour method called after all Update functions have been called. This is useful to order script execution. For example a follow camera should always be implemented in LateUpdate because it tracks objects that might have moved inside Update.
    /// </summary>
    void LateUpdate()
    {
        // The transform target may not destroy on level load,
        // so we need to cover corner cases where the Main Camera is different everytime we load a new scene, and reconnect when that happens
        if (cameraTransform == null && isFollowing)
        {
            OnStartFollowing();
        }
        // only follow is explicitly declared
        if (isFollowing)
        {
            Apply();
        }
    }


    #endregion


    #region Public Methods


    /// <summary>
    /// Raises the start following event.
    /// Use this when you don't know at the time of editing what to follow, typically instances managed by the photon network.
    /// </summary>
    public void OnStartFollowing()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraTransform = Camera.main.transform;
        isFollowing = true;
        // we don't smooth anything, we go straight to the right camera shot
        //Cut();
    }


    #endregion


    #region Private Methods


    /// <summary>
    /// Follow the target smoothly
    /// </summary>
    void Apply()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        yRotation += mouseX;

        /*
        cameraTransform.rotation = Quaternion.Euler(xRotation, 0f, 0f);
        cameraTransform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        */
        cameraTransform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        this.transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

        cameraTransform.position = this.transform.position + offset;
        //transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }


    #endregion

}
