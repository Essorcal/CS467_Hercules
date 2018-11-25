using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon.asset", menuName = "Attack/Weapon")]
public class Weapon : AttackSystem
{
    public GameObject weaponPreb;

    public Attack ExecuteAttack(GameObject attacker)
    {
        if (attacker == null)
            return null;

        var attackerStats = attacker.GetComponent<CharacterStats>().characterDefinition;

        var attack = CreateAttack(attackerStats);
        return attack;
    }
}