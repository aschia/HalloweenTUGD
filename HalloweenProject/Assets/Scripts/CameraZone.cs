/*
 * CameraZone
 * 
 * ---
 * Upon collision with Player, tells CameraActin what it should be doing
 * ---
 * Last Edited: 10/05
 * Edited by: aschia
 * */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CameraZone : MonoBehaviour
{
    // vars
    public int newCameraMode = 0;                               // the camera mode to use upon being touched (0 for fixed, 1 for follow)
    public Transform newCameraTarget = null;                    // the camera target to use upon being touched (null for no target & fixed camera)

    [SerializeField]
    BoxCollider2D bcoll = null;                                 // the box collider for the zone

    public static CameraZone previousCameraZone = null;         // storage for the previously touched camera zone
    static float TIME_REACTIVATE = 0.25f;                       // time until a previously used camera zone tries to become active again

    // Start is called before the first frame update
    void Start()
    {
        if(bcoll == null) bcoll = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (previousCameraZone != null) Debug.Log("prev zone: " + previousCameraZone);
    }

    // reactivate collider
    void RetryCollider()
    {
        bcoll.enabled = true;
    }

    // collision with player
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collided with CameraZone");

        if (other.gameObject.GetComponent<Player>() == null) return;

        // check to make sure this isn't a repeat interaction
        if (this != previousCameraZone || previousCameraZone == null)
        {
            // if it isn't, lets set camera properties now
            if (CameraActin.CameraActinSingleton != null) {
                CameraActin.SetCameraTarget(newCameraTarget, this.gameObject);
                previousCameraZone = this;
                bcoll.enabled = false;
                Invoke("RetryCollider", TIME_REACTIVATE);
            }
        }
        else
        {
            // we can disable the collider and try again later
            bcoll.enabled = false;
            Invoke("RetryCollider", TIME_REACTIVATE);
        }
    }

    // show gizmo zones
    void OnDrawGizmos()
    {
        // determine color
        Gizmos.color = Color.yellow;
        if (newCameraMode == 0 || newCameraTarget == null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(bcoll.bounds.min, bcoll.bounds.max);
            Vector3 secondlinemin = new Vector3(bcoll.bounds.min.x, bcoll.bounds.min.y + bcoll.bounds.size.y);
            Vector3 secondlinemax = new Vector3(bcoll.bounds.max.x, bcoll.bounds.max.y - bcoll.bounds.size.y);
            Gizmos.DrawLine(secondlinemin, secondlinemax);
        }
        if (previousCameraZone == this) Gizmos.color = Color.green;
        // draw basic bounding box
        Gizmos.DrawWireCube(bcoll.bounds.center, bcoll.size);
        Gizmos.DrawWireSphere(transform.position, 1/32f);
    }

    // show gizmo connections
    void OnDrawGizmosSelected()
    {
        // determine whether we need connections
        if (newCameraTarget == null) return;
        // k we do, figure out the color
        Gizmos.color = Color.yellow;
        if (previousCameraZone == this) Gizmos.color = Color.green;
        // draw a line between this and the newCameraTarget
        Gizmos.DrawLine(transform.position, newCameraTarget.position);
    }
}
