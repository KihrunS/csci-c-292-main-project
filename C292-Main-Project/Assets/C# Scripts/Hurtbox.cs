using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    GameManager gameManager;

    [SerializeField] string direction;

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
        if (collision.gameObject.CompareTag("Player") && gameManager.SpikeDirection(direction))
        {
            gameManager.KillPlayer();
        }
    }
}
