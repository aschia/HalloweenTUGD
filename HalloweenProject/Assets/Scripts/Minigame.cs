/*
 * Minigame
 * 
 * ---
 * Minigame parent class
 * ---
 * 
 * */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame : MonoBehaviour
{
    public List<GameObject> minigameParticipants = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        // enable all controllers supplied to the dialoguemanager
        foreach (GameObject go in minigameParticipants)
        {
            DialogueActionable[] diact = go.GetComponents<DialogueActionable>();
            Debug.Log("diact: " + diact.ToString());
            foreach (DialogueActionable da in diact)
            {
                da.enabled = true;
            }
        }
        minigameParticipants = null;
    }
}
