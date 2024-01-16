using UnityEngine;
using UnityEngine.UI;

public class ImageSwap : MonoBehaviour
{
    [SerializeField] Sprite[] sprite;

    private Image image;

    private int imageIndex;

    private float alpha;

    private void Awake()
    {
         image = GetComponent<Image>();
        imageIndex = 0;

        SetImage(imageIndex);
    }

    private void SetImage(int index)
    {
        if (index < 0 || index >= sprite.Length) return;

        image.sprite = sprite[index];
    }

    public void OnClickRightButton()
    { 
        imageIndex += 1;
        imageIndex %= sprite.Length;
        SetImage(imageIndex);
    }

    public void OnClickLeftButton()
    {
        imageIndex -= 1;
        if (imageIndex < 0) imageIndex += sprite.Length - 1;
        SetImage(imageIndex);
    }

}
