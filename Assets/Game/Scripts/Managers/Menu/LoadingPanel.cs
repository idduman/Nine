using Garawell.Managers.Events;
using System;
using UnityEngine;

namespace Garawell.Managers.Menu
{
    public class LoadingPanel : PanelManager
    {
        public LooneyToonsScript looneyToons;

        public override void Appear(EventArgs args)
        {
            BoolArgs boolArgs = args as BoolArgs;
            if(boolArgs != null)
            {
                looneyToons.CloseTheCircle(appearTime, boolArgs.value);
                return;
            }
            looneyToons.CloseTheCircle(appearTime, false);
        }

        public override void Disappear()
        {
            //base.Disappear();
            looneyToons.OpenTheCircle(disappearTime);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}

