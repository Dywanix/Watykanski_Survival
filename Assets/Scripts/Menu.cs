using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject PlayersHud;
    int selectedClass;

    public void Play()
    {
        PlayersHud.SetActive(true);
    }

    public void ChooseClass(int which)
    {
        selectedClass = which;
        PlayerPrefs.SetInt("Class", selectedClass);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
