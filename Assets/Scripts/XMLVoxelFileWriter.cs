using UnityEngine;
using System.Collections;
using System.Xml;


public class XMLVoxelFileWriter
{

    public float playerPosX, playerPosY, playerPosZ;
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    //Write
    public static void SaveChunkToXMLFile(int[,,] voxelArray, string fileName)
    {
        XmlWriterSettings writerSettings = new XmlWriterSettings();
        writerSettings.Indent = true;

        XmlWriter xmlWriter = XmlWriter.Create(fileName + ".xml", writerSettings);
        xmlWriter.WriteStartDocument();

        xmlWriter.WriteStartElement("VoxelChunk");

        for (int x = 0; x < voxelArray.GetLength(0); x++)
        {
            for (int y = 0; y < voxelArray.GetLength(1); y++)
            {
                for (int z = 0; z < voxelArray.GetLength(2); z++)
                {
                    if (voxelArray[x, y, z] != 0)
                    {
                        xmlWriter.WriteStartElement("Voxel");
                        xmlWriter.WriteAttributeString("x", x.ToString());
                        xmlWriter.WriteAttributeString("y", y.ToString());
                        xmlWriter.WriteAttributeString("z", z.ToString());
                        xmlWriter.WriteString(voxelArray[x, y, z].ToString());
                        xmlWriter.WriteEndElement();
                    }
                }
            }
        }


        //xmlWriter.WriteStartElement("PlayerPos");
        //xmlWriter.WriteAttributeString("x,y,z", playerPos.ToString());
        //xmlWriter.WriteEndElement();


        xmlWriter.WriteEndElement();
        xmlWriter.WriteEndDocument();
        xmlWriter.Close();
    }


    //Read
    public static int[,,] LoadChunkFromXMLFile(int size, string fileName)
    {
        int[,,] voxelArray = new int[size, size, size];

        XmlReader xmlReader = XmlReader.Create(fileName + ".xml");
        while (xmlReader.Read())
        {
            if (xmlReader.IsStartElement("Voxel"))
            {
                int x = int.Parse(xmlReader["x"]);
                int y = int.Parse(xmlReader["y"]);
                int z = int.Parse(xmlReader["z"]);
                xmlReader.Read();
                int value = int.Parse(xmlReader.Value);
                voxelArray[x, y, z] = value;

                //if (xmlReader.IsStartElement("PlayerPos"))
                //{
                //    playerPosX = float.Parse(xmlReader.Read["("]);
                //    playerPosY = float.Parse(xmlReader.Read[","]);
                //    playerPosZ = float.Parse(xmlReader.Read[","]);
                //}
            }
        }
        xmlReader.Close();
        return voxelArray;
    }

    public static bool ReadStartAndEndPosition(out Vector3 start, out Vector3 end, string fileName)
    {
        bool foundStart = false;
        bool foundEnd = false;

        start = new Vector3(-1, -1, -1);
        end = new Vector3(-1, -1, -1);

        XmlReader xmlReader = XmlReader.Create(fileName + ".xml");

        while (xmlReader.Read())
        {
            if (xmlReader.IsStartElement("start"))
            {
                int x = int.Parse(xmlReader["x"]);
                int y = int.Parse(xmlReader["y"]);
                int z = int.Parse(xmlReader["z"]);

                start = new Vector3(x, y, z);
                foundStart = true;
            }

            if (xmlReader.IsStartElement("end"))
            {
                int x = int.Parse(xmlReader["x"]);
                int y = int.Parse(xmlReader["y"]);
                int z = int.Parse(xmlReader["z"]);

                end = new Vector3(x, y, z);
                foundEnd = true;
            }
        }
        return foundStart && foundEnd;
    }
}
