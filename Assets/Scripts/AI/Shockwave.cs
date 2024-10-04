using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    public float riseSpeed = 2f;
    public float maxHeight = 5f;
    public bool isRising = false;
    public float upwardForce = 500f;
    [SerializeField] private Vector3 initialPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        initialPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (isRising)
        {
            if (transform.position.y < initialPosition.y + maxHeight)
            {
                transform.Translate(Vector3.up * (riseSpeed * Time.deltaTime));
            }
            else
            {
                isRising = false;
                transform.position = new Vector3(0, 0, 0);
            }
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (isRising && collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.AddForce(Vector3.up * upwardForce);
            }
        }
    }

    public void StartRising()
    {
        isRising = true;
        transform.position = initialPosition;
    }
}
