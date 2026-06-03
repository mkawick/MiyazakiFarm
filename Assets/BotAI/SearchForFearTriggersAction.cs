using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SearchForFearTriggers", story: "Update [WatchOutForFearTrigger] and assign [FearTrigger] and [SafeSpot_NonFear]", category: "Action", id: "9b7363ae25cac4a082a81cde5bdc0cc5")]
public partial class SearchForFearTriggersAction : Action
{
    [SerializeReference] public BlackboardVariable<WatchOutForFearTriggers> WatchOutForFearTrigger;
    [SerializeReference] public BlackboardVariable<GameObject> FearTrigger;
    [SerializeReference] public BlackboardVariable<bool> IsAfraid;
    [SerializeReference] public BlackboardVariable<Vector3> SafeSpot_NonFear;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        FearTrigger.Value = WatchOutForFearTrigger.Value.UpdateDetector();
        if (FearTrigger.Value is null)
        {
            IsAfraid.Value = false;
            return Status.Failure;
        }

        IsAfraid.Value = true;
        SafeSpot_NonFear.Value = WatchOutForFearTrigger.Value.SelectSafePlace();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

