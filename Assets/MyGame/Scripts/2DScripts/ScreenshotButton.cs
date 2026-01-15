using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class ScreenshotButton : MonoBehaviour
{
    public Rect captureArea;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void DownloadPNG(byte[] data, int length, string filename);
#endif

    // Diese Methode im Button verwenden
    public void OnDownloadClicked()
    {
        StartCoroutine(Capture());
    }

    private IEnumerator Capture()
    {
        yield return new WaitForEndOfFrame();

        Texture2D tex = new Texture2D(
            (int)captureArea.width,
            (int)captureArea.height,
            TextureFormat.RGB24,
            false
        );

        tex.ReadPixels(captureArea, 0, 0);
        tex.Apply();

        byte[] png = tex.EncodeToPNG();

#if UNITY_WEBGL && !UNITY_EDITOR
        // 🌐 WebGL → Browser-Download
        DownloadPNG(png, png.Length, "bitrow.png");

#elif UNITY_EDITOR
        // 🖥️ Editor → lokal speichern
        string path = Application.dataPath + "/bitrow_editor.png";
        System.IO.File.WriteAllBytes(path, png);
        Debug.Log("Screenshot gespeichert: " + path);
#else
        // z. B. Standalone Build
        string path = Application.persistentDataPath + "/bitrow.png";
        System.IO.File.WriteAllBytes(path, png);
        Debug.Log("Screenshot gespeichert: " + path);
#endif

        Destroy(tex);
    }
}
