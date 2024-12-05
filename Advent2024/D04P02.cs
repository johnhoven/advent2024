using System.Numerics;

namespace Advent2024;

public static class D04P02
{

    private static int Search(List<string> lines, int newX, int newY, char searchFor)
    {
        char c = lines[newX][newY];
        if (c == searchFor){
            
            var c1 = lines[newX - 1][newY - 1];
            var c2 = lines[newX - 1][newY + 1];
            var c3 = lines[newX + 1][newY - 1];
            var c4 = lines[newX + 1][newY + 1];
            if ((c1 != c4 && (c1 == 'M' || c1 == 'S') && (c4 == 'M' || c4 == 'S')) && (c2 != c3 && (c2 == 'M' || c2 == 'S') && (c3 == 'M' || c3 == 'S')))
                return 1;

        }
        
        return 0;
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

        for (int i = 1; i < lines.Count - 1; i++){

            for (int j =1; j < y - 1; j++){
                
                int count = Search(lines, i, j, 'A');
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