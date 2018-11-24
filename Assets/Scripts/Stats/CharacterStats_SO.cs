using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "stats", menuName = "Character/Stats", order = 1)]

public class CharacterStats_SO : ScriptableObject
{
    public Events.EventIntegerEvent OnLevelUp;
    public Events.EventIntegerEvent OnPlayerDMG;
    public Events.EventIntegerEvent OnPlayerGainHP;
    public Events.EventIntegerEvent OnPlayerGainSanity;
    public UnityEvent OnPlayerDeath;
    public UnityEvent OnPlayerInit;

    [System.Serializable]
    public class CharLevel
    {
        public int maxHealth;
        public int maxSanity;
        public int maxGems;
        public int baseDamage;
        public int maxInventory;
        public int requiredXP;
    }

    #region Fields
    public bool isPlayer = false;

    public LootItem weapon { get; private set; }
    public LootItem misc1 { get; private set; }
    public LootItem misc2 { get; private set; }

    public int maxHealth = 0;
    public int currentHealth = 0;

    public int maxGems = 0;
    public int currentGems = 0;

    public int maxSanity = 0;
    public int currentSanity = 0;

    public int baseDamage = 0;
    public int currentDamage = 0;

    public int maxInventory = 0;
    public int currentInventory = 0;

    public int charExperience = 0;
    public int charLevel = 0;

    public CharLevel[] charLevels;
    #endregion

    #region Stat Increasers
    public void ApplyHP(int hpAmt)
    {
        if ((currentHealth + hpAmt) > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += hpAmt;
        }

        if (isPlayer)
            OnPlayerGainHP.Invoke(hpAmt);

    }

    public void ApplySanity(int sanityAmt)
    {
        if ((currentSanity + sanityAmt) > maxSanity)
        {
            currentSanity = maxSanity;
        }
        else
        {
            currentSanity += currentSanity;
        }
    }

    public void ApplyGems(int gemAmt)
    {
        if ((currentGems + gemAmt) > maxGems)
        {
            currentGems = maxGems;
        }
        else
        {
            currentGems += gemAmt;
        }
    }

    public void GiveXP(int xp)
    {
        charExperience += xp;
        if (charLevel < charLevels.Length)
        {
            int levelTarget = charLevels[charLevel].requiredXP;

            if (charExperience >= levelTarget)
                SetCharacterLevel(charLevel);
        }
    }

    public void EquipWeapon(LootItem weaponPickUp, Inventory charInventory, GameObject weaponSlot)
    {
        GameObject newWeapon;

        weapon = weaponPickUp;
        charInventory.inventoryDisplay[2].sprite = weaponPickUp.itemDefinition.itemIcon;
        newWeapon = Instantiate(weaponPickUp.itemDefinition.weaponSlotObject.weaponPreb, weaponSlot.transform);
        currentDamage = baseDamage + weapon.itemDefinition.itemAmount;
    }

   
    #endregion

    #region Stat Reducers
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (isPlayer)
            OnPlayerDMG.Invoke(amount);

        if (currentHealth <= 0)
        {
            Death();
        }
    }

    public void ReduceSanity(int amount)
    {
        currentSanity -= amount;

        if (currentSanity < 0)
        {
            currentSanity = 0;
        }
    }

    public bool UnEquipWeapon(LootItem weaponPickUp, Inventory charInventory, GameObject weaponSlot)
    {
        bool previousWeaponSame = false;

        if (weapon != null)
        {
            if (weapon == weaponPickUp)
            {
                previousWeaponSame = true;
            }
            charInventory.inventoryDisplay[2].sprite = null;
            DestroyObject(weaponSlot.transform.GetChild(0).gameObject);
            weapon = null;
            currentDamage = baseDamage;
        }

        return previousWeaponSame;
    }

    #endregion

    #region Levels
    private void Death()
    {
        if (isPlayer)
            OnPlayerDeath.Invoke();
    }

    public void SetCharacterLevel(int newLevel)
    {
        charLevel = newLevel + 1;

        maxHealth = charLevels[newLevel].maxHealth;
        currentHealth = charLevels[newLevel].maxHealth;

        maxSanity = charLevels[newLevel].maxSanity;
        currentSanity = charLevels[newLevel].maxSanity;

        maxGems = charLevels[newLevel].maxGems;

        baseDamage = charLevels[newLevel].baseDamage;

        if (weapon == null)
            currentDamage = charLevels[newLevel].baseDamage;
        else
            currentDamage = charLevels[newLevel].baseDamage + weapon.itemDefinition.itemAmount;

       
        maxInventory = charLevels[newLevel].maxInventory;

        if (charLevel > 1)
            OnLevelUp.Invoke(charLevel);
    }
    #endregion

}