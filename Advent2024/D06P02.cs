using System.Diagnostics;
using System.IO.Pipelines;
using System.Numerics;

namespace Advent2024;

public class D06P02Position
{
    public int Up { get; set; } = 0;
    public int Down  { get; set; } = 0;
    public int Left   { get; set; } = 0;
    public int Right   { get; set; } = 0;

    public void Clean(){
        if (Up == 2) Up = 0;
        if (Down == 2) Down = 0;
        if (Left == 2) Left = 0;
        if (Right == 2) Right = 0;
    }
}

public static class D06P02
{   private static void AddDict(Dictionary<int, List<int>> dict, int key, int value){
        if (!dict.TryGetValue(key, out List<int>? l))
            l = new List<int>();
        l.Add(value);
        dict[key] = l;
    }

    private static void RemoveDict(Dictionary<int, List<int>> dict, int key, int value){
        if (dict.TryGetValue(key, out List<int>? l))
            l.Remove(value);
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

    private static void PutPosition(Dictionary<int, D06P02Position> dict, int key, int direction, int value){
        if (!dict.TryGetValue(key, out D06P02Position? pos)){
            pos = new D06P02Position();
            dict[key] = pos;
        }

        if (direction == 1 && pos.Up != 1)
            pos.Up = value;
        if (direction == 2 && pos.Right != 1)
            pos.Right = value;
        if (direction == 3 && pos.Down != 1)
            pos.Down = value;
        if (direction == 4 && pos.Left != 1)
            pos.Left = value;
    }

    private static void CleanState(Dictionary<int, D06P02Position> dict){
        foreach(var item in dict){
            item.Value.Clean();
        }
    }

    private static void TrackPositions(Dictionary<int, List<int>> byRow, Dictionary<int, List<int>> byCol,
        Dictionary<int, D06P02Position> dict, int curX, int newX, int curY, int newY, int direction,
        int rows, int columns, HashSet<int> cyclePositions)
    {
        // Need to process route in order
        /*int swap;
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
        */
        bool oneProcessed = false;
        
        var resetX = curX;
        int lastX = 0, lastY = 0;
        bool xUp = curX <= newX;
        bool yUp = curY <= newY;

        

        while ((xUp && curX <= newX) || (!xUp && curX >= newX)){

            int dy = curY;
            while ( ( yUp && dy <= newY) || (!yUp && dy >= newY))
            {

                // Process the first item.  Increment.
                dict.TryGetValue(curX * 10000 + dy, out D06P02Position? existingState);
                CleanState(dict);
                                    // Don't put obstance in existing path
                if (oneProcessed && (existingState == null || (existingState.Down == 0 && existingState.Left == 0 && existingState.Right == 0 && existingState.Up == 0))){                    
                    // Before processing the next... 
                        // What if it contained an obstacle?
                        // Would we hit a previous node running in same direction?
                        // DONE Essentially need a copy of current state (maybe change from Int to a structure)
                                // DONE Track direction(s) that node was encountered.  0=not encountered 1=encountered real 2=encountered test
                                // DONE Temporarly insert obstacle into byRow and byCol objects
                                AddDict(byRow, curX, dy);
                                AddDict(byCol, dy, curX);

                                // Call variant of MoveAround that moves until we run out or until an existing object is hit
                                // As it trackspositions, it sets flag to 2
                                // If we see a 1 or 2 on our direction, report cycle
                                if (CheckCycles(dict, byRow, byCol, lastX, lastY, rows, direction, columns))
                                {
                                    cyclePositions.Add( curX * 10000 + dy );
                                    //if (false) Console.WriteLine($"Found cycle at ({curX},{dy})");
                                }

                                RemoveDict(byRow, curX, dy);
                                RemoveDict(byCol, dy, curX);
                }

                

                PutPosition(dict, curX * 10000 + dy, direction, 1);
                lastX = curX;
                lastY = dy;
                //dict.Add( curX * 10000 + dy);
                oneProcessed = true;
                if (yUp)
                    dy++;
                else
                    dy--;
            }

            if (xUp)
                curX++;
            else
                curX--;
        }
    }

    private static int MoveAround(Dictionary<int, List<int>> byRow, Dictionary<int, List<int>> byCol,
        int curX, int curY, int rows, int direction, int columns)
    {
        bool debug = false;
        Dictionary<int, D06P02Position> moveDict = new Dictionary<int, D06P02Position>();
        //int lastMoves = -1;
        //int moves = 0;
        HashSet<int> cyclePositions = new HashSet<int>();
        while (true){
            //lastMoves = moves;
            // Up
            if (direction == 1)
            {
                if (!byCol.TryGetValue(curY, out List<int>? things)){                
                    TrackPositions(byRow, byCol, moveDict, curX, 0, curY, curY, direction, rows, columns, cyclePositions);
                    break;
                }
                
                things.Sort();
                things.Reverse();
                var newX = things.FirstOrDefault( i => i < curX, -1);
                if (newX == -1)
                {                
                    TrackPositions(byRow, byCol, moveDict, curX, 0, curY, curY,  direction, rows, columns, cyclePositions);
                    break;
                }

                TrackPositions(byRow, byCol, moveDict, curX, newX + 1, curY, curY,  direction, rows, columns, cyclePositions);
                curX = newX + 1;
                direction = 2;
                if (debug) Console.WriteLine($"({curX},{curY})...{moveDict.Count}");
            }
            // Right
            if (direction == 2)
            {
                if (!byRow.TryGetValue(curX, out List<int>? things)){                
                    TrackPositions(byRow, byCol, moveDict, curX, curX, curY, columns - 1,  direction, rows, columns, cyclePositions);
                    break;
                }
                
                things.Sort();                
                var newY = things.FirstOrDefault( i => i > curY, -1);
                if (newY == -1)
                {                
                    TrackPositions(byRow, byCol, moveDict, curX, curX, curY, columns - 1,  direction, rows, columns, cyclePositions);
                    break;
                }

                TrackPositions(byRow, byCol, moveDict, curX, curX, curY, newY - 1,  direction, rows, columns, cyclePositions);
                curY = newY - 1;
                direction = 3;
                if (debug) Console.WriteLine($"({curX},{curY})...{moveDict.Count}");
            }
            // DOwn
            if (direction == 3)
            {
                if (!byCol.TryGetValue(curY, out List<int>? things)){                
                    TrackPositions(byRow, byCol, moveDict, curX, rows - 1, curY, curY,  direction, rows, columns, cyclePositions);
                    break;
                }
                
                things.Sort();     
                var newX = things.FirstOrDefault( i => i > curX, -1);
                if (newX == -1)
                {                
                    TrackPositions(byRow, byCol, moveDict, curX, rows - 1, curY, curY,  direction, rows, columns, cyclePositions);
                    break;
                }

                TrackPositions(byRow, byCol, moveDict, curX, newX - 1, curY, curY,  direction, rows, columns, cyclePositions);
                curX = newX - 1;
                direction = 4;
                if (debug) Console.WriteLine($"({curX},{curY})...{moveDict.Count}");
            }
            // Left
            if (direction == 4)
            {
                if (!byRow.TryGetValue(curX, out List<int>? things)){                
                    TrackPositions(byRow, byCol, moveDict, curX, curX, curY, 0,  direction, rows, columns, cyclePositions);
                    break;
                }
                
                things.Sort();        
                things.Reverse();        
                var newY = things.FirstOrDefault( i => i < curY, -1);
                if (newY == -1)
                {                
                    TrackPositions(byRow, byCol, moveDict, curX, curX, curY, 0,  direction, rows, columns, cyclePositions);
                    break;
                }

                TrackPositions(byRow, byCol, moveDict, curX, curX, curY, newY + 1,  direction, rows, columns, cyclePositions);
                curY = newY + 1;
                direction = 1;
                if (debug) Console.WriteLine($"({curX},{curY})...{moveDict.Count}");
            }

            //moves = moveDict.Count;
        }

        return cyclePositions.Count;
    }

  private static bool CheckCycles(Dictionary<int, D06P02Position> moveDict, 
        Dictionary<int, List<int>> byRow, Dictionary<int, List<int>> byCol,
        int curX, int curY, int rows, int direction, int columns)
    {
        bool debug = false;        
        //int lastMoves = -1;
        //int moves = moveDict.Count;
        while (true){
            //lastMoves = moves;
            // Up
            if (direction == 1)
            {
                if (!byCol.TryGetValue(curY, out List<int>? things)){                
                    return false;
                }
                
                things.Sort();
                things.Reverse();
                var newX = things.FirstOrDefault( i => i < curX, -1);
                if (newX == -1)
                {                
                    return false;
                }

                if (TrackPositionsCycleCheck(byRow, byCol, moveDict, curX, newX + 1, curY, curY,  direction))
                    return true;
                curX = newX + 1;
                direction = 2;
                if (debug) Console.WriteLine($"({curX},{curY})...{moveDict.Count}");
            }
            // Right
            if (direction == 2)
            {
                if (!byRow.TryGetValue(curX, out List<int>? things)){                
                    return false;
                }
                
                things.Sort();                
                var newY = things.FirstOrDefault( i => i > curY, -1);
                if (newY == -1)
                {                
                    return false;
                }

                if (TrackPositionsCycleCheck(byRow, byCol, moveDict, curX, curX, curY, newY - 1,  direction))
                    return true;
                curY = newY - 1;
                direction = 3;
                if (debug) Console.WriteLine($"({curX},{curY})...{moveDict.Count}");
            }
            // DOwn
            if (direction == 3)
            {
                if (!byCol.TryGetValue(curY, out List<int>? things)){                
                    return false;
                }
                
                things.Sort();     
                var newX = things.FirstOrDefault( i => i > curX, -1);
                if (newX == -1)
                {                
                    return false;
                }

                if (TrackPositionsCycleCheck(byRow, byCol, moveDict, curX, newX - 1, curY, curY,  direction))
                    return true;
                curX = newX - 1;
                direction = 4;
                if (debug) Console.WriteLine($"({curX},{curY})...{moveDict.Count}");
            }
            // Left
            if (direction == 4)
            {
                if (!byRow.TryGetValue(curX, out List<int>? things)){                
                    return false;
                }
                
                things.Sort();        
                things.Reverse();        
                var newY = things.FirstOrDefault( i => i < curY, -1);
                if (newY == -1)
                {                
                    return false;
                }

                if (TrackPositionsCycleCheck(byRow, byCol, moveDict, curX, curX, curY, newY + 1,  direction))
                    return true;
                curY = newY + 1;
                direction = 1;
                if (debug) Console.WriteLine($"({curX},{curY})...{moveDict.Count}");
            }

            //moves = moveDict.Count;
        }

        //return false;
    }

    private static bool TrackPositionsCycleCheck(Dictionary<int, List<int>> byRow, Dictionary<int, List<int>> byCol,
        Dictionary<int, D06P02Position> dict, int curX, int newX, int curY, int newY, int direction)
    {
        // Need to process route in order
        /*int swap;
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
        */
        bool oneProcessed = false;
        
        var resetX = curX;
        bool xUp = curX <= newX;
        bool yUp = curY <= newY;

        

        while ((xUp && curX <= newX) || (!xUp && curX >= newX)){

            int dy = curY;
            while ( ( yUp && dy <= newY) || (!yUp && dy >= newY))
            {
                var key = curX * 10000 + dy;
                if (oneProcessed && dict.TryGetValue(key, out D06P02Position? pos)){
                    if (direction == 1 && pos.Up > 0) return true;
                    if (direction == 2 && pos.Right > 0) return true;
                    if (direction == 3 && pos.Down > 0) return true;
                    if (direction == 4 && pos.Left > 0) return true;
                }

                PutPosition(dict, curX * 10000 + dy, direction, 2);                
                oneProcessed = true;
                if (yUp)
                    dy++;
                else
                    dy--;
            }

            if (xUp)
                curX++;
            else
                curX--;
        }

        return false;
    }

}