using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO.Pipelines;
using System.Numerics;
using System.Text;
using System.Xml;

namespace Advent2024;

public class D14RobotP2
{
    public int Id { get; set; }

    public int Row { get; set; }
    public int Column { get; set; }
    public int RowVelocity { get; set; }
    public int ColumnVelocity { get; set; }


}

public class D14P02
{      
    public int Height { get; set; }
    public int Width { get; set; }
    

    public List<D14RobotP2> Robots { get; set; } = new List<D14RobotP2>();

    public long Process(string filename, int width, int height){                

        this.Height = height;
        this.Width = width;
        var id = 0;
        
        using (var r = FileUtil.Open(filename)){         
            string? line = r.ReadLine();
            while (line != null){

                D14RobotP2 robot = new D14RobotP2();
                robot.Id = id++;

                var s1 = line.Split(new char[] { ' ', ',', '=' });
                robot.Column = int.Parse( s1[1] );
                robot.Row = int.Parse( s1[2]);
                robot.ColumnVelocity = int.Parse( s1[4] );
                robot.RowVelocity = int.Parse( s1[5] );

                //robot.Print();

                this.Robots.Add(robot);
                line = r.ReadLine();                
            }            
        }
        
        return PassTimeAllRobots();
    }   

    private void PassTime(D14RobotP2 robot, bool[] grid)
    {
        var y = robot.Column;  
        var totalMoveY = (robot.ColumnVelocity * 1);
        var finalPositionY = (y + totalMoveY) % this.Width;
        if (finalPositionY < 0)
            finalPositionY = finalPositionY + this.Width;

        var x = robot.Row;  
        var totalMoveX = (robot.RowVelocity * 1);
        var finalPositionX = (x + totalMoveX) % this.Height;
        if (finalPositionX < 0)
            finalPositionX = finalPositionX + this.Height;

        robot.Row = finalPositionX;
        robot.Column = finalPositionY;

        var pos = finalPositionX * this.Width + finalPositionY;
        grid[pos] = true;
    }

    private int PassTimeAllRobots(){

        var cont = true;
        int seconds = 0;
        bool[] grid = new bool[ this.Width * this.Height ];        
        

        do{
            seconds++;

            Array.Fill(grid, false);

            foreach(var robot in this.Robots)
                PassTime(robot, grid);

            var inARow = 0;            
            var qualified= false;
            for(var i = 0; i < grid.Length; i++){
                if (grid[i]){
                    inARow++;
                    if (inARow == 15)
                    {
                        qualified = true;
                        break;
                    }
                }
                else{
                    inARow = 0;
                }
            }

            if (qualified){
                PrintGrid(grid);

                Console.WriteLine($"This is iteration {seconds}");
                Console.WriteLine("Continue? ");
                cont = Console.ReadLine() == "1";            
            }

        }while(cont);
        return seconds;
    }   



    private void PrintGrid(bool[] grid){
        Console.Clear();

        StringBuilder sb = new StringBuilder(); 
        for(var i = 0; i < grid.Length; i++){
            
            var c = grid[i] ? 'X' : ' ';
            sb.Append(c);

            if (i % this.Width == this.Width - 1)
                sb.AppendLine();

        }
        
        Console.Write(sb.ToString());
    }

}