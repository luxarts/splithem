using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBackToMain : MonoBehaviour
{
    public void BackToMain()
    {
        SceneManager.GoToMainMenu();
    }
}
