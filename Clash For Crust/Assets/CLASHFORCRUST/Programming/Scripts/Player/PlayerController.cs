using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.InputSystem.UI;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    //private static PlayerController instance;
    //
    //public static PlayerController Instance
    //{
    //    get
    //    {
    //        if (!instance)
    //        {
    //            instance = FindObjectOfType<PlayerController>();
    //        }
    //        return instance;
    //    }
    //}
    public enum ActivePowerup
    { DONUT, CHILIPEPPER, FIREWORK, POPCORN, CRUST, SCREW, NONE }

    public int seagullRotationSpeed = 0;
    public ActivePowerup activePowerup;

    private Coroutine powerUpCoroutine;

    public BasicAttackManager basicAttackManager;
    public SeagullSounds seagullSounds;

    //Private Variables
    private int popcornAmmo = 0;

    //private Transform transform;
    public Vector2 movInput2;

    private Vector2 lookInput2;
    private Rigidbody rb;
    private Vector3 aimDirection = Vector3.zero;
    private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;

    //private Animator animator;

    //player health
    [SerializeField]
    private int stamina;

    [SerializeField]
    private bool DebugPoo = false;

    private int origStam;

    private bool canDmg;
    [SerializeField] private float invulnTime;

    public int Stamina
    { get { return stamina; } }

    //if the player is healing
    private bool isHealing;

    public bool IsHealing
    { get { return isHealing; } }

    private Quaternion lastRot;

    //Control sticks
    private Vector3 movInput;

    private Vector3 lookInput;

    [Header("Movement Variables")]
    [SerializeField] private bool moving;

    public bool controllable; // public so basic attacks can use it
    public bool stunned;

    public bool spinning;
    [SerializeField] private float normalSpeed;
    [SerializeField] private float crustSpeed;
    public float speed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float rotSpeed;
    [SerializeField] private float stunDuration;

    [Header("Kockback Forces")]
    [SerializeField] private float knockbackBasicAttack;

    [SerializeField] private float knockbackPopcorn;
    [SerializeField] private float knockbackFirework;
    [SerializeField] private float knockbackChilliPepper;
    [SerializeField] private float knockbackDonut;

    [Header("Object Refs")]
    [SerializeField] private Collider spinHitBox;

    [SerializeField] private Animator animator;
    public GameObject crustObject;
    public GameObject screwObject;
    [SerializeField] private GameObject fWObject;
    [SerializeField] private GameObject dynamiteObj;
    [SerializeField] private ParticleSystem burn;
    [SerializeField] private ParticleSystem stun;
    [SerializeField] private ParticleSystem feather;
    [SerializeField] private ParticleSystem wing1;
    [SerializeField] private ParticleSystem wing2;

    [Header("Damage Values")]
    [SerializeField] private int flamethrowerDmg;

    [SerializeField] private int popCornDmg;
    [SerializeField] private int donutDmg;
    [SerializeField] private int fireWorkDmg;
    [SerializeField] private float squawkCooldown;

    public GameObject beakPos;
    public SpriteRenderer circleIndidcator;

    private SkinnedMeshRenderer mesh;
    [SerializeField] private Material screwMat;
    [SerializeField] private Material goldMat;
    [SerializeField] private Material invisibleMat;
    public Material currentMat;

    private float flashingTimer;

    private CrustMode crustScript;

    public int playerNum;
    private bool breathin = false;
    private bool screwActive = false;

    public bool canPickUpCrust;
    public bool canSwallow;
    public bool canSquawk;

    public float currentDistance;

    public bool testbool;

    [SerializeField] int powerUpShotVal;

    [SerializeField] private int shotMeter;

    [SerializeField] private PowerUpUI pUI;
    public GameObject crustCircle;

    public MultiplayerEventSystem mes;
    public InputSystemUIInputModule uiModule;

    private void Awake()
    {
        spinning = false;
        moving = false;
        canDmg = true;
        stunned = false;
        controllable = true;

        crustSpeed = speed * 0.9f;
        normalSpeed = speed;

        stamina = RoundManager.Instance.MaxHealth;

        //transform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        playerInputActions = new PlayerInputActions();
        seagullSounds = GetComponent<SeagullSounds>();
        mes = GetComponent<MultiplayerEventSystem>();
        uiModule = GetComponent<InputSystemUIInputModule>();
        //animator = GetComponent<Animator>();
        playerInputActions.NoPowerUp.Enable();
        //playerInputActions.NoPowerUp.Movement.performed += UpdateMovement;
        StartCoroutine(IdleAnimChooser());
        origStam = stamina;
        canPickUpCrust = true;
        playerNum = GetComponent<PlayerInput>().user.index;
    }

    private void Start()
    {
        currentDistance = Vector3.Distance(transform.position, Camera.main.transform.position);
        crustScript = GetComponent<CrustMode>();
        //crustScript.playerNum = playerNum;
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        currentMat = mesh.material;
        //FindObjectOfType<PlayerInputManager>().gameObject.GetComponent<PIMController>().RegisterPlayer(GetComponent<PlayerInput>().user.index);
        switch (playerNum)
        {
            case 0:
                gameObject.tag = "Player1";
                circleIndidcator.color = Color.blue; //Registers player tags
                break;

            case 1:
                gameObject.tag = "Player2";
                circleIndidcator.color = Color.red;
                break;

            case 2:
                gameObject.tag = "Player3";
                circleIndidcator.color = Color.green;
                break;

            case 3:
                gameObject.tag = "Player4";
                circleIndidcator.color = Color.yellow;
                break;
        }
        activePowerup = ActivePowerup.NONE;

        stamina = RoundManager.Instance.MaxHealth;
        isHealing = false;
        canSwallow = false;
        canSquawk = true;

        if (AudioManager.Instance.pirateMode)
            fWObject = dynamiteObj;
    }

    private void FixedUpdate()
    {
        if (controllable && !stunned)
            UpdateMovement();
        if (activePowerup == ActivePowerup.POPCORN && popcornAmmo <= 0)
        {
            if (activePowerup != ActivePowerup.CRUST)
                activePowerup = ActivePowerup.NONE;
            animator.ResetTrigger("PCFire");
        }

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
    }

    private void Update()
    {
        //BasicAttackInput();

        animator.SetInteger("PopcornAmmo", popcornAmmo);

        if (Vector3.Distance(transform.position, Camera.main.transform.position) > currentDistance + 0.5f)
        {
            transform.localScale += new Vector3(0.015f, 0.015f, 0.015f);
            currentDistance = Vector3.Distance(transform.position, Camera.main.transform.position);
        }
        else if (Vector3.Distance(transform.position, Camera.main.transform.position) < currentDistance - 0.5f)
        {
            transform.localScale -= new Vector3(0.015f, 0.015f, 0.015f);
            currentDistance = Vector3.Distance(transform.position, Camera.main.transform.position);
        }

        if (transform.localScale.x < 3.9f)
        {
            transform.localScale = new Vector3(3.9f, 3.9f, 3.9f);
        }

        if (transform.localScale.x > 4.2f)
        {
            transform.localScale = new Vector3(4.2f, 4.2f, 4.2f);
        }

    }

    public void OnMove(InputAction.CallbackContext ctx) => movInput2 = ctx.ReadValue<Vector2>();

    public void OnLook(InputAction.CallbackContext ctx) => lookInput2 = ctx.ReadValue<Vector2>();

    private void UpdateMovement()
    {
        movInput = new Vector3(movInput2.x, 0, movInput2.y);
        lookInput = new Vector3(lookInput2.x, 0, lookInput2.y);
        if (movInput != Vector3.zero)
        {
            if (controllable)
            {
                if (WorldData.Instance)
                {
                    WorldData.Instance.UpdateStats(playerNum, WorldData.statsType.movement, Time.deltaTime);
                }
            }
            moving = true;
            rb.AddForce(movInput * speed * Time.deltaTime);
        }
        else
        {
            moving = false;
            if (rb.velocity.x <= 0.1f && rb.velocity.z <= 0.1f)
            {
                rb.velocity = Vector3.zero;
            }
            //rb.velocity = Vector3.zero;
        }

        animator.SetBool("WalkingToo", moving);
        if (!spinning)
        {
            UpdateRotation();
        }
        // Debug.LogWarning(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
    }

    public void UpdateRotation()
    {
        //Vector3 relative;

        //if (moving) //If moving rotate with movement
        //{
        //    relative = (transform.position + movInput) - transform.position;
        //    Quaternion rot = Quaternion.LookRotation(relative, Vector3.up);
        //    lastRot = transform.rotation;
        //}
        //else
        //{
        //    transform.rotation = lastRot;
        //    Debug.Log("Right stick moving");
        //    relative = (transform.position + lookInput) - transform.position;
        //    Quaternion rot = Quaternion.LookRotation(relative, Vector3.up);
        //}

        if (movInput.x != 0.0f || movInput.z != 0.0f)
        {
            // transform.rotation = Quaternion.LookRotation(new Vector3(movInput.x, 0, movInput.z)); //Keep rotation in the direction of movement
            aimDirection = Vector3.Lerp(aimDirection, new Vector3(movInput.x, 0, movInput.z), seagullRotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(aimDirection);
        }
        else if (lookInput.x != 0.0f || lookInput.z != 0.0f)
        {
            //transform.rotation = Quaternion.LookRotation(new Vector3(lookInput.x, 0, lookInput.z)); //If not moving look where the player is aiming
            aimDirection = Vector3.Lerp(aimDirection, new Vector3(lookInput.x, 0, lookInput.z), seagullRotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(aimDirection);
        }
    }

    // Picking up a powerup
    private void OnTriggerEnter(Collider other)
    {
        PowerUpPickup powerUpPickup = other.GetComponent<PowerUpPickup>();
        if (activePowerup == ActivePowerup.NONE) // Ensures player cant pickup other powerups while they are holding one
        {
            if (powerUpPickup != null)
            {
                if (powerUpPickup.canBePickedUp)
                {
                    if (other.CompareTag("DonutPickup"))
                    {
                        activePowerup = ActivePowerup.DONUT;

                        animator.SetTrigger("PCStart");
                        seagullSounds.PlayEating();
                        WorldData.Instance.UpdateStats(playerNum, WorldData.statsType.totalDonuts, 1);
                        powerUpPickup.CollidedWithPlayer(this);
                    }
                    else if (other.CompareTag("ChiliPepperPickup"))
                    {
                        animator.SetTrigger("PCStart");
                        activePowerup = ActivePowerup.CHILIPEPPER;
                        breathin = true;

                        seagullSounds.PlayEating();
                        WorldData.Instance.UpdateStats(playerNum, WorldData.statsType.totalPeppers, 1);
                        powerUpPickup.CollidedWithPlayer(this);
                    }
                    else if (other.CompareTag("FireworkPickup"))
                    {
                        activePowerup = ActivePowerup.FIREWORK;

                        AudioManager.Instance.PlayAudio(AudioManager.AudioType.Firework, false);
                        seagullSounds.PlaySound(SeagullSounds.SeagullSoundType.Firework);

                        animator.SetTrigger("Firework");

                        fWObject.SetActive(true);

                        //PowerUpManager.Instance.FireworkActive(gameObject); // instantly use the powerup
                        WorldData.Instance.UpdateStats(playerNum, WorldData.statsType.totalFireworks, 1);
                        powerUpPickup.CollidedWithPlayer(this);
                    }
                    else if (other.CompareTag("ScrewPickup"))
                    {
                        animator.SetTrigger("PCStart");
                        AudioManager.Instance.PlayAudio(AudioManager.AudioType.ScrewPickup, false);
                        seagullSounds.PlaySound(SeagullSounds.SeagullSoundType.Screw);
                        //activePowerup = ActivePowerup.SCREW;
                        screwActive = true;
                        mesh.material = screwMat;

                        powerUpPickup.CollidedWithPlayer(this);
                    }
                    else if (other.CompareTag("PopcornPickup"))
                    {
                        animator.SetTrigger("PCStart");
                        activePowerup = ActivePowerup.POPCORN;
                        popcornAmmo = 3;
                        seagullSounds.PlayEating();
                        WorldData.Instance.UpdateStats(playerNum, WorldData.statsType.totalPopcorn, 1);
                        powerUpPickup.CollidedWithPlayer(this);
                    }
                    else if (other.CompareTag("CrustPickup"))
                    {
                        if (canPickUpCrust)
                        {
                            if (RoundManager.Instance.HoldingCrust == -1)
                            {
                                RoundManager.Instance.SetHoldingCrust(playerNum);
                                activePowerup = ActivePowerup.CRUST;

                                AudioManager.Instance.PlayAudio(AudioManager.AudioType.GetCrust, false);
                                seagullSounds.PlaySound(SeagullSounds.SeagullSoundType.CrustGurgle);

                                animator.SetTrigger("Crustin");
                                speed = crustSpeed;
                                if (!RoundManager.Instance.screwMode)
                                    crustObject.SetActive(true);
                                else
                                    screwObject.SetActive(true);
                                crustScript.enabled = true;

                                if (screwActive)
                                    mesh.material = goldMat;

                                powerUpPickup.CollidedWithPlayer(this);
                            }
                        }
                    }
                    else if (other.CompareTag("FryPickup"))
                    {
                        seagullSounds.PlayEating();
                        powerUpPickup.CollidedWithPlayer(this);
                    }
                }
            }
        }
        else if (activePowerup == ActivePowerup.CRUST)
        {
            if (other.CompareTag("FireworkPickup"))
            {
                AudioManager.Instance.PlayAudio(AudioManager.AudioType.Firework, false);
                seagullSounds.PlaySound(SeagullSounds.SeagullSoundType.Firework);

                animator.SetTrigger("Firework");
                fWObject.SetActive(true);
                //PowerUpManager.Instance.FireworkActive(gameObject); // instantly use the powerup
                WorldData.Instance.UpdateStats(playerNum, WorldData.statsType.totalFireworks, 1);

                powerUpPickup.CollidedWithPlayer(this);
            }
            else if (other.CompareTag("FryPickup"))
            {
                seagullSounds.PlayEating();
                powerUpPickup.CollidedWithPlayer(this);
            }
            else if (other.CompareTag("ScrewPickup"))
            {
                animator.SetTrigger("PCStart");
                AudioManager.Instance.PlayAudio(AudioManager.AudioType.ScrewPickup, false);
                seagullSounds.PlaySound(SeagullSounds.SeagullSoundType.Screw);
                //activePowerup = ActivePowerup.SCREW;
                screwActive = true;
                mesh.material = goldMat;

                powerUpPickup.CollidedWithPlayer(this);
            }
        }
        else if ((activePowerup == ActivePowerup.DONUT || activePowerup == ActivePowerup.POPCORN || activePowerup == ActivePowerup.CHILIPEPPER))
        {

            if (other.CompareTag("CrustPickup"))
            {
                if (canPickUpCrust)
                {
                    if (RoundManager.Instance.HoldingCrust == -1)
                    {
                        SwallowPowerUp();
                        RoundManager.Instance.SetHoldingCrust(playerNum);
                        activePowerup = ActivePowerup.CRUST;

                        AudioManager.Instance.PlayAudio(AudioManager.AudioType.GetCrust, true);
                        seagullSounds.PlaySound(SeagullSounds.SeagullSoundType.CrustGurgle);

                        animator.SetTrigger("Crustin");
                        speed = crustSpeed;
                        if (!RoundManager.Instance.screwMode)
                            crustObject.SetActive(true);
                        else
                            screwObject.SetActive(true);
                        crustScript.enabled = true;

                        if (screwActive)
                            mesh.material = goldMat;

                        powerUpPickup.CollidedWithPlayer(this);
                    }
                }
            }
            else if (other.CompareTag("FryPickup"))
            {
                seagullSounds.PlayEating();
                powerUpPickup.CollidedWithPlayer(this);
            }

        }

        if (other.CompareTag("DonutProjectile") && other.GetComponentInParent<DonutPowerUp>().sender != playerNum)
        {
            if (!stunned && canDmg)
            {
                TakeDamage(donutDmg);
                CheckKiller(donutDmg, other.GetComponentInParent<DonutPowerUp>().sender);
                AudioManager.Instance.PlayAudio(AudioManager.AudioType.Donut, false);
                Knockback(other, knockbackDonut);
            }
        }
        else if (other.CompareTag("PopcornProjectile") && other.GetComponentInParent<PopcornPowerUp>().sender != playerNum)
        {
            if (!stunned && canDmg)
            {
                TakeDamage(popCornDmg);
                CheckKiller(popCornDmg, other.GetComponent<PopcornPowerUp>().sender);
                AudioManager.Instance.PlayAudio(AudioManager.AudioType.PopCorn, false);
                Knockback(other, knockbackPopcorn);
            }
        }
        else if (other.CompareTag("FireworkProjectile") && other.gameObject.GetComponentInParent<PlayerController>().activePowerup == ActivePowerup.FIREWORK)
        {
            if (!stunned && canDmg)
            {
                TakeDamage(fireWorkDmg);
                CheckKiller(fireWorkDmg, other.GetComponentInParent<PlayerController>().playerNum);
                Knockback(other, knockbackFirework);
            }
        }
        else if (other.CompareTag("SpinHitBox"))
        {
            if (!stunned && canDmg && !RoundManager.Instance.GameStart)
            {
                TakeDamage(1);
                CheckKiller(1, other.GetComponentInParent<PlayerController>().playerNum);
                WorldData.Instance.UpdateStats(other.GetComponentInParent<PlayerController>().playerNum, WorldData.statsType.spinHits, 1);
                Knockback(other, knockbackBasicAttack);
            }
        }
        else if (other.CompareTag("DashHitBox") && !spinning)
        {
            if (!stunned && canDmg && !RoundManager.Instance.GameStart)
            {
                TakeDamage(1);
                CheckKiller(1, other.GetComponentInParent<PlayerController>().playerNum);
                AudioManager.Instance.PlayAudio(AudioManager.AudioType.Smash, false);
                WorldData.Instance.UpdateStats(other.GetComponentInParent<PlayerController>().playerNum, WorldData.statsType.smashHits, 1);
                Knockback(other, knockbackBasicAttack);
            }
        }
    }

    // Input to use the powerup
    public void OnFire(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            switch (activePowerup)
            {
                case ActivePowerup.DONUT:

                    animator.SetTrigger("Donut");
                    activePowerup = ActivePowerup.NONE;
                    break;

                case ActivePowerup.POPCORN:
                    //PowerUpManager.Instance.PopcornActive(this.gameObject);
                    animator.SetTrigger("PCFire");
                    break;

                case ActivePowerup.CHILIPEPPER:
                    PowerUpManager.Instance.ChiliPepperActive(this.gameObject);
                    if (breathin)
                    {
                        animator.SetTrigger("Chilli");
                    }
                    break;

                case ActivePowerup.FIREWORK:
                    //  You cannot get here - Drew
                    break;
            }
            //   if (activePowerup == ActivePowerup.POPCORN && popcornAmmo > 0)
            //   {
            //       //activePowerup = ActivePowerup.NONE;
            //   }
            //    else if (activePowerup != ActivePowerup.CHILIPEPPER) // make sure fire breath duration is completed before setting activepowerup to none
            //    {
            //        if (activePowerup == ActivePowerup.CRUST) { return; }
            //
            //        activePowerup = ActivePowerup.NONE;
            //        //popcornAmmo = 0;
            //    }
        }
    }

    public void DonutFireAnimThing()
    {
        PowerUpManager.Instance.DonutActive(this.gameObject);
    }

    public void ChilliEnd()
    {
        breathin = false;
        //activePowerup = ActivePowerup.NONE;
        animator.ResetTrigger("Chilli");
    }

    public void PopcornAnimFire()
    {
        if (popcornAmmo == 3)
            WorldData.Instance.UpdateStats(playerNum, WorldData.statsType.powerups, 1);

        seagullSounds.PlaySound(SeagullSounds.SeagullSoundType.PopCorn);
        PowerUpManager.Instance.PopcornActive(this.gameObject);
        popcornAmmo--;
    }

    public void SpinAnimStart()
    {
        wing1.Play();
        wing2.Play();
    }

    public void SpinAnimEnd()
    {
        wing1.Stop();
        wing2.Stop();
    }

    public void FireworkAnimeFire()
    {
        PowerUpManager.Instance.FireworkActive(gameObject);
    }

    private IEnumerator IdleAnimChooser()
    {
        for (; ; )
        {
            int randoIdleAnim2 = Random.Range(4, 7);
            int randoIdleAnim = Random.Range(1, 3);
            yield return new WaitForSeconds(randoIdleAnim2);
            if (!moving)
            {
                animator.SetInteger("IdleVar", randoIdleAnim);
            }
            yield return new WaitForSeconds(1.0f);
            animator.SetInteger("IdleVar", 0);
        }
    }

    public void Knockback(Collider other, float knockbackForce)
    {

        Vector3 direction = (transform.position - other.transform.position).normalized;
        if (!stunned)
        {
            animator.SetTrigger("Knock");
        }
        feather.Play();
        if (other.gameObject.CompareTag("Wave"))
        {
            StartCoroutine(KnockForcer(new Vector3(0.2f, 0.12f, -0.97f), knockbackForce));
        }
        else
        {
            StartCoroutine(KnockForcer(direction, knockbackForce));
        }

    }

    private IEnumerator KnockForcer(Vector3 direction, float knockbackForce)
    {
        this.GetComponent<Rigidbody>().AddForce(direction * knockbackForce);
        yield return new WaitForSeconds(0.05f);
        this.GetComponent<Rigidbody>().AddForce(direction * knockbackForce);
    }

    public void TakeDamage(int val)
    {
        if (canDmg)
        {

            //GetComponentInChildren<Renderer>().material.color = Color.red;
            if (screwActive)
            {
                mesh.material = currentMat;
                screwActive = false;
            }
            else
            {
                stamina -= val;

                if (stamina < 0)
                {
                    stamina = 0;
                }

                if (!RoundManager.Instance.hideHud)
                {
                    if (FeatherEvents.Instance)
                    {
                        FeatherEvents.Instance.ThisPlayerLostHealth(playerNum);
                    }
                    pUI.UpdateStaminaValue();
                }

                seagullSounds.PlaySound(SeagullSounds.SeagullSoundType.Hurt);
                //WorldData.Instance.UpdateStats(playerNum, WorldData.statsType.totalDamage, val); this is the wrong stat, damage dealt not taken
            }

            if (stamina == 0)
            {
                isHealing = true;
            }
            if (stamina == 0)
                ActivateStun(); //If Seagull has no health after taking damage stun him
            else
                StartCoroutine(Damaging(invulnTime));
        }
    }

    private IEnumerator Damaging(float invulnTime)
    {
        canDmg = false;
        StartCoroutine(InvulnRenderer());
        yield return new WaitForSeconds(invulnTime);
        canDmg = true;
    }

    public void GainStamina()
    {
        stamina++;
    }

    public void UpdateHealing(bool isIt)
    {
        isHealing = isIt;
    }

    private void ActivateStun()
    {
        AudioManager.Instance.PlayAudio(AudioManager.AudioType.Stun, false);
        StartCoroutine(Stunned());
    }

    private IEnumerator Stunned()
    {
        if (activePowerup == ActivePowerup.CRUST)
        {
            RevertBack();
        }

        WorldData.Instance.UpdateStats(playerNum, WorldData.statsType.knockedOut, 1);
        seagullSounds.PlaySound(SeagullSounds.SeagullSoundType.Stun);

        int randoStunAnim = Random.Range(1, 3);

        if (!stunned) { animator.SetInteger("Stunned", randoStunAnim); }
        controllable = false;
        stunned = true;
        canDmg = false;
        isHealing = true;
        canPickUpCrust = false;

        stun.Play();

        yield return new WaitForSeconds(1.5f);
        for (stamina = 1; stamina < RoundManager.Instance.MaxHealth + 1; stamina++)
        {
            if (!RoundManager.Instance.hideHud)
            {
                FeatherEvents.Instance.ThisPlayerGainedHealth(playerNum);
            }
            yield return new WaitForSeconds(0.8f);
        }
        stamina = RoundManager.Instance.MaxHealth;
        pUI.UpdateStaminaValue();

        //stamina = origStam;
        controllable = true;
        stunned = false;
        canDmg = true;
        canPickUpCrust = true;
        stun.Stop();
        animator.SetInteger("Stunned", 0);
    }

    public void OnSSpin(InputAction.CallbackContext ctx)
    {
        if ((activePowerup == ActivePowerup.NONE) && controllable)
        {
            StartCoroutine(basicAttackManager.SeagullSpin(spinHitBox));
        }
    }

    public void OnSSmash(InputAction.CallbackContext ctx)
    {
        if ((activePowerup == ActivePowerup.NONE) && controllable)
        {
            StartCoroutine(basicAttackManager.SeagullSmash(rb));
        }
    }

    public void OnSShot(InputAction.CallbackContext ctx)
    {
        if ((activePowerup == ActivePowerup.NONE) && controllable && shotMeter == 5)
        {
            StartCoroutine(basicAttackManager.SeagullShot(transform, playerInput));
        }
    }

    public void OnSStep(InputAction.CallbackContext ctx)
    {
        if (activePowerup == ActivePowerup.CRUST && controllable)
        {
            StartCoroutine(basicAttackManager.SeagullStep(rb));
        }
    }

    public void OnSwallow(InputAction.CallbackContext ctx)
    {
        SwallowPowerUp();
    }

    //called by golden crust script, reverts values back to normal after losing the crust
    public void RevertBack()
    {
        if (activePowerup == ActivePowerup.CRUST)
        {
            activePowerup = ActivePowerup.NONE;
            speed = normalSpeed;
            crustScript.LoseCrust();
        }
    }

    public void SetStamina(int val)
    {
        stamina = val;
    }

    public void StartGame()
    {
        controllable = true;
        canDmg = true;
        isHealing = false;
    }

    public void IncrementShotMeter()
    {
        shotMeter++;
        if (shotMeter > 5)
        {
            shotMeter = 5;
        }
        if (!RoundManager.Instance.hideHud)
        {
            HudManager.Instance.IncreaseShotMeter(playerNum, shotMeter);
        }
    }

    public void ResetShotMeter()
    {
        shotMeter = 0;
        if (HudManager.Instance)
        {
            if (!RoundManager.Instance.hideHud)
            {
                HudManager.Instance.ResetShotMeter(playerNum);
            }
        }
    }

    //Checks to see who stunned you
    private void CheckKiller(int dmg, int playerId)
    {
        WorldData.Instance.UpdateStats(playerId, WorldData.statsType.totalDamage, dmg);

        if (stamina <= dmg && !stunned)
        {
            WorldData.Instance.UpdateStats(playerId, WorldData.statsType.knockOuts, 1);
        }
    }

    private void SwallowPowerUp()
    {

        if ((activePowerup == ActivePowerup.DONUT || activePowerup == ActivePowerup.POPCORN || activePowerup == ActivePowerup.CHILIPEPPER))
        {
            seagullSounds.PlayEating();
            activePowerup = ActivePowerup.NONE;
            WorldData.Instance.players[this.playerNum][WorldData.statsType.calorieCount]++;
            FeatherEvents.Instance.ThisPlayerGainedCalories(this.playerNum);

            for (int i = 0; i < powerUpShotVal; i++)
            {
                IncrementShotMeter();
            }

        }
    }

    private IEnumerator InvulnRenderer()
    {
        for (int i = 0; i < 5; i++)
        {
            mesh.material = invisibleMat;

            yield return new WaitForSeconds(invulnTime / 10);

            mesh.material = currentMat;

            yield return new WaitForSeconds(invulnTime / 10);
        }

        yield return null;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Wave"))
        {
            if (stamina > 0)
            {
                TakeDamage(RoundManager.Instance.MaxHealth);
            }
            Knockback(collision.collider, 5000f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("ShitPoolHitbox"))
        {
            //Debug.LogWarning("Rollin in the poo  " + playerNum);

            if (!stunned && canDmg && !RoundManager.Instance.GameStart && other.GetComponentInChildren<ShitSlow>().playerNum != playerNum)
            {
                TakeDamage(1);
                CheckKiller(1, other.GetComponentInChildren<ShitSlow>().playerNum);
            }

        }
        else if (other.CompareTag("PepperProjectile") && other.GetComponent<ChilliBreath>().playerNum != playerNum)
        {
            burn.Play();
            if (!stunned && canDmg)
            {
                TakeDamage(flamethrowerDmg);
                AudioManager.Instance.PlayAudio(AudioManager.AudioType.Chili, false);
                WorldData.Instance.UpdateStats(other.GetComponent<ChilliBreath>().playerNum, WorldData.statsType.fireDamage, 1); //This may or may not work xD! JkJkJkJk no touchy!111
                CheckKiller(flamethrowerDmg, other.GetComponent<ChilliBreath>().playerNum);
                Knockback(other, knockbackChilliPepper);
            }
        }
    }

    public void OnPause(InputAction.CallbackContext ctx)
    {
        //Debug.Log("ctx duration " + ctx.duration);

        if (!RoundManager.Instance.GameStart && !RoundManager.Instance.GameOver)
        {
            if (!PauseMenu.Instance.PauseBarStatus())
                PauseMenu.Instance.StartPauseBar(ctx, playerNum);

            if (ctx.performed)
                PauseMenu.Instance.PauseButtonPerformed(playerNum);

            if (ctx.canceled)
                PauseMenu.Instance.ResetPauseBuffer();
        }

    }

    public void OnSSquawk(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !stunned && controllable && canSquawk)
        {
            StartCoroutine(SeagullSquawking());
        }

    }

    private IEnumerator SeagullSquawking()
    {
        canSquawk = false;
        seagullSounds.PlaySound(SeagullSounds.SeagullSoundType.Squawk);
        animator.SetTrigger("Squawk");
        yield return new WaitForSeconds(squawkCooldown);
        canSquawk = true;
    }
}