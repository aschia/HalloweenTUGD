using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnimplementedGame : Minigame
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Player.AwardCandy();
            Destroy(gameObject);
        }
    }
}
