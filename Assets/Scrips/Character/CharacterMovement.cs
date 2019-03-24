﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CharacterMovement : MonoBehaviour
{
    public bool isHorizonControlRotate = true;
    public float rotationSpeed = 50.0f;
    public float movementSpeed = 300.0f;
    public float limitOfRotationRightCrood;
    public float limitOfRotationLeftCrood;
    public float fuelBurnRate;

    private new Rigidbody rigidbody;
    public GameObject meshObject;
    private Ship ship;

    public GameObject menu1;

    private float rotationDest;
    private float rotationStart;
    private float lerpTime = 0;

    private IEnumerator stopSoundSmooth;
    private bool isSmoothing = false;

    private AudioSource audioSource;

    enum RotateStateCode
    {
        Left,Right,middle
    }
    private RotateStateCode rotateState;
    private RotateStateCode RotateState
    {
        get
        {
            return rotateState;
        }
        set
        {
            if(value != rotateState)
            {
                ChangeRotateStateTo(value);
            }
            
        }
    }

    enum BoostSoundStateCode
    {
        Stoped,Launching,Looping
    }
    private BoostSoundStateCode boostSoundState;
    private BoostSoundStateCode BoostSoundState
    {
        get
        {
            return boostSoundState;
        }
        set
        {
            if(value != boostSoundState)
            {
                ChangeBoostStateTo(value);

            }
        }
    }

    void ChangeBoostStateTo(BoostSoundStateCode destState)
    {
        AudioClip stateSound = null;
        switch(destState)
        {
            case BoostSoundStateCode.Launching: stateSound = AudioClipLibrary.GetInstance().GetAudioFromLibrary("Engine Startup"); break;
            case BoostSoundStateCode.Looping: case BoostSoundStateCode.Stoped:   stateSound = AudioClipLibrary.GetInstance().GetAudioFromLibrary("Engine loop"); break;
        }

        audioSource.clip = stateSound;

        boostSoundState = destState;
    }

    void ChangeRotateStateTo(RotateStateCode destState)
    {
        rotationStart = meshObject.transform.rotation.eulerAngles.z;

        switch(destState)
        {
            case RotateStateCode.Left:   rotationDest = limitOfRotationLeftCrood;   break;
            case RotateStateCode.Right:  rotationDest = limitOfRotationRightCrood;   break;
            case RotateStateCode.middle: rotationDest = 0; break;
        }
        rotateState = destState;
        lerpTime = 0;
    }

    void Start()
    {
        ship = Ship.Instance;
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        float fuelUpdate;

        Vector3 rotationCoord = new Vector3(0, horizontalAxis * Time.deltaTime * rotationSpeed);

        if (!menu1.activeSelf) {
            if (isHorizonControlRotate) {
                if (horizontalAxis != 0)
                    if (horizontalAxis < 0) {
                        RotateState = RotateStateCode.Right;
                    }
                    else {
                        RotateState = RotateStateCode.Left;
                    }
                else RotateState = RotateStateCode.middle;

                transform.Rotate(rotationCoord);

                ProcessRotateState();
            }
        }

        // Calculate forward speed
        if (!menu1.activeSelf) {
            if (Input.GetAxis("Vertical") != 0 && ship.currentFuel > 0) {
                // Hold shift to break, has lower fuel consumption
                if (Input.GetKey(KeyCode.LeftShift)) {
                    rigidbody.AddRelativeForce(Vector3.forward * Input.GetAxis("Vertical") * Time.deltaTime * movementSpeed);
                    fuelUpdate = fuelBurnRate * Time.deltaTime;
                }
                else {
                    rigidbody.AddRelativeForce(Vector3.forward * Input.GetAxis("Vertical") * Time.deltaTime * movementSpeed * 5);
                    fuelUpdate = fuelBurnRate * Time.deltaTime * 10;
                }
                ship.currentFuel -= fuelUpdate;
            }
        }
    }

    void ProcessRotateState()
    {
        if (lerpTime <= 1)
        {
            float rotateAngle = Mathf.LerpAngle(rotationStart, rotationDest, lerpTime);
            meshObject.transform.localEulerAngles = new Vector3(0, 0, rotateAngle);
            lerpTime += Time.deltaTime;
        }
    }
}
