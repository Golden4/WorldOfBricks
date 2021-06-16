public class SettingsScreen : ScreenBase<SettingsScreen>
{
    public override void OnActivate()
    {
        MessageBox.ShowStatic("settings", MessageBox.BoxType.Settings).ShowSettings(); ;

        AdManager.Ins.showBanner();
    }

    public override void OnDeactivate()
    {
        AdManager.Ins.hideBanner();
    }
}
