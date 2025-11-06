using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]

public class Spring : MonoBehaviour
{
    GameManager gameManager;
    new BoxCollider2D collider;
    SpriteRenderer spriteRenderer;

    [SerializeField] private float springTime;
    [SerializeField] private float hangTime;
    [SerializeField] private float cooldown;
    [SerializeField] private Sprite up;
    [SerializeField] private Sprite down;

    private bool active = true;

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
            if (active)
            {
                gameManager.SpringJump(springTime, hangTime);
                active = false;
                spriteRenderer.sprite = down;
                StartCoroutine("Cooldown");
            }
        }
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        active = true;
        spriteRenderer.sprite = up;
    }
}
