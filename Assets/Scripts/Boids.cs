using System.Collections.Generic;
using UnityEngine;

public class Boids : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private int amount = 10;
    [SerializeField] private GameObject prefab;

    [Header("Boid Settings")]
    [SerializeField] private float avoidDistance = 1f;
    [SerializeField] private float centerGravityDivider = 100f;
    [SerializeField] private float avarageBoidSpeedDivider = 2f;
    [SerializeField] private float maxBoidSpeed = 3f;
    [SerializeField] private float randomnessFactor = 1f;
    [SerializeField] private float neighborhoodRadius = 3f;

    [Header("Bounds")]
    [SerializeField] float maxX;
    [SerializeField] float minX;
    [SerializeField] float maxY;
    [SerializeField] float minY;

    private List<Boid> boids = new();

    private void Start()
    {
        // Spawn all boids
        for (int i = 0; i < amount; i++) 
        {
            Vector3 randomVector = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
            boids.Add(Instantiate(prefab, randomVector, Quaternion.identity).GetComponent<Boid>());
        }
    }

    private void FixedUpdate()
    {
        MoveAllBoids();
    }

    private void MoveAllBoids()
    {
        Vector3 v1 = new();
        Vector3 v2 = new();
        Vector3 v3 = new();

        foreach (Boid boid in boids)
        {
            boid.velocity = Vector3.zero;
            v1 = (GetAvarageBoidPosistion(boid) - boid.transform.position) / centerGravityDivider;
            v2 = MoveBoidAwayFromOthers(boid);
            v3 = GetAvarageBoidVelocity(boid) / avarageBoidSpeedDivider;

            // Add a small random vector to the velocity
            Vector3 newRandomVector = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0);
            boid.currentRandomVector = Vector3.Lerp(boid.currentRandomVector, newRandomVector, 0.1f);

            boid.velocity = boid.velocity + v1 + v2 + v3 + boid.currentRandomVector * randomnessFactor + BoundPosistion(boid);

            // Limit boid speed
            if (boid.velocity.magnitude > maxBoidSpeed)
            {
                boid.velocity = boid.velocity.normalized * maxBoidSpeed;
            }

            boid.transform.position += boid.velocity;
        }
    }

    private Vector3 GetAvarageBoidPosistion(Boid currentBoid)
    {
        Vector3 sum = new Vector3();
        int count = 0;

        foreach (Boid boid in boids)
        {
            float distance = Vector3.Distance(currentBoid.transform.position, boid.transform.position);
            if (boid != currentBoid && distance <= neighborhoodRadius)
            {
                sum += boid.transform.position;
                count++;
            }
        }

        if (count == 0)
        {
            return currentBoid.transform.position;
        }
        else
        {
            return sum / count;
        }
    }

    private Vector3 MoveBoidAwayFromOthers(Boid boid)
    {
        Vector3 sum = new();

        foreach(Boid otherBoid in boids)
        {
            if(boid != otherBoid)
            {
                if (Vector3.Distance(boid.transform.position, otherBoid.transform.position) < avoidDistance)
                {
                    sum = sum - (otherBoid.transform.position - boid.transform.position);
                }
            }
        }

        return sum;
    }

    private Vector3 GetAvarageBoidVelocity(Boid boid)
    {
        Vector3 sum = new();

        foreach (Boid otherBoid in boids)
        {
            if (boid != otherBoid)
            {
                sum = sum + otherBoid.velocity;
            }
        }

        sum = sum / (boids.Count - 1);

        return sum;
    }

    // Give boids tendency to move away from borders
    private Vector3 BoundPosistion(Boid boid)
    {
        Vector3 newVector = new();

        if (boid.transform.position.x < minX)
        {
            newVector.x = 1;
        }
        else if (boid.transform.position.x > maxX)
        {
            newVector.x = -1;
        }

        if(boid.transform.position.y < minY)
        {
            newVector.y = 1;
        }
        else if (boid.transform.position.y > maxY)
        {
            newVector.y = -1;
        }

        return newVector;
    }
}