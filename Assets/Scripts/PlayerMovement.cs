using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController controller;
    private Vector3 playerMovement;
    [SerializeField]
    private float walkSpeed;

    void Update()
    {
        // movement
        playerMovement = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;

        // apply velocity
        Debug.Log(playerMovement);
        transform.position += playerMovement * Time.deltaTime * walkSpeed;
    }
}
