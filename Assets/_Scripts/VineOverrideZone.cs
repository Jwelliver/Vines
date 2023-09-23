using UnityEngine;

// public enum VineZoneMode
// {
//     ALL, // Override all vines in zone
//     SINGLE // Override only the first vine in the zone
// }

public enum VineZoneOverrideAction
{
    OVERRIDE_ENTIRE_CONFIG,
    ENSURE_VINE_REACHABLE
}


public class VineOverrideZone : MonoBehaviour
{
    // public Collider2D zoneTrigger;
    public float zoneWidth = 10f;
    public float zoneHeight = 1000f;
    public float maxVinesToOverride = 5f;
    public VineFactoryConfig vineFactoryConfigOverride;
    public VineZoneOverrideAction overrideAction = VineZoneOverrideAction.OVERRIDE_ENTIRE_CONFIG;
    private int nVinesOverriden = 0;

    //  ? later, You can optionally use currentConfig passed from the VineFactory to manipulate the config and return only the specified overriden properties instead of replacing the entire currentConfig;
    public VineFactoryConfig QueryBounds(Vector2 queryPosition, VineFactoryConfig currentConfig)
    {   // This is called from the vineFactory before creating a vine at the queryPosition. 
        if (nVinesOverriden >= maxVinesToOverride) { return null; }
        Vector2 distance = queryPosition - (Vector2)transform.position;
        if (Mathf.Abs(distance.x) <= zoneWidth && Mathf.Abs(distance.y) < zoneHeight)
        {
            nVinesOverriden++;
            return GetVineOverride(queryPosition, currentConfig);
        }
        return null;
    }

    VineFactoryConfig GetVineOverride(Vector2 queryPosition, VineFactoryConfig currentConfig)
    {
        if (overrideAction == VineZoneOverrideAction.OVERRIDE_ENTIRE_CONFIG)
        {
            return vineFactoryConfigOverride;
        }
        if (overrideAction == VineZoneOverrideAction.ENSURE_VINE_REACHABLE)
        {
            VineFactoryConfig currentConfigCopy = currentConfig.Copy();
            // yOffset is the amount above the object that the average vine will end.
            // float yOffset = 5f;
            // get distance from vine anchor point to the transform's height + yOffset
            float targetYPosition = Mathf.Abs(queryPosition.y - transform.position.y);
            // calculate nSegments needed to reach the targetYPosition
            int nSegments = (int)(targetYPosition / currentConfigCopy.segmentLength);
            // Set new segment length with variance
            int variance = 5;
            currentConfigCopy.length.min = nSegments - variance;
            currentConfigCopy.length.max = nSegments + variance;
            return currentConfigCopy;
        }
        // return full config by default
        return vineFactoryConfigOverride;
    }



}

