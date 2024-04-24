using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoatRockingManager : MonoBehaviour
{
    private static BoatRockingManager instance;
    public static BoatRockingManager Instance { get { return instance; } }

    public enum shipStateType { left, middle, right }

    [SerializeField] shipStateType shipState;

    [SerializeField] private Camera shipCam;
    Vector3 midCamPos;
    Vector3 leftCamPos = new Vector3(0, 0, -20f);
    Vector3 rightCamPos = new Vector3(0, 0, 3f);

    [SerializeField] GameObject boatGO;
    float boatZRot;
    Vector3 newRotation;

    bool shipRotating;


    [SerializeField] float rotSpeed, tiltValue, objectWaitTime;

    public float rad;
    private Vector3 baseRot;

    private float camRotZ;

    public float speed = 0.5f;

    private float elapsedTime;
    private bool danRotate = true;
    float rotAmount = 10f; // ADJUST FOR AMOUNT OF ROTATION
    float rotDuration = 1.0f; // ADJUST FOR SPEED OF ROTATION

    private Coroutine rotateCoroutine;

    const float deg90 = Mathf.PI / 2f, deg180 = Mathf.PI, deg270 = Mathf.PI * 3f / 2f, deg360 = Mathf.PI * 2;

    private void Awake()
    {
        if (!instance)
            instance = this;

        shipRotating = false;
        midCamPos = shipCam.transform.eulerAngles;
        leftCamPos = new Vector3(shipCam.transform.eulerAngles.x, shipCam.transform.eulerAngles.y, shipCam.transform.eulerAngles.z - tiltValue);
        rightCamPos = new Vector3(shipCam.transform.eulerAngles.x, shipCam.transform.eulerAngles.y, shipCam.transform.eulerAngles.z + tiltValue);
        //Debug.Log(midCamPos);

    }

    private void Start()
    {
        AudioManager.Instance.PirateModeToggle(true);
        shipState = shipStateType.middle;
        //camRotZ = 0f;
    }

    private void Update()
    {

        //shipCam.transform.rotation = Quaternion.Euler(midCamPos.x, midCamPos.y, 10f);

        if (rotateCoroutine == null)
            rotateCoroutine = StartCoroutine(CameraRotation());



        //if (shipState == shipStateType.left)
        {
            shipRotating = true;

            //if (shipCam.transform.rotation.eulerAngles.z >= leftCamPos.z)
            // {
            //Debug.Log("Rotating Left");
            //Debug.Log("shipCam.transform.rotation.eulerangles.z " + shipCam.transform.rotation.eulerAngles.z + " midCampos + -tiltValue " + midCamPos.z + -tiltValue);
            //shipCam.transform.localRotation = Quaternion.Euler(Vector3.RotateTowards(shipCam.transform.rotation.eulerAngles, leftCamPos, 0.003f * Time.deltaTime, 0f));

            //shipCam.transform.Rotate(new Vector3(midCamPos.x, midCamPos.y, Mathf.Lerp(0, -3f, 3f * Time.deltaTime)));

            //camRotZ += Time.deltaTime * 100f;
            //}
            //else
            {
                //Debug.Log("Ship rotating done");
                //shipRotating = false;
            }

        }
    }

    private IEnumerator CameraRotation()
    {
        rad = 0;

        bool stoppedLeft = false, stoppedRight = false;

        while (true)
        {
            /*float rotPerFrame = rotAmount * Time.deltaTime / rotDuration;
        
            float targetRotation = danRotate ? 1f : -1f;
        
            shipCam.transform.Rotate(0f, 0f, rotPerFrame * targetRotation);
        
            if (Mathf.Abs(shipCam.transform.rotation.eulerAngles.z) >= Mathf.Abs(rotAmount))
            {
                danRotate = !danRotate;
            }*/

            rad += Time.deltaTime * deg90 * speed;

            if (rad > deg360)
            {
                rad -= deg360;
                stoppedLeft = false;
                stoppedRight = false;
            }

            if (rad > deg90 && !stoppedRight)
            {
                yield return new WaitForSeconds(1f);
                stoppedRight = true;
            }

            if (rad > deg270  && !stoppedLeft)
            {
                yield return new WaitForSeconds(1f);
                stoppedLeft = true;
            }

            if (rad > deg180 && rad < deg270)
            {
                shipState = shipStateType.left;   
            }
            else if (rad < deg90)
            {
                shipState = shipStateType.right;
            }


            float rotation = Mathf.Sin(rad) * rotAmount;

            shipCam.transform.rotation = Quaternion.Euler(46.36f, 17.384f, rotation);


            yield return null;
        }
    }

    //private IEnumerator RotateShip()
    //{
    //    float rad180 = Mathf.PI;
    //    float rad90 = rad180 / 2;
    //    float rad270 = rad180 * (3f / 2f);
    //    float rad360 = rad180 * 2;

    //    while (!RoundManager.Instance)
    //    {
    //        yield return null;
    //    }

    //    while (RoundManager.Instance.GameStart)
    //    {
    //        yield return null;
    //    }

    //    while (!RoundManager.Instance.GameOver)
    //    {
    //        Debug.Log("in here");
    //        rad += Time.deltaTime * Mathf.PI / 4;

    //        if (rad > rad360)
    //        {
    //            rad -= rad360;
    //        }

    //        if ((rad > rad90 && rad < rad90 + 0.01f) || (rad > rad270 && rad < rad270 + 0.01f))
    //        {
    //            yield return new WaitForSeconds(3f);
    //        }

    //        float rotAdd = Mathf.Sin(rad);
    //        float rotZ = rotAdd * 15f + baseRot.z;

    //        boatGO.transform.rotation = Quaternion.Euler(baseRot.x, baseRot.y, rotZ);

    //        if ((rad > 0 && rad < rad90) || rad > rad270 && rad < rad360)
    //        {
    //            shipState = shipStateType.right;
    //        }
    //        if ((rad > rad90 && rad < rad270))
    //        {
    //            shipState = shipStateType.left;
    //        }

    //        yield return null;
    //    }

    //}

    public shipStateType GetShipState()
    {
        return shipState;
    }

}
