using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin_Animation_Trigger : MonoBehaviour
{
    private Enemy_Goblin enemy => GetComponentInParent<Enemy_Goblin>();

    private void AnimationTrigger()
    {
        enemy.AnimationTrigger();
    }

}
