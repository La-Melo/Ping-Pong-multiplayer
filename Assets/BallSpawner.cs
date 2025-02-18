using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class BallSpawner : NetworkBehaviour
{
    [Tooltip("The ball prefab (must have a NetworkObject component).")]
    public GameObject ballPrefab;

    [Tooltip("Spawn position for the ball.")]
    public Vector2 spawnPosition;

    private bool ballSpawned = false;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            if (NetworkManager.Singleton.ConnectedClients.Count == 2)
            {
                StartCoroutine(SpawnAfterDelay());
            }
            else
            {
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            }
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!ballSpawned && NetworkManager.Singleton.ConnectedClients.Count == 2)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            StartCoroutine(SpawnAfterDelay());
        }
    }

    private IEnumerator SpawnAfterDelay()
    {
        Debug.Log("Two players connected. Starting 3-second countdown...");
        yield return new WaitForSeconds(3f);
        SpawnBall();
    }

    void SpawnBall()
    {
        if (ballPrefab == null)
        {
            Debug.LogError("BallSpawner: Ball prefab is not assigned!");
            return;
        }

        GameObject ballInstance = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);
        NetworkObject netObj = ballInstance.GetComponent<NetworkObject>();

        if (netObj != null)
        {
            ulong ownerClientId = GetBallOwnerClientId();
            if (ownerClientId != 0)
            {
                netObj.SpawnWithOwnership(ownerClientId);
            }
            else
            {
                netObj.Spawn();
            }
            ballSpawned = true;
            Debug.Log("Ball spawned after 3-second delay");
        }
        else
        {
            Debug.LogError("BallSpawner: The ball prefab does not have a NetworkObject component.");
        }
    }

    ulong GetBallOwnerClientId()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.ClientId != NetworkManager.Singleton.LocalClientId)
            {
                return client.ClientId;
            }
        }
        return NetworkManager.Singleton.LocalClientId;
    }
}