using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathSpawner : MonoBehaviour
{
    public List<Vector3> pathPositions;
    public List<Vector3> pathRotations;
    Rigidbody rb;
    private float recordFrequency = 0.5f;
    private float recordTimer;
    [SerializeField] private Vector3 destPosition;
    [SerializeField] private Vector3 destRotation;
    private int maxPathPoints = 15;
    bool isMoving = false;
    bool isRecalling = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        recordTimer = recordFrequency;
    }

    private void Update()
    {
        if (isRecalling)
        {
            // disable the sphere's self move physic
            rb.isKinematic = true;

            float distance = Vector3.Distance(transform.position, destPosition);

            if (distance <= 0.1f)
            {
                StartRecallingPath();
            }
            else
            {
                float speed = distance / recordFrequency;
                transform.position = Vector3.MoveTowards(transform.position, destPosition, speed * Time.deltaTime);

            }
        }

        else
        {
            rb.isKinematic = false;
            // if object is moving, start spawning path prefabs
            isMoving = rb.velocity.magnitude == 0 ? false : true;

            if (isMoving)
            {
                recordTimer -= Time.deltaTime;

                if (recordTimer <= 0)
                {
                    RecordPathPoint(transform.position, transform.eulerAngles);
                    recordTimer = recordFrequency;
                }
            }

        }
    }

    private void RecordPathPoint(Vector3 position, Vector3 rotation)
    {
        // if there are too many pathpoints, remove the oldest one
        if (pathPositions.Count >= maxPathPoints && pathRotations.Count >= maxPathPoints)
        {
            pathPositions.RemoveAt(0);
            pathRotations.RemoveAt(0);
        }
        pathPositions.Add(position);
        pathRotations.Add(rotation);
    }

    public void StartRecallingPath()
    {
        if (pathPositions.Count <= 0 || pathRotations.Count <= 0)
        {
            isRecalling = false;
            return;
        }

        destPosition = pathPositions[pathPositions.Count - 1];
        destRotation = pathRotations[pathRotations.Count - 1];

        pathPositions.RemoveAt(pathPositions.Count - 1);
        pathRotations.RemoveAt(pathRotations.Count - 1);
        isRecalling = true;
    }
}
