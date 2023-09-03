using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zeff.Core.Parser;
using Zeff.Extensions;

namespace Match_3
{
    public class QuestUIData : IBuildData
    {
        public string ID;
        public string Name;
        public string Description;
        public Dictionary<string, int> Collection;
        public Dictionary<string, int> Rewards;
    }
    
    public class UIQuest : BaseScreen
    {
        [SerializeField] private QuestItem questItemPrefab;
        [SerializeField] private RectTransform questItemParent;
        [SerializeField] private Button btnClose;

        protected override void Awake()
        {
            base.Awake();
            var questDaily = ZParserStatic.Instance.QuestDesign.GetAll();

            foreach (var quest in questDaily)
            {
                var questItem = Instantiate(questItemPrefab, questItemParent);
                questItem.SetData(new QuestUIData
                {
                    ID = quest.Key,
                    Name = quest.Value.Name,
                    Description = quest.Value.Description,
                    Collection = quest.Value.Collection.TryToParserDictionary(),
                    Rewards = quest.Value.Rewards.TryToParserDictionary(),
                });
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            ProfileDataService.Instance.GetQuestDailyChanged();
            btnClose.onClick.AddListener(OnClickClose);
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            btnClose.onClick.RemoveListener(OnClickClose);
        }

        private void OnClickClose()
        {
            Hide();
        }


        public void Show()
        {
            gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
