using System.Collections;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Spawns a Ball prefab after a countdown when exactly two players are in the game.
/// The session owner (server) is responsible for spawning the ball.
/// </summary>
public class Spawner : NetworkBehaviour
{
    public static Spawner Instance;
    [Tooltip("The Ball prefab to spawn. Assign this in the Inspector.")]
    public GameObject PrefabToSpawn;

    [Tooltip("The position where the Ball will be spawned.")]
    public Vector3 ballSpawnPosition = new Vector3(0, 0, 0);

    private bool ballSpawned = false;

    public override void OnNetworkSpawn()
    {
        Debug.Log($"[Spawner] OnNetworkSpawn called. IsServer: {IsServer}, IsClient: {IsClient}, IsOwner: {IsOwner}");

        if (IsSessionOwner) // Ensure only the server starts the spawn process
        {
            StartCoroutine(CheckPlayersAndSpawn());
        }
    }

    private IEnumerator CheckPlayersAndSpawn()
    {
        Debug.Log("[Spawner] Checking for players...");

        while (!ballSpawned)
        {
            yield return new WaitForSeconds(1f);

            int playerCount = GameObject.FindGameObjectsWithTag("Player").Length;
            Debug.Log($"[Spawner] Current Player Count: {playerCount}");

            if (playerCount == 2) // Ensure exactly 2 players exist
            {
                Debug.Log("[Spawner] Two players found. Starting countdown...");
                StartCoroutine(CountdownAndSpawn());
                yield break;
            }
        }
    }

    private IEnumerator CountdownAndSpawn()
    {
        Debug.Log("[Spawner] Ball will spawn in 3 seconds...");
        yield return new WaitForSeconds(1f);
        Debug.Log("[Spawner] Ball will spawn in 2 seconds...");
        yield return new WaitForSeconds(1f);
        Debug.Log("[Spawner] Ball will spawn in 1 second...");
        yield return new WaitForSeconds(1f);

        Debug.Log("[Spawner] Calling SpawnBallServerRpc...");
        SpawnBall();
    }

    /// <summary>
    /// Server spawns the ball and syncs it across all clients.
    public void SpawnBall()
    {
        if (!IsSessionOwner || ballSpawned || PrefabToSpawn == null)
        {
            Debug.LogWarning($"[Spawner] SpawnBallServerRpc() was called but conditions were not met. IsServer: {IsSessionOwner}, ballSpawned: {ballSpawned}, PrefabToSpawn: {PrefabToSpawn != null}");
            return;
        }

        Debug.Log("[Spawner] Spawning the ball...");
        GameObject ballInstance = Instantiate(PrefabToSpawn, ballSpawnPosition, Quaternion.identity);
        NetworkObject networkObject = ballInstance.GetComponent<NetworkObject>();

        if (networkObject != null)
        {
            networkObject.Spawn(true); // Ensures ball is replicated for all clients
            ballSpawned = true;
            Debug.Log("[Spawner] Ball successfully spawned and synced across clients!");
        }
        else
        {
            Debug.LogError("[Spawner] ERROR: Spawned Ball does not have a NetworkObject component!");
        }
    }
}
