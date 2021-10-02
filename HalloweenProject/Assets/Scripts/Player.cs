/*
 * Player
 * 
 * ---
 * Player collision and movements
 * ---
 * Last Edited: 10/02
 * Edited by: aschia
 * */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class Player : MonoBehaviour
{
    private BoxCollider2D boxColl;

    private void Start()
    {
        boxColl = GetComponent<BoxCollider2D>();
    }
}
