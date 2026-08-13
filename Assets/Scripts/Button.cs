using System;
using UnityEngine;

public class Button : MonoBehaviour
{
    public event Action OnPlayerPressed;

    [SerializeField] private AreaTrigger areaTrigger;
    [SerializeField] private Player player;
    [SerializeField] private float weightToPress;
    private const string playerTag = "Player";
    private bool isPlayerOnTop = false;
    private bool isPressed = false;

    private void Awake()
    {
        areaTrigger.OnTriggerEntered += HandleButtonPress;
        areaTrigger.OnTriggerExited += HandleButtonStopPress;
    }

    private void Update()
    {
        if (isPlayerOnTop)
        {
            Debug.Log("player arriba");
        }
        else
        {
            Debug.Log("player abajo");
        }
    }


    private void HandleButtonPress(Collider collider)
    {
        if (collider.gameObject.CompareTag(playerTag))
        {
             if (player.GetWeight >= weightToPress && !isPressed)
            {
                isPlayerOnTop = true;
                isPressed = true;
                OnPlayerPressed?.Invoke();
            }
        }
    }

    private void HandleButtonStopPress(Collider collider)
    {
        if (collider.gameObject.CompareTag(playerTag) && !isPressed)
        {
            isPlayerOnTop = false;
        }
    }

    private void OnDestroy()
    {
        areaTrigger.OnTriggerEntered -= HandleButtonPress;
        areaTrigger.OnTriggerExited -= HandleButtonStopPress;
    }
}
