using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseOverSelect : MonoBehaviour
{      
    public void mouseHover(GameObject btn)
    {        
        if(EventSystem.current.currentSelectedGameObject!=btn)
            EventSystem.current.SetSelectedGameObject(btn);
    }
}
