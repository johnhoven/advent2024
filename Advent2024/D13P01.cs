using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO.Pipelines;
using System.Numerics;
using System.Xml;

namespace Advent2024;

public class D13P01
{      
    const int ButtonACost = 3;
    const int ButtonBCost = 1;

    public long Process(string filename){                
        List<string> lines = new List<string>();
        using (var r = FileUtil.Open(filename)){         
            string? line = r.ReadLine();
            while (line != null){
                lines.Add(line);
                line = r.ReadLine();                
            }            
        }
        
        return ProcessLines(lines);
    }   


    private long ProcessLines(List<string> lines){        

        long sum = 0;

        for (int i = 0; i < lines.Count; i = i + 4)
        {
            var aline = lines[i + 0];
            var bline = lines[i + 1];
            var pline = lines[i + 2];

            sum += ProcessLine(aline, bline, pline);
        }

        return sum;
    }   

    private (int x, int y) GetXY(string line){
        var splits = line.Split(':', StringSplitOptions.RemoveEmptyEntries)[1].Trim().Split(',', StringSplitOptions.RemoveEmptyEntries);
        var x = int.Parse(splits[0].Trim().Split('+')[1]);
        var y = int.Parse(splits[1].Trim().Split('+')[1]);
        return (x, y);
    }

    private (int x, int y) GetPrize(string line){
        var splits = line.Split(':', StringSplitOptions.RemoveEmptyEntries)[1].Trim().Split(',', StringSplitOptions.RemoveEmptyEntries);
        var x = int.Parse(splits[0].Trim().Split('=')[1]);
        var y = int.Parse(splits[1].Trim().Split('=')[1]);
        return (x, y);
    }

    private long ProcessLine(string aline, string bline, string pline)
    {
        var aVals = GetXY(aline);
        var bVals = GetXY(bline);
        var prizeVals = GetPrize(pline);

        //   94*A + 22B = 8400
        //   34*A + 67B = 5400
        //   8400 - 22B = 94A
        //   (8400 - 22B)/94 = A
        // 34* ((8400-22B)/94) + 67B = 5400
        //  3038.29 - 7.957B + 67B = 5400
        // 59B = 2361.71
        // B = ~40

        // Cramer's Rule
        double a1 = aVals.x, b1 = bVals.x, c1 = prizeVals.x;
        double a2 = aVals.y, b2 = bVals.y, c2 = prizeVals.y;
        
        double D = a1 * b2 - a2 * b1;    // Main determinant
        double Dx = c1 * b2 - c2 * b1;    // Determinant for A
        double Dy = a1 * c2 - a2 * c1;    // Determinant for B

        if (D == 0 )
        {
            // Console.WriteLine("No unique solution exists (determinant is zero).");
            return 0;
        }
        else
        {
            // Calculate A and B using Cramer's Rule
            double A = Dx / D;
            double B = Dy / D;

            // Not interested in fractional button pushes
            if (Math.Floor(A) != A || Math.Floor(B) != B)
                return 0;

            return (long)A * ButtonACost + (long)B * ButtonBCost;
        }
    }
 
}