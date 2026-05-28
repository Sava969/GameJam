using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    public GameObject ItemPrefab;
    public float Radius = 1f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) SpawnObjectAtRandom();
    }

    void SpawnObjectAtRandom()
    {
        // get a random point inside a circle (Vector2)
        Vector2 randomOffset = Random.insideUnitCircle * Radius;

        // convert to Vector3 and offset from this object's position
        Vector2 spawnPos = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

        Instantiate(ItemPrefab, spawnPos, Quaternion.identity);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, Radius);
    }
}
