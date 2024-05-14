using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

[System.Serializable]
public class Node
{
    public bool isWall;
    public bool isRoad;
    public bool isPoint;
    public bool isplatform;
    public Node ParentNode;

    public int x, y, G, H;
    //public int x, y, G, H;
    
    public int F { get { return G + H; } }
    public Node(bool _isWall,bool _isRoad,bool _isPoint,bool _isPlatform, int _x, int _y) { isWall = _isWall;isRoad = _isRoad;isPoint = _isPoint;isplatform = _isPlatform; x = _x; y = _y; }
}
public class pathfinding : MonoBehaviour
{
    public Rigidbody2D rg2d;

    public Vector2Int bottomLeft, topRight, startPos, targetPos;
    public List<Node> FinalNodeList;
    public bool allowDiagonal, dontCrossCorner;

    int sizeX, sizeY;
    Node[,] NodeArray;
    Node StartNode, TargetNode, CurNode;
    List<Node> OpenList, ClosedList;
    List<string> NodeDistanceList;

    bool PointCheck = false;

    public Transform _startPos, _targetPos;


    public void PathFinding()
    {
        // NodeArray의 크기 정해주고, isWall, x, y 대입
        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;
        NodeArray = new Node[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                bool isWall = false;//이거 때문에 기본적으로 지나갈수있다고 생각
                bool isRoad = false;
                bool isPoint = false;
                bool isplatform = false;
                foreach (Collider2D col in Physics2D.OverlapCircleAll(new Vector2(i + bottomLeft.x, j + bottomLeft.y), 0.4f))
                {
                    if (col.gameObject.layer == LayerMask.NameToLayer("Wall")) isWall = true;
                    if (col.gameObject.layer == LayerMask.NameToLayer("Road")) isRoad = true;
                    if (col.gameObject.layer == LayerMask.NameToLayer("Point")) isPoint = true;
                    if (col.gameObject.layer == LayerMask.NameToLayer("Platform")) isplatform = true;
                }

                NodeArray[i, j] = new Node(isWall,isRoad,isPoint,isplatform, i + bottomLeft.x, j + bottomLeft.y);
            }
        }


        // 시작과 끝 노드, 열린리스트와 닫힌리스트, 마지막리스트 초기화
        StartNode = NodeArray[startPos.x - bottomLeft.x, startPos.y - bottomLeft.y];
        TargetNode = NodeArray[targetPos.x - bottomLeft.x, targetPos.y - bottomLeft.y];
        //Debug.Log((int)_startPos.position.x + ":" + (int)_startPos.position.y);
        //Debug.Log((int)_targetPos.position.x + ":" + (int)_targetPos.position.y);
        //StartNode = NodeArray[(int)_startPos.position.x,(int)_startPos.position.y];
        //TargetNode = NodeArray[(int)_targetPos.position.x, (int)_targetPos.position.y];
        //StartNode = NodeArray[-7,-4];
        //TargetNode = NodeArray[3, -4];

        OpenList = new List<Node>() { StartNode };
        ClosedList = new List<Node>();
        FinalNodeList = new List<Node>();
        NodeDistanceList = new List<string>();


        while (OpenList.Count > 0)
        {
            // 열린리스트 중 가장 F가 작고 F가 같다면 H가 작은 걸 현재노드로 하고 열린리스트에서 닫힌리스트로 옮기기
            CurNode = OpenList[0];
            string leftright = "";
            for (int i = 1; i < OpenList.Count; i++)
                if (OpenList[i].F <= CurNode.F && OpenList[i].H < CurNode.H) {//가중치 계산해서 넣는 실질적인 부분
                    CurNode = OpenList[i]; 
                }

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);


