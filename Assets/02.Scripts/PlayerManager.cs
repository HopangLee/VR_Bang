using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


/// <summary>
/// Player manager.
/// Handles fire Input and Beams.
/// </summary>
public class PlayerManager : MonoBehaviourPunCallbacks
{
    #region Private Fields
    private CharacterController controller;

    private float speed = 5f;
    private float gravity = -9.8f;
    private Vector3 velocity = new Vector3(0, -2f, 0);
    #endregion



    #region MonoBehaviour CallBacks

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
    /// </summary>
    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization phase.
    /// </summary>
    void Start()
    {
        CameraCtrl _cameraCtrl = this.gameObject.GetComponent<CameraCtrl>();
        if (_cameraCtrl != null)
        {
            if (photonView.IsMine)
            {
                _cameraCtrl.OnStartFollowing();
            }
        }
        else
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
        }
    }

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity on every frame.
    /// </summary>
    void Update()
    {
        // DEBUG test
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

       
        // 중력
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    #endregion


    #region Custom

    #endregion
}
