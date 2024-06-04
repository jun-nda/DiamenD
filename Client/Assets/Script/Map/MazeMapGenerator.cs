using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class MazeMapGenerator : MonoBehaviour {
    private enum MapItemType {
        WALL,
        SPACE,
        GATE,
    }

    public int mapSize = 81;
    private List<List<MapItemType>> mazeMapData = new List<List<MapItemType>>();
    private List<List<Vector2>> originPointMap = new List<List<Vector2>>();
    private List<Vector2> originPoints = new List<Vector2>();

    private List<Vector2> usedPointList; // 走过的就记录
    private List<Vector2> lastPointList; // 记录实时路径

    GameObject cubePrefab;
    GameObject characterPrefab;
    
    void Awake () {
        cubePrefab = Resources.Load<GameObject>("Prefabs/Cube");
        characterPrefab = Resources.Load<GameObject>("Prefabs/HumanMouse");
        for (int y = 0; y < mapSize; y++)
        {
            List<MapItemType> yList = new List<MapItemType>();
            mazeMapData.Add(yList);
            for (int x = 0; x < mapSize; x++)
            {
                yList.Add(MapItemType.WALL);
            }
        }
        
        // 第一版先按照隔一个一个小房间来算
        // 第一步：撒点,逻辑分层，一维
        for (int y = 1; y < mapSize; y+=2) {
            for (int x = 1; x < mapSize; x+=2) {
                originPoints.Add(new Vector2(x, y));
                mazeMapData[x][y] = MapItemType.SPACE;
            }
        }
        
        // 一维数据放到二维数组里
        // 分两步算是逻辑分层吧，方便后面随机撒点做处理
        float ox = originPoints[0].x;
        float oy = originPoints[0].y;
        int yIndex = 0;
        foreach (Vector2 originPoint in originPoints) {
            if (Math.Abs(originPoint.x - ox) < 0.0001f) {
                originPointMap.Add(new List<Vector2>());
            }

            if (!(Math.Abs(originPoint.y - oy) < 0.0001f)) {
                yIndex++;
                oy = originPoint.y;
            }
            originPointMap[yIndex].Add(originPoint);
        }
        
        // 随机找到一个起始点
        Vector2 startPoint = originPoints[Random.Range(0, originPoints.Count - 1)];
        Vector2 curPoint = startPoint;

        usedPointList = new List<Vector2>(); // 走过的就记录
        lastPointList = new List<Vector2>(); // 记录实时路径
        List<Vector2> toUsePointList = new List<Vector2>();

        bool isFinish = false;
        while (true) {
            usedPointList.Add(curPoint);
            toUsePointList.Clear();
            findNextPoint(curPoint, toUsePointList);

            for (int i = toUsePointList.Count - 1; i >= 0; --i) {
                Vector2 toUsePoint = toUsePointList[i];
                bool canUse = true;
                foreach (var usedPoint in usedPointList) {
                    if (toUsePoint == usedPoint) {
                        canUse = false;
                    }
                }
                if (!canUse) {
                    toUsePointList.Remove(toUsePoint);
                }
            }
            
            // 
            if (toUsePointList.Count == 0) {
                if (lastPointList.Count != 0) {
                    curPoint = lastPointList[lastPointList.Count - 1];
                    lastPointList.Remove(curPoint);
                }
                else {
                    isFinish = true;
                }
            }
            else {
                Vector2 selectPoint = toUsePointList[Random.Range(0, toUsePointList.Count)];
                Vector2 destWallLoc = (curPoint + selectPoint) / 2;

                mazeMapData[(int)destWallLoc.x][(int)destWallLoc.y] = MapItemType.SPACE;
                lastPointList.Add(curPoint);
                curPoint = selectPoint;
            }
            
            Debug.Log(curPoint);
            if (isFinish == true) {
                break;
            }
        }

        bool characterInit = false;
        for (int y = 0; y < mapSize; ++y) {
            for (int x = 0; x < mapSize; x++) {
                MapItemType type = mazeMapData[x][y];
                Vector3 pos = new Vector3(x * 2f,2.5f, y * 2f);

                if (type == MapItemType.WALL) {
                    bool leftHasWall = x - 1 >= 0 && mazeMapData[x - 1][y] == MapItemType.WALL;
                    bool rightHasWall = x + 1 < mazeMapData.Count && mazeMapData[x + 1][y] == MapItemType.WALL;
                    bool frontHasWall = y - 1 >= 0 && mazeMapData[x][y - 1] == MapItemType.WALL;
                    bool behindHasWall = y + 1 < mazeMapData.Count && mazeMapData[x][y + 1] == MapItemType.WALL;                   
                    // 看哪个方向有墙壁，如果有 就按这个方向缩放
                    if ( (leftHasWall||rightHasWall) &&(frontHasWall || behindHasWall)) {
                        GameObject wallObjHorizon = WallEntityGenerate(pos);
                        wallObjHorizon.transform.localScale = new Vector3(2f, 5f, 0.2f);
                        GameObject wallObjVertical = WallEntityGenerate(pos);
                        wallObjVertical.transform.localScale = new Vector3(0.2f, 5f, 2f);     
                    }else if (leftHasWall || rightHasWall) {
                        GameObject wallObjHorizon = WallEntityGenerate(pos);              
                        wallObjHorizon.transform.localScale = new Vector3(2f, 5f, 0.2f);  
                    }else if (frontHasWall || behindHasWall) {
                        GameObject wallObjVertical = WallEntityGenerate(pos);              
                        wallObjVertical.transform.localScale = new Vector3(0.2f, 5f, 2f);   
                    }
                }else if (characterInit == false && type == MapItemType.SPACE) {
                    Instantiate(characterPrefab, pos, Quaternion.identity);
                    characterInit = true;
                }
            }
        }
    }
    
    private void findNextPoint (Vector2 curPoint, List<Vector2> toUsePointList) {
        int size = originPointMap.Count;
        Vector2 pointIndexInMap = new Vector2();
        for (int y = 0; y < size; y++) {
            bool canBreak = false;
            for (int x = 0; x < size; x++) {
                if (originPointMap[x][y] == curPoint) {
                    pointIndexInMap.x = x;
                    pointIndexInMap.y = y;
                    canBreak = true;
                    break;
                }
            }
            if (canBreak) break;
        }
        
        // 上下左右找相邻空地
        if (pointIndexInMap.x - 1 >= 0) {
            toUsePointList.Add(originPointMap[(int)pointIndexInMap.x - 1][(int)pointIndexInMap.y]);
        }
        if (pointIndexInMap.x + 1 < size) {
            toUsePointList.Add(originPointMap[(int)pointIndexInMap.x + 1][(int)pointIndexInMap.y]);
        }
        if (pointIndexInMap.y - 1 >= 0) {
            toUsePointList.Add(originPointMap[(int)pointIndexInMap.x][(int)pointIndexInMap.y - 1]);
        }
        if (pointIndexInMap.y + 1 < size) {
            toUsePointList.Add(originPointMap[(int)pointIndexInMap.x][(int)pointIndexInMap.y + 1]);
        }
    }
    
    private GameObject WallEntityGenerate (Vector3 pos) {
        GameObject wallObj = GameObject.Instantiate(cubePrefab, pos, Quaternion.identity);
        MeshRenderer renderer = wallObj.GetComponent<MeshRenderer>();
        if (renderer) {
            Material sharedMaterial = renderer.sharedMaterial;
            Material cloneMaterial = Instantiate(sharedMaterial);
            // float colorRandomR = Random.Range(0f, 1f);
            // float colorRandomG = Random.Range(0f, 1f);
            // float colorRandomB = Random.Range(0f, 1f);

            // cloneMaterial.SetColor("_Color", new Color(colorRandomR,colorRandomG,colorRandomB,1));
            renderer.sharedMaterial = cloneMaterial;
        }
        return wallObj;
    }
    
}
