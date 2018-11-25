using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public CharacterStats_SO characterDefinition_Template;
    public CharacterStats_SO characterDefinition;
    
    public GameObject characterWeaponSlot;
     
    #region Initializations
    private void Awake()
    {
        if (characterDefinition_Template != null)
            characterDefinition = Instantiate(characterDefinition_Template);
    }

    void Start()
    {
        
    }
    #endregion

    #region Stat Increasers
    public void ApplyHealth(int hpAmt)
    {
        characterDefinition.ApplyHP(hpAmt);
    }
    
    #endregion

    #region Stat Reducers
    public void TakeDamage(int amount)
    {
        characterDefinition.TakeDamage(amount);
    }

    public void ReduceSanity(int amount)
    {
        characterDefinition.ReduceSanity(amount);
    }
    #endregion
    

    #region Reporters
    public int GetCurrentHealth()
    {
        return characterDefinition.currentHealth;
    }

    public int GetCurrentSanity()
    {
        return characterDefinition.currentSanity;
    }
    public int GetMaxHealth()
    {
        return characterDefinition.maxHealth;
    }
    
  
    public Weapon GetCurrentWeapon()
    {
        if (characterDefinition.weapon != null)
        {
            return characterDefinition.weapon.itemDefinition.weaponSlotObject;
        }
        else
        {
            return null;
        }
    }

    public int GetDamage()
    {
        return characterDefinition.currentDamage;
    }


    #endregion

    #region Initializers

    public void SetInitialHP(int hp)
    {
        characterDefinition.maxHealth = hp;
        characterDefinition.currentHealth = hp;
    }        

    public void SetInitialDamage(int damage)
    {
        characterDefinition.baseDamage = damage;
        characterDefinition.currentDamage = damage;
    }


    #endregion
}