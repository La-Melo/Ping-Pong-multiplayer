using Unity.Netcode;
using UnityEngine;

public class PowerUpMovement : NetworkBehaviour
{
    [Tooltip("The speed at which the power-up moves.")]
    public float speed = 3f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError("[PowerUpMovement] Rigidbody2D is missing!");
    }

    public override void OnNetworkSpawn()
    {
        if (IsSessionOwner)
        {
            Launch();
        }
    }

    public void Launch()
    {
        if (!IsSessionOwner)
            return;

        float xDir = Random.Range(0, 2) == 0 ? -1f : 1f;
        float yDir = Random.Range(0, 2) == 0 ? -1f : 1f;
        Vector2 velocity = new Vector2(xDir, yDir).normalized * speed;

        rb.linearVelocity = velocity;
        Debug.Log("[PowerUpMovement] Power-Up Launched with velocity: " + velocity);
    }
}
