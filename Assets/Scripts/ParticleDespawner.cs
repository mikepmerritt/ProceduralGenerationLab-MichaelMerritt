using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDespawner : MonoBehaviour
{
    // a script for an empty object with a big tile-sized trigger to put on safe tiles
    // used in level 4
    public void OnTriggerEnter(Collider other)
    {
        // if the item in the trigger is the particle swarm, trigger its death animation
        if(other.gameObject.GetComponent<ParticleSwarmMovement>() != null)
        {
            other.gameObject.GetComponent<ParticleSwarmMovement>().StartDeath();
        }
    }
}
