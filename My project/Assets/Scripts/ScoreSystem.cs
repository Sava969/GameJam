using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public int value;

    private void Start()
    {
        value = 10;
    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            ScoreCounter.Instance.IncreaseScore(value);
        }
    }
}
