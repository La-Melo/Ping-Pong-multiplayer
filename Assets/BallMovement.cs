using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(NetworkObject))]
public class BallMovement : NetworkBehaviour
{
    [Tooltip("The speed at which the ball moves.")]
    public float speed = 5f;

    [Tooltip("Reference to the Rigidbody2D component.")]
    public Rigidbody2D rb;

    private Vector2 startPosition;

    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (rb == null)
            Debug.LogError("BallMovement: Rigidbody2D component not found!");
    }

    public override void OnNetworkSpawn()
    {
        startPosition = transform.position;

        // Only the server should check for players and launch the ball.
        if (IsServer)
        {
            if (ArePlayersInScene())
            {
                Launch();
            }
        }
    }

    /// <summary>
    /// Checks if both Player1 and Player2 are present in the scene.
    /// </summary>
    private bool ArePlayersInScene()
    {
        return GameObject.FindWithTag("Player1") != null && GameObject.FindWithTag("Player2") != null;
    }

    void Launch()
    {
        float xMovement = Random.Range(0, 2) == 0 ? -1f : 1f;
        float yMovement = Random.Range(0, 2) == 0 ? -1f : 1f;

        rb.linearVelocity = new Vector2(xMovement * speed, yMovement * speed);
    }

    public void ResetBall()
    {
        rb.linearVelocity = Vector2.zero;
        transform.position = startPosition;
        Launch();
    }
}