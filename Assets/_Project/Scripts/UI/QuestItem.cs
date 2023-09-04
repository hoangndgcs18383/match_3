using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zeff.Extensions;

namespace Match_3
{
    public class QuestItem : MonoBehaviour,IBuildItem
    {
        [SerializeField] private TMP_Text questDescription;
        [SerializeField] private TMP_Text questProgress;
        [SerializeField] private TMP_Text claimText;
        [SerializeField] private Image progressFill;
        [SerializeField] private Button btnClaim;
        
        private QuestUIData _questData;

        private void OnEnable()
        {
            btnClaim.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            btnClaim.onClick.RemoveListener(OnClick);
        }

        public void Initialize()
        {
        }

        public void SetData(IBuildData data)
        {
            _questData = (QuestUIData) data;
            questDescription.SetText(_questData.Description);
            ProfileDataService.Instance.OnQuestDailyChanged += OnQuestDailyChanged;
        }

        private void OnDestroy()
        {
            ProfileDataService.Instance.OnQuestDailyChanged -= OnQuestDailyChanged;
        }

        private void OnQuestDailyChanged(QuestProcessData questProcessData)
        {
            if(questProcessData.ID != _questData.ID)
                return;

            switch (questProcessData.State)
            {
                case QuestState.Claimed:
                case QuestState.InProgress:
                    btnClaim.interactable = false;
                    btnClaim.targetGraphic.color = Color.gray;
                    claimText.SetText(questProcessData.State == QuestState.InProgress ?  "Reward" : "Claimed");
                    break;
                case QuestState.Completed:
                    btnClaim.interactable = true;
                    btnClaim.targetGraphic.color = Color.white;
                    claimText.SetText("Claim");
                    break;
            }

            if (questProgress != null)
            {
                questProgress.SetText($"{questProcessData.Current}/<color=#FFDF00>{questProcessData.Total}</color>");
            }

            if (progressFill != null)
            {
                progressFill.fillAmount = questProcessData.Current / (float) questProcessData.Total;
            }
        }

        public void OnClick()
        {
            ProfileDataService.Instance.ClaimQuestDaily(_questData.ID, b =>
            {
                if (b)
                {
                    btnClaim.interactable = false;
                    btnClaim.targetGraphic.color = Color.gray;
                    claimText.SetText("Claimed");
                    UIManager.Current.ShowRewardUI(_questData.Rewards);
                }
                else
                {
                    //Handle error
                    btnClaim.interactable = true;
                    btnClaim.targetGraphic.color = Color.white;
                    claimText.SetText("Claim");
                }
            });
        }
    }
}
