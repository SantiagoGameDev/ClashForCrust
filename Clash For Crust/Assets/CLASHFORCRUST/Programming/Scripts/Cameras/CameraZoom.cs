using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public GameObject mainCam;
    public GameObject zoomedCam;
    private GameObject crust;

    private void Awake()
    {
        mainCam.SetActive(false);
        zoomedCam.SetActive(true);
    }

    private void Start()
    {
        StartCoroutine(ZoomCamera());
    }

    IEnumerator ZoomCamera()
    {
        while (crust == null)
        {
            crust = GameObject.FindWithTag("CrustZoomCameraPosition");
            yield return null;
        }

        Vector3 crustPos = crust.transform.position;
        zoomedCam.transform.position = new Vector3(crustPos.x, zoomedCam.transform.position.y, crustPos.z);

        yield return new WaitForSeconds(0.5f);

        zoomedCam.SetActive(false);
        mainCam.SetActive(true);

        while (WorldData.Instance.gameOver == false)
        {
            yield return null;
        }

        zoomedCam.SetActive(true);
        mainCam.SetActive(false);

        GameObject fattestSeagull = WorldData.Instance.GetPlayerWithHighestScore();
        zoomedCam.transform.position = fattestSeagull.transform.Find("ZoomCamPoint").position;
    }
}
