using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : DialogueActionable
{
    // vars
    //private SpriteRenderer spRend;
    //private int sprInd = 0;

    public List<Diatext> dialogueText = new List<Diatext>();

    public int timesTalked = 0;

    //[SerializeField]
    //List<Sprite> faceSprites;
    [SerializeField]
    int defaultFace = 0;                // manages the direction the npc faces by default (0=down/front,1=right,2=up/back,3=left)
    [SerializeField]
    bool defaultFacePriority = false;   // whether or not the npc should return to their default face direction upon ending dialogue

    GameObject touchPlayer = null;


    // Start is called before the first frame update
    void Start()
    {
        if (spRend == null) spRend = transform.GetChild(0).GetComponent<SpriteRenderer>();

        if (defaultFace == 3)
        {
            spRend.gameObject.transform.localScale = new Vector3(-1, 1, 1);
            sprIndex = 1;
        }
        else sprIndex = defaultFace;
        UpdateSprite();
    }

    public void OnEnable()
    {
        if (spRend == null) return;

        if (defaultFacePriority)
        {
            if (defaultFace == 3)
            {
                spRend.gameObject.transform.localScale = new Vector3(-1, 1, 1);
                sprIndex = 1;
            }
            else sprIndex = defaultFace;
            UpdateSprite();
        }
        if (timesTalked < dialogueText.Count) timesTalked++;
    }

    // Update is called once per frame
    void Update()
    {
        // if it isn't, lets set dialogue properties now
        if (touchPlayer != null && Input.GetKeyDown(KeyCode.E) && DialogueManager.DiaManagerSingleton != null && DialogueManager.DiaManagerSingleton.text == null)
        {
            // set up var for holding image flip upon horizontal interactiones
            int newInd = -1;
            Vector3 newScale = new Vector3(1, 1, 1);

            // determine face direction (should face opposite of the direction player is facing)
            if (touchPlayer.GetComponent<Player>().sprIndex == 0) newInd = 2;   // player facing down? npc face upwards
            if (touchPlayer.GetComponent<Player>().sprIndex == 1) {
                newInd = 1;                                                     // player facing left/right? npc face right/left
                newScale.x = 1 * -touchPlayer.transform.GetChild(0).transform.localScale.x;
            }
            if (touchPlayer.GetComponent<Player>().sprIndex == 2) newInd = 0;   // player facing up? npc face down

            // for changing face
            if (newInd != -1)
            {
                // reset to default scale real quick since we'll be changing stuff
                spRend.gameObject.transform.localScale = new Vector3(1, 1, 1);
                // update the sprite image
                sprIndex = newInd;
                UpdateSprite();
            }
            // for facing left/right
            if (newScale.x != 1) spRend.gameObject.transform.localScale = newScale;

            // update player stuff
            touchPlayer.GetComponent<Player>().imgIndex = 0;
            touchPlayer.GetComponent<Player>().UpdateSprite();

            // let's start the actual text stuff
            List<string> diaText = null;
            if (timesTalked >= dialogueText.Count) diaText = dialogueText[dialogueText.Count-1].text;
            else diaText = dialogueText[timesTalked].text;
            DialogueManager.StartDialogue(diaText, new List<GameObject>{this.gameObject,touchPlayer.gameObject},dialogueText[timesTalked].minigameContTriggered);
        }
    }

    // collision with player
    void OnTriggerEnter2D(Collider2D other)
    {
        if (touchPlayer == null)
        {
            Debug.Log("Touch NPC");

            if (other.gameObject.GetComponent<Player>() == null) return;

            touchPlayer = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>()) touchPlayer = null;
    }


}
