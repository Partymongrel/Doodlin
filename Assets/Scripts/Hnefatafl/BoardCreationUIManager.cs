using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCreationUIManager : MonoBehaviour
{

    public static BoardCreationUIManager Instance;

    public GameObject additionalRulesPanel;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void ShowAdditionalRulesPanel()
    {
        additionalRulesPanel.SetActive(true);
    }

    public void HideAdditionalRulesPanel()
    {
        additionalRulesPanel.SetActive(false);
    }

}
