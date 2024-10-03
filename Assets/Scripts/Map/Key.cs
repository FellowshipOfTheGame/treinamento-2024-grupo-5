using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public void UseKey() => Destroy(gameObject);
    public void GetKey() => gameObject.SetActive(false);
}
