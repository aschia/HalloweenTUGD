using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    // vars
    private SpriteRenderer spRend;

    public List<string> text = null;
    public List<Sprite> moveSprites;

    GameObject touchPlayer = null;


    // Start is called before the first frame update
    void Start()
    {
        spRend = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // if it isn't, lets set dialogue properties now
        if (touchPlayer != null && Input.GetKeyDown(KeyCode.E) && DialogueManager.DiaManagerSingleton != null && DialogueManager.DiaManagerSingleton.text == null)
        {
            /*Vector2 Point_1 = new Vector2(touchPlayer.transform.position.x,touchPlayer.transform.position.y);
            Vector2 Point_2 = new Vector2(transform.position.x,transform.position.y);
            float angle = Mathf.Atan2(Point_2.y - Point_1.y, Point_2.x - Point_1.x) * 180 / Mathf.PI;*/
            if (touchPlayer.GetComponent<Player>().sprIndex == 0) spRend.sprite = moveSprites[2];
            if (touchPlayer.GetComponent<Player>().sprIndex == 1) spRend.sprite = moveSprites[1];
            if (touchPlayer.GetComponent<Player>().sprIndex == 2) spRend.sprite = moveSprites[0];


            DialogueManager.StartDialogue(text);
            touchPlayer.gameObject.GetComponent<Player>().enabled = false;
            DialogueManager.DiaManagerSingleton.enabledOnEnd = touchPlayer.gameObject;
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
