using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public struct GrowStatus
{
    public float nextGrowTime;
    public int growLevel;
    public bool hasSprouted;
    public bool onlyScalesVertically;
    public Vector3 originalScale;
    public float maxLevel;
    public float growthInterval;
    
    public GameObject instance;
    public FarmPlant plant;
    public int spawnLocationIndex;
}

struct SpawnPlotSetup
{
    public GameObject spawnPlot;
    public int index;
    public bool isInUse;
}

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] [Range(1, 60)] private float periodBetweenSpawns = 1f;
    [SerializeField] [Range(1, 10)] private float periodVariance = 1f;

    [FormerlySerializedAs("spawnableLocations")] [SerializeField] public GameObject[] spawnPlots;
    [FormerlySerializedAs("spawnableObjectPrefabs")] [SerializeField] public GameObject spawnableObjectPrefab;
    
    
    [SerializeField] [Range(1, 60)] private float periodBetweenGrowth = 1f;
    [SerializeField] [Range(1, 60)] private float maxFood = 1f;

    [SerializeField] private bool onlyScalesVertically = false;
    
    private Rect spawnRegion;
    private float nextSpawnTime;
    private List<GrowStatus> livingPlants;
    private List<SpawnPlotSetup> spawnPlotUse;
    
    void Start()
    {
        if (spawnableObjectPrefab.activeSelf == true)
        { 
            spawnableObjectPrefab.SetActive(false);
        }

        if (spawnableObjectPrefab.TryGetComponent<FarmPlant>(out var farmPlant) == false)
        {
            Debug.LogError("bad prefab for farm");
        }

        spawnPlotUse = new List<SpawnPlotSetup>();
        for (int i=0; i<spawnPlots.Length; i++)
        {
            var plot =  spawnPlots[i];
            spawnPlotUse.Add(new SpawnPlotSetup
                {
                    spawnPlot = plot, 
                    index = i, 
                    isInUse = false
                });  
        }
        nextSpawnTime = Time.time + periodBetweenSpawns;
        livingPlants = new List<GrowStatus>();
    }

    void Update()
    {
        if (nextSpawnTime < Time.time)
        {
            Spawn();
            nextSpawnTime = Time.time + periodBetweenSpawns + (Random.value * periodVariance - periodVariance/2);
        }
        Grow();
    }

    private void Spawn()
    {
        if (spawnPlots.Length <= livingPlants.Count)
            return;

        int index = findOpenPlot();//livingPlants.Count;
        if (index == -1)
            return;
        
        var temp = spawnPlotUse[index];
        temp.isInUse = true;
        spawnPlotUse[index] = temp;
        
   /*     if (index >= 1) // testing only
            return;*/
        var plant = new GrowStatus(); 
            
        plant.growLevel = 0;
        plant.hasSprouted = true;
        plant.nextGrowTime = Time.time + periodBetweenGrowth;
        plant.growthInterval = periodBetweenGrowth;
        plant.instance = Instantiate(spawnableObjectPrefab);
        plant.instance.SetActive(true);
        plant.instance.transform.parent = transform;
        plant.instance.transform.localPosition = spawnPlots[index].gameObject.transform.localPosition;
        plant.spawnLocationIndex = index;
        plant.plant = plant.instance.GetComponent<FarmPlant>();
        plant.maxLevel = maxFood;
        plant.originalScale = plant.instance.transform.localScale;
        plant.onlyScalesVertically = onlyScalesVertically;
        plant.plant.OnPlantDied += PlantDied;
        
        plant.plant.growStatus = plant;// be sure to do this last
        
        plant.plant.ScalePlant(0);
        livingPlants.Add(plant);
    }
    
    int findOpenPlot()
    {
        if (livingPlants.Count >= spawnPlots.Length)
            return -1;

        int numUsed = spawnPlotUse.Count(item => item.isInUse);
        if (numUsed == spawnPlotUse.Count)
            return -1;

        var availableSpots = CollectListOfUnUsedLocations();
        int selectionId = Random.Range(0, availableSpots.Count);
        Debug.Log("random plot choosen: " + selectionId + " from: " + availableSpots.Count);

        var spot = availableSpots[selectionId];
        
        var actualPlot = spawnPlotUse[spot.index];
        actualPlot.isInUse = true;
        spawnPlotUse[spot.index] = actualPlot;
        
        Debug.Log("random plot id: " + spot.index + " spawnPlot: " + actualPlot.spawnPlot.name);
        return spot.index;
    }
    
    
    private List<SpawnPlotSetup> CollectListOfUnUsedLocations()
    {
        List<SpawnPlotSetup>  locations =  new List<SpawnPlotSetup>();
        for (int i = 0; i < spawnPlotUse.Count; i++)
        {
            if (spawnPlotUse[i].isInUse == false)
            {
                locations.Add(spawnPlotUse[i]);
            }
        }

        return locations;
    }

    void PlantDied(GameObject growth)
    {
        var farmPlant = growth.GetComponent<FarmPlant>();
        foreach(var plant in livingPlants)
        {
            if (plant.instance == farmPlant.growStatus.instance)
            {
                int index = farmPlant.growStatus.spawnLocationIndex;
                Debug.Log("PlantDied: " + index + " spawnPlot: " + farmPlant.name);
                
                livingPlants.Remove(plant);
                
                var temp = spawnPlotUse[index];
                temp.isInUse = false;
                spawnPlotUse[index] = temp;
                
                break;
            }
        }
        
        Destroy(growth);
    }
    
    private void Grow()
    {
        for(int i=0; i<livingPlants.Count; i++)
        {
            var growth = livingPlants[i];
            growth.plant.Grow();
        }
    }

}

/*
public struct EntityDimensions : IComponentData
{
    public float3 Size;
}

var transform = SystemAPI.GetComponent<LocalTransform>(entity);
float scale = transform.Scale;
            
public class DimensionsBaker : Baker<MeshRenderer>
{
    public override void Bake(MeshRenderer authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new EntityDimensions {
            Size = authoring.bounds.size
        });
    }
}*/