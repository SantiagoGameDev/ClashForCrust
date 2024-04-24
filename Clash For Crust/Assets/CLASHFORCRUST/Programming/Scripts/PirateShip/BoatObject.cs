using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoatObject : MonoBehaviour
{
    [SerializeField] float slideSpeed, distanceFromPoint, knockBackForce = 2500f;
    [SerializeField] Transform leftPos, midPos, rightPos;
    private Collider collider;

    private Vector3 basePos;

    private bool sliding;
    private bool objectWait;

    public void Start()
    {
        collider = GetComponent<Collider>();
        sliding = true;
        objectWait = false;

        basePos = transform.localPosition;
        
    }

    public void FixedUpdate()
    {
        UpdatePosition();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController pc))
            pc.Knockback(collider, knockBackForce);
    }

    public void UpdatePosition()
    {

        CheckIfSliding();

        if (sliding)
        {
            if (BoatRockingManager.Instance.GetShipState() == BoatRockingManager.shipStateType.left)
            {

                if (Vector3.Distance(transform.position, leftPos.position) >= distanceFromPoint)
                {
                    transform.position = Vector3.MoveTowards(this.transform.position, leftPos.position, slideSpeed * Time.deltaTime);
                }

            }
            else if (BoatRockingManager.Instance.GetShipState() == BoatRockingManager.shipStateType.right)
            {
                if (Vector3.Distance(transform.position, rightPos.position) >= distanceFromPoint)
                {
                    transform.position = Vector3.MoveTowards(this.transform.position, rightPos.position, slideSpeed * Time.deltaTime);
                }
            }

            //float rad = BoatRockingManager.Instance.rad;

            //Vector3.move

            //float newX = Mathf.Sin(rad) * 7f;
            //transform.localPosition = new Vector3(basePos.x + newX, basePos.y, basePos.z);
        }

    }

    private void CheckIfSliding()
    {
        if (!sliding)
        {
            if (Vector3.Distance(transform.position, leftPos.position) >= distanceFromPoint)
            {
                sliding = true;
            }
            else if (Vector3.Distance(transform.position, rightPos.position) >= distanceFromPoint)
            {
                sliding = true;
            }
            else
                sliding = false;
        }
    }

    private IEnumerator ObjectHold()
    {
        if (!sliding)
        {
            yield return new WaitForSeconds(1f);
            sliding = true;
        }

    }

}
