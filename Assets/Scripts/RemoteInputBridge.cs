using UnityEngine;
using UnityEngine.UI;   // <-- important!

public class RemoteInputBridge : MonoBehaviour
{
    [Header("Buttons to trigger")]
    public Button cookieButton;    // center cookie button
    public Button sidebarButton;   // upgrades / sidebar button

    void Update()
    {
        // ENTER => click the cookie button
        if (Input.GetKeyDown(KeyCode.Return) && cookieButton != null)
        {
            cookieButton.onClick.Invoke();
        }

        // LEFT ARROW => click the sidebar/upgrade button
        if (Input.GetKeyDown(KeyCode.LeftArrow) && sidebarButton != null)
        {
            sidebarButton.onClick.Invoke();
        }
    }
}

