using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinder : MonoBehaviour
{

    public VoxelChunk voxelChunk;
    GameObject cube;
    bool traversing = false;
    float lerpTime;

    Vector3 startPosition = new Vector3(8, 4, 14);
    Vector3 endPosition = new Vector3(11, 4, 2);
    Vector3 offset = new Vector3(0.5f, 0.5f, 0.5f);



    void Update()
    {
        if (!traversing)
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                Stack<Vector3> path = BreadthFirstSearch(startPosition, endPosition, voxelChunk);

                if (path.Count > 0)
                {
                    StartCoroutine(LerpAlongPath(path));
                }
            }


            if (Input.GetKeyDown(KeyCode.L))
            {
                Stack<Vector3> path = Djikstras(startPosition, endPosition, voxelChunk);

                if (path.Count > 0)
                {
                    StartCoroutine(LerpAlongPath(path));
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Vector3 s, e;
            if (XMLVoxelFileWriter.ReadStartAndEndPosition(out s, out e, "AssessmentChunk1"))
            {
                startPosition = s;
                print("Start = " + startPosition);
                endPosition = e;
                print("End = " + endPosition);
            }
        }
    }


    public void StartEverything()
    {
        Time.timeScale = 1f; //make sure this is 1!
        if (!traversing)
        {

            Stack<Vector3> path = BreadthFirstSearch(startPosition, endPosition, voxelChunk);

            if (path.Count > 0)
            {
                StartCoroutine(LerpAlongPath(path));
            }
        }

        Vector3 s, e;
        if (XMLVoxelFileWriter.ReadStartAndEndPosition(out s, out e, "AssessmentChunk1"))
        {
            startPosition = s;
            print("Start = " + startPosition);
            endPosition = e;
            print("End = " + endPosition);
        }

        Stack<Vector3> path2 = Djikstras(startPosition, endPosition, voxelChunk);

        if (path2.Count > 0)
        {
            StartCoroutine(LerpAlongPath(path2));
        }
    }

    Stack<Vector3> BreadthFirstSearch(Vector3 start, Vector3 end, VoxelChunk vc)
    {
        Stack<Vector3> waypoints = new Stack<Vector3>();

        Dictionary<Vector3, Vector3> visitedParent = new Dictionary<Vector3, Vector3>();
        Queue<Vector3> q = new Queue<Vector3>();
        bool found = false;
        Vector3 current = start;

        q.Enqueue(start);

        while (q.Count > 0 && !found)
        {
            current = q.Dequeue();
            if (current != end)
            {
                List<Vector3> neighbourList = new List<Vector3>();
                neighbourList.Add(current + new Vector3(1, 0, 0));
                neighbourList.Add(current + new Vector3(-1, 0, 0));
                neighbourList.Add(current + new Vector3(0, 0, 1));
                neighbourList.Add(current + new Vector3(0, 0, -1));

                foreach (Vector3 n in neighbourList)
                {
                    if ((n.x >= 0 && n.x < vc.GetChunkSize()) && n.z >= 0 && n.z < vc.GetChunkSize())
                    {
                        if (vc.IsTraversable(n))
                        {
                            if (!visitedParent.ContainsKey(n))
                            {
                                visitedParent[n] = current;
                                q.Enqueue(n);
                            }
                        }
                    }
                }
            }
            else
            {
                found = true;
            }
        }

        if (found)
        {
            while (current != start)
            {
                waypoints.Push(current + offset);
                current = visitedParent[current];
            }
            waypoints.Push(start + offset);
        }
        return waypoints;
    }

    IEnumerator LerpAlongPath(Stack<Vector3> path)
    {
        traversing = true;
        //LERPTIME SHALL BE CHANGED UPON BLOCK UNDER WE ARE 
        //lerpTime = 1.0f;

        if (cube != null)
        {
            DestroyObject(cube);
        }
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Vector3 current = path.Pop();
        cube.transform.position = current;

        while (path.Count > 0)
        {
            Vector3 target = path.Pop();
            float currentTime = 0.0f;

            if (voxelChunk.isBelowStone(current))
            {
                lerpTime = 1.0f;
            }

            if (voxelChunk.isBelowDirt(current))
            {
                lerpTime = 3.0f;
            }

            while (currentTime < lerpTime)
            {
                currentTime += Time.deltaTime;
                cube.transform.position = Vector3.Lerp(current, target, currentTime / lerpTime);
                yield return 0;
            }

            cube.transform.position = target;
            current = target;
        }

        traversing = false;
    }

    Stack<Vector3> Djikstras(Vector3 start, Vector3 end, VoxelChunk vc)
    {
        Stack<Vector3> waypoint = new Stack<Vector3>();

        Dictionary<Vector3, int> d = new Dictionary<Vector3, int>();
        Dictionary<Vector3, Vector3> p = new Dictionary<Vector3, Vector3>();
        bool found = false;
        int newDist = 0;
        Vector3 current = new Vector3(0, 0, 0);

        d[start] = 0;

        List<Vector3> l = new List<Vector3>();

        for (int x = 0; x < vc.terrainArray.GetLength(0); x++)
        {
            for (int y = 1; y < vc.terrainArray.GetLength(1); y++)
            {
                for (int z = 0; z < vc.terrainArray.GetLength(2); z++)
                {
                    Vector3 n = new Vector3(x, y, z);
                    if (vc.IsTraversable(n))
                        l.Add(n);
                }
            }
        }
        while (l.Count > 0 && !found)
        {
            int smallestDistance = int.MaxValue;

            foreach (Vector3 n in l)
            {
                if (d.ContainsKey(n))
                {
                    if (d[n] < smallestDistance)
                    {
                        smallestDistance = d[n];
                        current = n;
                    }
                }
            }
            l.Remove(current);

            if (current != end)
            {
                List<Vector3> neighbourList = new List<Vector3>();
                neighbourList.Add(current + new Vector3(1, 0, 0));
                neighbourList.Add(current + new Vector3(-1, 0, 0));
                neighbourList.Add(current + new Vector3(0, 0, 1));
                neighbourList.Add(current + new Vector3(0, 0, -1));

                foreach (Vector3 n in neighbourList)
                {
                    if (l.Contains(n))
                    {
                        if (vc.terrainArray[(int)n.x, (int)n.y - 1, (int)n.z] == 3)
                        {
                            newDist = 1;
                        }
                        if (vc.terrainArray[(int)n.x, (int)n.y - 1, (int)n.z] == 2)
                        {
                            newDist = 3;
                        }
                        if (!d.ContainsKey(n) || newDist < d[n])
                        {
                            d[n] = newDist;
                            p[n] = current;
                        }
                    }
                }
            }
            else
                found = true;
        }
        if (found)
        {
            while (current != start)
            {
                waypoint.Push(current + offset);
                current = p[current];
            }
            waypoint.Push(current + offset);
        }
        return waypoint;
    }
}