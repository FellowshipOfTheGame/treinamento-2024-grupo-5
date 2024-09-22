using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mantega;
using TMPro;

public class Test_Ranking : MonoBehaviour
{
    [Header("Movement Statistics")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Rigidbody playerRb;
    public TMP_Text text_playerSpeed;
    public float speedRecord;
    void Start()
    {
        if(playerMovement == null)
            Generics.ReallyTryGetComponent(gameObject, out playerMovement);
        playerRb = playerMovement.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float playerSpeed = new Vector2(playerRb.velocity.x, playerRb.velocity.z).magnitude;
        if(playerSpeed > speedRecord)
            speedRecord = playerSpeed;
        SetMovementStatistics(playerSpeed);
    }

    public void SetMovementStatistics(float currentSpeed)
    {
        if (text_playerSpeed == null)
            return;
        text_playerSpeed.text = $"Velocity: {currentSpeed}\nRecord: {speedRecord}";
    }
}
