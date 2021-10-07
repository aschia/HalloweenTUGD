/*
 * DialogueActionable
 * 
 * ---
 * Parent class for all dialogue-related classes
 * ---
 * Last Edited: aschia (10/07)
 * 
 * */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueActionable : MonoBehaviour
{
    public List<Sprite> faceSprites;
    [HideInInspector]
    public int sprIndex = 0;

    protected SpriteRenderer spRend = null;

    void Start()
    {
        if (spRend == null) spRend = GetComponent<SpriteRenderer>();
    }

    public void UpdateSprite()
    {
        spRend.sprite = faceSprites[sprIndex];
    }
}
