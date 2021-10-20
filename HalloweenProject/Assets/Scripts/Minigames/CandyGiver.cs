using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyGiver : Minigame
{
    // Start is called before the first frame update
    void Start()
    {
        Player.AwardCandy();
        Destroy(gameObject);
    }
}
