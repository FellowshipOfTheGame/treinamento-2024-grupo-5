using UnityEngine;
using UnityEngine.Serialization;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float gravity;
    [SerializeField] private int damage;
    private Vector3 _velocity;
    private HPController _playerHealthSystem;

    public void Initialize(GameObject player, Vector3 velocity)
    {
        _velocity = GetComponent<Rigidbody>().velocity;
        _velocity = velocity;

        _playerHealthSystem = player.GetComponent<HPController>();
    }

    void Update()
    {
        // Aplicar gravidade à flecha
        _velocity.y += gravity * Time.deltaTime;
        GetComponent<Rigidbody>().velocity = _velocity;

        // Alinhar a flecha na direção do movimento
        transform.rotation = Quaternion.LookRotation(_velocity);
    }

    private void LateUpdate()
    {
        transform.forward = -transform.right;
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log(other.gameObject.tag);
        // GetComponent<Rigidbody>().isKinematic = true;
        
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Acertou o jogador");
            other.gameObject.GetComponent<HPController>().LoseHP(damage);
            //other.gameObject.GetComponent<Transform>().parent.GetComponentInParent<HPController>().LoseHP(damage);
        }
        
        Destroy(gameObject);
    }
}
