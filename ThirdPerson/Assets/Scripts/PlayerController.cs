using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Variables")]
    [SerializeField]
    float moveSpeed = 3.0f;

    [Header("References")]
    [SerializeField]
    Transform mainCamera;
    [SerializeField]
    BoxCollider swordCollider;
    [SerializeField]
    GameObject swordModel;

    public CinemachineFreeLook normal;
    public CinemachineFreeLook aiming;


    public Transform target;
    public Transform curvepoint;
    private bool isThrown = false;
    private bool isReturning = false;
    private float time = 0.0f;
    private Vector3 thrownPosition;
    private Transform initialRotation;

    Rigidbody rb;
    Rigidbody sb;
    Animator anim;

    bool startedCombo = false;
    float timeSinceButtonPressed = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        sb = swordModel.GetComponent<Rigidbody>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        initialRotation = swordModel.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        var camForward = mainCamera.forward;
        var camRight = mainCamera.right;

        camForward.y = 0;
        camForward.Normalize();
        camRight.y = 0;
        camRight.Normalize();

        var moveDirection = (camForward * v * moveSpeed) + (camRight * h * moveSpeed);

        transform.LookAt(transform.position + moveDirection);
        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);

        anim.SetFloat("moveSpeed", Mathf.Abs(moveDirection.magnitude));

        if(Input.GetButtonDown("Jump") && !startedCombo && (!isThrown && !isReturning))
        {
            anim.SetTrigger("swordCombo");
            startedCombo = true;
        }

        if(Input.GetButtonDown("Jump") && startedCombo && (!isThrown && !isReturning))
        {
            timeSinceButtonPressed = 0;
        }

        if (Input.GetButton("Aiming"))
        {
            aiming.Priority = 25;
        }
        else
        {
            aiming.Priority = 0;
        }

        if (Input.GetButton("Aiming")&&Input.GetButtonDown("ThrowAxe"))
        {
            if(!isThrown && !isReturning)
            {
                Debug.Log("Throw Sword");
                isThrown = true;
                anim.SetTrigger("Throw Sword");
                TurnOnSwordCollider();
            }
            else if (isThrown && !isReturning)
            {
                Debug.Log("Return Sword");
                sb.transform.parent = null;
                isThrown = false;
                isReturning = true;
                TurnOffSwordCollider();
                ReturnSword();
            }

        }

        //if(isThrown && swordCollider)

        if (isReturning)
        {
            if (time < 1)
            {
                sb.position = getBQCPoint(time,thrownPosition,curvepoint.position,target.position);
                time += Time.deltaTime;
            }
            else
            {
                ResetSword();
            }
        }

        timeSinceButtonPressed += Time.deltaTime;
    }

    public void PotentialComboEnd()
    {
        TurnOffSwordCollider();

        if (timeSinceButtonPressed < 0.5f)
            return;

        anim.SetTrigger("stopCombo");
        startedCombo = false;
        timeSinceButtonPressed = 0;
        
    }

    public void EndOfCombo()
    {
        startedCombo = false;
        timeSinceButtonPressed = 0;
        TurnOffSwordCollider();
    }

    public void TurnOnSwordCollider()
    {
        swordCollider.enabled = true;
    }

    public void TurnOffSwordCollider()
    {
        swordCollider.enabled = false;
    }
    public void ThrowSword()
    {
        swordModel.transform.parent = null;
        sb.isKinematic = false;
        sb.AddForce(Camera.main.transform.TransformDirection(Vector3.forward)*50, ForceMode.Impulse);
        sb.AddTorque(sb.transform.TransformDirection(Vector3.right) * 100, ForceMode.Impulse);
    }

    public void SwordThrowHit()
    {
        
    }

    public void ReturnSword()
    {
        sb.velocity = Vector3.zero;
        sb.isKinematic = true;
        thrownPosition = sb.position;
    }

    public void ResetSword()
    {
        sb.velocity = Vector3.zero;
        sb.isKinematic = true;
        isReturning = false;
        swordModel.transform.parent = target;
        sb.position = target.position;
        swordModel.transform.rotation = initialRotation.rotation;
    }

    //found the sword return in an arc in a tutorial video on youtube
    Vector3 getBQCPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = (uu * p0) + (2 * u * t * p1) + (tt * p2);
        return p;
    }
}
