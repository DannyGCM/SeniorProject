using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerTEST : MonoBehaviour
{
    public GameObject target;
    public bool stop = false;

    public float spawnTime;
    public float spawnDelay;

    public int count = 1;
    public int spawnLimit = 2;
    public float rangeX = 1;
    public float rangeY = 1;
    public float rangeZ = 1;

    private Vector3 pos;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("spawnObject", spawnTime, spawnDelay);
    }

    public void spawnObject()
    {

        if (this.count >= spawnLimit) { this.stop = true; }
        this.count += 1;

        this.pos = transform.position;
        this.pos[0] += Random.Range(-rangeX, rangeX);
        this.pos[1] += Random.Range(-rangeY, rangeY);
        this.pos[2] += Random.Range(-rangeZ, rangeZ);


        Instantiate(target, this.pos, transform.rotation);
        if (this.stop)
        {
            CancelInvoke();
        }
    }
}
