using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Player : MonoBehaviour
{
    [SerializeField] private float acceleration;
    [SerializeField] private float terminalVelocity;
    [SerializeField] private float jumpImpulse;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private AreaTrigger floorDetectionTrigger;
    [SerializeField] private float deacceleration;
    [SerializeField] private CapsuleCollider legsCollider;
    [SerializeField] private GameObject aircraftContainer;

    [SerializeField] private GameObject aircraftPreab;
    [SerializeField] private GameObject shootingPoint;

    [SerializeField] private GameObject crane;

    [SerializeField] private float weight;

    private ForceMode jumpForceMode = ForceMode.Impulse;
    private ForceMode walkForceMode = ForceMode.Acceleration;

    private Quaternion lastRotation = Quaternion.identity;

    private float axisInput = 0f;
    private float prevAxisInput = 0f;

    private float slowFallSpeed = 0.06f;

    private bool hasCrane = false;
    private bool shouldSlowFall = false;

    private bool isGrounded = false;

    private bool shouldJump = false;

    private const string groundTag = "Ground";

    private int unlockT = 0;

    public float GetWeight { get { return weight; } }

    private void Awake()
    {
        floorDetectionTrigger.OnTriggerEntered += HandleFloorDetectionTriggerEnter;
        floorDetectionTrigger.OnTriggerExited += HandleFloorDetectionTriggerExit;

        crane.SetActive(false);
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
                shouldJump = true;
            }
        }
        else if (Input.GetButton("Jump"))
        {
            if (hasCrane && !isGrounded && rb.linearVelocity.y < 0f)
            {
                shouldSlowFall = true;
                isCraneActive = true;
            }
        }

        crane.SetActive(isCraneActive);

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (unlockT == 0)
            {
                weight += 10f;
                Debug.Log("Ganaste peso");
                unlockT++;
            }
            else if (unlockT == 1)
            {
                hasCrane = true;
                Debug.Log("Desbloqueaste la grulla");
                unlockT++;
            }
        }

        if (Input.GetButtonDown("Shoot"))
        {
            GameObject auxAircraft = Instantiate(aircraftPreab, shootingPoint.transform.position, Quaternion.identity, aircraftContainer.transform);

            Aircraft auxAircraftComp = auxAircraft.GetComponent<Aircraft>();

            auxAircraftComp.direction = gameObject.transform.forward;
        }

        if (axisInput > 0f)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
        }
        if (axisInput < 0f)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.left, Vector3.up);
        }


        if (isGrounded)
        {
            legsCollider.material.dynamicFriction = 1f;
        }
        else
        {
            legsCollider.material.dynamicFriction = 0f;
        }

        Debug.Log("peso del player" + weight);
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

        rb.AddForce(new Vector3(axisInput * acceleration, 0f, 0f), walkForceMode);

        rb.linearVelocity = new Vector3(Mathf.Clamp(rb.linearVelocity.x, -terminalVelocity, terminalVelocity), rb.linearVelocity.y, rb.linearVelocity.z);
    }


    private void OnDestroy()
    {
        floorDetectionTrigger.OnTriggerEntered -= HandleFloorDetectionTriggerEnter;
        floorDetectionTrigger.OnTriggerExited -= HandleFloorDetectionTriggerExit;
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
}
