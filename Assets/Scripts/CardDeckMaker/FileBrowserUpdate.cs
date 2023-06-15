using AnotherFileBrowser.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FileBrowserUpdate : MonoBehaviour
{
    public CardMakerUI cardMakerUI;

    public void OpenFileBrowser()
    {
        var bp = new BrowserProperties();
        bp.filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
        bp.filterIndex = 0;

        new FileBrowser().OpenFileBrowser(bp, path =>
        {
            cardMakerUI.imagePath = path;

            //Load image from local path with UWR
            StartCoroutine(LoadImage(path, cardMakerUI.cardArt));
        });
    }

    public IEnumerator LoadImage(string path, RawImage image)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(path))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                var uwrTexture = DownloadHandlerTexture.GetContent(uwr);

                //calculate art size
                float artWidth = image.rectTransform.rect.width;
                float scale = artWidth / uwrTexture.width;

                //change size of cardArt
                image.rectTransform.sizeDelta = new Vector2(artWidth, uwrTexture.height * scale);
                //set texture
                image.texture = uwrTexture;
            }
        }
    }
}
