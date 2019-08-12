using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour {

    //Parent object inventory item
    public Transform parentPanel;

    //Item info to build inventory items
    public List<Sprite> itemSprites;
    public List<string> itemNames;
    public List<int> itemAmounts;

    //Starting template item
    public GameObject startItem;

    List<InventoryItemScript> inventoryList;

	// Use this for initialization
	void Start ()
    {
        inventoryList = new List<InventoryItemScript>();
        for (int i = 0; i < itemNames.Count; i++)
        {
            //Create a duplicate of the starter item
            GameObject inventoryItem = (GameObject)Instantiate(startItem);

            //UI items need to be parented by the canvas or an object within the canvas
            inventoryItem.transform.SetParent(parentPanel);

            //Original start item is disabled - so the duplicate must be enabled
            inventoryItem.SetActive(true);

            //Get InventoryItemScript component so we can set the data
            InventoryItemScript iis = inventoryItem.GetComponent<InventoryItemScript>();
            iis.itemSprite.sprite = itemSprites[i];
            iis.itemNameText.text = itemNames[i];
            iis.itemName = itemNames[i];
            iis.itemAmountText.text = itemAmounts[i].ToString();
            iis.itemAmount = itemAmounts[i];
            //Keep a list of the inventory items
            inventoryList.Add(iis);
        }
        DisplayListInOrder();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void DisplayListInOrder()
    {
        //Height of item plus space between each
        float yOffset = 55f;
        //Use the start position for the first item
        Vector3 startPosition = startItem.transform.position;
        foreach (InventoryItemScript iis in inventoryList)
        {
            iis.transform.position = startPosition;
            //set position of next item using offset
            startPosition.y -= yOffset;
        }
    }

    public void SelectionSortInventory()
    {
        //iterate through every item int he list except last
        for (int i = 0; i < inventoryList.Count-1; i++)
        {
            int minIndex = i;
            //iterate through unsorted potion of list
            for (int j = i; j<inventoryList.Count; j++)
            {
                if (inventoryList[j].itemAmount < inventoryList[minIndex].itemAmount)
                {
                    minIndex = j;
                }
            }
            //Swap the minimum item into position
            if (minIndex!=i)
            {
                InventoryItemScript iis = inventoryList[i];
                inventoryList[i] = inventoryList[minIndex];
                inventoryList[minIndex] = iis;
            }
        }
        //Display the list in the correct order
        DisplayListInOrder();
    }

    List<InventoryItemScript> QuickSort(List<InventoryItemScript> listIn)
    {
        if (listIn.Count <= 1)
        {
            return listIn;
        }
        //Naive pivot selection
        int pivotIndex = 0;
        //Left and right lists
        List<InventoryItemScript> leftList = new List<InventoryItemScript>();
        List<InventoryItemScript> rightList = new List<InventoryItemScript>();

        for (int i = 1; i < listIn.Count; i++)
        {
            //Compare amounts to determine list to add to
            if (listIn[i].itemAmount > listIn[pivotIndex].itemAmount)
            {
                //Greater than pivto to left list
                leftList.Add(listIn[i]);
            }
            else
            {
                //Smaller than pivot to right list
                rightList.Add(listIn[i]);
            }
        }

        //Recurse left list
        leftList = QuickSort(leftList);
        //Recurse right list
        rightList = QuickSort(rightList);

        //Concatenate lists: left + pivot + right
        leftList.Add(listIn[pivotIndex]);
        leftList.AddRange(rightList);

        return leftList;
    }

    List<InventoryItemScript> MergeSortAmountUp(List<InventoryItemScript> listIn)
    {
        if (listIn.Count > 1)
        {
            int mid = listIn.Count / 2;

                List<InventoryItemScript> leftList = new List<InventoryItemScript>();
                List<InventoryItemScript> rightList = new List<InventoryItemScript>();

            for (int i = 0; i < mid; i++)
            {
                leftList.Add(listIn[i]);
            }
            leftList = MergeSortAmountUp(leftList);

            for (int i = mid; i < listIn.Count; i++)
            {
                rightList.Add(listIn[i]);
            }
            rightList = MergeSortAmountUp(rightList);

            listIn = MergeAmountUp(leftList, rightList);
        }
        return listIn;
    }

    List<InventoryItemScript> MergeSortAmountDown(List<InventoryItemScript> listIn)
    {
        if (listIn.Count > 1)
        {
            int mid = listIn.Count / 2;

            List<InventoryItemScript> leftList = new List<InventoryItemScript>();
            List<InventoryItemScript> rightList = new List<InventoryItemScript>();

            for (int i = 0; i < mid; i++)
            {
                leftList.Add(listIn[i]);
            }
            leftList = MergeSortAmountDown(leftList);

            for (int i = mid; i < listIn.Count; i++)
            {
                rightList.Add(listIn[i]);
            }
            rightList = MergeSortAmountDown(rightList);

            listIn = MergeAmountDown(leftList, rightList);
        }
        return listIn;
    }


    List<InventoryItemScript> MergeAmountDown(List<InventoryItemScript> rightList, List<InventoryItemScript> leftList)
    {

        List<InventoryItemScript> merged = new List<InventoryItemScript>();

        int i = 0;
        int j = 0;

        while(i < leftList.Count || j < rightList.Count)
        {
            if(i < leftList.Count && j < rightList.Count)
            {
                if (leftList[i].itemAmount < rightList[j].itemAmount)
                {
                    merged.Add(rightList[j]);
                    j++;
                }
                else
                {
                    merged.Add(leftList[i]);
                    i++;
                }
            }

            else if (i < leftList.Count)
            {
                merged.Add(leftList[i]);
                i++;
            }
            else if (j < rightList.Count)
            {
                merged.Add(rightList[j]);
                j++;
            }
        }

        return merged;
    }

    List<InventoryItemScript> MergeAmountUp(List<InventoryItemScript> rightList, List<InventoryItemScript> leftList)
    {

        List<InventoryItemScript> merged = new List<InventoryItemScript>();

        int i = 0;
        int j = 0;

        while (i < leftList.Count || j < rightList.Count)
        {
            if (i < leftList.Count && j < rightList.Count)
            {
                if (leftList[i].itemAmount > rightList[j].itemAmount)
                {
                    merged.Add(rightList[j]);
                    j++;
                }
                else
                {
                    merged.Add(leftList[i]);
                    i++;
                }
            }

            else if (i < leftList.Count)
            {
                merged.Add(leftList[i]);
                i++;
            }
            else if (j < rightList.Count)
            {
                merged.Add(rightList[j]);
                j++;
            }
        }

        return merged;
    }

    List<InventoryItemScript> MergeAlphabetically(List<InventoryItemScript> rightList, List<InventoryItemScript> leftList)
    {

        List<InventoryItemScript> merged = new List<InventoryItemScript>();

        int i = 0;
        int j = 0;

        while (i < leftList.Count || j < rightList.Count)
        {
            if (i < leftList.Count && j < rightList.Count)
            {
                if (i >= rightList.Count || j < leftList.Count)
                {
                    merged.Add(rightList[j]);
                    j++;
                }
                else
                {
                    merged.Add(leftList[i]);
                    i++;
                }
            }

            else if (i < leftList.Count)
            {
                merged.Add(leftList[i]);
                i++;
            }
            else if (j < rightList.Count)
            {
                merged.Add(rightList[j]);
                j++;
            }
        }

        return merged;
    }

    public void StartMergeSortAmountDown()
    {
        inventoryList = MergeSortAmountDown(inventoryList);
        DisplayListInOrder();
    }

    public void StartMergeSortAmountUp()
    {
        inventoryList = MergeSortAmountUp(inventoryList);
        DisplayListInOrder();
    }

    public void StartQuickSort()
    {
        inventoryList = QuickSort(inventoryList);
        DisplayListInOrder();
    }

    public void UpdateInventory()
    {
        for (int i = 0; i < itemNames.Count; i++)
        {
            inventoryList[i].itemAmountText.text = itemAmounts[i].ToString();
            inventoryList[i].itemAmount = itemAmounts[i];
        }
        DisplayListInOrder();
    }
}
