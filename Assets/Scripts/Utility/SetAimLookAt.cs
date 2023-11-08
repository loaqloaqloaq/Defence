using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SetAimLookAt : MonoBehaviour
{
    private RigBuilder rigBuilder;
    private MultiAimConstraint[] aimConstarint;


    private void Awake()
    {
        rigBuilder = GetComponentInParent<RigBuilder>();
        aimConstarint = GetComponentsInChildren<MultiAimConstraint>();

        if (rigBuilder == null || aimConstarint == null)
        {
            Debug.Log("Null Reference");
            return;
        }

        var aimTarget = GameObject.FindWithTag("Aim").transform;

        foreach (MultiAimConstraint component in GetComponentsInChildren<MultiAimConstraint>())
        {
            var data = component.data.sourceObjects;
            data.SetTransform(0, aimTarget);
            component.data.sourceObjects = data;
        }

        rigBuilder.Build();
    }

}
