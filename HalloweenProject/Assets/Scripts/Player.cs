/*
 * Player
 * 
 * ---
 * Player collision and movements
 * Also eventually interaction with objects ig
 * ---
 * Last Edited: 10/02
 * Edited by: aschia
 * */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(BoxCollider2D))]
public class Player : DialogueActionable
{
    private BoxCollider2D boxColl;
    //private SpriteRenderer spRend;

    private Vector3 moveDelta;
    public float moveSpeed = 0.5f;
    public static int candyCollected = 0;
    public Text candyText = null;
    //public List<Sprite> faceSprites;
    //public int sprIndex;

    public static Player PlayerSingleton = null;
    #region PlayerSingleton
    void SetPlayerSingleton()
    {
        if (PlayerSingleton == null)
        {
            PlayerSingleton = this;
        }
        else
        {
            PlayerSingleton = null;
        }
    }
    #endregion

    private void Start()
    {
        if (spRend == null) spRend = transform.GetChild(0).GetComponent<SpriteRenderer>();
        boxColl = GetComponent<BoxCollider2D>();
    }

    private void Awake()
    {
        SetPlayerSingleton();

        if (PlayerSingleton == null) return;
    }

    private void FixedUpdate()
    {
        //get input
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        //reset movedelta
        moveDelta = new Vector3(x, y, 0);

        //swap sprite direction
        int newInd = -1;
        if (moveDelta.x != 0)
        {
            newInd = 1;
            transform.GetChild(0).localScale = new Vector3(1 * x, 1, 1);
        }
        if (moveDelta.y != 0)
        {
            newInd = 0;
            if (y > 0) newInd = 2;
        }

        if (newInd != -1)
        {
            //if (sprIndex != newInd) imgIndex = 1.5f;
            imgIndex += imgSpeed * Time.deltaTime;

            sprIndex = newInd;
            //Debug.Log(newInd);
            UpdateSprite();
        }
        else if (imgIndex != 0)
        {
            imgIndex = 0;
            UpdateSprite();
        }

        //move it move it
        transform.Translate(moveDelta * moveSpeed * Time.deltaTime);

        /*Debug.Log(x);
        Debug.Log(y);*/
    }

    public static void AwardCandy()
    {
        candyCollected++;
        PlayerSingleton.candyText.text = "Candy collected: " + candyCollected;
    }
}
