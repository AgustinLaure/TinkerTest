using UnityEngine;

public class MovesUIHandler : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private SpriteRenderer[] spriteRenderers;

    private Quaternion initialRotation;
    private Vector3 initIalLocalPos;

    private int spriteCount = 0;

    private const int maxMoves = 6;
    public int GetMaxMoves { get { return maxMoves; } }

    private void Awake()
    {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.transform.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        initialRotation = transform.rotation;
        initIalLocalPos = transform.localPosition;
    }

    public void AddSymbol(Origami.Moves move)
    {
        spriteRenderers[spriteCount].transform.gameObject.SetActive(true);
        spriteRenderers[spriteCount].sprite = sprites[(int)move];

        if (spriteCount > 0)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - 0.23f);
        }

        spriteCount++;
    }

    public void ClearSymbols()
    {
        transform.localPosition = initIalLocalPos;
        spriteCount = 0;
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.transform.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        transform.rotation = initialRotation;
    }
}
