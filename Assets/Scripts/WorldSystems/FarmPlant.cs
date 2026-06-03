using System.Collections;
using UnityEngine;

public class FarmPlant : MonoBehaviour
{
    [SerializeField] private float foodMax;
    private float foodRemaining;

    internal GrowStatus growStatus;
    bool isStillAlive = true;
    internal System.Action<GameObject> OnPlantDied;

    public float EatPlant()
    {
        if (foodRemaining <= 0)
            return 0;
        
        foodRemaining--;
        growStatus.growLevel--;
        if (foodRemaining == 0)
        {
            isStillAlive = false;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            StartCoroutine(WaitToDieCompletely());
        }

        // reset start grow time
        growStatus.nextGrowTime = Time.time + growStatus.growthInterval;
        
        ScalePlant(growStatus.growLevel);
        
        return 1;
    }

    IEnumerator WaitToDieCompletely()
    {
        yield return new WaitForSeconds(1f);
        OnPlantDied(gameObject);
    }

    public bool IsEdible()
    {
        return foodRemaining > 0;
    }

    public void Grow()
    {
        if (isStillAlive == false)
            return;
        
        if (growStatus.hasSprouted == false)
        {
            if (Utils.Tools.AdvanceExpry(ref growStatus.nextGrowTime, growStatus.growthInterval)) 
            {
                growStatus.hasSprouted = true;
            }
            return;
        }

        if (Utils.Tools.AdvanceExpry(ref growStatus.nextGrowTime, growStatus.growthInterval))
        {
            ++growStatus.growLevel;
            if (growStatus.growLevel >= growStatus.maxLevel)
            {
                growStatus.growLevel = (int)growStatus.maxLevel;
                growStatus.nextGrowTime = float.MaxValue; // no more growing until eating.   
            }

            ++foodRemaining;
            if (foodRemaining > foodMax)
            {
                foodRemaining = foodMax;
            }

            ScalePlant(growStatus.growLevel);
        }
    }

    internal void ScalePlant(float plantScale)
    {
        if (growStatus.onlyScalesVertically)
        {
            var origScalingYOnly = growStatus.originalScale;
            var newHeight = (plantScale / growStatus.maxLevel);
            origScalingYOnly.y = origScalingYOnly.y * newHeight;
            transform.localScale = origScalingYOnly;
        }
        else
        {
            transform.localScale = growStatus.originalScale * (plantScale / growStatus.maxLevel);
        }
    }
}
