using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class GrabCube : MonoBehaviour
{
    private Interactable interactable;
    public GameObject head;

    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<Interactable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void onHandHoverBegin(Hand hand)
    {
        hand.ShowGrabHint();
    }

    private void onHandHoverEnd(Hand hand)
    {
        hand.HideGrabHint();
    }

    private void HandHoverUpdate(Hand hand) {
        GrabTypes grabType = hand.GetGrabStarting();
        bool isGrabEnding = hand.IsGrabEnding(gameObject);

        // GRAB THE OBJECT
        if(interactable.attachedToHand == null && grabType != GrabTypes.None)
        {
            hand.AttachObject(gameObject, grabType);
            hand.HoverLock(interactable);
            hand.HideGrabHint();

            Debug.Log(transform.rotation);
        }
        // RELEASE
        else if(isGrabEnding)
        {
            hand.DetachObject(gameObject);
            hand.HoverUnlock(interactable);
        }
    }
}
