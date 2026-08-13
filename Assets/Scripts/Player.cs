using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float acceleration;
    [SerializeField] private float terminalVelocity;
    [SerializeField] private float jumpImpulse;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private AreaTrigger floorDetectionTrigger;
    [SerializeField] private float deacceleration;
    [SerializeField] private CapsuleCollider legsCollider;

    [SerializeField] private float weight;

    private ForceMode jumpForceMode = ForceMode.Impulse;
    private ForceMode walkForceMode = ForceMode.Acceleration;

    private float axisInput = 0f;

    private bool isGrounded = false;

    private bool shouldJump = false;

    private const string groundTag = "Ground";

    public float GetWeight { get { return weight; } }

    private void Awake()
    {
        floorDetectionTrigger.OnTriggerEntered += HandleFloorDetectionTriggerEnter;
        floorDetectionTrigger.OnTriggerExited += HandleFloorDetectionTriggerExit;
    }

    private void Update()
    {
        axisInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            shouldJump = true;
        }

        if (Input.GetKey(KeyCode.Tab))
        {
            weight += 10f;
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

        rb.AddForce(new Vector3(axisInput * acceleration, 0f, 0f), walkForceMode);

        rb.linearVelocity = new Vector3(Mathf.Clamp(rb.linearVelocity.x, -terminalVelocity, terminalVelocity), rb.linearVelocity.y, rb.linearVelocity.z);

        if (Mathf.Abs(rb.linearVelocity.x) > terminalVelocity)
        {
        }
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
