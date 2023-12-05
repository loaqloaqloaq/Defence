using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbnormalityType { 
    STOP,FIRE
}
public interface Abnormality
{
    void AddAbnormality(AbnormalityType at) ;

    void RemoveAbnormality(AbnormalityType at);

    int[] GetAbnormality();
}
