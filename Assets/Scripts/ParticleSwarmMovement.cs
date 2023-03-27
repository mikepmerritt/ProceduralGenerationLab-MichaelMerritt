using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSwarmMovement : MonoBehaviour
{
    public float speed;
    public PlayerMovement player;

    void Update()
    {
        if(player == null)
        {
            player = GameObject.FindObjectOfType<PlayerMovement>();
        }
        else
        {
            transform.LookAt(player.transform);
            transform.localPosition += this.transform.forward * speed * Time.deltaTime;
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.GetComponent<PlayerMovement>() != null)
        {
            Destroy(other.gameObject);
        }
    }
}
