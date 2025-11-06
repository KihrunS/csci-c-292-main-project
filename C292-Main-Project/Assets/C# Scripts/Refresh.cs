using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]

public class Refresh : MonoBehaviour
{
    GameManager gameManager;
    new BoxCollider2D collider;
    SpriteRenderer spriteRenderer;

    [SerializeField] private float respawnTime;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        collider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (gameManager.CanRefresh())
            {
                gameManager.DashRefresh();
                collider.enabled = false;
                spriteRenderer.enabled = false;
                StartCoroutine("Respawn");
            }
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        collider.enabled = true;
        spriteRenderer.enabled = true;
    }
}
