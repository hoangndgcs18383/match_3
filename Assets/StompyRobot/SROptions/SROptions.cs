using System.ComponentModel;
using Match_3;
using SRF.Service;
using UnityEngine;
using UnityEngine.Scripting;

public delegate void SROptionsPropertyChanged(object sender, string propertyName);

#if !DISABLE_SRDEBUGGER
[Preserve]
#endif
public partial class SROptions : INotifyPropertyChanged
{
    private static SROptions _current;

    public static SROptions Current
    {
        get { return _current; }
    }

#if !DISABLE_SRDEBUGGER
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void OnStartup()
    {
        _current = new SROptions(); // Need to reset options here so if we enter play-mode without a domain reload there will be the default set of options.
        SRServiceManager.GetService<SRDebugger.Internal.InternalOptionsRegistry>().AddOptionContainer(Current);
    }
#endif

    public event SROptionsPropertyChanged PropertyChanged;
    
#if UNITY_EDITOR
    [JetBrains.Annotations.NotifyPropertyChangedInvocator]
#endif
    public void OnPropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, propertyName);
        }

        if (InterfacePropertyChangedEventHandler != null)
        {
            InterfacePropertyChangedEventHandler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    private event PropertyChangedEventHandler InterfacePropertyChangedEventHandler;

    event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
    {
        add { InterfacePropertyChangedEventHandler += value; }
        remove { InterfacePropertyChangedEventHandler -= value; }
    }
    
    
    #region SROptions
    
    [Category("Cheats")]
    public void AddGold()
    {
        RewardManager.Current.AddCoin(10000);
    }
    
    
    [Category("Quality Settings")]
    public void SetVeryLowQuality()
    {
        QualitySettings.SetQualityLevel(0);
    }
    
    [Category("Quality Settings")]
    public void SetLowQuality()
    {
        QualitySettings.SetQualityLevel(1);
    }
    
    [Category("Quality Settings")]
    public void SetMediumQuality()
    {
        QualitySettings.SetQualityLevel(2);
    }
    
    [Category("Quality Settings")]
    public void SetHighQuality()
    {
        QualitySettings.SetQualityLevel(3);
    }
    
    [Category("Quality Settings")]
    public void SetVeryHighQuality()
    {
        QualitySettings.SetQualityLevel(4);
    }
    
    [Category("Quality Settings")]
    public void SetUltraQuality()
    {
        QualitySettings.SetQualityLevel(5);
    }
    
    [Category("Quality Settings")]
    public void SetMuteBackgroundMusic()
    {
        SoundManager.Current.StopMusicBackground();
    }
    
    [Category("Quality Settings")]
    public void SetUnmuteBackgroundMusic()
    {
        SoundManager.Current.PlayMusicBackground();
    }

    [Category("Level")]
    public void ResetLevel()
    {
        GameManager.Current.RestartLevel();
    }
    
    [Category("Level")]
    public void LoadLevel()
    {
        GameManager.Current.LoadNextLevel();
    }
    
    private int _levelReload = 0;
    
    [NumberRange(0, 50)]
    [Category("Level")]
    public int LevelReload {
        get => _levelReload;
        set => _levelReload = value;
    }
    
    [Category("Level")]
    public void ReloadLevelAt()
    {
        GameManager.Current.ReloadLevelAt(_levelReload);
    }
    
    [Category("Level")]
    public void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        GameManager.Current.RestartLevel();
    }

    #endregion
}
