﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameMenu : MonoBehaviour {

    public GameObject menu;

    public TextMeshProUGUI hpText, sanityText, lvlText, xpText;
    public Slider xpSlider;
    public Image playerImg;

    private CharacterStats playerStats;

    public ItemButton[] itemBtns;
    public string selectedItem;
    public Item activeItem;
    public TextMeshProUGUI itemName, itemDesc, useButton;


    public static GameMenu instance;

	// Use this for initialization
	void Start () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Cancel"))
        {
            if (menu.activeInHierarchy)
            {
                menu.SetActive(false);
                //Time.timeScale = 1;
                //Cursor.visible = false;
                PlayerController.instance.canMove = true;


            }
            else
            {
                UpdateMainStats();
                menu.SetActive(true);
                //Cursor.visible = true;
                //Time.timeScale = 0;
                PlayerController.instance.canMove = false;



            }
        }

	}

    public void UpdateMainStats()
    {
        playerStats = CharacterStats.instance;
        //nameText.text = playerStats.characterDefinition.charName;
        hpText.SetText("HP: {0}/{1}", playerStats.GetCurrentHealth(), playerStats.GetMaxHealth());
        sanityText.SetText("Sanity: {0}/{1} ", playerStats.GetCurrentSanity(), playerStats.GetMaxSanity());
        lvlText.SetText("Lvl: {0}", playerStats.GetLevel());
        xpText.SetText("{0}/{1}", playerStats.GetXP(), playerStats.GetRequiredXP());
        xpSlider.maxValue = playerStats.GetRequiredXP();
        xpSlider.value = playerStats.GetXP();
        playerImg.sprite = playerStats.characterDefinition.sprite;
    }

    public void SetItemButtons()
    {
        GameController.control.SortItems();

        for (int i = 0; i < itemBtns.Length; i++)
        {
            itemBtns[i].btnVal = i;
            //Show items in inventory
            if (GameController.control.itemsInventory[i] != "") //If item at this location in inventory
            {
                itemBtns[i].btnImg.gameObject.SetActive(true);
                itemBtns[i].btnImg.sprite = GameController.control.GetItemInfo(GameController.control.itemsInventory[i]).itemSprite;
                itemBtns[i].sizeTxt.text = GameController.control.numberOfItems[i].ToString();
            } else //if no item at that location 
            {
                itemBtns[i].btnImg.gameObject.SetActive(false);
                itemBtns[i].sizeTxt.text = "";
            }
        }
        
    }

    public void SelectItem(Item item)
    {
        activeItem = item;
        if (activeItem.isItem)
        {
            useButton.text = "Use";
        }

        if (activeItem.isWeapon || activeItem.isArmor)
        {
            useButton.text = "Equip";
        }

        itemName.text = activeItem.itemName;
        itemDesc.text = activeItem.description;
    }
    public void UseItem()
    {
        activeItem.Use();
        if (activeItem.isItem)
        {
            DiscardItem();
        }
        
    }

    public void DiscardItem()
    {
        if (activeItem != null)
        {
            GameController.control.RemoveItem(activeItem.itemName);
        }
    }


}
