using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    // this entire script is in charge of the player movement established first in level 1
    [SerializeField]
    private float rotationSpeed, rotationSpeedPrecise, power, maxPower, powerIncrement, pointerMin, pointerMax;
    [SerializeField]
    private GameObject pointer;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private bool charging = false;
    [SerializeField]
    private Material notChargingMaterial, chargingMaterial, maxChargeMaterial;

    // charge force when held
    public void OnMouseDown()
    {
        charging = true;
    }

    // apply force when released and clear variables
    public void OnMouseUp()
    {
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
            UpdatePointer();
        }

        // spacebar launch
        if(Input.GetKeyUp(KeyCode.Space))
        {
            LaunchBall();
        }
    }

    // launch the ball forward
    public void LaunchBall()
    {
        rb.AddForce(this.transform.forward * power, ForceMode.Impulse);
        charging = false;
        power = 0;
        UpdatePointer();
    }

    // rescale the pointer
    public void UpdatePointer()
    {
        // determine how much to scale pointer by
        float pointerSize = Mathf.Lerp(pointerMin, pointerMax, (power/maxPower));

        // scale pointer size and make it stay in front of the player
        pointer.transform.localScale = new Vector3(pointer.transform.localScale.x, pointerSize, pointer.transform.localScale.z);
        pointer.transform.localPosition = new Vector3(pointer.transform.localPosition.x, pointer.transform.localPosition.y, pointerSize);

        // pick material
        if (pointerSize == pointerMin)
        {
            pointer.GetComponent<MeshRenderer>().material = notChargingMaterial;
        }
        else if (pointerSize == pointerMax)
        {
            pointer.GetComponent<MeshRenderer>().material = maxChargeMaterial;
        }
        else
        {
            pointer.GetComponent<MeshRenderer>().material = chargingMaterial;
        }
    }
}
