using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class pathGeneratorScript : MonoBehaviour
{
    // Number associated to which side of hex
    //    6   1
    //   5     2
    //    4   3
    const int side1Blocked = 0b000001;
    const int side2Blocked = 0b000010;
    const int side3Blocked = 0b000100;
    const int side4Blocked = 0b001000;
    const int side5Blocked = 0b010000;
    const int side6Blocked = 0b100000;


    public Sprite pathSprite;
    private TileBase pathTile;
    public Tilemap pathLevelMap;
    public Tilemap towerLevelMap;

    public Vector3Int startingTile;
    public Vector3Int targetTile;

    public bool pathReady;
    public bool pathBlocked;
    public bool buildPath;

    class pathNode
    {
        public Vector3Int position;
        public bool pathTilePlaced;
        public int sidesBlocked;
        public bool tileSearchExausted;
    };
    List<pathNode> currentPath = new List<pathNode>();

    bool checkForTargetTile(Vector3Int newPosition)
    {
        // Path complete when last node is the target
        if (newPosition == targetTile)
        {
            pathReady = true;
        }

        return pathReady;
    }

    bool tryTilePosition2()
    {
        var newNode = new pathNode();
        newNode.position = currentPath[currentPath.Count - 1].position;

        newNode.position.x += 1;

        if (towerLevelMap.GetTile(newNode.position) != null)
        {
            // Tower exists on this tile

            // Set prev node side blocked
            currentPath[currentPath.Count - 1].sidesBlocked |= side2Blocked;

#if DEBUG_PATH_GEN
            Debug.Log("Tower exists on this tile. sidesBlocked=" + currentPath[currentPath.Count - 1].sidesBlocked);
#endif
        }
        else if (pathLevelMap.GetTile(newNode.position) != null)
        {
            // Path already exists on this tile

            // Set prev node side blocked
            currentPath[currentPath.Count - 1].sidesBlocked |= side2Blocked;
        }
        else
        {
            if (checkForTargetTile(newNode.position))
                return false;

            // Block new node form backtracking
            newNode.sidesBlocked |= side5Blocked;

            // New Node position is available, place tile
            pathLevelMap.SetTile(newNode.position, pathTile);
            newNode.pathTilePlaced = true;

            // Add new node to list
            currentPath.Add(newNode);

            return true;
        }

        return false;
    }

    bool tryTilePosition5()
    {
        var newNode = new pathNode();
        newNode.position = currentPath[currentPath.Count - 1].position;

        newNode.position.x -= 1;

        if (towerLevelMap.GetTile(newNode.position) != null)
        {
            // Tower exists on this tile

            // Set prev node side blocked
            currentPath[currentPath.Count - 1].sidesBlocked |= side5Blocked;

#if DEBUG_PATH_GEN
            Debug.Log("Tower exists on this tile. sidesBlocked=" + currentPath[currentPath.Count - 1].sidesBlocked);
#endif
        }
        else if (pathLevelMap.GetTile(newNode.position) != null)
        {
            // Path already exists on this tile

            // Set prev node side blocked
            currentPath[currentPath.Count - 1].sidesBlocked |= side5Blocked;
        }
        else
        {
            if (checkForTargetTile(newNode.position))
                return false;

            // Block new node form backtracking
            newNode.sidesBlocked |= side2Blocked;

            // New Node position is available, place tile
            pathLevelMap.SetTile(newNode.position, pathTile);
            newNode.pathTilePlaced = true;

            // Add new node to list
            currentPath.Add(newNode);

            return true;
        }

        return false;
    }

    bool tryTilePosition1()
    {
        var newNode = new pathNode();
        newNode.position = currentPath[currentPath.Count - 1].position;

        // Hex tiles r wierd
        if ((currentPath[currentPath.Count - 1].position.y % 2) == 1)
            newNode.position.x += 1;
        newNode.position.y += 1;

        if (towerLevelMap.GetTile(newNode.position) != null)
        {
            // Tower exists on this tile

            // Set prev node side blocked
            currentPath[currentPath.Count - 1].sidesBlocked |= side1Blocked;

#if DEBUG_PATH_GEN
            Debug.Log("Tower exists on this tile. sidesBlocked=" + currentPath[currentPath.Count - 1].sidesBlocked);
#endif
        }
        else if (pathLevelMap.GetTile(newNode.position) != null)
        {
            // Path already exists on this tile

            // Set prev node side blocked
            currentPath[currentPath.Count - 1].sidesBlocked |= side1Blocked;
        }
        else if (checkForTargetTile(newNode.position) == false)
        {
            // New Node position is available, place tile
            pathLevelMap.SetTile(newNode.position, pathTile);
            newNode.pathTilePlaced = true;

            // Block new node form backtracking
            newNode.sidesBlocked |= side4Blocked;

            // Add new node to list
            currentPath.Add(newNode);

            return true;
        }

        return false;
    }

    bool tryTilePosition6()
    {
        var newNode = new pathNode();
        newNode.position = currentPath[currentPath.Count - 1].position;

        // Hex tiles r wierd
        if ((currentPath[currentPath.Count - 1].position.y % 2) == 0)
            newNode.position.x -= 1;
        newNode.position.y += 1;

        if (towerLevelMap.GetTile(newNode.position) != null)
        {
            // Tower exists on this tile

            // Set prev node side blocked
            currentPath[currentPath.Count - 1].sidesBlocked |= side6Blocked;

#if DEBUG_PATH_GEN
            Debug.Log("Tower exists on this tile. sidesBlocked=" + currentPath[currentPath.Count - 1].sidesBlocked);
#endif
        }
        else if (pathLevelMap.GetTile(newNode.position) != null)
        {
            // Path already exists on this tile

            // Set prev node side blocked
            currentPath[currentPath.Count - 1].sidesBlocked |= side6Blocked;
        }
        else if (checkForTargetTile(newNode.position) == false)
        {
            // New Node position is available, place tile
            pathLevelMap.SetTile(newNode.position, pathTile);
            newNode.pathTilePlaced = true;

            // Block new node form backtracking
            newNode.sidesBlocked |= side3Blocked;

            // Add new node to list
            currentPath.Add(newNode);

            return true;
        }

        return false;
    }

    bool tryTilePosition3()
    {
        var newNode = new pathNode();
        newNode.position = currentPath[currentPath.Count - 1].position;

        // Hex tiles r wierd
        if ((currentPath[currentPath.Count - 1].position.y % 2) == 1)
            newNode.position.x += 1;
        newNode.position.y -= 1;

        if (towerLevelMap.GetTile(newNode.position) != null)
        {
            // Tower exists on this tile

            // Set prev node side blocked
            currentPath[currentPath.Count - 1].sidesBlocked |= side3Blocked;

#if DEBUG_PATH_GEN
            Debug.Log("Tower exists on this tile. sidesBlocked=" + currentPath[currentPath.Count - 1].sidesBlocked);
#endif
        }
        else if (pathLevelMap.GetTile(newNode.position) != null)
        {
            // Path already exists on this tile

            // Set prev node side blocked
            currentPath[currentPath.Count - 1].sidesBlocked |= side3Blocked;
        }
        else if (checkForTargetTile(newNode.position) == false)
        {
            // New Node position is available, place tile
            pathLevelMap.SetTile(newNode.position, pathTile);
            newNode.pathTilePlaced = true;

            // Block new node form backtracking
            newNode.sidesBlocked |= side6Blocked;

            // Add new node to list
            currentPath.Add(newNode);

            return true;
        }

        return false;
    }

    bool tryTilePosition4()
    {
        var newNode = new pathNode();
        newNode.position = currentPath[currentPath.Count - 1].position;

        // Hex tiles r wierd
        if ((currentPath[currentPath.Count - 1].position.y % 2) == 0)
            newNode.position.x -= 1;
        newNode.position.y -= 1;

        if (towerLevelMap.GetTile(newNode.position) != null)
        {
            // Tower exists on this tile

            // Set prev node side blocked
            currentPath[currentPath.Count - 1].sidesBlocked |= side4Blocked;

#if DEBUG_PATH_GEN
            Debug.Log("Tower exists on this tile. sidesBlocked=" + currentPath[currentPath.Count - 1].sidesBlocked);
#endif
        }
        else if (pathLevelMap.GetTile(newNode.position) != null)
        {
            // Path already exists on this tile

            // Set prev node side blocked
            currentPath[currentPath.Count - 1].sidesBlocked |= side4Blocked;
        }
        else if (checkForTargetTile(newNode.position) == false)
        {
            // New Node position is available, place tile
            pathLevelMap.SetTile(newNode.position, pathTile);
            newNode.pathTilePlaced = true;

            // Block new node form backtracking
            newNode.sidesBlocked |= side1Blocked;

            // Add new node to list
            currentPath.Add(newNode);

            return true;
        }

        return false;
    }

    bool searchForNextTile()
    {
        bool canPlacePath = false;
        
        if (currentPath[currentPath.Count - 1].position.x < targetTile.x)
        {
            // Try exploring to tiles on the X-axis first
            if ((currentPath[currentPath.Count - 1].sidesBlocked & side2Blocked) == 0)
            {
#if DEBUG_PATH_GEN
                Debug.Log("tryTilePosition2");
#endif
                tryTilePosition2();
            }
            else if (currentPath[currentPath.Count - 1].position.y < targetTile.y)
            {
                if ((currentPath[currentPath.Count - 1].sidesBlocked & side1Blocked) == 0)
                {
#if DEBUG_PATH_GEN
                    Debug.Log("tryTilePosition1 - 1");
#endif
                    tryTilePosition1();
                }
                else if ((currentPath[currentPath.Count - 1].sidesBlocked & side3Blocked) == 0)
                {
#if DEBUG_PATH_GEN
                    Debug.Log("tryTilePosition3 - 1");
#endif
                    tryTilePosition3();
                }
                else
                {
#if DEBUG_PATH_GEN
                    Debug.Log("tryTilePosition4 - 1");
#endif
                    tryTilePosition4();
                }
            }
            else
            {
                if ((currentPath[currentPath.Count - 1].sidesBlocked & side3Blocked) == 0)
                {
#if DEBUG_PATH_GEN
                    Debug.Log("tryTilePosition3 - 2");
#endif
                    tryTilePosition3();
                }
                else if ((currentPath[currentPath.Count - 1].sidesBlocked & side1Blocked) == 0)
                {
#if DEBUG_PATH_GEN
                    Debug.Log("tryTilePosition1 - 2");
#endif
                    tryTilePosition1();
                }
                else
                {
#if DEBUG_PATH_GEN
                    Debug.Log("tryTilePosition4 - 2");
#endif
                    tryTilePosition4();
                }
            }           
        }
        else
        {
#if DEBUG_PATH_GEN
            Debug.Log("currentPath.Count=" + currentPath.Count + " currentPath.x=" + currentPath[currentPath.Count - 1].position.x + " targetTile.x=" + targetTile.x);
#endif

            // Try exploring to tiles on the X-axis first
            if ((currentPath[currentPath.Count - 1].sidesBlocked & side5Blocked) == 0)
            {
#if DEBUG_PATH_GEN
                Debug.Log("tryTilePosition5");
#endif
                tryTilePosition5();
            }
            else if (currentPath[currentPath.Count - 1].position.y < targetTile.y)
            {
                if ((currentPath[currentPath.Count - 1].sidesBlocked & side6Blocked) == 0)
                {
#if DEBUG_PATH_GEN
                    Debug.Log("tryTilePosition6 - 1");
#endif
                    tryTilePosition6();
                }
                else if ((currentPath[currentPath.Count - 1].sidesBlocked & side4Blocked) == 0)
                {
#if DEBUG_PATH_GEN
                    Debug.Log("tryTilePosition4 - 3");
#endif
                    tryTilePosition4();
                }
                else
                {
#if DEBUG_PATH_GEN
                    Debug.Log("tryTilePosition3 - 3");
#endif
                    tryTilePosition3();
                }
            }
            else
            {
                if ((currentPath[currentPath.Count - 1].sidesBlocked & side4Blocked) == 0)
                {
#if DEBUG_PATH_GEN
                    Debug.Log("tryTilePosition4 - 4");
#endif
                    tryTilePosition4();
                }
                else if ((currentPath[currentPath.Count - 1].sidesBlocked & side6Blocked) == 0)
                {
#if DEBUG_PATH_GEN
                    Debug.Log("tryTilePosition6 - 2");
#endif
                    tryTilePosition6();
                }
                else
                {
#if DEBUG_PATH_GEN
                    Debug.Log("tryTilePosition3 - 4");
#endif
                    tryTilePosition3();
                }
            }
        }

        // If we have searched all sides of a tile, exaust it
        if (currentPath.Count == 1)
        {
            // If all tiles around the starting tile are blocked, we cannot find a path
            if (currentPath[0].sidesBlocked == (side1Blocked | side2Blocked | side3Blocked | side4Blocked | side5Blocked | side6Blocked))
            {
                pathBlocked = true;
            }
        }
        else
        {
            if (currentPath[currentPath.Count - 1].sidesBlocked == (side1Blocked | side2Blocked | side3Blocked | side4Blocked | side5Blocked | side6Blocked))
            {
                // Set prev node side adjasent to exhausted node to blocked
                if (currentPath[currentPath.Count - 2].position.x < currentPath[currentPath.Count - 1].position.x)
                {
                    if (currentPath[currentPath.Count - 2].position.y > currentPath[currentPath.Count - 1].position.y)
                        currentPath[currentPath.Count - 2].sidesBlocked |= side3Blocked;
                    else if (currentPath[currentPath.Count - 2].position.y < currentPath[currentPath.Count - 1].position.y)
                        currentPath[currentPath.Count - 2].sidesBlocked |= side1Blocked;
                    else
                        currentPath[currentPath.Count - 2].sidesBlocked |= side2Blocked;
                }
                else
                {
                    if (currentPath[currentPath.Count - 2].position.y > currentPath[currentPath.Count - 1].position.y)
                        currentPath[currentPath.Count - 2].sidesBlocked |= side4Blocked;
                    else if (currentPath[currentPath.Count - 2].position.y < currentPath[currentPath.Count - 1].position.y)
                        currentPath[currentPath.Count - 2].sidesBlocked |= side5Blocked;
                    else
                        currentPath[currentPath.Count - 2].sidesBlocked |= side6Blocked;
                }                

                // Pop exhausted node from list
                currentPath.RemoveAt(currentPath.Count - 1);
            }
            /*
            // If 5 of the sides are blocked, exhausted the tile
            if ((currentPath[currentPath.Count - 1].sidesBlocked == (side2Blocked | side3Blocked | side4Blocked | side5Blocked | side6Blocked)) ||
                (currentPath[currentPath.Count - 1].sidesBlocked == (side1Blocked | side3Blocked | side4Blocked | side5Blocked | side6Blocked)) ||
                (currentPath[currentPath.Count - 1].sidesBlocked == (side1Blocked | side2Blocked | side4Blocked | side5Blocked | side6Blocked)) ||
                (currentPath[currentPath.Count - 1].sidesBlocked == (side1Blocked | side2Blocked | side3Blocked | side5Blocked | side6Blocked)) ||
                (currentPath[currentPath.Count - 1].sidesBlocked == (side1Blocked | side2Blocked | side3Blocked | side4Blocked | side6Blocked)) ||
                (currentPath[currentPath.Count - 1].sidesBlocked == (side1Blocked | side2Blocked | side3Blocked | side4Blocked | side5Blocked)) )
            {
                // Set prev node side adjasent to exhausted node to blocked
                int prevTileSideBlocked = 0;
                switch (currentPath[currentPath.Count - 1].sidesBlocked ^ 0b111111)
                {
                    case side1Blocked: prevTileSideBlocked = side4Blocked; break;
                    case side2Blocked: prevTileSideBlocked = side5Blocked; break;
                    case side3Blocked: prevTileSideBlocked = side6Blocked; break;
                    case side4Blocked: prevTileSideBlocked = side1Blocked; break;
                    case side5Blocked: prevTileSideBlocked = side2Blocked; break;
                    case side6Blocked: prevTileSideBlocked = side3Blocked; break;
                    default: break;
                }
                currentPath[currentPath.Count - 2].sidesBlocked = prevTileSideBlocked;

                // Pop exhausted node from list
                currentPath.RemoveAt(currentPath.Count - 1);
            }
            */
        }

        return false;
    }

    public void generatePath()
    {
        buildPath = true;

        /*
        while(!pathReady && !pathBlocked)
        {
            // Set first node to be our starting node
            if (currentPath.Count == 0)
            {
                var newNode = new pathNode();
                newNode.position = startingTile;
                currentPath.Add(newNode);
            }
            
            searchForNextTile();            
        }
        */
    }

    public void erasePath()
    {
        // Set all tiles to empty
        foreach (pathNode node in currentPath)
        {
            if (node.pathTilePlaced)
                pathLevelMap.SetTile(node.position, null);
        }

        buildPath = false;
        pathReady = false;
        currentPath.Clear();
    }

    public void newTowerLocation(Vector3Int newTowerPos)
    {
        bool reGeneratePath = false;

        if (pathReady)
        {
            // Search through all of the towers
            foreach (pathNode node in currentPath)
            {
                // If the towers location is on top of the path
                if (node.pathTilePlaced)
                {
                    if (node.position == newTowerPos)
                        reGeneratePath = true;
                }
            }

            // Re generate the creep path
            if (reGeneratePath)
            {
                erasePath();
                generatePath();
            }
        }
    }

    public Vector3Int[] getPathList()
    {
        Vector3Int[] returnArray = new Vector3Int[currentPath.Count];

        for (int index=0; index<currentPath.Count; index++)
        {
            returnArray[index] = currentPath[index].position;
        }

        return returnArray;
    }

    // Start is called before the first frame update
    void Start()
    {
        pathTile = CustomTile.CreateCustomTile(pathSprite, 0, 0);


        //testing 
        generatePath();
    }

    // Update is called once per frame
    void Update()
    {
        if (!pathReady && !pathBlocked && buildPath)
        {
            // Set first node to be our starting node
            if (currentPath.Count == 0)
            {
                var newNode = new pathNode();
                newNode.position = startingTile;
                currentPath.Add(newNode);
            }

            searchForNextTile();
        }
    }
}
