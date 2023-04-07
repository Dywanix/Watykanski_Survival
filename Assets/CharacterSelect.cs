using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour
{
    public GameObject[] classInfo, gunInfo;
    public int gunsPerClass;
    int selectedClass, selectedGun;

    public void SelectClass(int which)
    {
        classInfo[selectedClass].SetActive(false);
        selectedClass = which;
        classInfo[selectedClass].SetActive(true);
    }

    public void SelectGun(int which)
    {
        for (int i = 0; i < gunInfo.Length; i++)
        {
            gunInfo[i].SetActive(false);
        }
        selectedGun = which;
        gunInfo[selectedGun + selectedClass * gunsPerClass].SetActive(true);
    }

    public void Proceed()
    {
        PlayerPrefs.SetInt("Class", selectedClass);
        PlayerPrefs.SetInt("Gun", selectedGun);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
