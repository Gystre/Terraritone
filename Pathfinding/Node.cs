using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct Node
{
    public int F; // G + H(heuristic cost = distance from starting node) cost
    public int G; //distance from starting node
    public ushort PX; // parent node info
    public ushort PY;
    public byte Status;
    public byte PZ;
    public short JumpLength;

    public Node UpdateStatus(byte newStatus)
    {
        Node newNode = this;
        newNode.Status = newStatus;
        return newNode;
    }
}
