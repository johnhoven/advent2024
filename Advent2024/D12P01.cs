using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO.Pipelines;
using System.Numerics;
using System.Xml;

namespace Advent2024;

public class D12P01
{      
    public int RowCount { get; set; }
    public int ColumnCount { get; set; }

    public List<string> Rows { get; set; } = new List<string>();

    HashSet<int> Visited = new HashSet<int>();


    public long Process(string filename){                
        using (var r = FileUtil.Open(filename)){         
            string? line = r.ReadLine();
            
            while (line != null){
                 this.Rows.Add(line);
               this.RowCount++;
               this.ColumnCount = line.Length;
                line = r.ReadLine();
            }            
        }
        
        return IterateLands();
    }   

    private long IterateLands(){
        long sum = 0;
        for( var i = 0; i < RowCount; i++){
            for (var j = 0; j < ColumnCount; j++){
                sum += ProcessGarden(i, j);
            }
        }
        return sum;
    }

    private long ProcessGarden(int x, int y){
        int  key = x * 1000 + y;
        if (Visited.Contains(key))
            return 0;

        char searchChar = this.Rows[x][y];
        Queue<int> queue = new Queue<int>();
        queue.Enqueue(key);
        return ProcessQueue(queue, searchChar);
        
    }

    private long ProcessQueue(Queue<int> queue, char searchChar){
        int size = 0;
        HashSet<int> perimeter = new HashSet<int>();
        do{
            if (!queue.TryDequeue(out int key)){
                Console.WriteLine($"Char {searchChar} = {size} x {perimeter.Count} = {size * perimeter.Count}");
                return size * perimeter.Count;
            }

            if (Visited.Contains(key))
                continue;

            size++;
            Visited.Add(key);
            var y = key % 1000;
            var x = (key - y) / 1000;

            TrySpreadWrapped(queue, searchChar, x - 1, y, ref perimeter, 0); // Up
            TrySpreadWrapped(queue, searchChar, x, y - 1, ref perimeter, 1); // Left
            TrySpreadWrapped(queue, searchChar, x, y + 1, ref perimeter, 2); // Right
            TrySpreadWrapped(queue, searchChar, x + 1, y, ref perimeter, 3); // Down

        }while(true);
    }

    private void TrySpreadWrapped(Queue<int> queue, char searchChar, int x, int y, ref HashSet<int> perimeter, int direction)
    {       
        var key = x * 1000 + y; 
        var result = TrySpread(queue, searchChar, x, y);
        if (result == 1){         
            queue.Enqueue(key);            
        }
        else if (result < 0){

            // key
            // millions digits represents direction of perimter
            // thousands digits represents row
            // rest represents column
            perimeter.Add( direction * 1000000 + key);
        }
    }

    private int TrySpread(Queue<int> queue, char searchChar, int x, int y){
        var key = x * 1000 + y;
        if (x >= 0 && x < RowCount && y >= 0 && y < ColumnCount){
            char c = this.Rows[x][y];
            if (c == searchChar){
                if (this.Visited.Contains(key))
                    return 0; // already visited
                return 1;
            }            
            return -2;
        }
        return -1; // Off grid, it's a perimter
    }

}