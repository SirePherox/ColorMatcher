using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class SettingsPanelController : MonoBehaviour
{
    [Header("References")]
    private Animator animator;

    [Header("Button References")]
    [SerializeField] private Button musicMutedController;
    [SerializeField] private Button soundEffectsMutedController;
    [SerializeField] private Button resetGameProgress;

    [Header("Sprite References")]
    [SerializeField] private GameObject musicMutedIcon;
    [SerializeField] private GameObject soundEffectsMutedIcon;

    [SerializeField]
    private bool isMusicMuted;
    [SerializeField]
    private bool isSoundEffectMuted;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        UpdateUIOnStart();

        //add listeners
        musicMutedController.onClick.AddListener(UpdateMusicMutedStatus);
        soundEffectsMutedController.onClick.AddListener(UpdateSoundEffectMutedStatus);
        resetGameProgress.onClick.AddListener(ResetGameProgress);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        isMusicMuted = SoundController.Instance.isBGMusicMuted;
        isSoundEffectMuted = SoundController.Instance.isSoundEffectsMuted;
    }

    public void OpenPanel()
    {
        SoundController.Instance.PlayButtonClick();
        animator.SetTrigger("Open");
    }

    public void ClosePanel()
    {
        SoundController.Instance.PlayButtonClick();
        animator.SetTrigger("Close");
    }

    private  void UpdateUIOnStart()
    {
        musicMutedIcon.SetActive(isMusicMuted);
        soundEffectsMutedIcon.SetActive(isSoundEffectMuted);
    }

    private void UpdateMusicMutedStatus()
    {
        isMusicMuted = !isMusicMuted;
        musicMutedIcon.SetActive(isMusicMuted);
    }

    private void UpdateSoundEffectMutedStatus()
    {
        isSoundEffectMuted = !isSoundEffectMuted;
        soundEffectsMutedIcon.SetActive(isSoundEffectMuted);
    }

    private void ResetGameProgress()
    {
        if (PlayerPrefs.HasKey(GamePrefabsNames.CURRENT_LEVEL))
        {
            PlayerPrefs.SetInt(GamePrefabsNames.CURRENT_LEVEL, 1);
        }
        if (PlayerPrefs.HasKey(GamePrefabsNames.HIGHSCORE))
        {
            PlayerPrefs.SetInt(GamePrefabsNames.HIGHSCORE, 0);
        }

        //reload scene to show changes
        SceneLoader.Instance.ReloadCurrentScenee();
    }
}
