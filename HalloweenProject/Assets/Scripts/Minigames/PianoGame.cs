using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// note container
[System.Serializable]
public class NoteSheet
{
    public List<int> notes = new List<int>();
    public float time = 3f;
}

public class PianoGame : Minigame
{
    // state constants
    enum gStates
    {
        listen = 0,
        play = 1,
        results = 2,
    }
    // vars
    int gameState = (int)gStates.listen;
    public List<NoteSheet> gameNotes = new List<NoteSheet>();
    public List<int> roundNotes = new List<int>();
    public List<int> plrNotes = new List<int>();
    public List<AudioClip> sfx = new List<AudioClip>();
    List<KeyCode> plrKeyNames = new List<KeyCode> { KeyCode.Alpha1,KeyCode.Alpha2,KeyCode.Alpha3,KeyCode.Alpha4,KeyCode.Alpha5 };

    int roundIndex = 0;
    float roundTimer = 0f;
    float roundTick = 0f;
    float lastRoundTick = 0f;
    float leeway = 0;
    int score = 0;

    // components
    Image host = null;
    GameObject hostKeys = null;
    GameObject plrKeys = null;
    GameObject gameText = null;
    [SerializeField]
    AudioSource audKey = null;
    [SerializeField]
    AudioSource audTick = null;

    // Start is called before the first frame update
    void Start()
    {
        Transform cnv = transform.GetChild(0);
        host = cnv.GetChild(1).GetComponent<Image>();
        hostKeys = cnv.GetChild(2).gameObject;
        plrKeys = cnv.GetChild(3).gameObject;
        gameText = cnv.GetChild(0).gameObject;
        //audKey = GetComponent<AudioSource>();
        RoundSetup();
    }

    // setup
    void RoundSetup()
    {
        // find out if we done with game
        if (roundIndex >= gameNotes.Count)
        {

        }

        // grab stuff
        NoteSheet ns = gameNotes[roundIndex];
        roundNotes = ns.notes;
        roundTimer = ns.time;
        roundTick = roundTimer / roundNotes.Count;
        lastRoundTick = roundTimer + roundTick;
        leeway = roundTimer * 0.33f;
    }

    // Update is called once per frame
    void Update()
    {
        // exit if in results screen
        if (gameState == (int)gStates.results) return;

        // decrease timer
        roundTimer -= Time.deltaTime;
        // try to tick
        if (roundTimer < lastRoundTick-roundTick)
        {
            // play the metronome noise
            audTick.Play();
            lastRoundTick = (lastRoundTick - roundTick);
            // get note to play
            int noteIndex = 1 / (roundNotes[(int)(lastRoundTick / gameNotes[roundIndex].time) * roundNotes.Count]);
            // try to play note
            if (gameState == (int)gStates.listen)
            {
                // find out if time to stop
                if (noteIndex >= roundNotes.Count) gameState = (int)gStates.play;
                // play the note
                audKey.clip = sfx[noteIndex];
                audKey.Play();
            }
            else if (gameState == (int)gStates.play)
            {
                // find out if round is over
                if (noteIndex >= roundNotes.Count)
                {
                    // increment round number
                    roundIndex++;
                    // find out if game is over
                    if (roundTimer >= gameNotes.Count)
                    {
                        // go to results
                        gameState = (int)gStates.results;
                    }
                    else
                    {
                        // reset for round
                        plrNotes = new List<int>();
                        RoundSetup();
                    }
                }
            }
        }
        // try to take inputs
        if (gameState == (int)gStates.play)
        {
            // get player input
            int buttonInput = -1;
            for (int i = 0; i < plrKeyNames.Count; i++)
            {
                // find out which key is pressed
                if (Input.GetKeyDown(plrKeyNames[i]))
                {
                    buttonInput = i;
                }
            }
            // see if a button was pressed
            if (buttonInput != -1) 
            {
                // add button to list
                /*int noteIndex = 1 / (roundNotes[(int)(lastRoundTick / gameNotes[roundIndex].time) * roundNotes.Count]);
                plrNotes.Add(buttonInput);
                // check if button was correct
                if (noteIndex >= roundNotes.Count) Debug.Log("aaaaa");
                if (buttonInput == roundNotes[noteIndex])
                {
                    // figure out timing
                    if (Mathf.Abs(roundTimer - lastRoundTick) <= leeway) score++;
                }*/
            }
        }
    }
}
