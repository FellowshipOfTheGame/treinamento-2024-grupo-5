using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] public GameObject victoryUI;
    [SerializeField] public GameObject gameOverUI;
    private FirstPerson cameraController; 

    private void Start()
    {
        Time.timeScale = 1;

        // Encontra o objeto com a tag "MainCamera" e o componente FirstPerson
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCamera != null)
        {
            cameraController = mainCamera.GetComponent<FirstPerson>();
            // Volta o movimento da câmera
            cameraController.ResumeCameraMovement();
        }

        HideCursor();
     
        gameOverUI.SetActive(false);
        victoryUI.SetActive(false);

    }
    
    public void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public void GameOver()
    {
        gameOverUI.SetActive(true);
        ShowCursor();
        
        if (cameraController != null)
        {
            cameraController.StopCameraMovement();
        }

        GetComponent<SoundEffectsController>().PlayGameOverSound();

        Time.timeScale = 0;
    }

    public void Victory()
    {
        victoryUI.SetActive(true);
        ShowCursor();

        if (cameraController != null)
        {
            cameraController.StopCameraMovement();
        }

        if(TryGetComponent<SoundEffectsController>(out SoundEffectsController soundEffectsController))
            soundEffectsController.PlayVictorySound();

        Time.timeScale = 0;
    }
}
