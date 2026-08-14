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

    private float boxFallTime = 3f;

    private Coroutine box1FallCoroutine = null;

    private Coroutine box2FallCoroutine = null;

    private void Start()
    {
        rope1Collider = rope1.GetComponentInChildren<AreaCollider>();

        button1.OnPlayerPressed += HandleButton1Press;
        rope1Collider.OnColliderEntered += HandleRope1Enter;
    }

    private void HandleButton1Press()
    {
        if (box1FallCoroutine == null)
        {
            box1FallCoroutine = StartCoroutine(ObjectFallCoroutine(box1, box1EndPoint, boxFallTime));
        }
    }

    //private IEnumerator Box1FallAnim()
    //{
    //    float t = 0f;
    //
    //    Vector3 startingPos = box1StartPoint.transform.position;
    //
    //    while (t < 1f)
    //    {
    //        t += Time.deltaTime / boxFallTime;
    //
    //        Vector3 newPos = box1.transform.position;
    //
    //        newPos = Vector3.Lerp(startingPos, box1EndPoint.transform.position, t);
    //
    //        box1.transform.position = new Vector3(box1.transform.position.x, newPos.y, box1.transform.position.z);
    //
    //        yield return null;
    //    }
    //}

    private IEnumerator ObjectFallCoroutine(GameObject fallingObject, GameObject endPoint, float fallTime)
    {
        float t = 0f;

        Vector3 startingPos = fallingObject.transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime / fallTime;

            Vector3 newPos = fallingObject.transform.position;

            newPos = Vector3.Lerp(startingPos, endPoint.transform.position, t);

            fallingObject.transform.position = new Vector3(fallingObject.transform.position.x, newPos.y, fallingObject.transform.position.z);

            yield return null;
        }
    }

    private void HandleRope1Enter(Collision collision)
    {
        Destroy(rope1);

        if (box2FallCoroutine == null)
        {
            box2FallCoroutine = StartCoroutine(ObjectFallCoroutine(box2, box2Endpoint, boxFallTime));
        }
    }

    private void OnDestroy()
    {
        button1.OnPlayerPressed -= HandleButton1Press;
        rope1Collider.OnColliderEntered -= HandleRope1Enter;
    }
}
