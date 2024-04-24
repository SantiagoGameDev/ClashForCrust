using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
public class BasicAttackManager : MonoBehaviour
{
    private Rigidbody playerRB;

    private float dashTimer = 0f;
    private float dashDuration = 0.05f;
    public bool attackActive = false;
    public bool isDashing = false;

    // Seagull Shot Variables
    public Transform shotObject;

    private Vector3 aimDirection = Vector3.zero;
    private Vector3 currentAimDirection;
    [SerializeField] private float aimDirectionLerpSpeed = 5.0f;
    private bool seagullShotActive = false;
    public GameObject shotObjectInstance;

    private float shotDuration = 0.0f;

    public GameObject seagullShotPool;

    public GameObject shotCircleIndicator;
    private GameObject shotCircleIndicatorInstance;

    [SerializeField] private float shotSpeed = 10f;
    [SerializeField] private float timeStep = 0.1f; // used for detail of the line
    [SerializeField] private int numSteps = 50; // number of points on the line (contributes to length of the line)
    [SerializeField] private float aimDistance = 10f; // how far the seagull can aim the shot

    //Cooldowns
    [SerializeField] private float sStepCD;
    [SerializeField] private float sSmashCD;
    private bool canStep = true;
    private bool canSmash = true;

    private Vector3 initialPosition;
    private Vector3 initialVelocity;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private PlayerController pc;
    [SerializeField] private Animator animator;
    [SerializeField] private Material[] mats = new Material[4];
    [SerializeField] private Collider dashHitbox;



    private SeagullSounds seagullSounds;

    private void Awake()
    {
        seagullSounds = GetComponentInParent<SeagullSounds>();
    }

    public IEnumerator SeagullSpin(Collider seagullSpinHitbox)
    {
        if (attackActive == false)
        {
           
            pc.spinning = true;
            attackActive = true;
            seagullSounds.PlaySound(SeagullSounds.SeagullSoundType.Spin);
            animator.SetTrigger("Spin");

            Collider hitbox = seagullSpinHitbox.GetComponent<Collider>();

            hitbox.enabled = true;

            yield return new WaitForSeconds(0.9f); // how long the seagull spin hitbox is active for

            hitbox.enabled = false;
            pc.SpinAnimEnd();
            pc.spinning = false;

            attackActive = false;
        }
    }

