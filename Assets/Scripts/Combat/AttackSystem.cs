using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackSystem.asset", menuName = "AttackSystem/BaseAttack")]
public class AttackSystem : ScriptableObject
{
    public float Cooldown;

    public float minDamage;
    public float maxDamage;
    public float critMult;
    public float critChance;

    public Attack CreateAttack(CharacterStats_SO attackerStats)
    {
        float baseHit = attackerStats.baseDamage;
        baseHit += Random.Range(minDamage, maxDamage);

        bool isCrit = Random.value < critChance;
        if (isCrit)
            baseHit *= critMult;

        return new Attack((int)baseHit, isCrit);
    }

}
