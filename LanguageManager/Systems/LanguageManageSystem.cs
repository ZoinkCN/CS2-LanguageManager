using Game;
using Game.SceneFlow;
using Game.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageManager.Systems
{
    public class LanguageManageSystem : UISystemBase
    {
        protected override void OnUpdate()
        {
            base.OnUpdate();

            GameManager.instance.localizationManager.activeLocaleId;
        }
    }
}
