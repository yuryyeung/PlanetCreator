using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshMover : MonoBehaviour
{
    public Planet Planet;

    void Start()
    {
        Planet = this.GetComponent<Planet>();
    }
    
    void Update()
    {
        Vector3 centre = Planet.shapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.centre;
        centre += new Vector3(Time.deltaTime, 0, 0);
        Planet.shapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.centre = centre;
        Planet.GeneratePlanet();
    }

    private void OnApplicationQuit()
    {
        if (Planet == null) return;
        Planet.shapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.centre = Vector3.zero;
    }
}
