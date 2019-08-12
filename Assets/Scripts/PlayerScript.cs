using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{

    public Text textureText;
    public GameObject inventory, grass, dirt, stone, sand;
    public VoxelChunk voxelChunk;
    public InventoryManager inventoryScript;
    int currentTexture = 1;

    bool PickThisBLock(out Vector3 v, float dist)
    {
        v = new Vector3();
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, dist))
        {

            v = hit.point - hit.normal / 2;

            v.x = Mathf.Floor(v.x);
            v.y = Mathf.Floor(v.y);
            v.z = Mathf.Floor(v.z);
            DropBlock(v);
            return true;
        }
        return false;
    }

    bool PickEmptyBlock(out Vector3 v, float dist)
    {

        v = new Vector3();
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, dist))
        {
            v = hit.point + hit.normal / 2;

            v.x = Mathf.Floor(v.x);
            v.y = Mathf.Floor(v.y);
            v.z = Mathf.Floor(v.z);
            return true;
        }
        return false;
    }

    void Update()
    {
        if(Time.timeScale > 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Vector3 v;
                if (PickThisBLock(out v, 4))
                {
                    voxelChunk.SetBlock(v, 0);
                }
            }
            if (Input.GetButtonDown("Fire2"))
            {
                Vector3 v;
                if (PickEmptyBlock(out v, 4))
                {
                    voxelChunk.SetBlock(v, currentTexture);
                }
            }
            if (Input.GetKeyDown("q"))
            {
                currentTexture++;

                if (currentTexture > 4)
                {
                    currentTexture = 1;
                }

                switch (currentTexture)
                {
                    case 1:
                        textureText.text = "Current Texture: Grass";
                        break;
                    case 2:
                        textureText.text = "Current Texture: Dirt";
                        break;
                    case 3:
                        textureText.text = "Current Texture: Stone";
                        break;
                    case 4:
                        textureText.text = "Current Texture: Sand";
                        break;
                    default:
                        textureText.text = "Current Texture: Grass";
                        break;
                }
            }

            if (Input.GetKeyDown("b"))
            {
                if (inventory.activeSelf)
                    inventory.SetActive(false);
                else
                    inventory.SetActive(true);
            }
        }
    }

    void DropBlock(Vector3 v)
    {
        switch (voxelChunk.GetBlockType(v))
        {
            case 1:
                Instantiate(grass, v, Quaternion.identity);
                break;
            case 2:
                Instantiate(dirt, v, Quaternion.identity);
                break;
            case 3:
                Instantiate(stone, v, Quaternion.identity);
                break;
            case 4:
                Instantiate(sand, v, Quaternion.identity);
                break;
            default:
                Instantiate(grass, v, Quaternion.identity);
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Grass")
        {
            inventoryScript.itemAmounts[0]++;
            inventoryScript.UpdateInventory();
            Destroy(collision.gameObject);
        }

        if(collision.gameObject.tag == "Dirt")
        {
            inventoryScript.itemAmounts[1]++;
            inventoryScript.UpdateInventory();
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "Sand")
        {
            inventoryScript.itemAmounts[2]++;
            inventoryScript.UpdateInventory();
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "Stone")
        {
            inventoryScript.itemAmounts[3]++;
            inventoryScript.UpdateInventory();
            Destroy(collision.gameObject);
        }
    }
}
