using UnityEngine;
using UnityEngine.UI;
using Fungus;

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
        Button_OK = transform.Find("Panel/Button_OK").GetComponent<Button>();
        Toggle_FullScreen.onValueChanged.AddListener(Toggle_FullScreen_OnValueChanged);
        Toggle_Window.onValueChanged.AddListener(Toggle_Window_OnValueChanged);
        Slider_Music.onValueChanged.AddListener(Slider_Music_OnValueChanged);
        Slider_Sound.onValueChanged.AddListener(Slider_Sound_OnValueChanged);
        Button_OK.onClick.AddListener(delegate (){this.OnClick(Button_OK.gameObject);});
        Screen.fullScreen = IsFullScreen;
    }
	
	// Update is called once per frame
	void Update ()
    {
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
        //Debug.Log("Slider_Music_OnValueChanged = "+ value);
        //AudioManagerEx.Instance.SetAudioVolume(value);
        FungusManager.Instance.MusicManager.SetAudioVolume(value,0,null);
    }

    void Slider_Sound_OnValueChanged(float value)
    {
        AudioManagerEx.Instance.soundvolume = value;
        //AudioManagerEx.Instance.PlaySound(soundClip);
        FungusManager.Instance.MusicManager.PlaySound(soundClip, value);
    }

    public void OnClick(GameObject sender)
    {
        switch (sender.name)
        {
            case "Button_OK":
                UIManager.Instance.m_UIList["SettingDialog"].SetActive(false);
                UIManager.Instance.m_UIList["SubMenu"].SetActive(true);
                break;
        }
    }
}
