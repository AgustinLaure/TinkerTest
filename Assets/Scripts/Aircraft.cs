using UnityEngine;

public class Aircraft : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;

    [SerializeField] private AreaCollider areaCollider;

    public Vector3 direction = Vector3.forward;

    private void Awake()
    {
        areaCollider.OnColliderEntered += HandleCollision;
    }

    private void Update()
    {
        gameObject.transform.position = gameObject.transform.position + direction * (speed * Time.deltaTime);
    }

    private void HandleCollision(Collision collision)
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        areaCollider.OnColliderEntered -= HandleCollision;
    }
}
