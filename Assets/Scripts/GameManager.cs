using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Button button1;
    [SerializeField] private GameObject box1;

    [SerializeField] private GameObject box2;
    [SerializeField] private GameObject box2Endpoint;
    [SerializeField] private GameObject rope1;

    private AreaCollider rope1Collider;

    [SerializeField] private GameObject box1StartPoint;
    [SerializeField] private GameObject box1EndPoint;

    [SerializeField] private GameObject stage2cameraPos;
    [SerializeField] private Camera camera;
    [SerializeField] private AreaTrigger stage2trigger;
    [SerializeField] private GameObject stage2InvisibleWall;
    [SerializeField] private GameObject player;

    [SerializeField] private GameObject[] layers;
    [SerializeField] private Collider[] colliders;

    [SerializeField] private Collider playerFirstCollider;
    [SerializeField] private Collider playerSecondCollider;

    private float boxFallTime = 3f;
    private float cameraMoveTime = 1.7f;

    private Coroutine box1FallCoroutine = null;

    private Coroutine box2FallCoroutine = null;

    private Coroutine cameraStage2Corutine = null;

    private Coroutine snapBackCorutine = null;

    private const string playerTag = "Player";

    private bool cameraStage2Moved = false;

    private int layer = 0;
    private int lastLayer = 0;
    private int maxLayer = 2;

    private Player playerComponent;

    private void Start()
    {
        rope1Collider = rope1.GetComponentInChildren<AreaCollider>();

        button1.OnPlayerPressed += HandleButton1Press;
        rope1Collider.OnColliderEntered += HandleRope1Enter;

        stage2trigger.OnTriggerEntered += HandleStage2;

        playerComponent = player.GetComponent<Player>();

        playerComponent.OnOverlap += HandlePlayerOverlap;
    }

    private void Update()
    {
        if (snapBackCorutine != null) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (layer + 1 <= maxLayer)
            {
                ChangeLayer(1);
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (layer - 1 >= 0)
            {
                ChangeLayer(-1);
            }
        }

        foreach (Collider collider in colliders)
        {
            if (collider == null || !collider.gameObject.activeInHierarchy) continue;

            if (Mathf.Abs(collider.transform.position.z - player.transform.position.z) > 0.5f)
                continue;

            bool areColliding1 = Physics.ComputePenetration(
                playerFirstCollider, playerFirstCollider.bounds.center, playerFirstCollider.transform.rotation,
                collider, collider.bounds.center, collider.transform.rotation,
                out Vector3 direction, out float distance
            );

            bool areColliding2 = Physics.ComputePenetration(
                playerSecondCollider, playerSecondCollider.bounds.center, playerSecondCollider.transform.rotation,
                collider, collider.bounds.center, collider.transform.rotation,
                out Vector3 direction2, out float distance2
            );

            if ((areColliding1 && distance >= 0.3f) || areColliding2 && distance2 >= 0.3f)
            {
                SetBack();
                break;
            }
        }
    }

    private void SetBack()
    {
        if (snapBackCorutine == null)
        {
            snapBackCorutine = StartCoroutine(SnapBackCoroutine());
        }
    }

    private IEnumerator SnapBackCoroutine()
    {
        playerComponent.SetCanMove = false;

        yield return new WaitForSeconds(0.5f);

        ChangeLayer(lastLayer - layer);

        playerComponent.SetCanMove = true;

        snapBackCorutine = null;
    }

    private void ChangeLayer(int direction)
    {
        lastLayer = layer;
        layer += direction;
        playerComponent.CurrentPlaneDist = playerComponent.CurrentPlaneDist + direction;

        layers[layer].SetActive(true);

        if (lastLayer < layer)
        {
            layers[lastLayer].SetActive(false);
        }

        //for (int i = layer - 1; i >= 0; i--)
        //{
        //    layers[i].SetActive(false);
        //}

        camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, camera.transform.position.z + 1f * direction);
        player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z + 1f * direction);
    }

    private void HandlePlayerOverlap()
    {
        //  Debug.Log("asd");
    }

    private void HandleButton1Press()
    {
        if (box1FallCoroutine == null)
        {
            box1FallCoroutine = StartCoroutine(MoveObjectTowards(box1, box1EndPoint, boxFallTime));
        }
    }

    private void HandleStage2(Collider collider)
    {
        if (collider.transform.CompareTag(playerTag) && !cameraStage2Moved)
        {
            if (cameraStage2Corutine == null)
            {
                cameraStage2Corutine = StartCoroutine(MoveObjectTowards(camera.gameObject, stage2cameraPos, cameraMoveTime));

                stage2InvisibleWall.SetActive(true);
                cameraStage2Moved = true;
            }
        }
    }

    private IEnumerator MoveObjectTowards(GameObject fallingObject, GameObject endPoint, float fallTime)
    {
        float t = 0f;

        Vector3 startingPos = fallingObject.transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime / fallTime;

            Vector3 newPos = fallingObject.transform.position;

            newPos = Vector3.Lerp(startingPos, endPoint.transform.position, t);

            fallingObject.transform.position = new Vector3(newPos.x, newPos.y, newPos.z);

            yield return null;
        }
    }

    private void HandleRope1Enter(Collision collision)
    {
        Destroy(rope1);

        if (box2FallCoroutine == null)
        {
            box2FallCoroutine = StartCoroutine(MoveObjectTowards(box2, box2Endpoint, boxFallTime));
        }
    }

    private void OnDestroy()
    {
        button1.OnPlayerPressed -= HandleButton1Press;
        rope1Collider.OnColliderEntered -= HandleRope1Enter;

        stage2trigger.OnTriggerEntered -= HandleStage2;

        playerComponent.OnOverlap -= HandlePlayerOverlap;
    }
}
