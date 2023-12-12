
#if UNITY_EDITOR
using UnityEngine;

[ExecuteInEditMode]
public class HideFlag : MonoBehaviour
{
    void Awake()
    {
        gameObject.hideFlags = HideFlags.HideInHierarchy;
    }
}
#endif
