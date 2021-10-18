/*
 * DialogueManager
 * 
 * ---
 * Dia he, dia ooo, dia ha, dia aah aah
 * Manages dialogue stuff. It's a singleton, so don't go instantiating it or nothing. 
 * Call it using StartDialogue(List<string> text, List<GameObject> participants)
 * ---
 * Last Edited: aschia (10/07)
 * 
 * */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    // Vars
    public List<string> text = null;
    public string text_prog = "";
    public int type_chunk = 1;
    public float type_delay = 0.125f;

    public int text_index = 0;

    public GameObject dialogueObj = null;
    public List<GameObject> dialogueParticipants = null;
    public GameObject triggeredMinigame = null;
    // ^^^ for all of the controllers that are deactivated upon starting dialogue and reactivated upon ending it

    public AudioClip[] sfx;

    public bool noiseOnType = true;

    float delay = 0;

    enum SFX_Clips
    {
        sfx_tick = 0,
        sfx_proceed = 1,
    }

    AudioSource aud;

    public static DialogueManager DiaManagerSingleton = null;
    #region DiaManagerSingleton
    void SetDiaManagerSingleton()
    {
        if (DiaManagerSingleton == null)
        {
            DiaManagerSingleton = this;
        }
        else
        {
            DiaManagerSingleton = null;
        }
    }
    #endregion

    private void Awake()
    {
        SetDiaManagerSingleton();

        if (DiaManagerSingleton == null) return;
    }

    // Start is called before the first frame update
    void Start()
    {
        delay = type_delay;
        aud = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!dialogueObj.activeSelf)
        {
            return;
        }

        // Call debug inputs
        DebugInput();

        // Determine if we're typing
        bool typing = true;
        if (text_index >= text.Count)
        {
            // Reached the end of typing
            Debug.Log("DialogueManager: Finished typing and showing text.");
            // We're done
            typing = false;
            // Clear the text
            UpdateText();
            // Disable overlay
            dialogueObj.SetActive(false);
            text_index = 0;
            text = null;
            text_prog = "";
            // try to start minigame
            if (triggeredMinigame != null)
            {
                GameObject tm = Instantiate(triggeredMinigame);
                tm.GetComponent<Minigame>().minigameParticipants = dialogueParticipants;
            }
            else
            {
                // enable all controllers supplied to the dialoguemanager
                foreach (GameObject go in dialogueParticipants)
                {
                    DialogueActionable[] diact = go.GetComponents<DialogueActionable>();
                    Debug.Log("diact: " + diact.ToString());
                    foreach (DialogueActionable da in diact)
                    {
                        da.enabled = true;
                    }
                }
                dialogueParticipants = null;
            }
            // clear minigame
            triggeredMinigame = null;
            /*if (enabledOnEnd.GetComponent<Player>() != null) {
                enabledOnEnd.GetComponent<Player>().enabled = true;
            }
            else
            {
                enabledOnEnd.SetActive(true);
                enabledOnEnd = null;
            }*/
            return;
        }

        // Arrow input
        bool pressed = Input.GetKeyDown(KeyCode.Space);
        // Check input
        if (pressed)
        {
            // Figure out whether to skip or move forward
            if (text_prog != text[text_index]) // Skip to end of typing this text entry
            {
                // Send feedback
                Debug.Log("DialogueManager: Skipped typing.");
                // Reset text prog
                text_prog = text[text_index];
                // Update text display
                UpdateText();
                return;
            }
            else // Move to the next index
            {
                // Send feedback
                Debug.Log("DialogueManager: Moved to text_index " + (text_index + 1));
                // Increment dialogue index
                text_index++;
                // Disable arrow
                dialogueObj.transform.GetChild(1).gameObject.SetActive(false);
                // Reset text prog
                text_prog = "";
                // Update text display
                UpdateText();
                // Play sfx
                PlaySFX((int)SFX_Clips.sfx_proceed);
                return;
            }
        }

        // We're typing 
        if (typing)
        {
            // Make sure we haven't finished typing this section
            if (text_prog != text[text_index])
            {
                // Count down until next character  
                if (delay > 0)
                {
                    // Countdown
                    delay -= Time.deltaTime;
                }
                else
                {
                    // Add a character
                    int char_ind = text_prog.Length;
                    int max_len = text[text_index].Length;
                    int delay_multi = 1;
                    if (char_ind + type_chunk < max_len)
                    {
                        // Check for tags
                        if (text[text_index][char_ind] == '<' && text[text_index].LastIndexOf('v') != char_ind)
                        {
                            // Grab tag and relevant info
                            string rem = text[text_index].Substring(char_ind, max_len - char_ind);
                            int end_pos = text[text_index].IndexOf(">", char_ind) + 1;
                            int tag_len = end_pos - char_ind;
                            string tag = text[text_index].Substring(char_ind, tag_len);
                            // Feedback
                            //Debug.Log("current char_ind: " + char_ind);
                            //Debug.Log(string.Format("Remainder of string: {0}, char_ind: {1}, end_pos: {2}, padding: {3}, tag: {4}", rem, char_ind, end_pos, tag_len, tag));
                            // Skip past the tag in the typewritter text
                            text_prog += tag;
                            char_ind = text_prog.Length;
                        }
                        else
                        {
                            // Determine type chunk
                            int chunk = type_chunk;
                            string chunk_text = text[text_index].Substring(char_ind, chunk);
                            // Determine delay
                            if (chunk_text.Contains(",") || chunk_text.Contains(";") || chunk_text.Contains(":")) delay_multi = 3;
                            else if (chunk_text.Contains(".") || chunk_text.Contains("?") || chunk_text.Contains("!") || chunk_text.Contains("~")) delay_multi = 6;
                            // Add chars to gradual text
                            text_prog += chunk_text;
                        }
                    }
                    else
                    {
                        text_prog = text[text_index];
                    }
                    // Update text display
                    UpdateText();
                    // Reset delay
                    delay = type_delay * delay_multi;

                    // Play noise if applicable
                    if (noiseOnType)
                    {
                        PlaySFX((int)SFX_Clips.sfx_tick);
                    }
                }
            }
            else // Move up an index if a button is pressed
            {
                // Enable next arrow
                GameObject arrow = dialogueObj.transform.GetChild(1).gameObject;
                if (!arrow.activeSelf) arrow.SetActive(true);
            }
        }
    }

    void DebugInput()
    {
        // For testing purposes
        bool debug_loop = Input.GetKey(KeyCode.C) && Input.GetKeyDown(KeyCode.UpArrow);
        if (debug_loop)
        {
            Debug.Log("DialogueManager(Debug): Restarted typewritter. ");
            text_prog = "";
            UpdateText();
        }
        bool debug_back = Input.GetKey(KeyCode.C) && Input.GetKeyDown(KeyCode.LeftArrow);
        if (debug_back && text_index > 0)
        {
            Debug.Log(string.Format("DialogueManager(Debug): Text_index went backwards from {0} and is now at {1}.", text_index, text_index - 1));
            text_prog = "";
            text_index--;
            UpdateText();
        }
        bool debug_skip = (Input.GetKey(KeyCode.C) && Input.GetKeyDown(KeyCode.DownArrow)) || (Input.GetKeyDown(KeyCode.Escape));
        if (debug_skip)
        {
            Debug.Log(string.Format("DialogueManager(Debug): Text_index skipped to its end index, {0}.", text.Count));
            text_prog = "";
            text_index = text.Count;
            UpdateText();
        }
    }

    // Update the text
    void UpdateText()
    {
        // Make sure overlay is enabled
        if (!dialogueObj.activeSelf) dialogueObj.SetActive(true);
        // Grab text object
        Text tx = dialogueObj.transform.GetChild(0).gameObject.GetComponent<Text>();
        // Construct string
        string txt = "";
        string tag_end = "</color>";
        bool found_tag = false;
        bool used_tag = false;
        // Go thru text_prog and build display string
        for (int i = 0; i < text_prog.Length; i++)
        {
            string tag = "";
            if (text[text_index][i] == '<')
            {
                int end_pos = text[text_index].IndexOf(">", i) + 1;
                int tag_len = end_pos - i;
                tag = text[text_index].Substring(i, tag_len);
                found_tag = true;
            }
            // Try to add tag
            if (tag != "")
            {
                // Add the tag
                if (tag[1] != '/')
                {
                    txt += tag;
                    i += tag.Length;
                    txt += text[text_index][i];
                }
                else
                {
                    txt += tag;
                    i += tag.Length;
                    used_tag = true;
                }
            }
            else
            {
                // Don't use tag
                txt += text_prog[i];
            }

            // Append a tag ender if one hasn't been reached yet in the text_prog proper
            if (i >= text_prog.Length - 1 && (found_tag && !used_tag))
            {
                txt += tag_end;
                i += tag_end.Length;
            }
        }
        // Feedback
        //Debug.Log("Display Output: " + txt);
        //Debug.Log("Current text_prog: " + text_prog);
        // Set text
        tx.text = txt;
    }

    // Creates a dialogue
    public static void StartDialogue(List<string> dialogueText)
    {
        // Sets text
        DiaManagerSingleton.text = dialogueText;
        DiaManagerSingleton.text_prog = "";
        // Enable dialogue
        if (!DiaManagerSingleton.dialogueObj.activeSelf) DiaManagerSingleton.dialogueObj.SetActive(true);
    }

    // Creates a dialogue
    public static void StartDialogue(List<string> dialogueText, List<GameObject> participants)
    {
        // Sets text
        DiaManagerSingleton.text = dialogueText;
        DiaManagerSingleton.text_prog = "";
        // Enable dialogue
        if (!DiaManagerSingleton.dialogueObj.activeSelf) DiaManagerSingleton.dialogueObj.SetActive(true);
        // disable relevant controllers
        foreach (GameObject go in participants)
        {
            DialogueActionable[] diact = go.GetComponents<DialogueActionable>();
            foreach (DialogueActionable da in diact)
            {
                da.enabled = false;
            }
        }
        // store the participants for later
        DiaManagerSingleton.dialogueParticipants = participants;
    }

    // Creates a dialogue
    public static void StartDialogue(List<string> dialogueText, List<GameObject> participants, GameObject minigame)
    {
        // Sets text
        DiaManagerSingleton.text = dialogueText;
        DiaManagerSingleton.text_prog = "";
        // Enable dialogue
        if (!DiaManagerSingleton.dialogueObj.activeSelf) DiaManagerSingleton.dialogueObj.SetActive(true);
        // disable relevant controllers
        foreach (GameObject go in participants)
        {
            DialogueActionable[] diact = go.GetComponents<DialogueActionable>();
            foreach (DialogueActionable da in diact)
            {
                da.enabled = false;
            }
        }
        // store the participants for later
        DiaManagerSingleton.dialogueParticipants = participants;
        if (minigame != null) DiaManagerSingleton.triggeredMinigame = minigame;
    }

    // Plays sfx
    void PlaySFX(int sfx_index)
    {
        // Don't play sfx if already playing one
        if (aud.isPlaying && aud.clip == sfx[(int)SFX_Clips.sfx_proceed]) return;

        // Play the sfx
        aud.clip = sfx[sfx_index];
        aud.Play();
    }
}
