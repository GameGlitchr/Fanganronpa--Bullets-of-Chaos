using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[System.Serializable]
public struct CharacterOpinion
{
    public int targetCharID;
    public string targetCharName;
    public float opinionValue;

    public int goodMemories;
    public int badMemories;
}

[System.Serializable]
public struct LocationOpinion
{
    public string locationName;
    public float opinionValue;
}


public class Character : CharacterBase, IInteractable
{

    public float Validity;
    public float agreeDegree;

    public bool bored;

    public NavMeshAgent agent;
    public Transform player;
    public Player playerAI;
    public bool playerInteracting;
    public LayerMask groundMask, playerMask;

    public Vector3 walkPoint;
    public Vector3 lastWalkPoint;
    bool walkPointSet;
    public float walkPointRange;
    private bool isWaiting;

    public Image frontImage;
    public Image backImage;

    public Collider privateSpace;
    public Collider personalSpace;
    public Collider publicSpace;

    public DialogUIManager dialogManager;

    private LocationData locationData;


    void Start()
    {
        base.Start();
        playerAI = PlayerManager.instance.GetComponent<Player>();
        GetIdentification();
        bored = true;
        playerInteracting = false;
        isWaiting = false;
        dialogManager = FindFirstObjectByType<DialogUIManager>();
        InitializeRelationships();
        InitializeLocationOpinions();
        locationData = FindFirstObjectByType<LocationData>();

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.autoBraking = false;
            agent.stoppingDistance = 0.5f;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        SnapToNavMesh();
        AssignCharacterMaterials();
        StartCoroutine(GoldfishMemory());
    }

    /// <summary>
    /// Pre-update checks to make sure character is in place with the right face
    /// </summary>

