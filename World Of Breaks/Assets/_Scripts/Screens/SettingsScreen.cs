using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsScreen : ScreenBase<SettingsScreen>
{
    public override void OnActivate()
    {
        MessageBox.ShowStatic("Settings", MessageBox.BoxType.Settings).ShowSettings(); ;

        AdManager.Ins.showBanner();
    }

    public override void OnDeactivate()
    {
        AdManager.Ins.hideBanner();
    }
}
