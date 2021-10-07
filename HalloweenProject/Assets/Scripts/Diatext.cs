/*
 * Diatext
 * 
 * ---
 * Basic container for all dialogue-instances, has a string list for text and whatnot
 * ---
 * 
 * 
 * */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Diatext
{
    public List<string> text = null;           // the text used in the dialogue event
}
