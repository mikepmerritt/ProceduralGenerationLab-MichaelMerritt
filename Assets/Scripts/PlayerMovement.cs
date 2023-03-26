using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed, rotationSpeedPrecise, power, maxPower, powerIncrement;
    [SerializeField]
    private GameObject pointer, powerPointer;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private bool charging = false;

    // charge force when held
    public void OnMouseDown()
    {
        Debug.Log("down");
        charging = true;
    }

    // apply force when released and clear variables
    public void OnMouseUp()
    {
        Debug.Log("up");
        LaunchBall();
    }

    void Update()
    {
        // camera controls
        // get input for rotation changes to camera
        Vector3 rotationChanges = new Vector3(Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal"), 0f).normalized;

        // slow camera speed for precise changes if shift is held
        float speedToUse = rotationSpeed;
        if(Input.GetKey(KeyCode.LeftShift))
        {
            speedToUse = rotationSpeedPrecise;
        }

        // apply rotation changes
        transform.eulerAngles += rotationChanges * Time.deltaTime * speedToUse;

        // if charging a force, increase size
        if(power < maxPower && (charging || Input.GetKey(KeyCode.Space)))
        {
            power += powerIncrement * Time.deltaTime;
        }

        // spacebar launch
        if(Input.GetKeyUp(KeyCode.Space))
        {
            LaunchBall();
        }
    }

    public void LaunchBall()
    {
        rb.AddForce(this.transform.forward * power, ForceMode.Impulse);
        charging = false;
        power = 0;
    }
}
