using System.Diagnostics;
using System.IO.Pipelines;
using System.Numerics;
using System.Xml;

namespace Advent2024;


/* public class D09Node
{
    public int FileId { get; set; }

    public bool IsSpace { get; set; }

    public int Size { get; set; }

    public D09Node? NextNode { get; set; }

    public D09Node? PrevNode { get; set; }
} */

public static class D09P02
{      
    public static long Process(string filename){                
       
        D09Node? root = null;
        D09Node? end = null;

        using (var r = FileUtil.Open(filename)){         
            string? line = r.ReadLine();
            bool isFile = true;
            int fileId = 0;
            
            while (line != null){
               
               for(int i = 0; i < line.Length; i++){
                    char c = line[i];
                    D09Node? node = null;
                    if (isFile){
                        node = new D09Node {
                           FileId = fileId++,
                            IsSpace = false,
                             Size = c - '0'
                        };
                        isFile = false;
                    }
                    else{
                        node = new D09Node {
                           FileId = -1,
                            IsSpace = true,
                            Size = c - '0'
                        };
                        isFile = true;
                    }

                    if (root == null){
                        root = end = node;
                    }
                    else{
                        node.PrevNode = end;
                        end!.NextNode = node;
                        end = node;
                    }

               }
               
                line = r.ReadLine();
            }            
        }       
       
        Compact(root!, end!);

        return CheckSum(root!);
    }   

    private static D09Node GetEnd(D09Node root, D09Node beforeNode)
    {
        var n = root;
        while (n.NextNode != null)
            n = n.NextNode;
        
        
        while (n!.IsSpace)
            n = n.PrevNode;
        
        return n;
    }

    private static D09Node? GetNextSpaceOfSize(D09Node root, D09Node beforeNode)
    {
        var n = root;
        while (n.NextNode != null)
        {
            if (beforeNode == n)
                return null;

            if (n.IsSpace && n.Size >= beforeNode.Size)
                return n;
            

            n = n.NextNode;
        }
            
        
        return null;
    }

    private static bool Infill(D09Node root, D09Node node)
    {

            var nextSpace = GetNextSpaceOfSize(root, node);
            if (nextSpace == null) return false;

            var prev = nextSpace.PrevNode;
            var fill = nextSpace.Size;
            if (node.Size < fill)
                fill = node.Size;

            D09Node newFileNode = new D09Node {
                //Char = node.Char,
                FileId = node.FileId,
                IsSpace = false,
                    PrevNode = prev,
                    Size = fill
            };

            D09Node newSpaceNode = new D09Node {
                //Char = node.Char,
                FileId = -1,
                IsSpace = true,
                NextNode = node.NextNode,
                PrevNode = node.PrevNode,
                Size = node.Size
            };

            node.Size = node.Size - fill;
            nextSpace.Size = nextSpace.Size - fill;

            

            newFileNode.NextNode = nextSpace; 
            prev!.NextNode = newFileNode;
            if (newFileNode.NextNode != null)
                newFileNode.NextNode.PrevNode = newFileNode;

        //}
        // Replace my position with new space node
        if (node.PrevNode != null)
            node.PrevNode.NextNode = newSpaceNode;
        if (node.NextNode != null)
            node.NextNode.PrevNode = newSpaceNode;
        // Cut-out the end node as its been fully inserted
        //node.PrevNode!.NextNode = node.NextNode;
        return true;
    }

    private static D09Node? PrevNode(D09Node node)
    {
        var lessThan = node.FileId;
        var n = node.PrevNode;
        while (n != null && (n.IsSpace || n.FileId > lessThan))
            n = n.PrevNode;
        return n;
    }

    private static void Compact(D09Node root, D09Node? end)
    {
        bool compacted = true;
        while (end != null) {
            compacted = false;

            D09Node? prev = PrevNode(end);

            //Console.WriteLine($"\nChecking {end.FileId.ToString()[0]}");
            compacted = Infill(root, end);
            Print(root);

            end = prev;
        }
    }

    private static void Print(D09Node root)
    {
        //Console.WriteLine("");
        var n = root;
        while (n != null){
            //if (!n.IsSpace)
            //Console.Write("".PadLeft(n.Size, n.IsSpace ? '.' : n.FileId.ToString()[0]));
            //Console.WriteLine($"{n.FileId}-{n.Size} Space:{n.IsSpace}");
            n = n.NextNode;
        }
    }

    private static long CheckSum(D09Node root)
    {
        long sum = 0;
        
        var n = root;
        int position = 0;
        while (n != null){
            if (!n.IsSpace)
            {
                for( int i = 0; i < n.Size; i++){
                    //Console.WriteLine($"{position}*{n.FileId}");
                    sum += position++ * n.FileId;
                }
            }
            else{
                position += n.Size;
            }
            n = n.NextNode;
        }
        return sum;
    }

}