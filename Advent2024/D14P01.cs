using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO.Pipelines;
using System.Numerics;
using System.Xml;

namespace Advent2024;

public class D14Robot
{
    public int Id { get; set; }

    public int Row { get; set; }
    public int Column { get; set; }
    public int RowVelocity { get; set; }
    public int ColumnVelocity { get; set; }

    public int FinalRow { get; set; }
    public int FinalColumn { get; set; }

    public void Print(){
        Console.WriteLine($"{Id}: ({Column},{Row}) v=({ColumnVelocity},{RowVelocity}) Final:({FinalColumn},{FinalRow})");
    }
}

public class D14P01
{      
    public int Height { get; set; }
    public int Width { get; set; }

    public int Seconds { get; set; } = 100;

    public List<D14Robot> Robots { get; set; } = new List<D14Robot>();

    public long Process(string filename, int width, int height){                

        this.Height = height;
        this.Width = width;
        var id = 0;
        
        using (var r = FileUtil.Open(filename)){         
            string? line = r.ReadLine();
            while (line != null){

                D14Robot robot = new D14Robot();
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
        
        PassTimeAllRobots();
        return ComputeSafetyScore();
    }   

    private void PassTime(D14Robot robot)
    {
        var y = robot.Column;  
        var totalMoveY = (robot.ColumnVelocity * this.Seconds);
        var finalPositionY = (y + totalMoveY) % this.Width;
        if (finalPositionY < 0)
            finalPositionY = finalPositionY + this.Width;

        var x = robot.Row;  
        var totalMoveX = (robot.RowVelocity * this.Seconds);
        var finalPositionX = (x + totalMoveX) % this.Height;
        if (finalPositionX < 0)
            finalPositionX = finalPositionX + this.Height;

        robot.FinalRow = finalPositionX;
        robot.FinalColumn = finalPositionY;
    }

    private void PassTimeAllRobots(){
        foreach(var robot in this.Robots)
            PassTime(robot);
    }   

    private long ComputeSafetyScore(){

        // All numbers are odd
        var centerColumn = this.Width / 2;
        var middleRow = this.Height / 2;

        int[] quads = new int[4] { 0, 0, 0, 0};
        
        foreach(var robot in this.Robots){

            robot.Print();

            if (robot.FinalRow == middleRow || robot.FinalColumn == centerColumn)
                continue;

            int quad = 0;
            if (robot.FinalRow > middleRow)
                quad += 2;
            if (robot.FinalColumn > centerColumn)
                quad += 1;

            quads[quad] = quads[quad] + 1;
        }

        Console.WriteLine($"Q1: {quads[0]} Q2: {quads[1]}  Q3: {quads[2]}  Q4: {quads[3]}");

        return quads[0] * quads[1] * quads[2] * quads[3];

    }
}