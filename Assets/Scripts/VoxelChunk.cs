using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;

public class VoxelChunk : MonoBehaviour
{

    public GameObject player;
    public InputField inputField;
    public GameObject loadSaveGO;
    public VoxelGenerator voxelGenerator;
    public int[,,] terrainArray;
    int chunkSize = 16;
    NetworkView networkView;

    public delegate void EventBlockChangedWithType(int blockType);
    public static event EventBlockChangedWithType OnEventBlockChanged;


    void Start()
    {
        voxelGenerator = GetComponent<VoxelGenerator>();
        terrainArray = new int[chunkSize, chunkSize, chunkSize];
        voxelGenerator.Initialise();

        networkView = GetComponent<NetworkView>();

        if (Application.loadedLevelName == "Scene3")
        {
            if (!networkView.isMine)
            {
                networkView.RPC("RequestTerrain", networkView.owner, Network.player);
            }
            else
            {
                InitialiseTerrain();
                CreateTerrain();
                voxelGenerator.UpdateMesh();
            }
        }
        else
        {
            InitialiseTerrain();
            CreateTerrain();
            voxelGenerator.UpdateMesh();
        }
    }


    void InitialiseTerrain()
    {
        // iterate horizontally on width
        for (int x = 0; x < terrainArray.GetLength(0); x++)
        {
            // iterate vertically
            for (int y = 0; y < terrainArray.GetLength(1); y++)
            {
                // iterate per voxel horizontally on depth
                for (int z = 0; z < terrainArray.GetLength(2); z++)
                {
                    // if we are operating on 4th layer
                    if (y == 3)
                    {
                        terrainArray[x, y, z] = 1;
                    }
                    //else if the the layer is below the fourth
                    else if (y < 3)
                    {
                        terrainArray[x, y, z] = 2;
                    }
                }
            }
        }
    }


    void CreateTerrain()
    {
        for (int x = 0; x < terrainArray.GetLength(0); x++)
        {
            for (int y = 0; y < terrainArray.GetLength(1); y++)
            {
                for (int z = 0; z < terrainArray.GetLength(2); z++)
                {
                    if (terrainArray[x, y, z] != 0)
                    {
                        string tex;

                        switch (terrainArray[x, y, z])
                        {
                            case 1:
                                tex = "Grass";
                                break;
                            case 2:
                                tex = "Dirt";
                                break;
                            case 3:
                                tex = "Stone";
                                break;
                            case 4:
                                tex = "Sand";
                                break;
                            default:
                                tex = "Grass";
                                break;
                        }

                        if (x == 0 || terrainArray[x - 1, y, z] == 0)
                        {
                            voxelGenerator.CreateNegativeXFace(x, y, z, tex);
                        }

                        if (x == terrainArray.GetLength(0) - 1 || terrainArray[x + 1, y, z] == 0)
                        {
                            voxelGenerator.CreatePositiveXFace(x, y, z, tex);
                        }

                        if (y == 0 || terrainArray[x, y - 1, z] == 0)
                        {
                            voxelGenerator.CreateNegativeYFace(x, y, z, tex);
                        }

                        if (y == terrainArray.GetLength(1) - 1 || terrainArray[x, y + 1, z] == 0)
                        {
                            voxelGenerator.CreatePositiveYFace(x, y, z, tex);
                        }

                        if (z == 0 || terrainArray[x, y, z - 1] == 0)
                        {
                            voxelGenerator.CreateNegativeZFace(x, y, z, tex);
                        }

                        if (z == terrainArray.GetLength(2) - 1 || terrainArray[x, y, z + 1] == 0)
                        {
                            voxelGenerator.CreatePositiveZFace(x, y, z, tex);
                        }
                    }
                }
            }
        }
    }


    public void SetBlock(Vector3 index, int blockType)
    {

        if ((index.x > 0 &&
            index.x < terrainArray.GetLength(0)) &&
            (index.y > 0 &&
            index.y < terrainArray.GetLength(1)) &&
            (index.z > 0 && index.z < terrainArray.GetLength(2)))
        {
            voxelGenerator.ClearPreviousData();
            terrainArray[(int)index.x, (int)index.y, (int)index.z] = blockType;

            CreateTerrain();

            voxelGenerator.UpdateMesh();

            OnEventBlockChanged(blockType);
        }
    }

