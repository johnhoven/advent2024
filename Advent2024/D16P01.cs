﻿using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO.Pipelines;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace Advent2024;

public class D16QueueItem
{
    public string State { get; set; }
    public long Score { get; set; }

    public int X { get; set; }
    public int Y { get; set; }
    public int Direction { get; set; }
}

public class D16P01
{      
    public List<char[]> Lines { get; set; } = new List<char[]>();
    public List<long[]> Scores { get; set; } = new List<long[]>();
    public int Rows { get; set; } = 0;
    public int Columns  { get; set; } = 0;
    public int EndX { get; set; } = -1;
    public int EndY { get; set; } = -1;

    public Queue<D16QueueItem> MyQueue { get; set; } = new Queue<D16QueueItem>();    

    public D16QueueItem? MinimumFinishedScore { get; set; } = null;

    private const int North = 0;
    private const int East = 1;
    private const int South = 2;
    private const int West = 3;

    private int TurnClock(int direction){
        return (direction + 1) % 4;
    }

    private int TurnCounterClock(int direction){
        if (direction == 0) return West;
        return (direction -  1);
    }

    public long Process(string filename){ 
       
        int startX = -1;
        int startY = -1;

        using (var r = FileUtil.Open(filename)){         
            string? line = r.ReadLine();
            while (line != null){  
                
                this.Columns = line.Length;
                Lines.Add(line.ToCharArray());

                var scores = new long[line.Length];
                Array.Fill(scores, long.MaxValue);
                this.Scores.Add(scores);

                var index = line.IndexOf('S');
                if (index > -1){
                    startX = this.Rows;
                    startY = index;
                }
                index = line.IndexOf('E');
                if (index > -1){
                    this.EndX = this.Rows;
                    this.EndY = index;
                }

                this.Rows++;
                line = r.ReadLine();                
            }            
        }
        
        D16QueueItem start = new D16QueueItem {
             Direction = East,
              Score = 0,
               State = String.Empty,
             X = startX,
             Y = startY
        };
        MyQueue.Enqueue(start);
        ProcessQueue();

        Console.WriteLine(this.MinimumFinishedScore!.State);

        return this.MinimumFinishedScore!.Score;
    }   


    private void ProcessQueue(){
        while (MyQueue.TryDequeue(out D16QueueItem? qi))
        {
            ProcessQueueItem(qi);
        }
    }
   
    private char GetDirectionChar(int direction){
        if (direction == 0) return '^';
        if (direction == 1) return '>';
        if (direction == 2) return 'v';
        return '<';
    }

    private void ProcessQueueItem(D16QueueItem qi){

        // This is my problem.  A shorter path may not visit this first, it's already marked as visited.  Doesn't attempt
        var bestSoFar = this.Scores[qi.X][qi.Y];
        if (qi.Score > bestSoFar)
            return;
        this.Scores[qi.X][qi.Y] = qi.Score; // Mark visited by replacing with a wall so no one else tries to go here

        // If we can't beat the score from here, early return
        if (MinimumFinishedScore != null && qi.Score > this.MinimumFinishedScore.Score)
            return;
        
        TrySpread(qi.X - 1, qi.Y, North, qi);
        TrySpread(qi.X + 1, qi.Y, South, qi);
        TrySpread(qi.X, qi.Y - 1, West, qi);
        TrySpread(qi.X, qi.Y + 1, East, qi);
    }

    private void TrySpread(int x, int y, int direction, D16QueueItem qi)
    {
        if (x >= 0 && x < this.Rows && y >= 0 && y < this.Columns){

            var c = this.Lines[x][y];
            if (c == 'E' || c == '.'){

                string s = qi.State;
                long sc = qi.Score;
                if (direction != qi.Direction){
                    
                    s += 'T';                    
                    sc += 1000;
                    if (Math.Abs(direction - qi.Direction) == 2){
                        s += 'T';
                        sc += 1000;
                    }
                }

                s += (x*1000 + y).ToString() + '.';
                sc += 1;

                var newQI = new D16QueueItem{
                    Direction = direction,
                    Score = sc,
                    State = s,
                    X = x,
                    Y = y
                };

                if (c == 'E'){
                    if (this.MinimumFinishedScore == null || sc < this.MinimumFinishedScore.Score){
                        this.MinimumFinishedScore = newQI;
                    }
                }
                else{
                    this.MyQueue.Enqueue(newQI);
                }
            }
        }
    }

}