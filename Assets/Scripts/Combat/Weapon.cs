using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon.asset", menuName = "Attack/Weapon")]
public class Weapon : AttackSystem
{
    public GameObject weaponPreb;

    public void ExecuteAttack(GameObject attacker, GameObject defender)
    {
        if (defender == null)
            return;

        // at this point the attack will connect
        var attackerStats = attacker.GetComponent<CharacterStats>();
        var defenderStats = defender.GetComponent<CharacterStats>();

        var attack = CreateAttack(attackerStats, defenderStats);

        var attackables = defender.GetComponentsInChildren(typeof(IAttackable));
        foreach (IAttackable a in attackables)
        {
            a.OnAttack(attacker, attack);
        }
    }
}