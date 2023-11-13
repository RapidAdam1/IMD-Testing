using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ItemStorageScript : MonoBehaviour
{
    GameObject[] Items = new GameObject[0];

    private void Awake()
    {
        Debug.Log(Items.Length);

    }
    public void AddItem(GameObject Item) 
    {
        if (Items.Contains(Item))
            return;
        System.Array.Resize(ref Items, Items.Length+1);
        Items[Items.Length-1] = Item;
        Debug.Log(Items.Length);

    }

    public GameObject GetItem(GameObject FindItem) 
    {
        if(!Items.Contains(FindItem))
            return null;

        foreach(GameObject Item in Items) 
        {
            if(Item.name == FindItem.name)
            {
                return Item;
            }
        }
        return null;
    }

    public void RemoveItem(GameObject RemoveItem) 
    {
        if (!GetItem(RemoveItem))
            return;
        int index = Array.IndexOf(Items, RemoveItem);
        Items[index] = null;
        System.Array.Sort(Items);
        System.Array.Resize(ref Items, Items.Length - 1);
        Destroy(RemoveItem); 
        Debug.Log(Items.Length);
    }

}
