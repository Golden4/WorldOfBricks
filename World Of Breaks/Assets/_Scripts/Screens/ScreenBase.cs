using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenBase<T> : ScreenBase where T : ScreenBase<T> {
    public static T Ins { get; private set; }

    public override void OnInit () {
        Ins = (T) this;
    }
    public override void OnActivate () {
        isActive = true;
    }

    public override void OnDeactivate () {
        isActive = false;
    }

    public override void OnCleanUp () { }

    public override void OnBackPressed () { }

    public void Activate () {
        ScreenController.Ins.ActivateScreen (screenType);
    }

    public void Deactivate () {
        ScreenController.Ins.DeactivateScreen (this);
    }
}

public abstract class ScreenBase : MonoBehaviour {
    public string screenType;

    [System.NonSerialized]
    public bool isActive;
    public bool disableScreenUnderneath;

    public abstract void OnInit ();
    public abstract void OnBackPressed ();
    public abstract void OnActivate ();
    public abstract void OnDeactivate ();
    public abstract void OnCleanUp ();

}