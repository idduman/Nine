using System;
using Garawell.Managers;
using Garawell.Managers.Game;
using Garawell.Managers;
using Garawell.Managers.Game;
using UnityEngine;

namespace EC.GameMechanics.IncrementalIdle
{
    public class UpgradePanel : ECMonoBehaviour
    {
        [SerializeField] private UpgradeItemData[] itemDatas;
        [SerializeField] private UpgradeButton[] upgradeButtons;

        public UpgradeItemData[] ItemDatas { get => itemDatas; set => itemDatas = value; }

        public void InitializeUpgrade()
        {
            InitializeItems();
            InitializeButtons();

            MainManager.Instance.EventManager.Register(EventTypes.LevelSuccess, OnLevelSuccess, true);
        }

        private void OnLevelSuccess(EventArgs arg0)
        {
            for (int i = 0; i < itemDatas.Length; i++)
            {
              //  itemDatas[i].ResetData();
            }
        }

        private void InitializeButtons()
        {
            for (int i = 0; i < upgradeButtons.Length; i++)
            {
                upgradeButtons[i].Initialize();
            }
        }

        private void InitializeItems()
        {
            for (int i = 0; i < itemDatas.Length; i++)
            {
                itemDatas[i].Initialize();
            }
        }

        public override void OnSceneLoadComplete()
        {
            InitializeUpgrade();
        }
    }
}

