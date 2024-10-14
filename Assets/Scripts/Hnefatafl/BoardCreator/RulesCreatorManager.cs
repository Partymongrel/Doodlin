using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulesCreatorManager : MonoBehaviour
{
    public static RulesCreatorManager Instance;

    private Ruleset workingRuleSet = new Ruleset();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(gameObject);
        }

    }

    private void OnEnable()
    {
        GameManager.Instance.MoveToPlaySceneEvent += SetRuleset;
    }
    private void OnDisable()
    {
        GameManager.Instance.MoveToPlaySceneEvent -= SetRuleset;
    }

    public void SetStrongKing(bool b)
    {
        workingRuleSet.strongKing = b;
    }

    private void SetRuleset()
    {
        GameManager.Instance.Ruleset = workingRuleSet;
    }

}
