using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionWheel : MonoBehaviour
{
    [SerializeField] GameObject wheel;

    [SerializeField] float rotVal;
    [SerializeField] float wheelSpeed;
    [SerializeField] int totalHats = 10;
    [SerializeField] int totalSkins = 10;

    [SerializeField] bool isSkin;

    int wheelIndex;
    float rotTimer;
    float newYRot = -9.0f;

    bool spinningR, spinningL;
    Vector3 eulerAngle;
    Vector3 newRot;

    private void Awake()
    {
        //wheel = GetComponent<GameObject>();
        eulerAngle = wheel.transform.eulerAngles;
        spinningR = false;
        spinningL = false;
        rotTimer = 0;

        if (!isSkin)
            rotVal = 360f / HatsAndSkins.Instance.Hats.Count;
        else
            rotVal = 360f / HatsAndSkins.Instance.Skins.Count;
    }

    private void Update()
    { 
        RotRight();
        RotLeft();
    }

    private void RotRight()
    {
        if (spinningR && !spinningL)
        {
            rotTimer += Time.deltaTime * wheelSpeed;

            if (rotTimer > 1)
                rotTimer = 1;

            if(!isSkin)
                wheel.transform.rotation = Quaternion.Euler(-90, -30, newYRot + rotTimer * rotVal); //-90 and -30 are starting rotations
            else
                wheel.transform.rotation = Quaternion.Euler(-90, -30, newYRot + rotTimer * rotVal); //-90 and 160 are starting rotations
            //wheel.transform.rotation = Quaternion.Euler(wheel.transform.eulerAngles.x, wheel.transform.eulerAngles.y, newYRot - rotTimer * rotVal); Branson pls help me fix this later xD

            wheelIndex++;

            if (!isSkin)
            {
                if (wheelIndex > totalHats - 1)
                    wheelIndex = 0;
            }
            else
            {
                if (wheelIndex > totalSkins - 1)
                    wheelIndex = 0;
            }
                

        }

        if (rotTimer == 1)
        {
            newYRot += rotTimer * rotVal;

            if (newYRot > 360)
                newYRot -= 360;

            spinningR = false;
            rotTimer = 0;

        }

    }

    private void RotLeft()
    {
        if (spinningL && !spinningR)
        {
            rotTimer += Time.deltaTime * wheelSpeed;

            if (rotTimer > 1)
                rotTimer = 1;

            if (!isSkin)
                wheel.transform.rotation = Quaternion.Euler(-90, -30, newYRot - rotTimer * rotVal); //-90 and -30 are starting rotations
            else
                wheel.transform.rotation = Quaternion.Euler(-90, -30, newYRot - rotTimer * rotVal); //-90 and 160 are starting rotations
            //wheel.transform.rotation = Quaternion.Euler(wheel.transform.eulerAngles.x, wheel.transform.eulerAngles.y, newYRot - rotTimer * rotVal); Branson pls help me fix this later xD

            wheelIndex--;

            if (wheelIndex > totalHats - 1)
                wheelIndex = 0;

        }

        if (rotTimer >= 1)
        {
            newYRot -= rotTimer * rotVal;

            if (newYRot < -360)
                newYRot += 360;

            spinningL = false;
            rotTimer = 0;

        }
    }

    public void SpinRight()
    {
        spinningR = true;
    }

    public void SpinLeft()
    {
        spinningL = true;
    }

    public bool CanSpin()
    {
        if (spinningR || spinningL)
            return false;
        else
            return true;
    }

    public int GetWheelIndex()
    {
        return wheelIndex;
    }

}
