using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    private float lastCollisionTime;
    private int totalCollisions;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("enemy"))
        {
            totalCollisions++;
            float timeNow = Time.fixedTime;
            Debug.Log("Hit detected. Time survived: " + (timeNow - lastCollisionTime) + ", Total collisions: "+totalCollisions);
            lastCollisionTime = Time.fixedTime;

            GameHandler.Instance.RemoveLife();
        }
    }
}
