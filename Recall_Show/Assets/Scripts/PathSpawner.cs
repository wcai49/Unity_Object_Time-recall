using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathSpawner : MonoBehaviour
{
    public List<Vector3> pathPositions;
    public List<Vector3> pathRotations;
    Rigidbody rb;
    private float recordFrequency = 0.05f;
    private float recordTimer;
    private float recallMaxTime = 10.0f;
    private float recallTimer;
    [SerializeField] private Vector3 destPosition;
    [SerializeField] private Vector3 destRotation;
    private int maxPathPoints = 120;
    bool isMoving = false;
    bool isRecalling = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        resetRecordTimer();
        resetRecallTimer();
    }

    private void Update()
    {
        // If the object is in recalling, then other regualar movements are disabled
        if (isRecalling)
        {
            // disable the sphere's self move physic
            rb.isKinematic = true;

            float distance = Vector3.Distance(transform.position, destPosition);
            // If arrived at the last path point, update the destination
            if (distance <= 0.1f)
            {
                StartRecallingPath();
            }
            else
            {
                // recall position
                float speed = distance / recordFrequency;
                transform.position = Vector3.MoveTowards(transform.position, destPosition, speed * Time.deltaTime);

                // recall rotation
                float angle = Quaternion.Angle(transform.rotation, Quaternion.Euler(destRotation));
                float angleSpeed = angle / recordFrequency;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(destRotation), angleSpeed * Time.deltaTime);
            }
        }

        // If the object is not in recalling, record the path points if it is moving
        else
        {
            rb.isKinematic = false;
            isMoving = rb.velocity.magnitude == 0 ? false : true;

            if (isMoving)
            {
                // For every recordFrequency time, record a path point with position and rotation
                recordTimer -= Time.deltaTime;

                if (recordTimer <= 0)
                {
                    RecordPathPoint(transform.position, transform.eulerAngles);
                    resetRecordTimer();
                }
            }

            // if the object stays for a long time, start remove the path points from the begining
            // every recordFrequency time
            else
            {
                recallTimer -= Time.deltaTime;
                if (recallTimer <= 0)
                {
                    if (pathPositions.Count > 0 && pathRotations.Count > 0)
                    {
                        recordTimer -= Time.deltaTime;
                        if (recordTimer <= 0)
                        {
                            pathPositions.RemoveAt(0);
                            pathRotations.RemoveAt(0);
                            resetRecordTimer();
                        }
                    }
                    else
                    {
                        resetRecallTimer();
                    }
                }

            }
        }
    }

    /**
     *
     * Record the path point with position and rotation
     * @param {position} the position of the path point
     * @param {rotation} the rotation of the path point
     */
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

    /**
     *
     * Try to recall on a object. If two pathPointsArray are empty, 
        meaning the object has arrived the start point;
        If not, set the last point as the next destination, 
        and remove it from the array after using it;
     */
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

    /**
     *
     * Reset the record timer
     */
    private void resetRecordTimer()
    {
        recordTimer = recordFrequency;
    }

    /**
     *
     * Reset the recall timer
     */
    private void resetRecallTimer()
    {
        recallTimer = recallMaxTime;
    }
}