    [RPC] public void SetBlockOnline(Vector3 index, int blockType)
    {

        if ((index.x > 0 &&
            index.x < terrainArray.GetLength(0)) &&
            (index.y > 0 &&
            index.y < terrainArray.GetLength(1)) &&
            (index.z > 0 && index.z < terrainArray.GetLength(2)))
        {
            voxelGenerator.ClearPreviousData();
            terrainArray[(int)index.x, (int)index.y, (int)index.z] = blockType;

            CreateTerrain();

            voxelGenerator.UpdateMesh();
        }
    }

    public int GetBlockType(Vector3 v)
    {
        return terrainArray[(int)v.x, (int)v.y, (int)v.z];
    }

    public bool isBelowStone(Vector3 v)
    {
        if (terrainArray[(int)v.x, (int)v.y - 1, (int)v.z] == 3)
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    public bool isBelowDirt(Vector3 v)
    {
        if (terrainArray[(int)v.x, (int)v.y - 1, (int)v.z] == 2)
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    public bool IsTraversable(Vector3 voxel)
    {
        bool isEmpty = terrainArray[(int)voxel.x, (int)voxel.y, (int)voxel.z] == 0;
        bool isBelowStone = terrainArray[(int)voxel.x, (int)voxel.y - 1, (int)voxel.z] == 3;
        bool isBelowDirt = terrainArray[(int)voxel.x, (int)voxel.y - 1, (int)voxel.z] == 2;
        return isEmpty && (isBelowStone || isBelowDirt);
    }

    public int GetChunkSize()
    {
        return chunkSize;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.F2))
        {
            if (loadSaveGO.activeSelf)
                loadSaveGO.SetActive(false);
            else
                loadSaveGO.SetActive(true);
        }
    }

    public void SaveChunkToXMLFile()
    {
        if (loadSaveGO.activeSelf)
        {
            XMLVoxelFileWriter.SaveChunkToXMLFile(terrainArray, inputField.text); //VoxelChunk
        }
        else
        {
            Debug.Log("Input field inactive");
        }
    }

    public void LoadChunkFromXMLFile()
    {
        if(loadSaveGO.activeSelf)
        {
            terrainArray = XMLVoxelFileWriter.LoadChunkFromXMLFile(16, inputField.text);
            CreateTerrain();
            voxelGenerator.UpdateMesh();
        }
        else
        {
            Debug.Log("Input field inactive");
        }
    }

    public void LoadPathfindingChunk()
    {
        terrainArray = XMLVoxelFileWriter.LoadChunkFromXMLFile(16, "AssessmentChunk1");
        CreateTerrain();
        voxelGenerator.UpdateMesh();
    }

    byte[] SerialiseTerrainToByteArray()
    {
        StringBuilder sb = new StringBuilder();
        for (int x = 0; x < terrainArray.GetLength(0);x++)
        {
            for(int y= 0; y < terrainArray.GetLength(1); y++)
            {
                for (int z = 0; z < terrainArray.GetLength(2);z++)
                {
                    if(terrainArray[x,y,z] != 0)
                    {
                        sb.Append(x + " " + y + " " + z + " " + terrainArray[x, y, z] + " ");
                    }
                }
            }
        }
        byte[] byteArray = Encoding.UTF8.GetBytes(sb.ToString());
        return byteArray;
    }

    void DeserialiseByteArrayToTerrain(byte[] byteArray)
    {
        terrainArray = new int[chunkSize, chunkSize, chunkSize];
        string terrainString = Encoding.UTF8.GetString(byteArray);
        string[] values = terrainString.Split(' ');
        print(values.Length);
        for (int i = 0; i < values.Length - 4; i += 4)
        {
            int x = int.Parse(values[i]);
            int y = int.Parse(values[i + 1]);
            int z = int.Parse(values[i + 2]);
            int v = int.Parse(values[i + 3]);
            terrainArray[x, y, z] = v;
        }
        CreateTerrain();
        voxelGenerator.UpdateMesh();
    }

    [RPC] void SendTerrainToClient(byte[] b)
    {
        DeserialiseByteArrayToTerrain(b);
    }

    [RPC] void RequestTerrain(NetworkPlayer client)
    {
        byte[] terrain = SerialiseTerrainToByteArray();
        networkView.RPC("SendTerrainToClient", client, terrain);
    }
}
