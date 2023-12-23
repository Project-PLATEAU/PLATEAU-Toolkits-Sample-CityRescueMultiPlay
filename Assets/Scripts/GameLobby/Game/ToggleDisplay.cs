using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleDisplay : MonoBehaviour
{
    [SerializeField] GameObject m_UiObject;

    public void HideUi()
    {
        m_UiObject.SetActive(false);
    }
}
