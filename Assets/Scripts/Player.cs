using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public event Action OnOverlap;

    public static string playerTag = "Player";

    [SerializeField] private float acceleration;
    [SerializeField] private float terminalVelocity;
    [SerializeField] private float jumpImpulse;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private AreaTrigger floorDetectionTrigger;
    [SerializeField] private float deacceleration;
    [SerializeField] private CapsuleCollider legsCollider;
    [SerializeField] private CapsuleCollider torsoCollider;
    [SerializeField] private GameObject aircraftContainer;
    [SerializeField] private float shootForce;


    [SerializeField] private AreaCollider bodyCollider;

    [SerializeField] private GameObject aircraftPreab;
    [SerializeField] private GameObject shootingPoint;

    [SerializeField] private GameObject crane;
    [SerializeField] private GameObject trace;

    [SerializeField] private float weight;

    private ForceMode jumpForceMode = ForceMode.Impulse;
    private ForceMode walkForceMode = ForceMode.Acceleration;

    private float axisInput = 0f;
    private float prevAxisInput = 0f;

    private float slowFallSpeed = 0.06f;

    private bool hasCrane = false;
    private bool shouldSlowFall = false;

    private bool isGrounded = false;

    private bool shouldJump = false;

    private const string groundTag = "Ground";

    private int unlockT = 0;

    private Coroutine overlapCheck = null;

    private const float overlapTime = 4f;

    [SerializeField] private float shootingPointDistance;

    private bool canMove = true;

    private float currentPlaneDist = 0;

    private Vector3 shootDir = Vector3.zero;
    private Vector3 mouseGlobalPos = Vector3.zero;
    private bool isHoldingTrigger = false;

    public bool SetCanMove { set { canMove = value; } }

    public float GetWeight { get { return weight; } }

    public float CurrentPlaneDist { set { currentPlaneDist = value; } get { return currentPlaneDist; } }

    private void Awake()
    {
        floorDetectionTrigger.OnTriggerEntered += HandleFloorDetectionTriggerEnter;
        floorDetectionTrigger.OnTriggerExited += HandleFloorDetectionTriggerExit;

        bodyCollider.OnColliderEntered += HandleBodyColliderEnter;

        bodyCollider.OnColliderExited += HandleBodyColliderExit;

        crane.SetActive(false);
        trace.SetActive(false);
    }

    private void Update()
    {
        prevAxisInput = axisInput;
        axisInput = Input.GetAxisRaw("Horizontal");

        bool isCraneActive = false;

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                if (canMove)
                {
                    shouldJump = true;
                }
            }
        }
        else if (Input.GetButton("Jump"))
        {
            if (hasCrane && !isGrounded && rb.linearVelocity.y < 0f)
            {
                if (canMove)
                {
                    shouldSlowFall = true;
                    isCraneActive = true;
                }
            }
        }

        crane.SetActive(isCraneActive);

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (unlockT == 0)
            {
                weight += 10f;
                //Debug.Log("Ganaste peso");
                unlockT++;
            }
            else if (unlockT == 1)
            {
                hasCrane = true;
                //Debug.Log("Desbloqueaste la grulla");
                unlockT++;
            }
        }

        if (canMove)
        {
            Vector3 closestPoint = torsoCollider.ClosestPoint(transform.position + shootDir * torsoCollider.bounds.extents.magnitude * 2f);

            if (Input.GetButton("Shoot"))
            {
                isHoldingTrigger = true;

                Plane worldPlane = new Plane(new Vector3(0f, 0f, -1f), currentPlaneDist);

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


                if (worldPlane.Raycast(ray, out float distance))
                {


                    trace.SetActive(true);

                    mouseGlobalPos = ray.GetPoint(distance);

                    mouseGlobalPos.y = Mathf.Clamp(mouseGlobalPos.y, transform.position.y, float.MaxValue);

                    if (Mathf.Abs(mouseGlobalPos.x - transform.position.x) + Mathf.Abs(mouseGlobalPos.y - transform.position.y) > 1f)
                    {
                        shootDir = (mouseGlobalPos - transform.position).normalized;

                        closestPoint = torsoCollider.ClosestPoint(transform.position + shootDir * torsoCollider.bounds.extents.magnitude * 2f);

                        trace.transform.position = closestPoint + shootDir * shootingPointDistance;

                        Vector3 upDir = Vector3.Cross(shootDir, Vector3.forward);
                        trace.transform.rotation = Quaternion.LookRotation(shootDir, upDir);
                    }
                }
            }

            if (Input.GetButtonUp("Shoot") && isHoldingTrigger)
            {
                GameObject auxAircraft = Instantiate(aircraftPreab, closestPoint + shootDir * shootingPointDistance, Quaternion.identity, aircraftContainer.transform);

                Aircraft auxAircraftComp = auxAircraft.GetComponent<Aircraft>();

                Vector3 upDir = Vector3.Cross(shootDir, Vector3.forward);
                auxAircraft.transform.rotation = Quaternion.LookRotation(shootDir, upDir);

                auxAircraftComp.direction = shootDir;

                isHoldingTrigger = false;

                trace.SetActive(false);
            }
        }

        if (canMove)
        {
            if (axisInput > 0f)
            {
                transform.rotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
            }
            if (axisInput < 0f)
            {
                transform.rotation = Quaternion.LookRotation(Vector3.left, Vector3.up);
            }
        }


        if (isGrounded)
        {
            legsCollider.material.dynamicFriction = 1f;
        }
        else
        {
            legsCollider.material.dynamicFriction = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (shouldJump)
        {
            rb.AddForce(new Vector3(0f, jumpImpulse, 0f), jumpForceMode);
            shouldJump = false;
        }

        if (shouldSlowFall)
        {
            if (rb.linearVelocity.y < 0f)
            {
                rb.useGravity = false;
                rb.AddForce(Physics.gravity * slowFallSpeed, ForceMode.Acceleration);
            }

            shouldSlowFall = false;
        }
        else
        {
            rb.useGravity = true;
        }

        if (canMove)
        {
            rb.AddForce(new Vector3(axisInput * acceleration, 0f, 0f), walkForceMode);
        }

        rb.linearVelocity = new Vector3(Mathf.Clamp(rb.linearVelocity.x, -terminalVelocity, terminalVelocity), rb.linearVelocity.y, rb.linearVelocity.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(new Vector3(0f, 0f, currentPlaneDist), new Vector3(0f, 0f, currentPlaneDist - 1f));
        Gizmos.DrawLine(transform.position, mouseGlobalPos);
    }

    private void HandleBodyColliderEnter(Collision collision)
    {
        overlapCheck = StartCoroutine(OverlapCheckCoroutine());
    }

    private void HandleBodyColliderExit(Collision collision)
    {
        StopCoroutine(overlapCheck);
    }

    private IEnumerator OverlapCheckCoroutine()
    {
        float timer = 0f;

        while (timer < overlapTime)
        {
            timer += Time.deltaTime;

            yield return null;
        }

        OnOverlap?.Invoke();
    }

    private void HandleFloorDetectionTriggerEnter(Collider collider)
    {
        if (collider.CompareTag(groundTag))
        {
            isGrounded = true;
        }
    }

    private void HandleFloorDetectionTriggerExit(Collider collider)
    {
        if (collider.CompareTag(groundTag))
        {
            isGrounded = false;
        }
    }

    private void OnDestroy()
    {
        floorDetectionTrigger.OnTriggerEntered -= HandleFloorDetectionTriggerEnter;
        floorDetectionTrigger.OnTriggerExited -= HandleFloorDetectionTriggerExit;

        bodyCollider.OnColliderEntered -= HandleBodyColliderEnter;
    }
}
