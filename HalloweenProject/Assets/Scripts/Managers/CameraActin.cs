/*
 * CameraActin
 * 
 * ---
 * Manages Camera movement in both overhead follow and fixed modes b/c I still don't trust Cinemachine :/
 * ---
 * Last Edited: 10/05
 * Edited by: aschia
 * */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraActin : MonoBehaviour
{
    // vars targeting system
    public Transform target = null;
    public int cameraMode = 0;
    public Player player = null;

    public static CameraActin CameraActinSingleton = null;
    #region CameraActin Singleton
    void SetCameraActin()
    {
        if (CameraActinSingleton == null)
        {
            CameraActinSingleton = this;
        }
        else
        {
            CameraActinSingleton = null;
        }
    }
    #endregion

    // enum enum do doo da doo do
    enum _CameraModes
    {
        still = 0,      // fixed camera, only reason it's not called fix is b/c the word's reserved
        follow = 1,     // follows a given transform around the place
    }


    // setup the singleton
    public void Awake()
    {
        SetCameraActin();

        if (CameraActinSingleton == null) return;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Test stuff
        //DialogueManager.StartDialogue(new List<string> { "Hello this is a test dialogue.", "Cool." },new List<GameObject> {player.gameObject});
        //DialogueManager.DiaManagerSingleton.enabledOnEnd = player.gameObject;
        //player.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // leave update if we're doing nothing
        if (target == null) return;

        // nintendo switch statement
        switch (cameraMode)
        {
            // why do i have to make explicit casts to int for an enum switch statement aaaaaaghhhh

            case (int)_CameraModes.still:
                // don't do anything, it's not supposed to move dum dum
            break;

            case (int)_CameraModes.follow:
                // lock to position
                transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
                break;
        }
    }

    public static void SetCameraTarget(Transform newtarget)
    {
        // toggles follow cam off
        if (newtarget == null) {
            CameraActinSingleton.target = null;
            CameraActinSingleton.cameraMode = 0;
            CameraActinSingleton.transform.position = new Vector3(newtarget.transform.position.x, newtarget.transform.position.y, CameraActinSingleton.transform.position.z);
            return;
        }

        // otherwise, proceed with setting follow cam target n stuff
        CameraActinSingleton.cameraMode = 0;
        CameraActinSingleton.target = newtarget;
    }

    public static void SetCameraTarget(Transform newtarget, GameObject caller)
    {
        Debug.Log("Setting new camera target settings.");

        // toggles follow cam off
        if (newtarget == null)
        {
            CameraActinSingleton.target = null;
            CameraActinSingleton.cameraMode = 0;

            CameraActinSingleton.transform.position = new Vector3( caller.transform.position.x,caller.transform.position.y,CameraActinSingleton.transform.position.z);

            return;
        }

        // otherwise, proceed with setting follow cam target n stuff
        CameraActinSingleton.cameraMode = 1;
        CameraActinSingleton.target = newtarget;
    }
}
