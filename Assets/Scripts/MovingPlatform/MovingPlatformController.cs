using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformController : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    [SerializeField] private Transform startPoint, endPoint;
    [SerializeField] private float changeDirectionDelay = 2;

    private Transform destinationTarget, departTarget;
    private float startTime;
    private float journeyLength;
    private bool isWaiting;

    private void Start()
    {
        departTarget = startPoint;
        destinationTarget = endPoint;

        startTime = Time.time;
        journeyLength = Vector3.Distance(departTarget.position, destinationTarget.position);

    }


    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (!isWaiting)
        {
            if (Vector3.Distance(transform.position, destinationTarget.position) > 0.01f)
            {
                float distCovered = (Time.time - startTime) * speed;
                float fractionOfJourney = distCovered / journeyLength;
                transform.position = Vector3.Lerp(departTarget.position, destinationTarget.position, fractionOfJourney);
            }
            else
            {
                isWaiting = true;
                StartCoroutine(ChangeDelay());
            }
        }
    }

    IEnumerator ChangeDelay()
    {
        yield return new WaitForSeconds(changeDirectionDelay);
        changeDestination();
        startTime = Time.time;
        journeyLength = Vector3.Distance(departTarget.position, destinationTarget.position);
        isWaiting = false;
    }

    void changeDestination()
    {
        if (departTarget == endPoint && destinationTarget == startPoint)
        {
            departTarget = startPoint;
            destinationTarget = endPoint;
        }
        else
        {
            departTarget = endPoint;
            destinationTarget = startPoint;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        // Obtém o transform raiz do objeto colidido
        Transform rootTransform = other.transform.root;

        if (rootTransform.CompareTag("Player"))
        {
            // Define a plataforma como pai do jogador
            rootTransform.parent = transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Obtém o transform raiz do objeto colidido
        Transform rootTransform = other.transform.root;

        if (rootTransform.CompareTag("Player"))
        {
            // Remove a plataforma como pai do jogador
            rootTransform.parent = null;
        }
    }
}
