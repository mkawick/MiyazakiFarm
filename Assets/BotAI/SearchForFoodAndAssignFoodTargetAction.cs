using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Random = UnityEngine.Random;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SearchForFoodAndAssignFoodTarget",
    story: "Run [FindFood] and assign [AvailableFood] and [FoodTarget] and [FoodFound]", 
    category: "Action", 
    id: "6fcbe4379c0b8720444de5e2f3e09dad")
]
public partial class SearchForFoodAndAssignFoodTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<FindFood> FindFood;
    [SerializeReference] public BlackboardVariable<List<GameObject>> AvailableFood;
    [SerializeReference] public BlackboardVariable<GameObject> FoodTarget;
    [SerializeReference] public BlackboardVariable<bool> FoodFound;
    
    
//    private float nextSpawnTime;
    
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (FoodFound.Value == true)// we already have food
        {
            return Status.Failure;
        }
        FindFood.Value.UpdateDetector();
        AvailableFood.Value = FindFood.Value.DetectedFood;
        if (AvailableFood.Value.Count == 0)
        {
       //     FoodFound.Value = false;
            return Status.Failure;
        }
        
        int foodSelection = Random.Range(0, AvailableFood.Value.Count - 1);
        FoodTarget.Value = AvailableFood.Value[foodSelection];

        FoodFound.Value = true;
        
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

