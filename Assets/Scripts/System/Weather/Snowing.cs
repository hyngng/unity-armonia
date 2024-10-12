using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowing : Weather
{
    protected ParticleSystem weather;

    void Start()
    {
        weather = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        
    }
}
