using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Animator startButton;
    public Animator settingsButton;
    public Animator dialog;
    public Animator contentPanel;
    public Animator Gear;

    public void ToggleMenu()
    {
        bool isHidden = contentPanel.GetBool("isHidden");
        contentPanel.SetBool("isHidden", !isHidden);
        bool isStart = Gear.GetBool("isStart");
        Gear.SetBool("isStart", !isStart);
    }

    public void OpenCloseSettings(bool isOpen)
    {
        startButton.SetBool("isHidden", isOpen);
        settingsButton.SetBool("isHidden", isOpen);
        dialog.SetBool("isHidden", !isOpen);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Main");
    }
}
