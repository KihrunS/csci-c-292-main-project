using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBlock : MonoBehaviour
{

    GameManager gameManager;

    [SerializeField] GameObject starPrefab;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Colliding!");
            if (gameManager.BlockBreak())
            {
                Debug.Log("Breaking!");
                Destroy(this.gameObject);
                GameObject.Instantiate(starPrefab, transform.position, Quaternion.identity);
            }
        }
    }
}
