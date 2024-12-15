using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO.Pipelines;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace Advent2024;

public class D15P01
{      
    public List<char[]> Map { get; set; } = new List<char[]>();
    public int RobotRow { get; set; }
    public int RobotCol { get; set; }

    public long Process(string filename){ 
        string moves = "";
        bool isMap = true;
        var row = 0;
        using (var r = FileUtil.Open(filename)){         
            string? line = r.ReadLine();
            while (line != null){
             
                if (string.IsNullOrEmpty(line))                
                    isMap = false;                    
                else if (isMap){                    
                    this.Map.Add(line.ToCharArray());

                    var index = line.IndexOf('@');
                    if (index > -1){
                        this.RobotRow = row;
                        this.RobotCol = index;
                    }

                    row++;
                }
                else
                    moves += line;

                line = r.ReadLine();                
            }            
        }
        
        ProcessMoves(moves);
        return SumGPSCoordinates();
    }   

    private void ProcessMoves(string moves)
    {
        for(int i = 0; i < moves.Length; i++){
            var c = moves[i];
            ProcessMove(c);
        }
    }

    private void ProcessMove(char c)
    {
        int directionX = 0;
        int directionY = 0;

        if (c == '^') directionX--;
        else if (c == 'v') directionX++;
        else if (c == '<') directionY--;
        else if (c == '>') directionY++;
        else throw new ApplicationException("WTF");

        if (TryMove(RobotRow + directionX, RobotCol + directionY, directionX, directionY)){
            this.Map[this.RobotRow][this.RobotCol] = '.';
            this.Map[this.RobotRow + directionX][this.RobotCol + directionY] = '@';
            this.RobotRow = this.RobotRow + directionX;
            this.RobotCol = this.RobotCol + directionY;
        }

        //this.Print(c);
    }

    private bool TryMove(int x, int y, int directionX, int directionY){

        var targetC = this.Map[x][y];
        if (targetC == 'O'){
            if (TryMove(x + directionX, y + directionY, directionX, directionY)){
                this.Map[x][y] = '.';
                this.Map[x + directionX][y + directionY] = 'O';
                return true;
            }
            return false;
        }
        else 
            return (targetC == '.');
    }

    private void Print(char move){
        var s = new StringBuilder();
        foreach(var line in this.Map)
            s.AppendLine( string.Join("", line) );

        Console.WriteLine();
        Console.Write("Move: " + move);
        Console.WriteLine();
        Console.WriteLine(s);
    }

    private long SumGPSCoordinates(){
        long sum = 0;

        for(int i = 1; i < this.Map.Count - 1; i++){
            var line = this.Map[i];
            for(int j = 1; j < line.Length - 1; j++){
                if (line[j] == 'O')
                    sum += i * 100 + j;
            }
        }

        return sum;

    }
}