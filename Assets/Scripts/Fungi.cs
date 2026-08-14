using UnityEngine;

public class Fungi : MonoBehaviour
{
    [SerializeField] private AreaCollider headCollider;

    [SerializeField] private float jumpImpulse;

    private void Awake()
    {
        headCollider.OnColliderEntered += HandleHeadHit;
    }

    private void HandleHeadHit(Collision collision)
    {
        if (collision.transform.CompareTag(Player.playerTag))
        {
            collision.rigidbody.AddForce(new Vector3(0f, jumpImpulse, 0f), ForceMode.Impulse);
        }
    }

    private void OnDestroy()
    {
        headCollider.OnColliderEntered -= HandleHeadHit;
    }
}
