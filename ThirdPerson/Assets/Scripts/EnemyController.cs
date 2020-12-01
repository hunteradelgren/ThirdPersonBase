using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement Variables")]
    [SerializeField]
    float moveSpeed = 1.0f;
    [SerializeField]
    float meleeRange = 1.0f;
    [SerializeField]
    float currentHealth = 3.0f;

    public LayerMask playerLayer;
    public LayerMask environmentLayer;

    [Header("References")]
    [SerializeField]
    PlayerController player;
    [SerializeField]
    Sight obstacleSight;

    public GameObject portal1;
    public GameObject portal2;

    Vector3 goalLocation;
    Vector3 currentTargetLocation;
    bool shouldMove = false;
    bool dead = false;

    float timeSinceChecked = 0;
    bool blocked = false;
    float timeSinceHit = 0;
    bool foundAppropriate = false;

    float timeSinceShot = 0;

    // Start is called before the first frame update
    void Awake()
    {
        shouldMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (dead)

            return;

        timeSinceHit += Time.deltaTime;
        timeSinceShot += Time.deltaTime;

        goalLocation = FindObjectOfType<PlayerController>().transform.position;

        //var fullVision = sight.Sight(90, 10, Color.red);
        var directlyInFront = obstacleSight.GetSightInformation();// (90, 3, 3, Color.blue);

        //blocked = false;

        for (int i = 0; i < directlyInFront.Length; i++)
        {
            var current = directlyInFront[i];

            if (current.seen == true && current.tag == "Environment")
            {
                // we've got something blocking us
                blocked = true;
            }
            if(current.seen == true && current.tag == "Player" && timeSinceShot >= 2)
            {
                Debug.Log("Player was Shot");
                player.playerHit();
                timeSinceShot = 0;
            }
        }

        Debug.Log(blocked);
        double distancetoGoal = Vector3.Distance(transform.position, goalLocation);
        double disP1toP2toGoal = Vector3.Distance(transform.position, portal1.transform.position)
            + Vector3.Distance(portal2.transform.position, goalLocation);
        double disP2toP1toGoal = Vector3.Distance(transform.position, portal2.transform.position)
            + Vector3.Distance(portal1.transform.position, goalLocation);

        Debug.Log("to player: " + distancetoGoal + "\ndisp1top2toGoal: " + disP1toP2toGoal + "\ndisp2top1toGoal" + disP2toP1toGoal);
        // if we're blocked, find the first sight that isn't obstructed and head that way
        if (blocked)
        {
            if (!foundAppropriate)
            {
                HandleBlocked(directlyInFront);
            }

            /*if (timeSinceChecked > 1)
            {
                HandleBlocked(directlyInFront);
                timeSinceChecked = 0;
            }*/
            
            
            if (distancetoGoal < meleeRange)
            {
                Debug.Log("target in range");
                blocked = false;
                currentTargetLocation = goalLocation;
                //goalLocation = Vector3.zero;
            }
            blocked = false;
        }

        else
        {
            if (disP1toP2toGoal < distancetoGoal || disP2toP1toGoal < distancetoGoal)
            {
                blocked = false;
                if (Vector3.Distance(transform.position, portal1.transform.position) < Vector3.Distance(transform.position, portal2.transform.position))
                {
                    Debug.Log("target portal 1");
                    currentTargetLocation = portal1.transform.position;
                }
                else
                {
                    Debug.Log("target portal 2");
                    currentTargetLocation = portal2.transform.position;
                }
            }
            else
            {
                Debug.Log("target player");
                currentTargetLocation = goalLocation;
                foundAppropriate = false;
            }
        }

        timeSinceChecked += Time.deltaTime;

        Debug.DrawLine(currentTargetLocation, currentTargetLocation + Vector3.up * 5, Color.green);


        if (shouldMove)
        {
            currentTargetLocation.y = 0;
            //transform.LookAt(currentTargetLocation);

            Quaternion toRotation = Quaternion.LookRotation(currentTargetLocation - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 0.5f * Time.deltaTime);
        }

        if (!shouldMove && Vector3.Distance(transform.position, goalLocation) > meleeRange * 2)
        {
            shouldMove = true;
        }


        if (Vector3.Distance(transform.position, goalLocation) < meleeRange)
        {
            shouldMove = false;
            //goalLocation = Vector3.zero;
        }


        var newVelocity = transform.forward * moveSpeed * (shouldMove ? 1 : 0);
        newVelocity.y = GetComponent<Rigidbody>().velocity.y;

        GetComponent<Rigidbody>().velocity = newVelocity;
        GetComponent<Animator>().SetFloat("moveSpeed", GetComponent<Rigidbody>().velocity.magnitude);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Sword" && !dead && timeSinceHit > 1.0f)
        {
            currentHealth -= 1;

            //Destroy(gameObject, 3.0f);
            if (currentHealth > 0)
            {
                GetComponent<Animator>().SetTrigger("hit" + Random.Range(0, 4));
                
            }

            else
            {
                GetComponent<CapsuleCollider>().enabled = false;
                dead = true;
                GetComponent<Animator>().SetTrigger("death");
                Destroy(this, 3);
            }

            timeSinceHit = 0f;
        }
    }

    void HandleBlocked(SightInformation[] directlyInFront)
    {
        Vector3 acceptableLocation = Vector3.zero;
        bool found = false;
        for (int i = 0; i < directlyInFront.Length; i++)
        {
            var current = directlyInFront[i];


            if (current.seen == false)
            {
                if (!found)
                {
                    acceptableLocation = current.location;
                    found = true;

                }
            }
        }

        //currentTargetLocation = (goalLocation + acceptableLocation) / 2;
        currentTargetLocation = acceptableLocation;
        foundAppropriate = true;
    }

    
}
