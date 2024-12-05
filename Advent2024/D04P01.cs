using System.Numerics;

namespace Advent2024;

[Flags]
public enum DirectionsAllowed
{
    NW = 1,
    N = 2,
    NE = 4,
    W = 8,
    E = 16,
    SW = 32,
    S = 64,
    SE = 128,
    All = 255
}

public static class D04P01
{
    private static Dictionary<char, char> nextSearchTerm = new Dictionary<char, char> {
        { 'X', 'M'},
        { 'M', 'A'},
        { 'A', 'S'}
    };

    private static int Search(List<string> lines, int maxY, int newX, int newY, char searchFor, DirectionsAllowed directionsAllowed)
    {
        char c = lines[newX][newY];
        if (c == searchFor && c == 'S'){
            Console.Write($"({newX},{newY}),");
            return 1;            
        }
        else if (c == searchFor){
            return SpreadOut(lines, newX, newY, maxY, nextSearchTerm[searchFor], directionsAllowed);
        }
        return 0;
    }

    private static int SpreadOut(List<string> lines, int curX, int curY, int maxY, char searchFor, DirectionsAllowed directionsAllowed)
    {
        int count = 0;

        if (curX > 0)
        {
            // Search up
            if ((directionsAllowed & DirectionsAllowed.N) == DirectionsAllowed.N)
                count += Search(lines, maxY, curX - 1, curY , searchFor, DirectionsAllowed.N);

            // And left
            if (curY > 0 && (directionsAllowed & DirectionsAllowed.NW) == DirectionsAllowed.NW)
                count += Search(lines, maxY, curX - 1, curY - 1, searchFor, DirectionsAllowed.NW);
            
            // And right
            if (curY < maxY - 1 && (directionsAllowed & DirectionsAllowed.NE) == DirectionsAllowed.NE)
                count += Search(lines, maxY, curX - 1, curY + 1, searchFor, DirectionsAllowed.NE);
        }

        // Left
        if (curY > 0 && (directionsAllowed & DirectionsAllowed.W) == DirectionsAllowed.W)
            count += Search(lines, maxY, curX, curY - 1, searchFor, DirectionsAllowed.W);

        // Right
        if (curY < maxY - 1  && (directionsAllowed & DirectionsAllowed.E) == DirectionsAllowed.E)
            count += Search(lines, maxY, curX, curY + 1, searchFor, DirectionsAllowed.E);

        if (curX < lines.Count - 1)
        {
            // Search down
            if ((directionsAllowed & DirectionsAllowed.S) == DirectionsAllowed.S)
                count += Search(lines, maxY, curX + 1, curY , searchFor, DirectionsAllowed.S);

            // And left
            if (curY > 0 && (directionsAllowed & DirectionsAllowed.SW) == DirectionsAllowed.SW)
                count += Search(lines, maxY, curX + 1, curY - 1, searchFor, DirectionsAllowed.SW);
            
            // And right
            if (curY < maxY - 1 && (directionsAllowed & DirectionsAllowed.SE) == DirectionsAllowed.SE)
                count += Search(lines, maxY, curX + 1, curY + 1, searchFor, DirectionsAllowed.SE);
        }

        return count;
    }

    public static Int64 Process(string filename){

        Int64 xmas = 0;
        int y = 0;
        List<string> lines = new List<string>();
        using (var r = FileUtil.Open(filename)){
         
            string? line = r.ReadLine();
           
            

            while (line != null){            
                lines.Add(line);
                y = line.Length;
                line = r.ReadLine();
            }             
        }

        for (int i = 0; i < lines.Count; i++){
            var line = lines[i];
            for (int j = 0; j < y; j++){
                var c = line[j];
                int count = Search(lines, y, i, j, 'X', DirectionsAllowed.All);
                if (count > 0)
                    Console.WriteLine($"from ({i}, {j})= {count}");
                xmas += count;
            }
        }
        // Start at 0,0
            // Continue: increment y then roll to x
            // If not = X, continue
            // If x, spread out in each direction
            

       return xmas;
    }

}