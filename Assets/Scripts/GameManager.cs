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

    private float boxFallTime = 3f;
    private float cameraMoveTime = 1.7f;

    private Coroutine box1FallCoroutine = null;

    private Coroutine box2FallCoroutine = null;

    private Coroutine cameraStage2Corutine = null;

    private const string playerTag = "Player";

    private bool cameraStage2Moved = false;

    private void Start()
    {
        rope1Collider = rope1.GetComponentInChildren<AreaCollider>();

        button1.OnPlayerPressed += HandleButton1Press;
        rope1Collider.OnColliderEntered += HandleRope1Enter;

        stage2trigger.OnTriggerEntered += HandleStage2;
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
    }
}
