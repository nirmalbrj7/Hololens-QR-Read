using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class QRPopup : MonoBehaviour
{
    public GameObject popupPrefab;
    private GameObject currentPopup;

    public void ShowPopup(string content)
    {
        if (currentPopup != null)
        {
            Destroy(currentPopup);
        }

        currentPopup = Instantiate(popupPrefab, transform.position + transform.forward, Quaternion.identity);
        currentPopup.GetComponentInChildren<TextMesh>().text = content;
    }
}