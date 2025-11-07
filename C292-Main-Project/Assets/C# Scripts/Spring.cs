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

    [SerializeField] private float cooldown;
    [SerializeField] private Sprite up;
    [SerializeField] private Sprite down;

    private bool active = true;
    private float tempY;

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
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (active)
            {
                gameManager.SpringJump();
                active = false;
                spriteRenderer.sprite = down;
                tempY = transform.position.y;
                transform.position = new Vector3(transform.position.x, -1.675f, transform.position.z);
                StartCoroutine("Cooldown");
            }
        }
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        active = true;
        spriteRenderer.sprite = up;
        transform.position = new Vector3(transform.position.x, tempY, transform.position.z);
    }
}
