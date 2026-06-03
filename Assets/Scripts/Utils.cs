using UnityEngine;

namespace Utils
{
    public static class Tools
    {
        public static bool AdvanceExpry(ref float nextTime, float interval)
        {
            if (nextTime < Time.time)
            {
                nextTime = Time.time + interval;
                return true;
            }

            return false;
        }
    }

}
