using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinPongBlock : MonoBehaviour
{
    [SerializeField] private Transform[] points;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private float lastWaitTime = 0f;
    private int i = 0;
    private bool canMoving = true;
    void Start()
    {

    }
    // Start is called before the first frame update
    void Update()
    {
        if (canMoving)
        {
            // 위치 이동
            if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
            {
                i++;
                canMoving = false;
                lastWaitTime = Time.time;
                if (i >= points.Length)
                {
                    i = 0;
                }
            }
            transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
        }
        else
        {
            if (Time.time - lastWaitTime > waitTime)
            {
                lastWaitTime = Time.time;
                canMoving = true;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        other.transform.SetParent(transform);
    }
    private void OnTriggerExit(Collider other)
    {
        other.transform.SetParent(null);
    }
}
