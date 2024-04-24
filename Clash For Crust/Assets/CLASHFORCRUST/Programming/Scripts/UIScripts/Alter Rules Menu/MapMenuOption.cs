using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMenuOption : MonoBehaviour
{
    public GameObject model;
    private Vector3 baseModelPosition;

    float rot;

    [SerializeField] float range = 0.25f;

    [SerializeField] bool canSpin = true;

    // Start is called before the first frame update
    void Start()
    {
        baseModelPosition = model.transform.localPosition;
        rot = 0;
    }

    // Update is called once per frame
    void Update()
    {
        rot += Time.deltaTime * 90;

        if (rot > 360)
        {
            rot -= 360;
        }

        if(canSpin)
            model.transform.rotation = Quaternion.Euler(0, rot, 0);

        float newY = Mathf.Sin(rot * Mathf.Deg2Rad) * range + baseModelPosition.y;

        model.transform.localPosition = new Vector3(baseModelPosition.x, newY, baseModelPosition.z);
    }
}
