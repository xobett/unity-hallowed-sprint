using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private int zOffset = 10;
    [SerializeField]
    [Range(1f, 4f)] private float speed = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target is not assigned!");
            Debug.Break();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Follow();
    }

    void Follow()
    {
        if (target == null) return;
        var position = new Vector3(transform.position.x, transform.position.y, zOffset);
        transform.position = Vector3.Lerp(position, target.position, speed * Time.deltaTime);
    }
}
