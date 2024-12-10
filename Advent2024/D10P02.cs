using System.Diagnostics;
using System.IO.Pipelines;
using System.Numerics;
using System.Xml;

namespace Advent2024;

/* public class D10QueueItem
{
    public int StartX { get; set; }

    public int StartY { get; set; }
    public int X { get; set; }

    public int Y { get; set; }

    public int Item { get; set; }
} */

public class D10P02
{      
    public List<string> Rows { get; set; } = new List<string>();
    Queue<D10QueueItem> queue = new Queue<D10QueueItem>();

    Dictionary<int, Dictionary<int, int>> trailHeadCounts = new Dictionary<int, Dictionary<int, int>>();

    public int Complete { get; set; } = 0;
    public int RowLength { get; set; } = 0;
    public int ColumnLength { get; set; } = 0;


    public long Process(string filename){                
        using (var r = FileUtil.Open(filename)){         
            string? line = r.ReadLine();
            ColumnLength = line!.Length;
            while (line != null){
                Rows.Add(line);     

                var cs = line.ToCharArray();
                for(int i = 0; i < cs.Length; i++){
                    if (cs[i] == '0'){
                        queue.Enqueue(new D10QueueItem { X = RowLength, Y = i, Item = 0, StartX = RowLength, StartY = i});
                    }
                }

                line = r.ReadLine();
                RowLength++;
            }            
        }
        
        FollowTrails();
        
        return CountTrailHeads();
    }   

    private void TryFollow(D10QueueItem item, int newX, int newY){
        if (newX >= 0 && newX < RowLength && newY >= 0 && newY < ColumnLength){
            var c = this.Rows[newX][newY] - '0';
            if (c == item.Item + 1){

                if (c == 9){
                    
                    int key = item.StartX * 10000 + item.StartY;
                    if (!trailHeadCounts.TryGetValue(key, out Dictionary<int, int>? ends))
                        ends = new Dictionary<int, int>();
                    
                    var key2 = newX * 10000 + newY;
                    if (!ends.TryGetValue(key2, out int count))
                        count = 0;
                    count++;
                    ends[key2] = count;

                    Console.WriteLine($"({item.StartX},{item.StartY}-->({newX},{newY}) up to {count})");
                    trailHeadCounts[key] = ends;
                }
                else{
                    queue.Enqueue ( new D10QueueItem {
                        Item = c,
                        StartX = item.StartX,
                        StartY = item.StartY,
                        X =newX,
                        Y =newY
                    });
                }
            }
        }        
   }

   private void FollowTrails(){
        while (queue.TryDequeue(out D10QueueItem? item)){
            TryFollow(item, item.X - 1, item.Y); // Up
            TryFollow(item, item.X + 1, item.Y); // Down
            TryFollow(item, item.X, item.Y - 1); // Left
            TryFollow(item, item.X, item.Y + 1); // Right

        } 
   }

   private long CountTrailHeads(){
        long sum = 0;
        foreach(var item in this.trailHeadCounts){
            foreach(var end in item.Value)
                sum += end.Value;
        }
        return sum;
   }

}