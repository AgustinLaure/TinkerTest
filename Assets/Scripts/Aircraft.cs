using System.Diagnostics;
using UnityEngine;

public class Aircraft : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;

    [SerializeField] private AreaCollider areaCollider;
    [SerializeField] private GameObject deadBody;
    [SerializeField] private GameObject aliveBody;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float accel;
    [SerializeField] private float accelDuration;
    [SerializeField] private float initialImpulse;

    private float zPos = 0f;

    private float accelDurationTimer = 0f;

    public Vector3 direction = Vector3.forward;

    private bool shouldImpulse = true;

    private Vector3 linearVelocity = Vector3.zero;

    private bool isPlane = true;
    private bool shouldTransition = false;

    private void Awake()
    {
        areaCollider.OnColliderEntered += HandleCollision;
    }

    private void Start()
    {
        zPos = transform.position.z;
    }

    private void Update()
    {
        if (isPlane)
        {
            linearVelocity = rb.linearVelocity;

            if (rb.linearVelocity.x != 0f || rb.linearVelocity.y != 0f)
            {
                Vector3 linear = rb.linearVelocity.normalized;
                Vector3 newUp = Vector3.Cross(linear, Vector3.forward);
                transform.rotation = Quaternion.LookRotation(linear, newUp);
            }
        }
    }

    private void FixedUpdate()
    {
        if (isPlane)
        {
            if (shouldImpulse)
            {
                rb.AddForce(direction * initialImpulse, ForceMode.Impulse);
                shouldImpulse = false;
            }

            accelDurationTimer += Time.deltaTime;

            if (accelDurationTimer < accelDuration)
            {
                float thisFrameAccel = accel - (accelDurationTimer / accelDuration);

                rb.AddForce(direction * thisFrameAccel, ForceMode.Acceleration);
            }
        }
    }

    private void LateUpdate()
    {
        if (!isPlane)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z);
            //transform.position = new Vector3(transform.position.x, transform.position.y, zPos);
        }

        if (shouldTransition)
        {
            //rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            isPlane = false;
            aliveBody.SetActive(false);
            deadBody.SetActive(true);
            shouldTransition = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + linearVelocity);
        //Gizmos.DrawLine(transform.position, transform.position + linearVelocity * 0.5f);
    }

    private void HandleCollision(Collision collision)
    {
        if (collision.transform.CompareTag("KeyItem"))
        {
            Destroy(gameObject);
        }
        else if (isPlane)
        {
            shouldTransition = true;
        }
    }

    private void OnDestroy()
    {
        areaCollider.OnColliderEntered -= HandleCollision;
    }
}