    private void SnapToNavMesh()
    {
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }
        else
        {
            Debug.LogWarning($"{charName} failed to snap to NavMesh. Current position: {transform.position}");
        }
    }

    private void AssignCharacterMaterials()
    {
        string firstName = charName.Split(' ')[0];

        string frontMatPath = $"Materials/M_{firstName}Front";
        string backMatPath = $"Materials/M_{firstName}Back";

        Material frontMat = Resources.Load<Material>(frontMatPath);
        Material backMat = Resources.Load<Material>(backMatPath);

        if (frontMat != null && frontImage != null)
        {
            frontImage.material = frontMat;
            Debug.Log($"{charName} assigned front material: {frontMat.name}");
        }
        else
        {
            Debug.LogWarning($"{charName} failed to assign front material: {frontMatPath}");
        }

        if (backMat != null && backImage != null)
        {
            backImage.material = backMat;
            Debug.Log($"{charName} assigned back material: {backMat.name}");
        }
        else
        {
            Debug.LogWarning($"{charName} failed to assign back material: {backMatPath}");
        }
    }

    /// <summary>
    /// main loop
    /// </summary>

    void Update()
    {
        if (PlayerManager.instance.isPaused)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;

        IncrementDespair();
        IncrementHope();
        GetDespairFlowRate();
        GetHopeFlowRate();
        UpdateMood();
        CheckHopeDespairThresholds();
        UpdateLocationAwareness();
        UpdateLocationTracking();
        IncrementStirCraze();

        if (imWALKINGhere && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            imWALKINGhere = false;
            //Debug.Log($"{charName} arrived at {currentLocation?.locationName ?? "somewhere"}");
        }

        if (currentLocationStatus == LocationStatus.InLocation)
            walkPointRange = 5f;
        else if (currentLocationStatus == LocationStatus.Nowhere)
            walkPointRange = 20f;
        else if (currentLocationStatus == LocationStatus.AtLocation)
            walkPointRange = 10f;

        if (bored && !isWaiting && !isStirCrazy && !imWALKINGhere)
            Wandering();
        else if (!bored && playerInteracting)
            StopAndStare();

        if (bored) ScanForApproach();

        ControlMinMax();

        if (!agent.isOnNavMesh)
        {
            Debug.LogError($"{charName} is NOT on the NavMesh!");
        }

        if (isStirCrazy && !isInteracting && !agent.pathPending && bored)
        {
            var targetLoc = ChooseNewLocation();
            if (targetLoc != null)
            {
                MoveToLocation(targetLoc);
                Debug.Log($"{charName} is moving to {targetLoc.locationName}");
                timeInCurrentLocation = 0f;
            }
        }
        //Debug.Log($"{charName} | Bored: {bored}, isWaiting: {isWaiting}, isInteracting: {isInteracting}");
    }

    private void IncrementDespair()
    {
        Despair += Time.deltaTime * GetDespairGrowthRate();
    }

    private void IncrementHope()
    {
        Hope += Time.deltaTime * GetHopeGrowthRate();
    }

    private float GetDespairGrowthRate()
    {
        float memoryFactor = Mathf.Clamp(TotalBadMemories() * 0.1f, 0f, 5f);

        if (charID == 2 || charID == 4)
            return 1000f;

        float baseRate;
        if (Mood >= 800) baseRate = 0.1f;
        else if (Mood >= 600) baseRate = 0.25f;
        else if (Mood >= 400) baseRate = 0.5f;
        else if (Mood >= 200) baseRate = 1f;
        else baseRate = 2f;

        float boostFactor = Mathf.InverseLerp(0f, 100f, Hope); // 0 to 1 as Hope increases
        return (baseRate + memoryFactor) * (1f + boostFactor);
    }

    private float GetHopeGrowthRate()
    {
        float memoryFactor = Mathf.Clamp(TotalGoodMemories() * 0.1f, 0f, 5f);

        float baseRate;
        if (Mood >= 800) baseRate = 2f;
        else if (Mood >= 600) baseRate = 1f;
        else if (Mood >= 400) baseRate = 0.5f;
        else if (Mood >= 200) baseRate = 0.25f;
        else baseRate = 0.1f;

        float boostFactor = Mathf.InverseLerp(0f, 100f, Despair); // 0 to 1 as Despair increases
        return (baseRate + memoryFactor) * (1f + boostFactor);
    }

    private void CheckHopeDespairThresholds()
    {
        if (Despair >= 100f)
        {
            Despair = 0f;
            Hope -= 50f;
            ForgetRandomGoodMemory();
        }

        if (Hope >= 100f)
        {
            Hope = 0f;
            Despair -= 50f;
            ForgetRandomBadMemory();
        }
    }

    private IEnumerator GoldfishMemory()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5f, 15f));

            float roll = Random.Range(0f, 100f);
            if (roll >= 50f)
            {
                ForgetRandomBadMemory();
                Debug.LogWarning($" GOLDFISH! {charName} forgot a BAD memory");
            }
            else
            {
                ForgetRandomGoodMemory();
                Debug.LogWarning($" GOLDFISH! {charName} forgot a GOOD memory");
            }
        }
    }


    private void ForgetRandomGoodMemory()
    {
        var memoryCandidates = opinionList
            .Select((op, index) => new { op, index })
            .Where(entry => entry.op.goodMemories > 0)
            .ToList();

        if (memoryCandidates.Count > 0)
        {
            var chosen = memoryCandidates[Random.Range(0, memoryCandidates.Count)];
            var opinion = opinionList[chosen.index];
            opinion.goodMemories--;
            opinionList[chosen.index] = opinion;

            Debug.Log($"{charName} forgot a good memory of {chosen.op.targetCharName}");
        }
    }

    private void ForgetRandomBadMemory()
    {
        var memoryCandidates = opinionList
            .Select((op, index) => new { op, index })
            .Where(entry => entry.op.badMemories > 0)
            .ToList();

        if (memoryCandidates.Count > 0)
        {
            var chosen = memoryCandidates[Random.Range(0, memoryCandidates.Count)];
            var opinion = opinionList[chosen.index];
            opinion.badMemories--;
            opinionList[chosen.index] = opinion;
            Debug.Log($"{charName} forgot a bad memory of {chosen.op.targetCharName}");
        }
    }

    public int TotalGoodMemories()
    {
        return opinionList.Sum(op => op.goodMemories);
    }

    public int TotalBadMemories()
    {
        return opinionList.Sum(op => op.badMemories);
    }



    private void UpdateMood()
    {
        // The actual "emotional corrosion" of mood happens here
        float despairDrain = Despair * GetDespairFlowRate();
        float hopeRestore = Hope * GetHopeFlowRate();
        Mood = Mathf.Clamp(Mood - Time.deltaTime * despairDrain + Time.deltaTime * hopeRestore, 0, 1000);
    }

    // This controls how much Despair affects Mood erosion
    private float GetDespairFlowRate()
    {
        if (Despair >= 90) return 0.5f;
        else if (Despair >= 60) return 0.3f;
        else if (Despair >= 30) return 0.1f;
        else if (Despair > 10) return 0.05f;
        else return 0.01f;
    }
    private float GetHopeFlowRate()
    {
        if (Hope >= 90) return 0.5f;
        else if (Hope >= 60) return 0.3f;
        else if (Hope >= 30) return 0.1f;
        else if (Hope > 10) return 0.05f;
        else return 0.01f;
    }




    /// <summary>
    /// character emotions and relationships
    /// </summary>
    /// 

    [SerializeField]
    private List<CharacterOpinion> opinionList = new List<CharacterOpinion>();

    public Dictionary<int, float> opinions = new Dictionary<int, float>();
    public List<int> besties = new List<int>(3);
    public List<int> enemies = new List<int>(3);
    public List<int> primeSuspects = new List<int>(3);
    public List<Transform> favoriteHangouts = new List<Transform>(3);

    // Sync to/from list:
    private void SyncOpinionsFromList()
    {
        opinions.Clear();
        foreach (var entry in opinionList)
        {
            if (!opinions.ContainsKey(entry.targetCharID))
                opinions.Add(entry.targetCharID, entry.opinionValue);
        }
    }

    private void SyncOpinionsToList()
    {
        for (int i = 0; i < opinionList.Count; i++)
        {
            if (opinions.ContainsKey(opinionList[i].targetCharID))
            {
                CharacterOpinion updated = opinionList[i];
                updated.opinionValue = opinions[updated.targetCharID];
                opinionList[i] = updated;
            }
        }

        // Add any new opinions that aren't already in the list
        foreach (var pair in opinions)
        {
            if (!opinionList.Any(op => op.targetCharID == pair.Key))
            {
                opinionList.Add(new CharacterOpinion
                {
                    targetCharID = pair.Key,
                    targetCharName = CharacterBase.allCharacterProfiles.ContainsKey(pair.Key)
                        ? CharacterBase.allCharacterProfiles[pair.Key].name
                        : "Unknown",
                    opinionValue = pair.Value,
                    goodMemories = 0,
                    badMemories = 0
                });
            }
        }
    }


    public void RegisterInteractionMemory(int targetCharID, bool good)
    {
        int index = opinionList.FindIndex(op => op.targetCharID == targetCharID);
        if (index != -1)
        {
            CharacterOpinion entry = opinionList[index];
            if (good)
                entry.goodMemories++;
            else
                entry.badMemories++;

            opinionList[index] = entry; // Re-assign modified struct back to list
        }
    }

    public void UpdateBestiesAndEnemies()
    {
        besties.Clear();
        enemies.Clear();

        // Sort by opinion descending for besties
        var topThree = opinions
            .Where(p => p.Value >= 80)
            .OrderByDescending(p => p.Value)
            .Take(3)
            .Select(p => p.Key);

        foreach (var id in topThree)
            besties.Add(id);

        // Sort by opinion ascending for enemies
        var bottomThree = opinions
            .Where(p => p.Value < 20)
            .OrderBy(p => p.Value)
            .Take(3)
            .Select(p => p.Key);

        foreach (var id in bottomThree)
            enemies.Add(id);
    }


    private void InitializeLocationOpinions()
    {
        LocationData locationData = FindFirstObjectByType<LocationData>();
        if (locationData == null) return;

        foreach (var loc in locationData.activeLocations)
        {
            if (!locationOpinions.Exists(op => op.locationName == loc.locationName))
            {
                locationOpinions.Add(new LocationOpinion
                {
                    locationName = loc.locationName,
                    opinionValue = 50f // Neutral default
                });
            }
        }
    }

    [System.Serializable]
    public struct LocationOpinion
    {
        public string locationName;
        public float opinionValue;
    }


    [SerializeField]
    public List<LocationOpinion> locationOpinions = new List<LocationOpinion>();

    public float GetLocationOpinion(string name)
    {
        foreach (var op in locationOpinions)
            if (op.locationName == name) return op.opinionValue;

        return 50f; // Neutral by default
    }

    public void ChangeLocationOpinion(string name, float delta)
    {
        for (int i = 0; i < locationOpinions.Count; i++)
        {
            if (locationOpinions[i].locationName == name)
            {
                locationOpinions[i] = new LocationOpinion
                {
                    locationName = name,
                    opinionValue = Mathf.Clamp(locationOpinions[i].opinionValue + delta, 0, 100)
                };
                return;
            }
        }

        // Add new if it doesn't exist
        locationOpinions.Add(new LocationOpinion
        {
            locationName = name,
            opinionValue = Mathf.Clamp(50f + delta, 0, 100)
        });
    }


    private void InitializeRelationships()
    {
        opinions.Clear();

        for (int i = 0; i < 16; i++)
        {
            if (i == charID) continue;

            if (inseparableBesties.Contains(i))
                opinions[i] = 100f;
            else if (swornEnemies.Contains(i))
                opinions[i] = 0f;
            else
                opinions[i] = 50f;
        }

        SyncOpinionsToList();
    }



    public void ChangeOpinion(int targetCharID, float opinionChange)
    {
        // Prevent opinion from changing if the target is a bestie or sworn enemy
        if (inseparableBesties.Contains(targetCharID))
        {
            opinions[targetCharID] = 100f;
            return;
        }

        if (swornEnemies.Contains(targetCharID))
        {
            opinions[targetCharID] = 0f;
            return;
        }

        if (opinions.ContainsKey(targetCharID))
        {
            opinions[targetCharID] = Mathf.Clamp(opinions[targetCharID] + opinionChange, 0, 100);
            SyncOpinionsToList();
        }
    }



    public void UpdatePrimeSuspects()
    {
        List<KeyValuePair<int, float>> sortedOpinions = new List<KeyValuePair<int, float>>(opinions);
        sortedOpinions.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

        primeSuspects.Clear();
        for (int i = 0; i < 3 && i < sortedOpinions.Count; i++)
            primeSuspects.Add(sortedOpinions[i].Key);
    }

    public void InviteToHangout(bool followMe, Transform hangoutSpot)
    {
        if (followMe)
            agent.SetDestination(player.position);
        else
            agent.SetDestination(hangoutSpot.position);
    }



    /// <summary>
    /// NPC behavior
    /// </summary>

    private void StopAndStare()  //at the player
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void Wandering()
    {
       
        if (!walkPointSet)
        {
            SearchWalkPoint();
            return;
        }

        if (!isWaiting && walkPointSet && walkPoint != lastWalkPoint)
        {
            agent.SetDestination(walkPoint);
            lastWalkPoint = walkPoint;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && agent.velocity.sqrMagnitude < 0.01f && !isWaiting)
        {
            StartCoroutine(WaitRoutine());
            return;
        }
    }

    private IEnumerator WaitRoutine()
    {
        if (isWaiting)
        {
            yield break;
        }

        isWaiting = true;
        agent.isStopped = true;

        float waitTime = Random.Range(1f, 10f);
        yield return new WaitForSeconds(waitTime);

        isWaiting = false;
        agent.isStopped = false;
        walkPointSet = false; // Force a new point next frame
        agent.ResetPath(); // <--- Force-clear stuck pathfinding
    }

    private bool IsValidNavMeshPath(Vector3 target)
    {
        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(target, path))
        {
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                return true;
            }
        }
        return false;
    }


    private void SearchWalkPoint()
    {
        UpdateLocationAwareness();

        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        Vector3 candidateOrigin = new Vector3(transform.position.x + randomX, transform.position.y + 5f, transform.position.z + randomZ);

        if (Physics.Raycast(candidateOrigin, Vector3.down, out RaycastHit hit, 10f, groundMask))
        {
            if (IsValidNavMeshPath(hit.point))
            {
                walkPoint = hit.point;
                walkPointSet = true;
            }
            else
            {
                Debug.LogWarning($"{charName} found walk point at {hit.point} but it's not reachable via NavMesh.");
            }
        }
        else
        {
            walkPointSet = false;
        }

        if (walkPoint == lastWalkPoint)
        {
            Debug.LogWarning($"{charName} got the same walk point twice in a row!");
        }
    }



    /// <summary>
    /// Location information migration implementation
    /// </summary>
    /// 

    public enum LocationStatus { Nowhere, NearLocation, AtLocation, InLocation }
    public LocationStatus currentLocationStatus = LocationStatus.Nowhere;
    public LocationData.LocationInfo currentLocation = null;

    private void UpdateLocationAwareness()
    {
        if (LocationData.Instance == null)
        {
            return;
        }

        currentLocationStatus = LocationStatus.Nowhere;
        currentLocation = null;

        foreach (var loc in LocationData.Instance.activeLocations)
        {
            if (LocationData.Instance.IsInsideLocation(privateSpace, loc))
            {
                currentLocationStatus = LocationStatus.InLocation;
                currentLocation = loc;
                return;
            }
            else if (LocationData.Instance.IsAtLocation(privateSpace, loc))
            {
                currentLocationStatus = LocationStatus.AtLocation;
                currentLocation = loc;
                return;
            }
            else if (LocationData.Instance.IsNearLocation(publicSpace, loc))
            {
                currentLocationStatus = LocationStatus.NearLocation;
                currentLocation = loc;
                return;
            }
        }
    }


    private string currentLocationName;
    private float timeInCurrentLocation;

    private void UpdateLocationTracking()
    {
        foreach (var loc in locationData.activeLocations)
        {
            if (locationData.IsInsideLocation(privateSpace, loc))
            {
                if (currentLocationName != loc.locationName)
                {
                    currentLocationName = loc.locationName;
                    timeInCurrentLocation = 0f;
                    Debug.Log($"{charName} entered {currentLocationName}");
                }
                else
                {
                    timeInCurrentLocation += Time.deltaTime;
                }

                return; // We're inside a location; no need to check others
            }
        }

        // If not inside any known location
        currentLocationName = null;
        timeInCurrentLocation = 0f;
    }


    public float stirCrazy = 0f;
    private float stirCrazyCap = 100f;

    public bool isStirCrazy => stirCrazy >= stirCrazyCap;
    public bool imWALKINGhere = false;

    private void IncrementStirCraze()
    {
        if (imWALKINGhere) return;

        float rate = 1f;

        switch (currentLocationStatus)
        {
            case LocationStatus.InLocation:
                // Starts comfy, becomes stir-crazy over time
                float boredomFactor = Mathf.InverseLerp(0f, 60f, timeInCurrentLocation); // 0 → 1 over 60 seconds
                rate = Mathf.Lerp(-0.5f, 2.5f, boredomFactor);
                break;

            case LocationStatus.AtLocation:
                rate = 0.25f; // slowly rising
                break;

            case LocationStatus.NearLocation:
                rate = 1.5f; // they’re close, getting antsy
                break;

            case LocationStatus.Nowhere:
                rate = 3f; // they're lost, stir-craze grows fast
                break;
        }

        stirCrazy = Mathf.Clamp(stirCrazy + rate * Time.deltaTime, 0f, stirCrazyCap);

        // Optional debug
        // Debug.Log($"{charName} stirCrazy: {stirCrazy:F2}");
    }

    private LocationData.LocationInfo ChooseNewLocation()
    {
        var options = locationData.activeLocations
            .Where(loc => loc.locationName != currentLocationName)
            .ToList();

        if (options.Count == 0) return null;

        float totalWeight = 0f;
        List<float> weights = new List<float>();

        foreach (var loc in options)
        {
            float opinion = GetLocationOpinion(loc.locationName);
            float weight = Mathf.Clamp(opinion, 1f, 100f); // Avoid 0
            weights.Add(weight);
            totalWeight += weight;
        }

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        for (int i = 0; i < options.Count; i++)
        {
            cumulative += weights[i];
            if (roll <= cumulative)
                return options[i];
        }

        return options.Last(); // Fallback
    }


    public void MoveToLocation(LocationData.LocationInfo targetLocation)
    {
        if (targetLocation == null || agent == null) return;

        // Target the center of the collider
        Vector3 center = targetLocation.locationLocation;

        // Optional: add slight random offset to prevent character stacking
        Vector3 randomOffset = new Vector3(
            Random.Range(-1f, 1f),
            0f,
            Random.Range(-1f, 1f)
        );

        Vector3 destination = center + randomOffset;

        agent.SetDestination(destination);

        Debug.Log($"{charName} is moving to {targetLocation.locationName} at {destination}");

        imWALKINGhere = true;
        stirCrazy = 0f;
    }


    private void Migrate()
    {
        //we'll get to this later dont worry about it yet
    }


    /// <summary>
    /// NPC to NPC interactions
    /// </summary>

    #region Interaction Framework

    public bool isInteracting;

    public List<Character> GetCharactersInPublicSpace()
    {
        int publicLayer = LayerMask.NameToLayer("Public Space");
        int layerMask = 1 << publicLayer;

        Collider[] colliders = Physics.OverlapBox(
            publicSpace.bounds.center,
            publicSpace.bounds.extents,
            Quaternion.identity,
            layerMask
        );

        List<Character> others = new List<Character>();

        foreach (var col in colliders)
        {
            Character other = col.GetComponentInParent<Character>();
            if (other != null && other != this)
            {
                others.Add(other);
            }
        }

        Debug.Log($"{charName} sees {others.Count} nearby: [{string.Join(", ", others.ConvertAll(o => o.charName))}]");
        return others;
    }


    float Timer = 5f;
    float Cooldown = 5f;
    private void ScanForApproach()
    {
        Timer -= Time.deltaTime;
       

        if (Timer <= 0f)
        {
            Debug.Log($"{charName} is scanning for available interactions...");
            DecideToApproach();
            Timer = Cooldown;
        }

    }

    public void AttemptNPCInteraction()
    {
        if (isInteracting || isWaiting || !bored) return;

        var candidates = GetCharactersInPublicSpace();
        if (candidates.Count == 0) return;

        Character target = ChooseInteractionTarget(candidates);

        if (target != null && !target.isInteracting)
        {
            StartCoroutine(InteractionRoutine(target));
        }
    }

    public Character ChooseInteractionTarget(List<Character> candidates)
    {
        Character chosen = null;
        float bestScore = float.MinValue;

        foreach (var other in candidates)
        {
            if (other.charID == this.charID) continue;

            float opinion = opinions.ContainsKey(other.charID) ? opinions[other.charID] : 50f;
            int good = opinionList.Find(op => op.targetCharID == other.charID).goodMemories;
            int bad = opinionList.Find(op => op.targetCharID == other.charID).badMemories;

            float score = opinion;

            if (isIntrovert)
            {
                score += good * 2 - bad;
                if (opinion >= 90) score += 50;
                else if (opinion < 90 && opinion > 35) score -= 25;
                else if (opinion <= 35) score -= 50;
            }
            else if (isExtrovert)
            {
                score += bad * 2 - good;
                if (opinion >= 80) score += 10;
                else if (opinion <= 50 && opinion > 20) score += 30;
                else if (opinion <= 20) score -= 20;
            }

            if (score > bestScore)
            {
                bestScore = score;
                chosen = other;
            }
        }

        // Add rejection possibility — maybe they don’t like anyone
        if (bestScore < 40f && Random.value < 0.5f)
        {
            Debug.Log($"{charName} decided no one is worth talking to.");
            return null;
        }

        Debug.Log($"{charName} is considering interacting with {(chosen ? chosen.charName : "nobody")}");
        return chosen;
    }


    public void DecideToApproach()
    {
        var people = GetCharactersInPublicSpace();

        if (people.Count == 0) return;

        Character target = ChooseInteractionTarget(people);

        if (target == null) return;

        bool wantsToApproach = false;

        if (isIntrovert)
            wantsToApproach = Random.value < (0.2f / people.Count); // Lower chance
        else if (isExtrovert)
            wantsToApproach = Random.value < (0.2f * people.Count); // Higher chance

        Debug.Log($"{charName} {(wantsToApproach ? "decided to" : "ignored")} approach to {target.charName}");

        if (wantsToApproach && !target.isInteracting)
            StartCoroutine(InteractionRoutine(target));
    }

    private IEnumerator InteractionRoutine(Character partner)
    {
        Debug.Log($"{charName} is beginning InteractionRoutine with {partner.charName}");

        isInteracting = true;
        bored = false;
        partner.ReceiveInteraction(this);

        // Try to move into partner's personal space
        bool reached = false;
        yield return StartCoroutine(MoveToPersonalSpace(partner, result => reached = result));

        if (!reached)
        {
            Debug.LogWarning($"{charName} failed to reach {partner.charName} and is aborting interaction.");
            isInteracting = false;
            partner.isInteracting = false;
            bored = true;
            yield break;
        }

        // Proceed with dialog
        List<int> myResults = new();
        List<int> theirResults = new();

        for (int i = 0; i < 3; i++)
        {
            var myStatement = GenerateStatement();
            yield return PerformStatement(partner, myStatement);
            myResults.Add(partner.InterpretStatement(this, myStatement));

            var theirStatement = partner.GenerateStatement();
            yield return partner.PerformStatement(this, theirStatement);
            theirResults.Add(InterpretStatement(partner, theirStatement));
        }

        ResolveInteractionResult(partner, myResults);
        partner.ResolveInteractionResult(this, theirResults);

        isInteracting = false;
        partner.isInteracting = false;
        bored = true;
        partner.bored = true;
    }


    public (bool flirt, bool insult, float confidence, float tone) GenerateStatement()
    {
        return (
            flirt: Random.value > 0.5f,
            insult: Random.value > 0.5f,
            confidence: Random.Range(0f, 1f),
            tone: opinions.ContainsKey(charID) ? opinions[charID] / 100f : 0.5f
        );
    }

    public IEnumerator PerformStatement(Character partner, (bool flirt, bool insult, float confidence, float tone) stmt)
    {
        yield return new WaitForSeconds(1f);

        string mood = stmt.insult
            ? (stmt.flirt ? "flirty insult" : "harsh insult")
            : (stmt.flirt ? "flirty comment" : "friendly remark");

        Debug.Log($"{charName} says a {mood} to {partner.charName}");

        yield return new WaitForSeconds(1f);
    }

    public int InterpretStatement(Character speaker, (bool flirt, bool insult, float confidence, float tone) stmt)
    {
        float appreciation = stmt.confidence + stmt.tone;
        if (stmt.flirt) appreciation += 0.2f;
        if (stmt.insult) appreciation -= 0.3f;

        int result = appreciation > 0.6f ? 1 : 0;
        Debug.Log($"{charName} {(result == 1 ? "appreciated" : "rejected")} the statement from {speaker.charName}");
        return result;
    }

    public void ResolveInteractionResult(Character partner, List<int> results)
    {
        int good = results.Count(r => r == 1);
        int bad = results.Count(r => r == 0);

        Debug.Log($"{charName} had {good} good times and {bad} bad times with {partner.charName}");

        float opinionDelta = (good - bad) * 5f; // Tune the multiplier as needed

        ChangeOpinion(partner.charID, opinionDelta);
        Debug.Log($"{charName}'s opinion of {partner.charName} changed by {opinionDelta}");


        // Register each good/bad memory properly
        for (int i = 0; i < good; i++)
            RegisterInteractionMemory(partner.charID, true);

        for (int i = 0; i < bad; i++)
            RegisterInteractionMemory(partner.charID, false);

        SyncOpinionsToList();
        UpdateBestiesAndEnemies();

        isInteracting = false;
        bored = true;
    }

    public void ReceiveInteraction(Character initiator)
    {
        isInteracting = true;
        bored = false;

    }

    private IEnumerator MoveToPersonalSpace(Character partner, System.Action<bool> callback)
    {
        Debug.Log($"{charName} is attempting to move to {partner.charName}'s personal space.");

        agent.isStopped = false;
        agent.SetDestination(partner.transform.position);

        float timeout = 5f;
        float timer = 0f;

        while (!personalSpace.bounds.Intersects(partner.personalSpace.bounds))
        {
            if (timer > timeout)
            {
                Debug.LogWarning($"{charName} timed out trying to reach {partner.charName}.");
                callback(false);
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.ResetPath();

        Debug.Log($"{charName} reached {partner.charName}'s personal space.");
        StopAndStareAt(partner);
        partner.StopAndStareAt(this);
        yield return new WaitForSeconds(1f);

        callback(true);
    }



    public IEnumerator StopAndStareAt(Character target)
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        float elapsed = 0f;
        while (Quaternion.Angle(transform.rotation, lookRotation) > 1f && elapsed < 1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }



    private void OnTriggerStay(Collider other)
    {
        //Debug.Log($"{charName}: OnTriggerStay hit with {other.name}");
        if (isInteracting || isWaiting || agent.pathPending) return;
        if (!other.CompareTag("Character")) return;
        if (!other.TryGetComponent<Character>(out Character otherChar)) return;
        if (otherChar == this) return;

        // 1. Personal space enforcement
        if (personalSpace.bounds.Intersects(otherChar.privateSpace.bounds))
        {
            // Always move away from anyone in personal space
            MoveAwayFrom(otherChar);
            Debug.Log($"{charName} is enforcing personal space from {otherChar.charName}");
            return;
        }

        // 2. Enemy avoidance in public space
        if (publicSpace.bounds.Intersects(otherChar.publicSpace.bounds))
        {
            if (opinions.TryGetValue(otherChar.charID, out float opVal) && opVal < 20f)
            {
                MoveAwayFrom(otherChar);
                Debug.Log($"{charName} is avoiding enemy {otherChar.charName}");
            }
        }
    }

    private void MoveAwayFrom(Character other)
    {
        Vector3 away = (transform.position - other.transform.position).normalized;
        Vector3 destination = transform.position + away * 3f;

        // Optional: Raycast or NavMesh check here if you want cleaner nav logic
        agent.SetDestination(destination);
    }



    #endregion





    /// <summary>
    /// NPC and Player interactions
    /// </summary>

    public void Argument(float damage, float truth, float type)
    {
        Debug.Log($"Received argument with damage: {damage}, truth: {truth}, type: {type}");

        if (type == 0) // Emotional
        {
            Validity = ((damage * truth) * emotionalIntelligence) / logicalIntelligence;
            Debug.Log($"That argument is about: {Validity}% valid");

            if (truth * trustTowardPlayer >= Random.Range(0, 100))
            {
                Validity *= 1.5f;
                Debug.Log("I believe you!");
            }
            else
            {
                Validity *= -1f;
                Debug.Log("I think you're full of shit.");
            }

            agreeDegree = ((Validity + (Confidence / 2)) + (trustTowardPlayer * truth)) - Despair;
            Confidence += agreeDegree - Despair;
        }
        else if (type == 1) // Logical
        {
            Validity = ((damage * truth) * logicalIntelligence) / emotionalIntelligence;
            Debug.Log($"That argument is about: {Validity}% valid");

            agreeDegree = ((Validity + (Confidence / 2)) + (trustTowardPlayer * truth)) / Despair;
            Confidence += agreeDegree - Despair;
        }
    }

    public void Interact()
    {
        bored = false;
        Debug.Log("Interacted with Character: " + charName);
    }

    private void ControlMinMax()
    {
        emotionalIntelligence = Mathf.Clamp(emotionalIntelligence, 10, 90);
        logicalIntelligence = Mathf.Clamp(logicalIntelligence, 10, 90);
        trustTowardPlayer = Mathf.Clamp(trustTowardPlayer, 0, 100);
        Despair = Mathf.Clamp(Despair, 5, 100);
        Confidence = Mathf.Clamp(Confidence, 5, 100);
    }
}
