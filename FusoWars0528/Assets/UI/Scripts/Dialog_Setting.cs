using UnityEngine;
using UnityEngine.UI;

public class Dialog_Setting : MonoBehaviour
{
    public AudioClip soundClip;
    Toggle Toggle_FullScreen;
    Toggle Toggle_Window;
    Slider Slider_Music;
    Slider Slider_Sound;
    Button Button_OK;
    bool IsFullScreen = true;

    // Use this for initialization
    void Start ()
    {
        Toggle_FullScreen = transform.Find("Panel/Toggle_FullScreen").GetComponent<Toggle>();
        Toggle_Window = transform.Find("Panel/Toggle_Window").GetComponent<Toggle>();
        Slider_Music = transform.Find("Panel/Slider_Music").GetComponent<Slider>();
        Slider_Sound = transform.Find("Panel/Slider_Sound").GetComponent<Slider>();
        Button_OK = transform.Find("Panel/Button_Back").GetComponent<Button>();
        Button_OK = transform.Find("Panel/Button_BackMenu").GetComponent<Button>();
        Toggle_FullScreen.onValueChanged.AddListener(Toggle_FullScreen_OnValueChanged);
        Toggle_Window.onValueChanged.AddListener(Toggle_Window_OnValueChanged);
        Slider_Music.onValueChanged.AddListener(Slider_Music_OnValueChanged);
        Slider_Sound.onValueChanged.AddListener(Slider_Sound_OnValueChanged);
        Button_OK.onClick.AddListener(delegate (){this.OnClick(Button_OK.gameObject);});
        Screen.fullScreen = IsFullScreen;
    }

    void Toggle_FullScreen_OnValueChanged(bool check)
    {
        if (!IsFullScreen)
        {
            IsFullScreen = true;
            Screen.fullScreen = true;
        }
    }

    void Toggle_Window_OnValueChanged(bool check)
    {
        if (IsFullScreen)
        {
            IsFullScreen = false;
            Screen.fullScreen = false;
        }
    }

    void Slider_Music_OnValueChanged(float value)
    {
        AudioManagerEx.Instance.SetAudioVolume(value);
    }

    void Slider_Sound_OnValueChanged(float value)
    {
        AudioManagerEx.Instance.soundvolume = value;
        AudioManagerEx.Instance.PlaySound(soundClip);
    }

    public void OnClick(GameObject sender)
    {
        switch (sender.name)
        {
            case "Button_Back":
                break;
            case "Button_BackMenu":
                break;
        }
    }
}
