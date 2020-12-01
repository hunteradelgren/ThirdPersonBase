using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour
{
    [SerializeField]
    float degreesOfSight = 360.0f;
    [SerializeField]
    float sightDistance = 200.0f;
    [SerializeField]
    int numberOfDivisions = 180;

    [SerializeField]
    LayerMask layers;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public SightInformation[] GetSightInformation()
    {
        return GetSightInformation(degreesOfSight, numberOfDivisions, sightDistance, Color.red);
    }

    public SightInformation[] GetSightInformation(float angle, int divisions, float range, Color color)
    {
        List<SightInformation> sights = new List<SightInformation>();

        int directionToggle = 1;

        for(int i = 0; i < divisions; i++)
        {
            float value = i * (angle / divisions) * directionToggle;

            Vector3 forward = transform.forward;

            Vector3 rotated = Quaternion.AngleAxis(value, Vector3.up) * forward;

            Vector3 startPoint = transform.position;

            Vector3 endPoint = startPoint + (rotated * range);

            RaycastHit hit;

            SightInformation s = new SightInformation();


            if (Physics.Raycast(startPoint, (rotated * range), out hit, range, layers))
            {
                // we hit a thing that qualifies
                endPoint = hit.point;
                s.tag = hit.transform.tag;
                s.seen = true;
            }

            s.location = endPoint;

            Debug.DrawLine(startPoint, endPoint, color);

            directionToggle *= -1; // start at center and flip from right to left to right instead of sweeping from left to right

            sights.Add(s);
        }

        return sights.ToArray();
    }
}

public class SightInformation
{
    public Vector3 location = Vector3.zero;
    public bool seen = false;
    public string tag = "";
}