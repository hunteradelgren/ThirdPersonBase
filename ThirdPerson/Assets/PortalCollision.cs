using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCollision : MonoBehaviour
{
    public Transform objectInPortal;
    Portal myPortal;
    // Start is called before the first frame update
    void Start()
    {
        myPortal = GetComponent<Portal>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player" || other.name == "Enemy")
        {
            objectInPortal = other.transform;

            var pivot = objectInPortal.transform;
            var directionToPivotFromTransform = pivot.position - transform.position;
            directionToPivotFromTransform.Normalize();

            var pivotToNormalDotProduct = Vector3.Dot(directionToPivotFromTransform, myPortal.normalVisible.forward);
            if (pivotToNormalDotProduct > 0)
            {
                var newPosition = Portal.TransformPositionBetweenPortals(myPortal, myPortal.otherPortal, objectInPortal.transform.position);

                var newRotation = Portal.TransformRotationBetweenPortals(myPortal, myPortal.otherPortal, objectInPortal.transform.rotation);

                objectInPortal.transform.SetPositionAndRotation(newPosition, newRotation);

                objectInPortal = null;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            objectInPortal = null;
        }
    }
}
