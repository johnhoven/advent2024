using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Reflection.Metadata;

namespace Advent2024;

public static class D06P01
{   
    private static void AddDict(Dictionary<int, List<int>> dict, int key, int value){
        if (!dict.TryGetValue(key, out List<int>? l))
            l = new List<int>();
        l.Add(value);
        dict[key] = l;
    }

    public static int Process(string filename){        
        
        int rows = 0;
        int columns = 0;
        int curX = 0, curY = 0;
        Dictionary<int, List<int>> byRow = new Dictionary<int, List<int>>();
        Dictionary<int, List<int>> byCol = new Dictionary<int, List<int>>();
        int direction = 4;
        using (var r = FileUtil.Open(filename)){         
            string? line = r.ReadLine();                     
            columns = line!.Length;
            //List<string> lines = new List<string>();
            

            while (line != null){            
                //lines.Add(line);              

                var index = line.IndexOf("^");
                if (index == -1)
                    index = line.IndexOf(">");
                else if (direction == 4)
                    direction = 1;

                if (index == -1)
                    index = line.IndexOf("V");
                else if (direction == 4)
                    direction = 2;

                if (index == -1)
                    index = line.IndexOf("<");
                else if (direction == 4)
                    direction = 3;

                if (index != -1){
                    curX = rows;
                    curY = index;
                }

                var start = 0;
                do{
                    index = line.IndexOf("#", start);
                    AddDict(byRow, rows, index);
                    AddDict(byCol, index, rows);
                    start = index + 1;
                } while (index > -1);

                rows++;
                line = r.ReadLine();
            }

            
        }

        int result = MoveAround(byRow, byCol, curX, curY, rows, direction, columns);
       
        return result;
    }

    private static void TrackPositions(HashSet<int> dict, int curX, int newX, int curY, int newY)
    {
        int swap;
        if (newX< curX){
            swap = newX;
            newX = curX;
            curX = swap;
        }
        if (newY< curY){
            swap = newY;
            newY = curY;
            curY = swap;
        }

        while (curX <= newX){

            int dy = curY;
            while (dy <= newY)
            {
                dict.Add( curX * 10000 + dy);

                dy++;
            }

            curX++;
        }
    }

    private static int MoveAround(Dictionary<int, List<int>> byRow, Dictionary<int, List<int>> byCol,
        int curX, int curY, int rows, int direction, int columns)
    {
        bool debug = true;
        HashSet<int> moveDict = new HashSet<int>();
        int lastMoves = -1;
        int moves = 0;
        while (moves != lastMoves){
            lastMoves = moves;
            // Up
            if (direction == 1)
            {
                if (!byCol.TryGetValue(curY, out List<int>? things)){                
                    //moves += curX - 0;
                    TrackPositions(moveDict, curX, 0, curY, curY);
                    break;
                }
                
                things.Sort();
                things.Reverse();
                var newX = things.FirstOrDefault( i => i < curX, -1);
                if (newX == -1)
                {                
                    //moves += curX - 0;
                    TrackPositions(moveDict, curX, 0, curY, curY);
                    break;
                }

                TrackPositions(moveDict, curX, newX + 1, curY, curY);
                //moves += curX - newX - 1;
                curX = newX + 1;
                direction = 2;
                if (debug) Console.WriteLine($"({curX},{curY})...{moveDict.Count}");
            }
            // Right
            if (direction == 2)
            {
                if (!byRow.TryGetValue(curX, out List<int>? things)){                
                    //moves += columns - curY - 1;
                    TrackPositions(moveDict, curX, curX, curY, columns - 1);
                    break;
                }
                
                things.Sort();                
                var newY = things.FirstOrDefault( i => i > curY, -1);
                if (newY == -1)
                {                
                    TrackPositions(moveDict, curX, curX, curY, columns - 1);
                    break;
                }

                TrackPositions(moveDict, curX, curX, curY, newY - 1);
                //moves += newY - curY - 1;
                curY = newY - 1;
                direction = 3;
                if (debug) Console.WriteLine($"({curX},{curY})...{moveDict.Count}");
            }
            // DOwn
            if (direction == 3)
            {
                if (!byCol.TryGetValue(curY, out List<int>? things)){                
                    //moves += rows - curX - 1;
                    TrackPositions(moveDict, curX, rows - 1, curY, curY);
                    break;
                }
                
                things.Sort();     
                var newX = things.FirstOrDefault( i => i > curX, -1);
                if (newX == -1)
                {                
                    //moves += rows - curX - 1;
                    TrackPositions(moveDict, curX, rows - 1, curY, curY);
                    break;
                }

                TrackPositions(moveDict, curX, newX - 1, curY, curY);
                //moves += newX - curX - 1;
                curX = newX - 1;
                direction = 4;
                if (debug) Console.WriteLine($"({curX},{curY})...{moveDict.Count}");
            }
            // Left
            if (direction == 4)
            {
                if (!byRow.TryGetValue(curX, out List<int>? things)){                
                    //moves += curY - 0;
                    TrackPositions(moveDict, curX, curX, curY, 0);
                    break;
                }
                
                things.Sort();        
                things.Reverse();        
                var newY = things.FirstOrDefault( i => i < curY, -1);
                if (newY == -1)
                {                
                    TrackPositions(moveDict, curX, curX, curY, 0);
                    break;
                }

                TrackPositions(moveDict, curX, curX, curY, newY + 1);
                //moves += curY - newY - 1;
                curY = newY + 1;
                direction = 1;
                if (debug) Console.WriteLine($"({curX},{curY})...{moveDict.Count}");
            }

            moves = moveDict.Count;
        }

        return moveDict.Count;
    }
  

}