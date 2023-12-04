using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class NotificationConnect : MonoBehaviour
{
    [Header("UI References :")]
    [SerializeField] GameObject uiCanvas;
    [SerializeField] Button uiLaterButton;
    [SerializeField] Button uiUpdateButton;
    [SerializeField] TextMeshProUGUI uiDescriptionText;
    [Space(20f)]
    [Header("Get Json :")]
    [SerializeField][TextArea(1, 5)] 
    string jsonDataUrl = "https://raw.githubusercontent.com/gilbraaaan/Upgrade-Popup-Notification-with-Json/main/CheckVersion.json";
    GameData latestGameData;

    void Start()
    {
        StopAllCoroutines();
        StartCoroutine(CheckForUpdates());
    }

    IEnumerator CheckForUpdates()
    {
        UnityWebRequest request = UnityWebRequest.Get(jsonDataUrl);
        request.chunkedTransfer = false;
        request.disposeDownloadHandlerOnDispose = true;
        request.timeout = 60;
        yield return request.Send();
        if (request.isDone)
        {
            if (!request.isNetworkError || !request.isHttpError)
            {
                latestGameData = JsonUtility.FromJson<GameData>(request.downloadHandler.text);
                if (!string.IsNullOrEmpty(latestGameData.Version) &&
                    !Application.version.Equals(latestGameData.Version))
                {
                    uiDescriptionText.text = latestGameData.Description;
                    ShowPopUp();
                }
            }
        }
        request.Dispose();
    }

    void ShowPopUp()
    {
        uiLaterButton.onClick.AddListener(() =>
        {
            HidePopUp();
        });
        uiUpdateButton.onClick.AddListener(() =>
        {
            Application.OpenURL(latestGameData.Url);
            HidePopUp();
        });
        uiCanvas.SetActive(true);
    }
    void HidePopUp()
    {
        uiCanvas.SetActive(false);
        uiLaterButton.onClick.RemoveAllListeners();
        uiUpdateButton.onClick.RemoveAllListeners();
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}

[System.Serializable]
public class GameData
{
    public string Description;
    public string Version;
    public string Url;
}
