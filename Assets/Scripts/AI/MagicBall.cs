using UnityEngine;

public class MagicBall : MonoBehaviour
{
    [SerializeField] private int damage;
    private Transform target;
    private float speed;
    private float lifetime;
    private float elapsedTime;

    public void Initialize(Transform target, float speed, float lifetime)
    {
        this.target = target;
        this.speed = speed;
        this.lifetime = lifetime;
        elapsedTime = 0f;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        //Debug.Log(other.gameObject.tag);
        GetComponent<Rigidbody>().isKinematic = true;
        
        if (other.gameObject.CompareTag("Player"))
        {
            //Debug.Log("Acertou o jogador");
            other.gameObject.GetComponent<HPController>().LoseHP(damage);
            //other.gameObject.GetComponent<Transform>().parent.GetComponentInParent<HPController>().LoseHP(damage);
        }
        
        Destroy(gameObject);
    }
}
