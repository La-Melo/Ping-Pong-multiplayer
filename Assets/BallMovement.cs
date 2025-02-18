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

    // Store the starting position for resetting the ball.
    private Vector2 startPosition;

    void Awake()
    {
        // If not assigned via Inspector, try to get the Rigidbody2D component.
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (rb == null)
            Debug.LogError("BallMovement: Rigidbody2D component not found!");
    }

    public override void OnNetworkSpawn()
    {
        // Save the starting position when the ball spawns on the network.
        startPosition = transform.position;

        // Only the server should launch the ball.
        if (IsServer)
        {
            Launch();
        }
    }

    /// <summary>
    /// Launches the ball in a random diagonal direction.
    /// </summary>
    void Launch()
    {
        // Randomly choose horizontal and vertical directions (-1 or 1).
        float xMovement = Random.Range(0, 2) == 0 ? -1f : 1f;
        float yMovement = Random.Range(0, 2) == 0 ? -1f : 1f;

        // Set the ball's velocity.
        rb.linearVelocity = new Vector2(xMovement * speed, yMovement * speed);
    }

    /// <summary>
    /// Resets the ball to its starting position and relaunches it.
    /// This can be called (from the server) when a point is scored.
    /// </summary>
    public void ResetBall()
    {
        rb.linearVelocity = Vector2.zero;
        transform.position = startPosition;
        Launch();
    }
}
