using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ShareController : MonoBehaviour
{
    bool sharing;
    public void ShareClick()
    {
        if (!sharing)
        {
            sharing = true;
            StartCoroutine(TakeSSAndShare());
        }
    }

    private IEnumerator TakeSSAndShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        // To avoid memory leaks
        Destroy(ss);

        string text = UIScreen.Ins.score + "в #worldofbreaks. My personal record - " + UIScreen.Ins.topScore + ".";

        new NativeShare().AddFile(filePath).SetSubject("World Of Breaks").SetText("My new Record").Share();

        // Share on WhatsApp only, if installed (Android only)
        //if( NativeShare.TargetExists( "com.whatsapp" ) )
        //	new NativeShare().AddFile( filePath ).SetText( "Hello world!" ).SetTarget( "com.whatsapp" ).Share();
    }
}
