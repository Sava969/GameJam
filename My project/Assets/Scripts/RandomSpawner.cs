using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // new Input System

public class RandomSpawner : MonoBehaviour
{
    public GameObject ItemPrefab;
    public float Radius = 1f;

    private void Update()
    {
        // new Input System: check space pressed this frame
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SpawnObjectAtRandom();
        }
    }

    void SpawnObjectAtRandom()
    {
        if (ItemPrefab == null)
        {
            Debug.LogWarning("RandomSpawner: ItemPrefab is not assigned.");
            return;
        }

        Vector2 randomOffset = Random.insideUnitCircle * Radius;
        Vector3 spawnPos = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

        Instantiate(ItemPrefab, spawnPos, Quaternion.identity);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, Radius);
    }
}
