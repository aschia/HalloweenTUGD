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
        getready = 3,
    }
    // vars
    int gameState = (int)gStates.listen;
    public List<NoteSheet> gameNotes = new List<NoteSheet>();
    public List<int> roundNotes = new List<int>();
    public List<int> plrNotes = new List<int>();
    public List<AudioClip> sfx = new List<AudioClip>();
    public List<AudioClip> result_sfx = new List<AudioClip>();
    List<KeyCode> plrKeyNames = new List<KeyCode> { KeyCode.Alpha1,KeyCode.Alpha2,KeyCode.Alpha3,KeyCode.Alpha4,KeyCode.Alpha5 };

    int roundIndex = 0;
    int noteIndex = -1;
    float roundTimer = 0f;
    float roundTick = 0f;
    float lastRoundTick = 0f;
    float leeway = 0.5f;
    int score = 0;
    public int scoreRequired = 10;

    // components
    Image host = null;
    Vector3 starthostpos = Vector3.zero;
    GameObject hostKeys = null;
    GameObject plrKeys = null;
    GameObject gameText = null;
    GameObject scoreText = null;
    [SerializeField]
    List<Sprite> keySprites = new List<Sprite>();
    [SerializeField]
    List<Sprite> hostSprites = new List<Sprite>();
    [SerializeField]
    AudioSource audKey = null;
    [SerializeField]
    AudioSource audTick = null;

    // Start is called before the first frame update
    void Start()
    {
        Transform cnv = transform.GetChild(0);
        host = cnv.GetChild(1).GetComponent<Image>();
        starthostpos = host.transform.position;
        hostKeys = cnv.GetChild(2).gameObject;
        plrKeys = cnv.GetChild(3).gameObject;
        gameText = cnv.GetChild(0).gameObject;
        scoreText = cnv.GetChild(4).gameObject;
        //audKey = GetComponent<AudioSource>();
        RoundSetup();
    }

    // setup
    void RoundSetup()
    {
        // find out if we done with game
        if (roundIndex >= gameNotes.Count)
        {
            return;
        }

        // grab stuff
        NoteSheet ns = gameNotes[roundIndex];
        roundNotes = ns.notes;
        roundTimer = ns.time;
        roundTick = roundTimer / roundNotes.Count;
        lastRoundTick = roundTimer + roundTick;
        leeway = roundTick * 0.5f;

        gameText.GetComponent<Text>().text = "Listen to the Notes!";
    }

    // Update is called once per frame
    void Update()
    {
        // exit if in results screen
        if (gameState == (int)gStates.results)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Destroy(gameObject);
                return;
            }
            else return;
        }

        // update score
        scoreText.GetComponent<Text>().text = "[Score]\n" + score;

        // decrease timer
        roundTimer -= Time.deltaTime;
        if (roundTimer <= 0)
        {
            noteIndex = -1;
            if (gameState == (int)gStates.listen)
            {
                host.GetComponent<Image>().sprite = hostSprites[0];
                for (int i = 0; i < hostKeys.transform.childCount; i++)
                {
                    Transform tr = hostKeys.transform.GetChild(i);
                    tr.GetComponent<Image>().sprite = keySprites[0];
                    plrKeys.transform.GetChild(i).GetComponent<Image>().sprite = keySprites[0];
                }

                roundTimer = gameNotes[roundIndex].time;
                roundTick = roundTimer/gameNotes[roundIndex].notes.Count;
                roundTimer = roundTick * 4;
                lastRoundTick = roundTimer + roundTick;

                gameState = (int)gStates.getready;

                host.transform.position = starthostpos;
                gameText.GetComponent<Text>().text = "Get Ready!";
            }
            else if (gameState == (int)gStates.getready)
            {
                roundTimer = gameNotes[roundIndex].time;
                lastRoundTick = roundTimer + roundTick;
                gameState = (int)gStates.play;
                host.transform.position = starthostpos;
                gameText.GetComponent<Text>().text = "Repeat the Notes!";
            }
            else
            {
                roundIndex++;
                if (roundIndex >= gameNotes.Count)
                {
                    gameState = (int)gStates.results;
                    string txt = "You Win!\n+1 Candy!";
                    if (score >= scoreRequired)
                    {
                        Player.AwardCandy();
                        audKey.clip = result_sfx[0];
                    }
                    else
                    {
                        audKey.clip = result_sfx[1];
                        txt = "You Lose...\nScore Needed: " + scoreRequired;
                    }
                    audKey.Play();
                    gameText.GetComponent<Text>().text = txt;
                    return;
                }
                else
                {
                    gameState = (int)gStates.listen;

                    for (int i = 0; i < hostKeys.transform.childCount; i++)
                    {
                        Transform tr = hostKeys.transform.GetChild(i);
                        tr.GetComponent<Image>().sprite = keySprites[0];
                        plrKeys.transform.GetChild(i).GetComponent<Image>().sprite = keySprites[0];
                    }

                    RoundSetup();
                }
            }
        }
        // try to tick
        if (roundTimer < lastRoundTick-roundTick)
        {
            // play the metronome noise
            audTick.Play();
            lastRoundTick = (lastRoundTick - roundTick);
            // get note to play
            /*float timeratio = (lastRoundTick / gameNotes[roundIndex].time);
            Debug.Log("timeratio: " + timeratio);
            int a = (int)((1f-timeratio) * roundNotes.Count);
            Debug.Log("a: " + a);
            int noteIndex = (a);*/
            noteIndex++;

            // try to play note
            if (gameState == (int)gStates.listen)
            {
                // play the note
                int note = roundNotes[noteIndex];
                if (note != 0) {
                    audKey.clip = sfx[note-1];
                    audKey.Play();
                    // move the host
                    Transform pos = hostKeys.transform.GetChild(note-1).GetComponent<RectTransform>().transform;
                    float posX = pos.position.x + (hostKeys.transform.GetChild(note-1).GetComponent<RectTransform>().rect.width / 2) + 8;
                    Debug.Log("posX: " + posX);
                    host.transform.position = new Vector3(posX, starthostpos.y - 40);

                    for (int i = 0; i < hostKeys.transform.childCount; i++)
                    {
                        Transform tr = hostKeys.transform.GetChild(i);
                        if (i == note-1) tr.GetComponent<Image>().sprite = keySprites[1];
                        else tr.GetComponent<Image>().sprite = keySprites[0];
                        host.GetComponent<Image>().sprite = hostSprites[1];
                    }
                }
                if (note == 0 || noteIndex >= roundNotes.Count)
                {
                    for (int i = 0; i < hostKeys.transform.childCount; i++)
                    {
                        Transform tr = hostKeys.transform.GetChild(i);
                        tr.GetComponent<Image>().sprite = keySprites[0];
                        host.GetComponent<Image>().sprite = hostSprites[0];
                    }
                }
            }
            else if (gameState == (int)gStates.play)
            {
                // find out if round is over
                /*if (noteIndex >= roundNotes.Count)
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
                }*/
            }
            else if (gameState == (int)gStates.getready)
            {
                int count = (int)((lastRoundTick) / (roundTick));
                Debug.Log(count);
                gameText.GetComponent<Text>().text = "Get Ready!\n["+count+"]";
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

                if (Input.GetKey(plrKeyNames[i]) )
                {
                    plrKeys.transform.GetChild(i).GetComponent<Image>().sprite = keySprites[1];
                }
                else
                {
                    plrKeys.transform.GetChild(i).GetComponent<Image>().sprite = keySprites[0];
                }
            }

            // see if a button was pressed
            if (buttonInput != -1) 
            {
                // add button to list
                /*float timeratio = (lastRoundTick / gameNotes[roundIndex].time);
                Debug.Log("timeratio: " + timeratio);
                int a = (int)((1f - timeratio) * roundNotes.Count);
                int noteIndex = roundNotes[a];*/
                plrNotes.Add(buttonInput);

                audKey.clip = sfx[buttonInput];
                audKey.Play();

                // check if button was correct
                if (noteIndex >= roundNotes.Count) Debug.Log("aaaaa");
                if (buttonInput == roundNotes[noteIndex]-1)
                {
                    // figure out timing
                    if (Mathf.Abs(roundTimer - lastRoundTick) <= leeway) score++;
                    else Debug.Log("wrong timing on note ind " + noteIndex);
                }
                else
                {
                    Debug.Log("wrong note on note ind " + noteIndex);
                }
            }
        }
    }
}