            // 마지막
            if (CurNode == TargetNode)
            {
                Node TargetCurNode = TargetNode;
                int _cnt = 0;
                while (TargetCurNode != StartNode)
                {
                    if (_cnt > 2000)
                    {
                        return;
                    }
                    if (TargetCurNode.isplatform)
                    {
                        Debug.Log(NodeArray[TargetCurNode.x - bottomLeft.x+1, TargetCurNode.y - bottomLeft.y+1].isplatform&& NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y + 1].isPoint);
                        Debug.Log(NodeArray[TargetCurNode.x - bottomLeft.x -1 , TargetCurNode.y - bottomLeft.y + 1].isplatform && NodeArray[TargetCurNode.x - bottomLeft.x -1, TargetCurNode.y - bottomLeft.y + 1].isPoint);
                        if((NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x + 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isplatform && NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x + 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isPoint||//우
                            NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x - 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isplatform && NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x - 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isPoint)){//좌
                            Debug.Log(NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y + 1].x+","+ NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y + 1].y
                                );
                            Node temp;
                            temp = TargetCurNode.ParentNode;
                            //TargetCurNode.ParentNode = NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y + 1];
                            TargetCurNode.ParentNode = NodeArray[temp.x - bottomLeft.x - 1, temp.y - bottomLeft.y + 1];
                            NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y + 1].ParentNode = temp;
                        }

                    }
                    FinalNodeList.Add(TargetCurNode);
                    if (TargetCurNode.x > TargetCurNode.ParentNode.x)
                    {
                        leftright = "오른쪽";
                    }
                    else
                    {
                        leftright = "왼쪽";
                    }
                    NodeDistanceList.Add(leftright);
                    TargetCurNode = TargetCurNode.ParentNode;//부모 설정부분
                }
                FinalNodeList.Add(StartNode);
                FinalNodeList.Reverse();
                NodeDistanceList.Add("none");
                NodeDistanceList.Reverse();
                int cnt = FinalNodeList.Count;
                /*for (int i = 0; i < FinalNodeList.Count; i++) {//여기 부터 내가 작성중인부분
                    //PointCheck = false; 
                    if (FinalNodeList[i].isPoint)
                    {
                        if (PointCheck)
                        {
                            Debug.Log(i + "건너뜀");
                            PointCheck = false;
                        
                            continue;
                        }
                        
                        //Debug.Log(FinalNodeList[i] + "는 포인트입니다");

                        int testnum = 1;
                        // Debug.Log(NodeArray[FinalNodeList[i].x + Mathf.Abs(testnum) * -1+bottomLeft.x,FinalNodeList[i].y+bottomLeft.y]);
                        //Debug.Log((FinalNodeList[i].x + Mathf.Abs(testnum) * -1) + "," + (FinalNodeList[i].y));
                        Debug.Log((NodeArray[FinalNodeList[i].x + Mathf.Abs(testnum) * -1 - bottomLeft.x, FinalNodeList[i].y - bottomLeft.y].x)+""+ (NodeArray[FinalNodeList[i].x + Mathf.Abs(testnum) * -1 - bottomLeft.x, FinalNodeList[i].y - bottomLeft.y].y));
                        while (!PointCheck)
                        {
                            //cnt++;
                            //Debug.Log(string.Format("{0} i값 {1} testnum값 {2}x {3}y", i, testnum, NodeArray[FinalNodeList[i].x + Mathf.Abs(testnum) - bottomLeft.x, FinalNodeList[i].y - bottomLeft.y].x, NodeArray[FinalNodeList[i].x + Mathf.Abs(testnum) - bottomLeft.x, FinalNodeList[i].y - bottomLeft.y].y));

                            if (NodeDistanceList[i] == "오른쪽")//이미 노드는 다 삽입 되어있어서 교체만 해주면 되는부분 연결만 새로 해당 부분 연결은 openlist에 넣고 finallist에 있는걸 연결한다
                            {
                                if (NodeArray[FinalNodeList[i].x+testnum-bottomLeft.x, FinalNodeList[i].y-bottomLeft.y].isPoint)
                                {
                                    
                                    Debug.Log("찾음" + NodeArray[FinalNodeList[i].x + Mathf.Abs(testnum)  - bottomLeft.x, FinalNodeList[i].y - bottomLeft.y].x + "" + NodeArray[FinalNodeList[i].x + Mathf.Abs(testnum)  - bottomLeft.x, FinalNodeList[i].y - bottomLeft.y].y);

                                    PointCheck = true;
                                    testnum = 0;
                                    //continue;
                                }
                                else
                                {
                                    testnum++;
                                }
                                //NodeArray[FinalNodeList[i].x, FinalNodeList[i].y] = new Node(false, true, true, FinalNodeList[i].x  +1, FinalNodeList[i].y  );
                            }
                            else if(NodeDistanceList[i] =="왼쪽")
                            {
                                if (NodeArray[FinalNodeList[i].x +Mathf.Abs(testnum)*-1-bottomLeft.x, FinalNodeList[i].y-bottomLeft.y].isPoint)
                                {
                                    Debug.Log("찾음"+ NodeArray[FinalNodeList[i].x + Mathf.Abs(testnum) * -1 - bottomLeft.x, FinalNodeList[i].y - bottomLeft.y].x+""+ NodeArray[FinalNodeList[i].x + Mathf.Abs(testnum) * -1 - bottomLeft.x, FinalNodeList[i].y - bottomLeft.y].y);
                                    PointCheck = true;
                                    testnum = 0;
                                    //continue;
                                }
                                else
                                {
                                    testnum++;
                                }
                                //NodeArray[FinalNodeList[i].x, FinalNodeList[i].y] = new Node(false, true, true, FinalNodeList[i].x - 1, FinalNodeList[i].y);
                            }
                        }
                        //i++;
                        continue;
                       
                        //cnt++;
                    }
                    //print(i + "번째는 " + FinalNodeList[i].x + ", " + FinalNodeList[i].y + NodeDistanceList[i]); 
                }*///디버그용
                Debug.Log(cnt);
                return;
            }


            // ↗↖↙↘
            if (allowDiagonal)
            {
                OpenListAdd(CurNode.x + 1, CurNode.y + 1);
                OpenListAdd(CurNode.x - 1, CurNode.y + 1);
                OpenListAdd(CurNode.x - 1, CurNode.y - 1);
                OpenListAdd(CurNode.x + 1, CurNode.y - 1);
            }

            // ↑ → ↓ ←
            OpenListAdd(CurNode.x, CurNode.y + 1);
            OpenListAdd(CurNode.x + 1, CurNode.y);
            OpenListAdd(CurNode.x, CurNode.y - 1);
            OpenListAdd(CurNode.x - 1, CurNode.y);
        }
    }

    void OpenListAdd(int checkX, int checkY)
    {
        // 상하좌우 범위를 벗어나지 않고, 벽이 아니면서, 닫힌리스트에 없다면
        if (checkX >= bottomLeft.x && checkX < topRight.x + 1 && checkY >= bottomLeft.y && checkY < topRight.y + 1 && !NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y].isWall && !ClosedList.Contains(NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y]))
        {
            // 대각선 허용시, 벽 사이로 통과 안됨
            if (allowDiagonal) if (NodeArray[CurNode.x - bottomLeft.x, checkY - bottomLeft.y].isWall && NodeArray[checkX - bottomLeft.x, CurNode.y - bottomLeft.y].isWall) return;

            // 코너를 가로질러 가지 않을시, 이동 중에 수직수평 장애물이 있으면 안됨
            if (dontCrossCorner) if (NodeArray[CurNode.x - bottomLeft.x, checkY - bottomLeft.y].isWall || NodeArray[checkX - bottomLeft.x, CurNode.y - bottomLeft.y].isWall) return;

            if(NodeArray[checkX-bottomLeft.x, checkY - bottomLeft.y].isRoad)//checkx와 checky값을 알아야함
            {
                Node NeighborNode = NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y];
                int MoveCost = CurNode.G + (CurNode.x - checkX == 0 || CurNode.y - checkY == 0 ? 10 : 14);


                // 이동비용이 이웃노드G보다 작거나 또는 열린리스트에 이웃노드가 없다면 G, H, ParentNode를 설정 후 열린리스트에 추가
                if (MoveCost < NeighborNode.G || !OpenList.Contains(NeighborNode))
                {
                    NeighborNode.G = MoveCost;
                    NeighborNode.H = (Mathf.Abs(NeighborNode.x - TargetNode.x) + Mathf.Abs(NeighborNode.y - TargetNode.y)) * 10;
                    NeighborNode.ParentNode = CurNode;

                    OpenList.Add(NeighborNode);
                }
            }


            // 이웃노드에 넣고, 직선은 10, 대각선은 14비용

        }
    }

    void OnDrawGizmos()
    {
        if (FinalNodeList.Count != 0) for (int i = 0; i < FinalNodeList.Count - 1; i++)
                Gizmos.DrawLine(new Vector2(FinalNodeList[i].x, FinalNodeList[i].y), new Vector2(FinalNodeList[i + 1].x, FinalNodeList[i + 1].y));
    }

    private void Update()
    {
        startPos.x = (int)_startPos.position.x;
        startPos.y = (int)_startPos.position.y;
        targetPos.x = (int)_targetPos.position.x;
        targetPos.y = (int)_targetPos.position.y;
        if (Input.GetKeyDown(KeyCode.F))
        {
            PathFinding();

        }
    }
}
