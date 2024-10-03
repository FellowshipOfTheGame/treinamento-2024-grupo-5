using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public string nextLevelSceneName;
    public Key key;

    public bool IsDoorKey(Key key) => this.key == key ? true : false;

    public void GoToNextLevel() => UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
}
