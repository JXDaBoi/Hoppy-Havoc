using UnityEngine;

public class TerrainDetailDistanceSetter : MonoBehaviour
{
    [Tooltip("Set the detail distance for trees and detail objects")]
    public float detailDistance = 100f; // Default value
    public Terrain[] terrains;

    void Start()
    {
        SetDetailDistance(detailDistance);
        terrains = FindObjectsOfType<Terrain>();
    }

    void Update()
    {
         SetDetailDistance(detailDistance);
    }

    private void SetDetailDistance(float distance)
    {

        foreach (Terrain terrain in terrains)
        {
            terrain.detailObjectDistance = distance; // Set distance for detail objects
            terrain.treeDistance = distance; // Set distance for trees
        }
    }
}