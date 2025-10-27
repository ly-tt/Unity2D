using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollow2D : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 0.15f;
    public Vector3 offset = new Vector3(0, 1, -10);
    public float minX, maxX;
    public bool followY = false;
    public float ySmoothSpeed = 0.2f;
    public Tilemap map; 
    void Start()
    {
        if (map != null)
        {
            Bounds bounds = map.localBounds;
            float halfWidth = Camera.main.orthographicSize * Camera.main.aspect;
            minX = bounds.min.x + halfWidth;
            maxX = bounds.max.x - halfWidth;
        }
    }

    void LateUpdate()
    {
        if (player == null) return;
        Vector3 target = player.position + offset;
        target.x = Mathf.Clamp(target.x, minX, maxX);
        if (!followY) target.y = transform.position.y;
        else target.y = Mathf.Lerp(transform.position.y, target.y, ySmoothSpeed);
        Vector3 smooth = Vector3.Lerp(transform.position, target, smoothSpeed);
        transform.position = new Vector3(smooth.x, smooth.y, transform.position.z);
    }
}

