using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour
{
    public void Gunslinger()
    {
        PlayerPrefs.SetString("Class", "Gunslinger");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Berserker()
    {
        PlayerPrefs.SetString("Class", "Berserker");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
