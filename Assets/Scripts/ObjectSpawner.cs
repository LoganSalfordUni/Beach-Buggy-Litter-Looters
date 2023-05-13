using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] Transform playerBody;
    [SerializeField] float playerBodyXOffset = 400f;//how much further than the players body should objects be spawned (where is off the camera)

    //position this game object beyond where the camera can see
    [Header("Prefabs")]
    [SerializeField] GameObject[] commonLitter;
    [SerializeField] GameObject rareLitter;
    [Space(1)]
    [SerializeField] GameObject peopleObstacles;
    [SerializeField] GameObject catObstacle;

    [Header("Litter")]
    [SerializeField] Vector2 litterSpawnBounds;
    [SerializeField, Tooltip("How much litter spawns per 100 game units")] private float litterSpawnRate = 300f;
    [SerializeField, Tooltip("The potential offset in the litter spawn rate.")] private Vector2 litterSpawnOffset;
    [SerializeField, Range(1,100)] private int rareLitterPercentage;
    private float litterSpawnDistanceLeft;

    //As a note. Since the players movement rate affects where objects spawn. the faster you move, the further apart they spawn. This is almost certainly undesired

    private void Start()
    {
        float startingLitterSpawnX = 10f;
        while (startingLitterSpawnX < playerBodyXOffset)
        {
            SpawnLitter(startingLitterSpawnX);
            startingLitterSpawnX += Mathf.Max(0.1f, (100 / (litterSpawnRate + Random.Range(litterSpawnOffset.x, litterSpawnOffset.y))));
        }



        litterSpawnDistanceLeft = (playerBody.position.x + playerBodyXOffset) + (100 / (litterSpawnRate + Random.Range(litterSpawnOffset.x, litterSpawnOffset.y)));
    }

    private void Update()
    {
        float spawnX = playerBody.position.x + playerBodyXOffset;

        //Litter
        if (litterSpawnDistanceLeft <= spawnX)
        {
            //litterSpawnDistanceLeft = spawnX + (100 / (litterSpawnRate + Random.Range(litterSpawnOffset.x, litterSpawnOffset.y)));
            litterSpawnDistanceLeft += (100 / (litterSpawnRate + Random.Range(litterSpawnOffset.x, litterSpawnOffset.y)));
            SpawnLitter(spawnX);
        }

    }

    void SpawnLitter(float spawnXOffset)
    {
        float spawnLocationZ = Random.Range(litterSpawnBounds.x, litterSpawnBounds.y);
        Vector3 spawnLocation = new Vector3(spawnXOffset, 0f, spawnLocationZ);

        bool spawnRare = (Random.Range(0.1f, 100f) < rareLitterPercentage);
        
        if (spawnRare)
        {
            Instantiate(rareLitter, spawnLocation, Quaternion.identity);
        }
        else
        {
            Instantiate(commonLitter[Random.Range(0,commonLitter.Length)], spawnLocation, Quaternion.identity);
        }

    }
}
