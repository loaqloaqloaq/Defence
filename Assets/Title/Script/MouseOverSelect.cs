using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseOverSelect : MonoBehaviour
{
    [SerializeField]
    private Selectable selectable = null;    
    public void mouseHover(GameObject btn)
    {        
        if(EventSystem.current.currentSelectedGameObject!=btn)
            EventSystem.current.SetSelectedGameObject(btn);
    }
}
