using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]

public class Controller2D : MonoBehaviour // This class comes from a tutorial by Sebastian Lague: https://youtube.com/playlist?list=PLFt_AvWsXl0f0hqURlhyIoAabKPgRsqjz&si=9xCdazuqRZ3RPJ5o, specifically episodes 1, 2, and 3.
{

    public LayerMask collisionMask;

    const float skinWidth = .015f;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    float horizontalRaySpacing;
    float verticalRaySpacing;

    new public BoxCollider2D collider;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

    // Debugging variables, by me
    [SerializeField] private bool upCollision;
    [SerializeField] private bool downCollision;
    [SerializeField] private bool leftCollision;
    [SerializeField] private bool rightCollision;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
        collisions.faceDir = 1;
    }

    void FixedUpdate() // for debugging, by me
    {
        upCollision = collisions.above;
        downCollision = collisions.below;
        leftCollision = collisions.left;
        rightCollision = collisions.right;
    }

    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();
        collisions.Reset();

        if (velocity.x != 0)
        {
            collisions.faceDir = (int)Mathf.Sign(velocity.x);
        }

        HorizontalCollisions(ref velocity);

        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }

        transform.Translate(velocity);
    }

    void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = collisions.faceDir;
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        if (Mathf.Abs(velocity.x) < skinWidth)
        {
            rayLength = 2 * skinWidth;
        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);


            if (hit)
            {
                velocity.x = (hit.distance - skinWidth) * directionX;
                rayLength = hit.distance;

                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
            }
        }
    }

    void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }
    }

    void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public int faceDir;

        public void Reset()
        {
            above = below = false;
            left = right = false;
        }
    }
}
