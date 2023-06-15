using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentPlayArea : MonoBehaviour
{
    //to make sure all opponent's cards inPlayArea bool is enabled

    // Update is called once per frame
    void Update()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<NewCardDrag>().inPlayArea = true;
        }
    }
}
