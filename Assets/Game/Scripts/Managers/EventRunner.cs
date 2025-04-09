using Sirenix.OdinInspector;
using Garawell.Managers.Currencies;
using UnityEngine;

namespace Garawell.Managers.Events
{
    public class EventRunner
    {
        [Button("LevelStart")] 
        public static void LevelStart()
        {
            MainManager.Instance.EventManager.InvokeEvent(EventTypes.LevelStart, new IntArgs(PlayerPrefs.GetInt("Level")));
        }

        public static void LevelFail()
        {
            MainManager.Instance.EventManager.InvokeEvent(EventTypes.LevelFail);
        }

        public static void LevelSuccess()
        {
            MainManager.Instance.EventManager.InvokeEvent(EventTypes.LevelSuccess);
        }

        //Level Restart is a good event to Unregister from your custom events
        public static void LevelRestart()
        {
            MainManager.Instance.EventManager.InvokeEvent(EventTypes.LevelRestart);
        }

        //Level Finish is a good event to Unregister from your custom events
        public static void LevelFinish()
        {
            MainManager.Instance.EventManager.InvokeEvent(EventTypes.LevelFinish);
        }

        public static void ChangeVibMode(bool vibModeOn)
        {
            MainManager.Instance.EventManager.InvokeEvent(EventTypes.VibrationChange, new BoolArgs(vibModeOn));
        }

        public static void EarnCurrency(int currencyId, int amount, bool addIncrementally)
        {
            MainManager.Instance.EventManager.InvokeEvent(EventTypes.CurrencyEarned, new CurrencyArgs(currencyId, amount, addIncrementally));
        }

        public static bool SpendCurrency(int currencyId, int amount, bool incremental)
        {
            Currency currency = MainManager.Instance.CurrencyManager.currencies[currencyId];
            if(amount > currency.currencyAmount)
            {
                return false;
            }
            MainManager.Instance.EventManager.InvokeEvent(EventTypes.CurrencySpent, new CurrencyArgs(currencyId, amount, incremental));
            return true;
        }

        public static void LoadSceneStart(bool levelFinish = false)
        {
            MainManager.Instance.EventManager.InvokeEvent(EventTypes.LoadSceneStart, new BoolArgs(levelFinish));
        }

        public static void LoadSceneFinish()
        {
            MainManager.Instance.EventManager.InvokeEvent(EventTypes.LoadSceneFinish);
        }

        public static void Stationary(Vector3Args args)
        {
            MainManager.Instance.EventManager.RunOnStationary(args);
        }

        public static void HoldStart(Vector3Args args)
        {
            MainManager.Instance.EventManager.InvokeEvent(EventTypes.OnHoldStart, args);
        }

        public static void HoldFinish(Vector3Args args)
        {
            MainManager.Instance.EventManager.InvokeEvent(EventTypes.OnHoldFinish, args);
        }

        public static void MoveCompleted()
        {
            MainManager.Instance.EventManager.InvokeEvent(EventTypes.OnMoveCompleted);
        }

        public static void PopupTutorial(int stage)
        {
            MainManager.Instance.EventManager.InvokeEvent(
                EventTypes.PopupTutorial, new IntArgs(stage));
        }


        public static void RemoteDataLoaded(bool loaded)
        {
            
        }

        public static void Revive()
        {
            
        }

        public static void RvStart(RvArgs rvArgs)
        {

        }
    }
}

