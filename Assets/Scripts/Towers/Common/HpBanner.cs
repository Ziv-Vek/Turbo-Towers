using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpBanner : MonoBehaviour
{
    [SerializeField] private TMP_Text hpText;

    public void SetCurrentHealth(int currentHealth)
    {
        hpText.text = Mathf.Max(0, currentHealth).ToString();
    }
}
