using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class ScreenshotButton : MonoBehaviour
{
    [Header("UI Bereich, der fotografiert werden soll")]
    public RectTransform captureTarget;

    [Tooltip("Nur nötig bei Canvas = Screen Space - Camera oder World Space. Bei Overlay leer lassen.")]
    public Camera uiCamera;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void DownloadPNG(byte[] data, int length, string filename);
#endif

    public void OnDownloadClicked()
    {
        StartCoroutine(Capture());
    }

    private IEnumerator Capture()
    {
        yield return new WaitForEndOfFrame();

        if (captureTarget == null)
        {
            Debug.LogError("captureTarget ist nicht gesetzt!");
            yield break;
        }

        Rect captureArea = GetScreenRectFromRectTransform(captureTarget, uiCamera);

        // Clamp, damit nix außerhalb des Screens liegt
        captureArea.x = Mathf.Clamp(captureArea.x, 0, Screen.width - 1);
        captureArea.y = Mathf.Clamp(captureArea.y, 0, Screen.height - 1);
        captureArea.width = Mathf.Clamp(captureArea.width, 1, Screen.width - captureArea.x);
        captureArea.height = Mathf.Clamp(captureArea.height, 1, Screen.height - captureArea.y);

        Texture2D tex = new Texture2D((int)captureArea.width, (int)captureArea.height, TextureFormat.RGBA32, false);

        tex.ReadPixels(captureArea, 0, 0);
        tex.Apply();

        byte[] png = tex.EncodeToPNG();

#if UNITY_WEBGL && !UNITY_EDITOR
        DownloadPNG(png, png.Length, "bitrow.png");
#elif UNITY_EDITOR
        string path = Application.dataPath + "/bitrow_editor.png";
        System.IO.File.WriteAllBytes(path, png);
        Debug.Log("Screenshot gespeichert: " + path);
#else
        string path = Application.persistentDataPath + "/bitrow.png";
        System.IO.File.WriteAllBytes(path, png);
        Debug.Log("Screenshot gespeichert: " + path);
#endif

        Destroy(tex);
    }

    private static Rect GetScreenRectFromRectTransform(RectTransform rt, Camera cam)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        // corners: 0 = unten links, 1 = oben links, 2 = oben rechts, 3 = unten rechts
        Vector2 bl = RectTransformUtility.WorldToScreenPoint(cam, corners[0]);
        Vector2 tl = RectTransformUtility.WorldToScreenPoint(cam, corners[1]);
        Vector2 tr = RectTransformUtility.WorldToScreenPoint(cam, corners[2]);

        float width = tr.x - bl.x;
        float height = tl.y - bl.y;

        return new Rect(bl.x, bl.y, width, height);
    }
}
