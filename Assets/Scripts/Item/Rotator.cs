using UnityEngine;

public class Rotator : MonoBehaviour
{
    //アイテムオブジェクトの回転処理
    [SerializeField] private float rotationSpeed = 60f;
    private float y = 0;
    private void FixedUpdate()
    {
        y += rotationSpeed * Time.fixedDeltaTime;
        transform.localRotation = Quaternion.Euler(0, y, 0);
    }
}