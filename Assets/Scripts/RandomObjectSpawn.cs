using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObjectSpawn : MonoBehaviour
{
    public Transform RedGem;
    public Transform GreenGem;
    public Transform BlueGem;

    public Transform[] redSpawns;
    public Transform[] greenSpawns;
    public Transform[] blueSpawns;
    
    // Start is called before the first frame update
    void Start()
    {
        // Red
        int redIndex = Random.Range(0, redSpawns.Length);
        RedGem.position = redSpawns[redIndex].position;


        // Green
        int greenIndex = Random.Range(0, greenSpawns.Length);
        GreenGem.position = greenSpawns[greenIndex].position;
    
        // Blue
        int indexNumber = Random.Range(0, blueSpawns.Length);
        BlueGem.position = blueSpawns[indexNumber].position;


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
