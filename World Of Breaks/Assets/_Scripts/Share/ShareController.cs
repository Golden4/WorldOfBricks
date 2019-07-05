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
            
            StartCoroutine(TakeSSAndShare());
        }
    }

    static string filePathToScreenshot = "";

    public static void CaptureScreenshot()
    {
        Texture2D ss; //= new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        //ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        //ss.Apply();
        
        filePathToScreenshot = Path.Combine(Application.persistentDataPath, "shared_img.png");
        ss = ScreenCapture.CaptureScreenshotAsTexture();
        File.WriteAllBytes(filePathToScreenshot, ss.EncodeToPNG());
        // To avoid memory leaks
        Destroy(ss);
        
    }

    private IEnumerator TakeSSAndShare()
    {
        sharing = true;
        yield return new WaitForEndOfFrame();
        
        string text = UIScreen.Ins.score + " in #worldofbreaks. My personal record - " + UIScreen.Ins.topScore + ".";

        new NativeShare().AddFile(filePathToScreenshot).SetSubject("World Of Breaks").SetText(text).Share();

        // Share on WhatsApp only, if installed (Android only)
        //if( NativeShare.TargetExists( "com.whatsapp" ) )
        //	new NativeShare().AddFile( filePath ).SetText( "Hello world!" ).SetTarget( "com.whatsapp" ).Share();
        sharing = false;
    }


}
