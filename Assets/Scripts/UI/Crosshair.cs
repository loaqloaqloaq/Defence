using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    public Image aimPointReticle; //照準位置
    public Image hitPointReticle; //実際に当たる位置

    public float smoothTime = 0.2f;

    private Camera screenCamera; //画面上に出ているカメラ
    private RectTransform crossHairRectTransform; //クロスヘアのRectTransform 

    private Vector2 currentHitPointVelocity;
    private Vector2 targetPoint; //表示先

    private void Awake()
    {
        screenCamera = Camera.main;
        crossHairRectTransform = hitPointReticle.GetComponent<RectTransform>();
    }

    //クロスヘアの表示・非表示
    public void SetActiveCrosshair(bool active)
    {
        hitPointReticle.enabled = active;
        aimPointReticle.enabled = active;
    }
    //的のWorld Position > ScreenPosition変換
    public void UpdatePosition(Vector3 worldPoint) 
    {
        targetPoint = screenCamera.WorldToScreenPoint(worldPoint);
    }

    private void Update()
    {
        if (!hitPointReticle.enabled) return;
        //表示先に移動
        crossHairRectTransform.position = Vector2.SmoothDamp(crossHairRectTransform.position, targetPoint,
            ref currentHitPointVelocity, smoothTime);
    }
}