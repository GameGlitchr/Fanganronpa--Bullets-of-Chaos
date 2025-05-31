using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour, IInteractable
{
    public int charID;
    public int studentID_Number;
    public string charName;
    public string charTalent;
    public string charRealTalent;

    public float emotionalIntelligence = 75;
    public float logicalIntelligence = 25;
    public float trustTowardPlayer = 20;
    public float Despair = 5;
    public float Confidence = 15;
    public float Validity;
    public float agreeDegree;

    public bool guiltyConscience;

    public bool bored;

    public NavMeshAgent agent;
    public Transform player;
    public Player playerAI;
    public LayerMask groundMask, playerMask;

    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    private bool isWaiting;


    public DialogUIManager dialogManager;

    void Start()
    {
        playerAI = PlayerManager.instance.GetComponent<Player>();
        GetIdentification();
        bored = true;
        isWaiting = false;
        dialogManager = FindFirstObjectByType<DialogUIManager>();
    }

    void Update()
    {

        if (PlayerManager.instance.isPaused)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;

        Despair += Time.deltaTime / 10;

        if (bored && !isWaiting)
        {
            Wandering();
        }
        else if (!bored)
        {
            StopAndStare();
        }
        ControlMinMax();
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other == GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider>())
        {
            bored = false;
            StopAndStare();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider>())
        {
            bored = true;
            Wandering();
        }
    }

    private void StopAndStare()
    {
        // Stop the NavMeshAgent
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        // Look at the player
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void Wandering()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet && !isWaiting)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f && !isWaiting)
        {
            walkPointSet = false;
            StartCoroutine(WaitRoutine());
        }
    }

    private IEnumerator WaitRoutine()
    {
        isWaiting = true;
        agent.isStopped = true;

        float waitTime = Random.Range(1f, 10f);
        Debug.Log($"Waiting for {waitTime} seconds.");
        yield return new WaitForSeconds(waitTime);

        agent.isStopped = false;
        isWaiting = false;
        Wandering();
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 3f, groundMask))
        {
            walkPointSet = true;
        }
    }

    public void Argument(float damage, float truth, float type)
    {
        Debug.Log($"Received argument with damage: {damage}, truth: {truth}, type: {type}");

        if (type == 0) // an emotional argument is being made
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
            Debug.Log($"I am {agreeDegree}% sure you are right");
            float oldCon = Confidence;
            Confidence += agreeDegree - Despair;
            float newCon = Confidence;

            if (newCon < oldCon)
            {
                trustTowardPlayer--;
                Debug.Log($"Confidence lowered from {oldCon} to {newCon}!");
                Debug.Log("Trust went down!");
            }
            else if (newCon > oldCon)
            {
                trustTowardPlayer++;
                Debug.Log($"Confidence raised from {oldCon} to {newCon}!");
                Debug.Log("Trust went up!");
            }
            else
            {
                Debug.Log("Confidence did not change or other error");
            }
        }
        else if (type == 1) // a logical argument is being made
        {
            Validity = ((damage * truth) * logicalIntelligence) / emotionalIntelligence;
            Debug.Log($"That argument is about: {Validity}% valid");

            agreeDegree = ((Validity + (Confidence / 2)) + (trustTowardPlayer * truth)) / Despair;
            Debug.Log($"I am {agreeDegree}% sure you are right");
            float oldCon = Confidence;
            Confidence += agreeDegree - Despair;
            float newCon = Confidence;

            Debug.Log($"Confidence raised from {oldCon} to {newCon}");
        }
    }

    private void GetIdentification()
    {
        if (charID == 0)
        {
            charName = "Ioko Kabuto";
            charTalent = "The Ultimate Huntress";
            studentID_Number = 381152006;
        }
        else if (charID == 1)
        {
            charName = "Tomeo Hatano";
            charTalent = "The Ultimate Lawyer";
            charRealTalent = "The Ultimate Liar";
            studentID_Number = 381152004;
        }
        else if (charID == 2)
        {
            charName = "Akeno Sakai";
            charTalent = "The Ultimate Novelist";
            charRealTalent = "The Ultimate Chaos";
            studentID_Number = 381152011;
        }
        else if (charID == 3)
        {
            charName = "Genjo Takenaka";
            charTalent = "The Ultimate Eidetiker";
            studentID_Number = 381152013;
        }
        else if (charID == 4)
        {
            charName = "Dayu Enatsu";
            charTalent = "The Ultimate Actor";
            charRealTalent = "The Ultimate Despair";
            studentID_Number = 381152001;
        }
        else if (charID == 5)
        {
            charName = "Risako Kitani";
            charTalent = "The Ultimate Thief";
            studentID_Number = 381152008;
        }
        else if (charID == 6)
        {
            charName = "Masaki Tamura";
            charTalent = "The Ultimate ???";
            charRealTalent = "The Ultimate Spy";
            studentID_Number = 381152014;
        }
        else if (charID == 7)
        {
            charName = "Akira Kawashima";
            charTalent = "The Ultimate ???";
            charRealTalent = "The Ultimate Secret Agent";
            studentID_Number = 381152007;
        }
        else if (charID == 8)
        {
            charName = "Soshu Nura";
            charTalent = "The Ultimate Simp";
            charRealTalent = "The Ultimate Politician";
            studentID_Number = 381152009;
        }
        else if (charID == 9)
        {
            charName = "Miki Sugiyama";
            charTalent = "The Ultimate Sailor";
            studentID_Number = 381152012;
        }
        else if (charID == 10)
        {
            charName = "Taira Hatake";
            charTalent = "The Ultimate Aviator";
            studentID_Number = 381152003;
        }
        else if (charID == 11)
        {
            charName = "Han Saeki";
            charTalent = "The Ultimate Archer";
            studentID_Number = 381152010;
        }
        else if (charID == 12)
        {
            charName = "Masago Haga";
            charTalent = "The Ultimate F1 Racer";
            studentID_Number = 381152002;
        }
        else if (charID == 13)
        {
            charName = "Uki Umezaki";
            charTalent = "The Ultimate Journalist";
            studentID_Number = 381152016;
        }
        else if (charID == 14)
        {
            charName = "Umeka Tanifuji";
            charTalent = "The Ultimate Demolitionist";
            studentID_Number = 381152015;
        }
        else if (charID == 15)
        {
            charName = "Tetsuo Ishimoto";
            charTalent = "The Ultimate Pirate";
            studentID_Number = 381152005;
        }
    }

    public void Interact()
    {
        bored = false;
        //dialogManager.ShowDialogUI(charName, "test message");

        Debug.Log("Interacted with Character: " + charName);
    }

    private void ControlMinMax()
    {
        if (emotionalIntelligence < 10)
        {
            emotionalIntelligence = 10;
        }
        if (emotionalIntelligence > 90)
        {
            emotionalIntelligence = 90;
        }
        if (logicalIntelligence < 10)
        {
            logicalIntelligence = 10;
        }
        if (logicalIntelligence > 90)
        {
            logicalIntelligence = 90;
        }
        if(trustTowardPlayer < 0)
        {
            trustTowardPlayer = 0;
        }
        if(trustTowardPlayer > 100)
        {
            trustTowardPlayer = 100;
        }
        if (Despair < 5)
        {
            Despair = 5;
        }
        if(Despair > 100)
        {
            Despair = 100;
        }
        if(Confidence < 5)
        {
            Confidence = 5;
        }
        if(Confidence > 100)
        {
            Confidence = 100;
        }
    }


}


