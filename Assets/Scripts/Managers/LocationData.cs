using System.Collections.Generic;
using UnityEngine;

public class LocationData : MonoBehaviour
{

    [System.Serializable]
    public class LocationInfo
    {
        public string locationName;
        public Vector3 locationLocation;
        public Collider outsideCollider;
        public Collider insideCollider; // Optional
    }

    public List<LocationInfo> activeLocations = new List<LocationInfo>();

    public static LocationData Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        GameObject[] locationObjects = GameObject.FindGameObjectsWithTag("Location");

        foreach (GameObject obj in locationObjects)
        {
            // Ensure this is a root object, not a child part of another location
            if (obj.transform.parent != null) continue;

            Collider outside = obj.GetComponent<Collider>();
            Collider inside = null;

            // Try to find a child with a collider for inside
            foreach (Transform child in obj.transform)
            {
                var col = child.GetComponent<Collider>();
                if (col != null)
                {
                    inside = col;
                    break;
                }
            }

            if (outside != null)
            {
                activeLocations.Add(new LocationInfo
                {
                    locationName = obj.name,
                    outsideCollider = outside,
                    insideCollider = inside,
                    locationLocation = obj.transform.position
                });

                Debug.Log($"Location loaded: {obj.name} (Inside: {(inside != null ? "Yes" : "No")})");
            }
        }
    }


    public LocationInfo GetLocationByName(string name)
    {
        return activeLocations.Find(loc => loc.locationName == name);
    }

    public bool IsInsideLocation(Collider privateCol, LocationInfo loc)
    {
        if (loc.insideCollider != null)
        {
            return loc.insideCollider.bounds.Contains(privateCol.bounds.min) &&
                   loc.insideCollider.bounds.Contains(privateCol.bounds.max);
        }
        return false;
    }

    public bool IsNearLocation(Collider publicCol, LocationInfo loc)
    {
        return loc.outsideCollider.bounds.Intersects(publicCol.bounds);
    }

    public bool IsAtLocation(Collider privateCol, LocationInfo loc)
    {
        return loc.outsideCollider.bounds.Contains(privateCol.bounds.min) &&
               loc.outsideCollider.bounds.Contains(privateCol.bounds.max);
    }


}
