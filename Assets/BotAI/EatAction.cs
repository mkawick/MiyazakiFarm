using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Eat", story: "[Self] takes health from [FoodSource] and clear [FoodFound] when done", category: "Events", id: "1fea6717472e325a1aaf25f767ccb51b")]
public partial class EatAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> FoodSource;
    [SerializeReference] public BlackboardVariable<bool> FoodFound;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (FoodSource.Value == null) // do not use "is not null"... 
        {
            ClearFoodFound();
            return Status.Failure;
        }
        
        var farmPlant = FoodSource.Value.GetComponent<FarmPlant>();
        if (farmPlant is null)
        {
            ClearFoodFound();
            return Status.Failure;
        }

        if (farmPlant.EatPlant() == 0)
        {
            FoodSource.Value = null;// no longer a source of nutrients
            ClearFoodFound();
            return Status.Failure;
        }
        
        return Status.Success;
    }

    void ClearFoodFound()
    {
        FoodFound.Value = false;
    }

    protected override void OnEnd()
    {
    }
}

