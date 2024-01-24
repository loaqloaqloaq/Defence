using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    [SerializeField] GameObject indicatorPrefab;
    Dictionary<GameObject, GameObject> indicators=new Dictionary<GameObject, GameObject>();

    static DamageIndicator i;
    public static DamageIndicator Instance
    {
        get { 
            if(i==null) i=FindObjectOfType<DamageIndicator>();
            return i;
        }
    }
    public static void Set(GameObject attacker) {
        Instance._set(attacker);
    }
    public static void Remove(GameObject key) {
        Instance.indicators[key].SetActive(false);
        Instance.indicators.Remove(key);
    }

    void _set(GameObject attacker) {
        if (attacker == null || attacker.activeSelf == false) return;
        Debug.Log(attacker);
        if (!indicators.ContainsKey(attacker)) {
            GameObject indicator = null;
            foreach (Transform child in transform) {
                if (child.gameObject.activeSelf == false) {
                    indicator=child.gameObject; 
                    indicator.SetActive(true);
                    break;
                }
            }
            if (indicator == null)
            {
                indicator = Instantiate(indicatorPrefab);
                indicator.transform.SetParent(transform);
            }
            indicators.Add(attacker, indicator);
        }

        if (indicators.ContainsKey(attacker)) indicators[attacker].GetComponent<DamageIdicatorController>().updatePos(attacker);
        else Debug.LogWarning("indicator not found");
    }   
}
