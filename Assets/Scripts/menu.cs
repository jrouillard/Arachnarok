using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class menu : MonoBehaviour
{
    // Start is called before the first frame update
    public string loadLevel;
    public GameObject mainButtons;
    public GameObject options;
    private enum MenusType {
        Main,
        Options
    }
    Resolution[] resolutions;
    public Dropdown dropdownMenu;
    public Toggle fullscreen;
    public Slider slider;
    private MenusType type = MenusType.Main;
    
    private void Start() {
        mainButtons.SetActive(false); 
        options.SetActive(false); 
        slider.value = AudioListener.volume;

    
        fullscreen.isOn  = Screen.fullScreen;
        ShowMain();
        SliderChange();
        resolutions = Screen.resolutions;
        for (int i = 0; i < resolutions.Length; i++)
        {
            dropdownMenu.options[i].text = ResToString(resolutions[i]);
            dropdownMenu.value = i;
            dropdownMenu.options.Add(new Dropdown.OptionData(dropdownMenu.options[i].text));
        }
    }
     
    string ResToString(Resolution res)
    {
        return res.width + " x " + res.height + " (" + res.refreshRate + " Hz)";
    }
    public void StartGame()
    {
        SceneManager.LoadScene(loadLevel);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void ChangeMenuType()
    {
        ChangeMenu();
    }
    public void ChangeResolution()
    {
        Screen.SetResolution(resolutions[dropdownMenu.value].width, resolutions[dropdownMenu.value].height, fullscreen.isOn);
    }
    public void SliderChange() {
        AudioListener.volume = slider.value;
    }
    private void ShowOptions() {
        mainButtons.SetActive(false);
        options.SetActive(true);
    }
    private void ShowMain() {
        mainButtons.SetActive(true);
        options.SetActive(false);
    }
    public void ToggleFullScreen() {
        Screen.fullScreen = fullscreen.isOn;
    }

    private void ChangeMenu() {
        if (type == MenusType.Main) {
            type = MenusType.Options;
            ShowOptions();
        } else {
            type = MenusType.Main;
            ShowMain();
        }
    }

}
