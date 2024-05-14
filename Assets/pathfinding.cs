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
        // NodeArray�� ũ�� �����ְ�, isWall, x, y ����
        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;
        NodeArray = new Node[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                bool isWall = false;//�̰� ������ �⺻������ ���������ִٰ� ����
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


        // ���۰� �� ���, ��������Ʈ�� ��������Ʈ, ����������Ʈ �ʱ�ȭ
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
            // ��������Ʈ �� ���� F�� �۰� F�� ���ٸ� H�� ���� �� ������� �ϰ� ��������Ʈ���� ��������Ʈ�� �ű��
            CurNode = OpenList[0];
            string leftright = "";
            for (int i = 1; i < OpenList.Count; i++)
                if (OpenList[i].F <= CurNode.F && OpenList[i].H < CurNode.H) {//����ġ ����ؼ� �ִ� �������� �κ�
                    CurNode = OpenList[i]; 
                }

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);


            // ������
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
                        if((NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x + 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isplatform && NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x + 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isPoint||//��
                            NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x - 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isplatform && NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x - 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isPoint)){//��
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
                        leftright = "������";
                    }
                    else
                    {
                        leftright = "����";
                    }
                    NodeDistanceList.Add(leftright);
                    TargetCurNode = TargetCurNode.ParentNode;//�θ� �����κ�
                }
                FinalNodeList.Add(StartNode);
                FinalNodeList.Reverse();
                NodeDistanceList.Add("none");
                NodeDistanceList.Reverse();
                int cnt = FinalNodeList.Count;
                /*for (int i = 0; i < FinalNodeList.Count; i++) {//���� ���� ���� �ۼ����κκ�
                    //PointCheck = false; 
                    if (FinalNodeList[i].isPoint)
                    {
                        if (PointCheck)
                        {
                            Debug.Log(i + "�ǳʶ�");
                            PointCheck = false;
                        
                            continue;
                        }
                        
                        //Debug.Log(FinalNodeList[i] + "�� ����Ʈ�Դϴ�");

                        int testnum = 1;
                        // Debug.Log(NodeArray[FinalNodeList[i].x + Mathf.Abs(testnum) * -1+bottomLeft.x,FinalNodeList[i].y+bottomLeft.y]);
                        //Debug.Log((FinalNodeList[i].x + Mathf.Abs(testnum) * -1) + "," + (FinalNodeList[i].y));
                        Debug.Log((NodeArray[FinalNodeList[i].x + Mathf.Abs(testnum) * -1 - bottomLeft.x, FinalNodeList[i].y - bottomLeft.y].x)+""+ (NodeArray[FinalNodeList[i].x + Mathf.Abs(testnum) * -1 - bottomLeft.x, FinalNodeList[i].y - bottomLeft.y].y));
                        while (!PointCheck)
                        {
                            //cnt++;
                            //Debug.Log(string.Format("{0} i�� {1} testnum�� {2}x {3}y", i, testnum, NodeArray[FinalNodeList[i].x + Mathf.Abs(testnum) - bottomLeft.x, FinalNodeList[i].y - bottomLeft.y].x, NodeArray[FinalNodeList[i].x + Mathf.Abs(testnum) - bottomLeft.x, FinalNodeList[i].y - bottomLeft.y].y));

                            if (NodeDistanceList[i] == "������")//�̹� ���� �� ���� �Ǿ��־ ��ü�� ���ָ� �Ǵºκ� ���Ḹ ���� �ش� �κ� ������ openlist�� �ְ� finallist�� �ִ°� �����Ѵ�
                            {
                                if (NodeArray[FinalNodeList[i].x+testnum-bottomLeft.x, FinalNodeList[i].y-bottomLeft.y].isPoint)
                                {
                                    
                                    Debug.Log("ã��" + NodeArray[FinalNodeList[i].x + Mathf.Abs(testnum)  - bottomLeft.x, FinalNodeList[i].y - bottomLeft.y].x + "" + NodeArray[FinalNodeList[i].x + Mathf.Abs(testnum)  - bottomLeft.x, FinalNodeList[i].y - bottomLeft.y].y);

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
                            else if(NodeDistanceList[i] =="����")
                            {
                                if (NodeArray[FinalNodeList[i].x +Mathf.Abs(testnum)*-1-bottomLeft.x, FinalNodeList[i].y-bottomLeft.y].isPoint)
                                {
                                    Debug.Log("ã��"+ NodeArray[FinalNodeList[i].x + Mathf.Abs(testnum) * -1 - bottomLeft.x, FinalNodeList[i].y - bottomLeft.y].x+""+ NodeArray[FinalNodeList[i].x + Mathf.Abs(testnum) * -1 - bottomLeft.x, FinalNodeList[i].y - bottomLeft.y].y);
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
                    //print(i + "��°�� " + FinalNodeList[i].x + ", " + FinalNodeList[i].y + NodeDistanceList[i]); 
                }*///����׿�
                Debug.Log(cnt);
                return;
            }


            // �֢آע�
            if (allowDiagonal)
            {
                OpenListAdd(CurNode.x + 1, CurNode.y + 1);
                OpenListAdd(CurNode.x - 1, CurNode.y + 1);
                OpenListAdd(CurNode.x - 1, CurNode.y - 1);
                OpenListAdd(CurNode.x + 1, CurNode.y - 1);
            }

            // �� �� �� ��
            OpenListAdd(CurNode.x, CurNode.y + 1);
            OpenListAdd(CurNode.x + 1, CurNode.y);
            OpenListAdd(CurNode.x, CurNode.y - 1);
            OpenListAdd(CurNode.x - 1, CurNode.y);
        }
    }

    void OpenListAdd(int checkX, int checkY)
    {
        // �����¿� ������ ����� �ʰ�, ���� �ƴϸ鼭, ��������Ʈ�� ���ٸ�
        if (checkX >= bottomLeft.x && checkX < topRight.x + 1 && checkY >= bottomLeft.y && checkY < topRight.y + 1 && !NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y].isWall && !ClosedList.Contains(NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y]))
        {
            // �밢�� ����, �� ���̷� ��� �ȵ�
            if (allowDiagonal) if (NodeArray[CurNode.x - bottomLeft.x, checkY - bottomLeft.y].isWall && NodeArray[checkX - bottomLeft.x, CurNode.y - bottomLeft.y].isWall) return;

            // �ڳʸ� �������� ���� ������, �̵� �߿� �������� ��ֹ��� ������ �ȵ�
            if (dontCrossCorner) if (NodeArray[CurNode.x - bottomLeft.x, checkY - bottomLeft.y].isWall || NodeArray[checkX - bottomLeft.x, CurNode.y - bottomLeft.y].isWall) return;

            if(NodeArray[checkX-bottomLeft.x, checkY - bottomLeft.y].isRoad)//checkx�� checky���� �˾ƾ���
            {
                Node NeighborNode = NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y];
                int MoveCost = CurNode.G + (CurNode.x - checkX == 0 || CurNode.y - checkY == 0 ? 10 : 14);


                // �̵������ �̿����G���� �۰ų� �Ǵ� ��������Ʈ�� �̿���尡 ���ٸ� G, H, ParentNode�� ���� �� ��������Ʈ�� �߰�
                if (MoveCost < NeighborNode.G || !OpenList.Contains(NeighborNode))
                {
                    NeighborNode.G = MoveCost;
                    NeighborNode.H = (Mathf.Abs(NeighborNode.x - TargetNode.x) + Mathf.Abs(NeighborNode.y - TargetNode.y)) * 10;
                    NeighborNode.ParentNode = CurNode;

                    OpenList.Add(NeighborNode);
                }
            }


            // �̿���忡 �ְ�, ������ 10, �밢���� 14���

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
