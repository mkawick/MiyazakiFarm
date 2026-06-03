using System;
using Unity.Behavior;

[BlackboardEnum]
public enum ChickenState
{
	Idle,
	Eating,
	SearchingForFood,
	RunningAway,
	StayingUntilTriggerLeaves,
	WalkingToFood
}
