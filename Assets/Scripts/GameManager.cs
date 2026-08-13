using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Button button1;
    [SerializeField] private GameObject box1;

    [SerializeField] private GameObject box1StartPoint;
    [SerializeField] private GameObject box1EndPoint;

    private float boxFallTime = 3f;

    private Coroutine box1FallCoroutine = null;

    private void Start()
    {
        button1.OnPlayerPressed += HandleButton1Press;
    }

    private void HandleButton1Press()
    {
        if (box1FallCoroutine == null)
        {
            box1FallCoroutine = StartCoroutine(Box1FallAnim());
        }
    }

    private IEnumerator Box1FallAnim()
    {
        float t = 0f;

        Vector3 startingPos = box1StartPoint.transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime / boxFallTime;

            Vector3 newPos = box1.transform.position;

            newPos = Vector3.Lerp(startingPos, box1EndPoint.transform.position, t);

            box1.transform.position = new Vector3(box1.transform.position.x, newPos.y, box1.transform.position.z);

            yield return null;
        }
    }

    private void OnDestroy()
    {
        button1.OnPlayerPressed -= HandleButton1Press;
    }
}
