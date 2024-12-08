using System.Diagnostics;
using System.IO.Pipelines;
using System.Numerics;

namespace Advent2024;


public static class D08P02
{   
    private static void AddPos(Dictionary<char, List<Position>> lists, char c, int x, int y)
    {
        if (!lists.TryGetValue(c, out List<Position>? list))
            list = new List<Position>();
        lists[c] = list;
        list.Add(new Position { X = x, Y = y});
    }

    private static int CheckAllCharCombinations(Dictionary<char, List<Position>> lists, int rows, int columns)
    {
        HashSet<int> antinodePositions = new HashSet<int>();
        foreach(var key in lists){
            Console.WriteLine(key.Key);
            CheckCharCombinations(antinodePositions, key.Value, rows, columns);
        }
        return antinodePositions.Count;
    }

    private static void AddIfInBounds(HashSet<int> antinodePositions, int x, int y, int rows, int columns)
    {
        int key = x * 10000 + y;
        if (x >= 0 && x < rows && y >= 0 && y < columns){
            if (!antinodePositions.Contains(key)){
                antinodePositions.Add(key);
                Console.WriteLine($"  In Bounds and New ({x},{y})");
            }
            else{
                Console.WriteLine($"  In Bounds DUPE ({x},{y})");
            }
        }
        else{
            Console.WriteLine($"  Out of Bounds ({x},{y})");
        }
    }

    private static void ExpandSignalUntilOutOfBounds(HashSet<int> antinodePositions, int rows, int columns,
        int deltaX, int deltaY, int x, int y)
    {
        var sx = x;
        var sy = y;

        AddIfInBounds(antinodePositions, x, y, rows, columns);
        do{
            x = x + deltaX;
            y = y + deltaY;
            AddIfInBounds(antinodePositions, x, y, rows, columns);
        }
        while ( x >= 0 && x < rows && (y >= 0 && y < columns));
        
        x = sx;
        y = sy;

        do{
            x = x - deltaX;
            y = y - deltaY;
            AddIfInBounds(antinodePositions, x, y, rows, columns);
        }
        while ( x < rows && x > 0 && (y >= 0 && y < columns));
    }

    private static void CheckCharCombinations(HashSet<int> antinodePositions, List<Position> lists, int rows, int columns)
    {
        for(int i = 0; i < lists.Count; i++){
            for(int j = i + 1; j < lists.Count; j++){

                var p1 = lists[i];
                var p2 = lists[j];

                var x1 = p1.X;
                var x2 = p2.X;
                var y1 = p1.Y;
                var y2 = p2.Y;

                // List is built in row order, so we know x1 < x2
                var rowDistance =  x1 - x2 ;
                var colDistance = Math.Abs( y1 - y2 );

                //var x3 = x1 + rowDistance;
                //var x4 = x2 - rowDistance;                
                
                //var y3 = y1;
                //var y4 = y2;
                if (y1 <= y2){
                    colDistance = colDistance * -1;
                }
                //else{
                    //y3 = y3 + colDistance;
                    //y4 = y4 - colDistance;
                //}

                //Console.WriteLine($"Nodes ({x1},{y1})&({x2},{y2})-->({x3},{y3})&({x4},{y4})");
                ExpandSignalUntilOutOfBounds(antinodePositions, rows, columns, rowDistance, colDistance, x1, y1);
                ExpandSignalUntilOutOfBounds(antinodePositions, rows, columns, -rowDistance, -colDistance, x2, y2);
                
                //AddIfInBounds(antinodePositions, x4, y4, rows, columns);
            }
        }
    }

    public static long Process(string filename){                
        int rows = 0;
        int columns = 0;
        Dictionary<char, List<Position>> lists = new Dictionary<char, List<Position>>();

        using (var r = FileUtil.Open(filename)){         
            string? line = r.ReadLine();                     
            
            while (line != null){
                columns = line.Length;

                var chars = line.ToCharArray();
                for(int y = 0; y < chars.Length; y++)
                {
                    var c = chars[y];
                    if (c != '.'){
                        AddPos(lists, c, rows, y);
                    }
                }

               
                line = r.ReadLine();
                rows++;
            }            
        }       
       
        return CheckAllCharCombinations(lists, rows, columns);
    }     
}