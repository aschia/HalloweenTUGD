using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class DialogueZone : MonoBehaviour
{
    //vars
    [SerializeField]
    BoxCollider2D bcoll = null;                                 // the box collider for the zone

    public List<string> text;
    public bool repeat = true;

    public static DialogueZone previousDiaZone = null;         // storage for the previously touched dialogue zone
    static float TIME_REACTIVATE = 0.25f;                      // time until a previously used dia zone tries to become active again



    // Start is called before the first frame update
    void Start()
    {
        if (bcoll == null) bcoll = GetComponent<BoxCollider2D>();
    }

    // reactivate collider
    void RetryCollider()
    {
        if (repeat) bcoll.enabled = true;
    }

    // collision with player
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collided with DialogueZone");

        if (other.gameObject.GetComponent<Player>() == null) return;

        // check to make sure this isn't a repeat interaction
        if (this != previousDiaZone || previousDiaZone == null)
        {
            // if it isn't, lets set dialogue properties now
            if (DialogueManager.DiaManagerSingleton != null)
            {
                DialogueManager.StartDialogue(text);
                previousDiaZone = this;
                bcoll.enabled = false;
                other.gameObject.GetComponent<Player>().enabled = false;
                DialogueManager.DiaManagerSingleton.enabledOnEnd = other.gameObject;
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
        Gizmos.color = Color.blue;
        if (previousDiaZone == this) Gizmos.color = Color.green;
        // draw basic bounding box
        Gizmos.DrawWireCube(bcoll.bounds.center, bcoll.size);
        Gizmos.DrawWireSphere(transform.position, 1 / 32f);
    }
}
