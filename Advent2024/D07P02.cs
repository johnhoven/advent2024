using System.Diagnostics;
using System.IO.Pipelines;
using System.Numerics;

namespace Advent2024;

public static class D07P02
{   

    public static long Process(string filename){                

        long total = 0;
        using (var r = FileUtil.Open(filename)){         
            string? line = r.ReadLine();                     
            
            while (line != null){

                var split1 = line.Split(new char[] { ':'}, StringSplitOptions.RemoveEmptyEntries);
                long desired = long.Parse(split1[0]);
                var nums = Array.ConvertAll<string, int>(  split1[1].Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries), (a) => Int32.Parse(a) );

                if (ProcessLine(desired, nums))
                    total += desired;

                line = r.ReadLine();
            }

            
        }

       
       
        return total;
    }   

    private static bool ProcessLine(long desired, int[] items)
    {
        List<long> runningTotals = new List<long>();
        runningTotals.Add(items[0]);

        for(int i = 1; i < items.Length; i++){
            List<long> newList = new List<long>();
            var newNum = items[i];

            foreach(var item in runningTotals){
                var add = item + newNum;
                var mult = item * newNum;
                var concat = long.Parse( item.ToString() + newNum.ToString() );

                if (add <= desired)
                    newList.Add(add);
                if (mult <= desired)
                    newList.Add(mult);
                if (concat <= desired)
                    newList.Add(concat);
            }

            runningTotals = newList;
            if (runningTotals.Count == 0)
                return false;
        }

        return runningTotals.Contains(desired);
    }

}