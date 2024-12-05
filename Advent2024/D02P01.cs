namespace Advent2024;

public static class D02P01
{
    public static int Process(string filename){
        using (var r = FileUtil.Open(filename)){     
            string? line = r.ReadLine();
           
            int increasingCount = 0;

            while (line != null){            
                
                if (CheckLineConditions(line))
                increasingCount++;
                
                line = r.ReadLine();
            }

            
            
            return increasingCount;
        }
    }

    public static bool CheckLineConditions(string line){
        var chars = line.Split(new char[] { ' '}, StringSplitOptions.RemoveEmptyEntries);
        var lastChar = Int32.Parse(chars[0]);
        bool? increasing = null;
        for(int i = 1; i < chars.Length; i++){
            var n = Int32.Parse(chars[i]);

            if (increasing == null){
                if (n > lastChar)
                    increasing = true;
                else if (n < lastChar)
                    increasing = false;
                else
                    return false;
            }

            if (increasing.Value && ((n - lastChar) < 1 || (n - lastChar) > 3))
                return false;
            if (!increasing.Value && ((lastChar - n) < 1 || (lastChar - n) > 3))
                return false;

                lastChar = n;
        }

        return true;
    }
}
