using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSwarmMovement : MonoBehaviour
{
    // the behaviors for the particle swarm on level 3 is described here
    // the particle system is in charge of the particles and death animation though

    public float speed;
    public PlayerMovement player;
    public Animator ani;

    void Update()
    {
        // if the reference to the player is null (because it was destroyed),
        // find the new player
        if(player == null)
        {
            player = GameObject.FindObjectOfType<PlayerMovement>();
        }
        // otherwise move toward the player
        else
        {
            transform.LookAt(player.transform);
            transform.localPosition += this.transform.forward * speed * Time.deltaTime;
        }
    }

    // if the swarm hits the player destroy the player
    public void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.GetComponent<PlayerMovement>() != null)
        {
            Destroy(other.gameObject);
        }
    }

    // play death animation (red burst) and disable hitbox when dying
    // used in level 4
    public void StartDeath()
    {
        ani.Play("Death");
        GetComponent<Collider>().enabled = false;
    }

    // used as an animation event
    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
