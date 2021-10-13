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
    public List<SpriteAnim> anims = new List<SpriteAnim>();
    [HideInInspector]
    public int sprIndex = 0;                // which sprite animation to play
    [HideInInspector]
    public float imgIndex = 0;                // which image of the animation to show
    [HideInInspector]
    public float imgSpeed = 0;              // how long between images in the animation

    protected SpriteRenderer spRend = null;

    void Start()
    {
        if (spRend == null) spRend = GetComponent<SpriteRenderer>();
    }

    public void UpdateSprite()
    {
        // check for overflow
        if (imgIndex > anims[sprIndex].images.Count) imgIndex = 0;

        // get the sprite
        int ind = (int)Mathf.Floor(imgIndex);
        spRend.sprite = anims[sprIndex].images[ind];

        // update anim speed
        imgSpeed = anims[sprIndex].imgSpeed;
    }
}