    public IEnumerator SeagullSmash(Rigidbody rb)
    {
        if (attackActive == false && canSmash)
        {
            canSmash = false;
            attackActive = true;
            animator.SetTrigger("Dash");
            seagullSounds.PlaySound(SeagullSounds.SeagullSoundType.Smash);

            playerRB = rb;
            //Debug.LogWarning("whatitdo");

            isDashing = true;
            dashHitbox.enabled = true;

            while (isDashing == true) // wait for dash to be completed in fixed update
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.5f); // give a bit of end lag to the smash (can change later)
            pc.controllable = true;
            attackActive = false;
            dashHitbox.enabled = false;

            yield return new WaitForSeconds(sSmashCD);
            canSmash = true;
        }
    }
    private void Start()
    {
        lineRenderer.material = mats[pc.playerNum];
    }
    private void Update()
    {
        // dont move the basic attack manager along with the seagull (might cause issues if seagull walks while having indicator on)
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    private void FixedUpdate()
    {
        if (isDashing && dashTimer < dashDuration)
        {
            dashTimer += Time.deltaTime;
            playerRB.AddForce(playerRB.transform.forward * 70000 * Time.fixedDeltaTime); // can adjust that number to whatever feels good
            pc.controllable = false;
        }

        if (dashTimer >= dashDuration)
        {
            dashTimer = 0f;
            isDashing = false;
        }
    }

    public IEnumerator SeagullShot(Transform playerTransform, PlayerInput playerInput)
    {
        if (attackActive == false && seagullShotActive == false) // replace seagullShotActive with the check for the shot meter being full
        {
            if (shotObjectInstance != null)
            {
                shotObjectInstance.transform.position = playerTransform.position;
            }

            pc.controllable = false;
            attackActive = true;
            animator.SetBool("PooHold", true);
            seagullShotActive = true;
            if (shotCircleIndicatorInstance != null)
            {
                shotCircleIndicatorInstance.SetActive(true);
            }

            bool shoot = false;

            //PlayerController player = GetComponentInParent<PlayerController>();

            while (shoot == false)
            {
                AimShot(playerTransform, out shotDuration, pc);

                yield return null;

                if (playerInput.actions["SeagullShot"].WasPressedThisFrame())
                {
                    shoot = true;
                }
            }

            WorldData.Instance.UpdateStats(pc.playerNum, WorldData.statsType.shotUses, 1);
            pc.ResetShotMeter();
            pc.controllable = true;

            Shoot();
            AudioManager.Instance.PlayAudio(AudioManager.AudioType.Shot, false);
            seagullSounds.PlaySound(SeagullSounds.SeagullSoundType.Shot);
            animator.SetBool("PooHold", false);
            StartCoroutine(ShotAttackCooldownReset()); // allows other attacks to be used while shot is airborne

            GameObject shotPoolObject = shotObjectInstance.transform.Find("ShitPool").gameObject; // ensure name is same as prefab
            GameObject shotPoolPlayerIndicator = FindChildWithTag(shotPoolObject); //GameObject.FindWithTag("SeagullShotPlayerIndicator");

            shotPoolPlayerIndicator.SetActive(false);
            shotPoolObject.GetComponentInChildren<ShitSlow>().playerNum = pc.playerNum;
            yield return new WaitForSeconds(shotDuration - 0.1f); // waits for shot to reach the end of it's trajectory

            seagullSounds.PlaySound(SeagullSounds.SeagullSoundType.ShotSplat);
            // disable projectile mesh and enable shot pool mesh
            shotObjectInstance.GetComponent<MeshRenderer>().enabled = false;
            //GameObject shotPoolObject = shotObjectInstance.transform.Find("ShitPool").gameObject; // ensure name is same as prefab
            shotPoolPlayerIndicator.SetActive(true);
            SetShotColor(shotPoolPlayerIndicator);
            shotPoolObject.GetComponent<Collider>().enabled = true;
            shotPoolObject.tag = "ShotPool"; //set tag for poo circle
            shotPoolObject.GetComponent<SpriteRenderer>().enabled = true;
            shotObjectInstance.GetComponent<Rigidbody>().velocity = Vector3.zero;

            yield return new WaitForSeconds(5.0f); // duration of shot pool

            shotPoolObject.GetComponent<Collider>().enabled = false;
            seagullShotActive = false;
            

            // disable shot object and reset meshs for next shot use
            shotObjectInstance.SetActive(false);
            shotObjectInstance.GetComponent<MeshRenderer>().enabled = true;
            shotPoolObject.GetComponent<SpriteRenderer>().enabled = false;
            shotPoolPlayerIndicator.SetActive(false);
        }
    }

    private GameObject FindChildWithTag(GameObject parent)
    {
        GameObject indicatorObject = null;

        foreach (Transform transform in parent.transform)
        {
            if (transform.CompareTag("SeagullShotPlayerIndicator"))
            {
                indicatorObject = transform.gameObject;
                break;
            }
        }

        return indicatorObject;
    }

    private void SetShotColor(GameObject shotPoolPlayerIndicator)
    {
        PlayerController player = GetComponentInParent<PlayerController>();

        switch (player.tag)
        {
            case "Player1":
                shotPoolPlayerIndicator.GetComponent<SpriteRenderer>().color = Color.blue;
                break;
            case "Player2":
                shotPoolPlayerIndicator.GetComponent<SpriteRenderer>().color = Color.red;
                break;
            case "Player3":
                shotPoolPlayerIndicator.GetComponent<SpriteRenderer>().color = Color.green;
                break;
            case "Player4":
                shotPoolPlayerIndicator.GetComponent<SpriteRenderer>().color = Color.yellow;
                break;
        }
    }

    private IEnumerator ShotAttackCooldownReset()
    {
        yield return new WaitForSeconds(0.5f);
        attackActive = false;
    }

    private void AimShot(Transform playerTransform, out float timeToPeak, PlayerController player)
    {
        initialPosition = playerTransform.position;

        Vector2 aimInputDirection = player.movInput2;
        Vector2 joystickInput = aimInputDirection.normalized;
        float inputMagnitude = joystickInput.magnitude;

        if (inputMagnitude > 0.1f)
        {
            aimDirection = Vector3.Lerp(aimDirection, new Vector3(joystickInput.x, 0f, joystickInput.y).normalized, aimDirectionLerpSpeed * Time.deltaTime);
        }

        currentAimDirection = aimDirection;

        playerTransform.rotation = Quaternion.LookRotation(-Vector3.Lerp(aimDirection, new Vector3(joystickInput.x, 0f, joystickInput.y).normalized, aimDirectionLerpSpeed * Time.deltaTime));

        Vector3 targetPosition = initialPosition + currentAimDirection * aimDistance;

        timeToPeak = shotSpeed / Mathf.Abs(Physics.gravity.y);

        float peakHeight = initialPosition.y + shotSpeed * timeToPeak - 0.5f * Mathf.Abs(Physics.gravity.y) * timeToPeak * timeToPeak;

        initialVelocity = (targetPosition - initialPosition) / timeToPeak + Vector3.up * (peakHeight - initialPosition.y) / timeToPeak;

        DrawIndicator();
    }

    private void DrawIndicator()
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = numSteps;

        Vector3 endPoint = Vector3.zero;

        for (int i = 0; i < numSteps; i++) // creates the line using the numSteps (length) and the timeStep (detail)
        {
            float time = i * timeStep;
            Vector3 point = CaclulateShotTrajectory(initialPosition, initialVelocity, time); // assigns that point to be properly aligned with the shot trajectory
            lineRenderer.SetPosition(i, point);

            endPoint = point; // ensures the last point created is assigned to be the end point
        }

        lineRenderer.SetPosition(numSteps - 1, endPoint);

        if (shotCircleIndicatorInstance == null)
        {
            shotCircleIndicatorInstance = Instantiate(shotCircleIndicator, endPoint, Quaternion.identity, transform);
        }
        else
        {
            shotCircleIndicatorInstance.transform.position = endPoint; // set the circle position to be at the end point of the line
        }
        shotCircleIndicatorInstance.GetComponent<Renderer>().material = mats[pc.playerNum];
    }

    private Vector3 CaclulateShotTrajectory(Vector3 initialPosition, Vector3 initialVelocity, float time)
    {
        Vector3 gravity = Physics.gravity;
        Vector3 trajectory = initialPosition + initialVelocity * time + 0.5f * gravity * time * time;

        if (trajectory.y < initialPosition.y)
        {
            trajectory.y = initialPosition.y;
        }

        return trajectory;
    }

    private void Shoot()
    {
        shotCircleIndicatorInstance.SetActive(false);
        lineRenderer.enabled = false;
        if (shotObjectInstance == null)
        {
            shotObjectInstance = Instantiate(shotObject.gameObject, initialPosition, shotObject.transform.rotation);
        }
        else
        {
            shotObjectInstance.SetActive(true);
            shotObjectInstance.transform.position = initialPosition;
        }

        shotObjectInstance.GetComponent<Rigidbody>().velocity = initialVelocity;
        //shotObjectInstance.GetComponent<Renderer>().material = mats[pc.playerNum];
    }

    public IEnumerator SeagullStep(Rigidbody rb)
    {
        if (canStep)
        {
            playerRB = rb;
            animator.SetTrigger("Dash");
            isDashing = true;
            //dashHitbox.enabled = true;

            while (isDashing == true) // wait for dash to be completed in fixed update
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.5f); // give a bit of end lag to the smash (can change later)

            pc.controllable = true;

            canStep = false;

            yield return new WaitForSeconds(sStepCD);

            canStep = true;

        }
    }
}