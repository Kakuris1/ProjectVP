using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;

    private IPlayerInput playerInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

#if UNITY_EDITOR || UNITY_STANDALONE
        playerInput = new PCInput();
#elif UNITY_ANDROID || UNITY_IOS
        playerInput = new MobileInput();
#endif

    }

    void FixedUpdate()
    {
        Vector2 input = playerInput.GetMovementInput();
        Vector3 moveDir = new Vector3(input.x, 0, input.y);
        rb.MovePosition(transform.position + moveDir * moveSpeed * Time.fixedDeltaTime);
    }

}
