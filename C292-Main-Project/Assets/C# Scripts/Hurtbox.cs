using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    GameManager gameManager;

    [SerializeField] string direction; // Needs to be set manually when spikes are rotated

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && gameManager.SpikeDirection(direction)) // Only kills player if they're moving towards the spikes (or still)
        {
            gameManager.KillPlayer();
        }
    }
}
