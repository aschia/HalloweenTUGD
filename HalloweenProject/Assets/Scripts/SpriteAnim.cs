/*
 * SpriteAnim
 * 
 * ---
 * Basic container for multi-image sprite animations.
 * ---
 * 
 * */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpriteAnim
{
    public List<Sprite> images = null;          // the images used in this animation
    public float imgSpeed = 2f;                 // speed at which images change (imgs per second)
}
