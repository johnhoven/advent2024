using System.Diagnostics;
using System.Globalization;
using System.IO.Pipelines;
using System.Numerics;
using System.Xml;

namespace Advent2024;

public class D11P02
{      
    Dictionary<long, long> StoneCount = new Dictionary<long, long>();

    private void Increment(long stoneId, long incrementBy = 1){
        if (!StoneCount.TryGetValue(stoneId, out long count))
            count = 0;
        count = count + incrementBy;
        StoneCount[stoneId] = count;
    }

    private void Blink(){
        var currentDict = StoneCount;
        StoneCount = new Dictionary<long, long>();

        foreach(var stone in currentDict){
            var strStone = stone.Key.ToString();
            if (stone.Key == 0)
                Increment(1, stone.Value);
            else if (strStone.Length % 2 == 0){
                Increment( long.Parse( strStone.Substring(0, (strStone.Length / 2)) ), stone.Value);
                Increment( long.Parse( strStone.Substring((strStone.Length / 2)) ), stone.Value);
            }
            else{
                Increment(stone.Key * 2024, stone.Value);
            }
        }

    }

    private void Blink75(){
        for(int i = 0; i < 75; i++){
            Blink();
        }
    }

    private long Count(){
        long sum = 0;
        foreach(var item in StoneCount){
            sum += item.Value;
        }
        return sum;
    }

    public long Process(string filename){                
        using (var r = FileUtil.Open(filename)){         
            string? line = r.ReadLine();
            
            while (line != null){
                 
                var stones = line.Split(new char[] { ' '}, StringSplitOptions.RemoveEmptyEntries);
                foreach(var stone in stones){
                    Increment(int.Parse(stone));
                }

                line = r.ReadLine();
            }            
        }
        
        Blink75();
        return Count();
    }   

}