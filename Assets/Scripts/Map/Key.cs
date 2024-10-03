using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Key : MonoBehaviour
{

    [SerializeField] public Image UIKey;

    private void Start()
    {
        HideUIKey();
        
    }
    public void UseKey()
    {
        HideUIKey();
        Destroy(gameObject);

    }
    public void GetKey() => gameObject.SetActive(false);

    public void ShowUIKey()
    {
        if (UIKey != null)
        {
            UIKey.enabled = true;  // Mostra a imagem
        }
    }

    public void HideUIKey()
    {
        if (UIKey != null)
        {
            UIKey.enabled = false;  // Esconde a imagem
        }
    }
}
