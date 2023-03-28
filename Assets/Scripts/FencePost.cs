using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FencePost : MonoBehaviour
{
    public GameObject next;
    public GameObject connector;

    // connects the fence post's connector beam to the next fence post
    // used first in level 4
    public void TurnToNext()
    {
        // make the beam face the next fence post
        connector.transform.LookAt(next.transform.GetChild(0));

        // stretch the beam if the angle is too steep for it to connect
        if(connector.transform.eulerAngles.x > 51f && connector.transform.eulerAngles.x < 309f)
        {
            connector.transform.localScale = new Vector3(1, 1, 3.6f);
        }
        else if(connector.transform.eulerAngles.x > 43f && connector.transform.eulerAngles.x < 318f)
        {
            connector.transform.localScale = new Vector3(1, 1, 2.9f);
        }
        else if(connector.transform.eulerAngles.x > 25f && connector.transform.eulerAngles.x < 335f)
        {
            connector.transform.localScale = new Vector3(1, 1, 2.4f);
        }
    }
}
