using UnityEngine;

public class GrenadeController : MonoBehaviour
{
    [SerializeField] private GameObject grenade;
    [SerializeField] private Transform throwRoot;
    [SerializeField] private Transform crosshairTarget;

    //
    private float timeBetThrow = 3.5f;
    private float throwTime;
    [SerializeField] private float delay = 2.0f;
    [SerializeField] private float throwPower = 15.0f;
    [SerializeField] private float throwAngle = 20.0f;

    //Remain, Max
    public int grenadeRemain { get; private set; }
    private const int maxGrenade = 3;

    //初期化
    private void Awake()
    {
        grenadeRemain = maxGrenade;
    }

    //グレネード使用が出来る状態か
    public bool isAvailable
    {
        get
        {
            return Time.time >= throwTime + timeBetThrow && grenadeRemain > 0;
        }
    }
    private GameObject grenadePref;

    public void ThrowGrenade()
    {
        throwTime = Time.time;

        Vector3 aimDirection = crosshairTarget.transform.position - throwRoot.position;
        //斜めに投げる
        // Vector3 throwDirection = Quaternion.AngleAxis(throwAngle, Vector3.right) * aimDirection.normalized;
        Vector3 throwDirection = aimDirection.normalized + new Vector3(0, throwAngle / 90.0f, 0);
        //インスタンス化したグレネードがなかったら生成
        if (!grenadePref)
        {
            grenadePref = Instantiate(grenade, throwRoot.position, Quaternion.identity);
        }
        else
        //インスタンス化したグレネードがあったら使用
        {
            grenadePref.gameObject.SetActive(true);
            grenadePref.transform.position = throwRoot.position;
            grenadePref.transform.eulerAngles = Vector3.zero;
        }
        --grenadeRemain;
        UpdateUI(grenadeRemain);

        //物理処理
        Rigidbody grenadeRb = grenadePref.GetComponent<Rigidbody>();
        grenadeRb.AddForce(throwDirection * throwPower, ForceMode.Impulse);　//
        grenadeRb.AddTorque(Vector3.back * 5.0f, ForceMode.Impulse);　//回転
        grenadePref.GetComponent<Grenade>().StartExplosion(delay);
    }

    //グレネードの数最大にする
    public void RefillGrenade()
    {
        grenadeRemain = maxGrenade;

        UpdateUI(grenadeRemain);
    }

    //グレネードの数を1個増やす
    public void AddGrenade()
    {
        if (grenadeRemain >= maxGrenade)
        {
            return;
        }
        ++grenadeRemain;

        UpdateUI(grenadeRemain);
    }

    //UI
    public void UpdateUI(int remain)
    {
        UIManager.Instance.UpdateGrenadeImage(remain);
    }
}
