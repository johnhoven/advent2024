namespace Advent2024;

public static class D02P02
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

    public static bool CheckLineConditions(string line, int skipI = -1){

        Console.WriteLine((skipI > -1 ? "  " : "") +  line);

        var chars = line.Split(new char[] { ' '}, StringSplitOptions.RemoveEmptyEntries);
        var lastChar = Int32.Parse(chars[0]);
        int i = 1;
        if (skipI == 0){
            lastChar = Int32.Parse(chars[1]);
            i = 2;
        }
        bool? increasing = null;
        bool skipped = skipI > -1;
        for(; i < chars.Length; i++){

            if (i == skipI)
                continue;

            var n = Int32.Parse(chars[i]);

            if (increasing == null){
                if (n > lastChar)
                    increasing = true;
                else if (n < lastChar)
                    increasing = false;
                else if (!skipped)
                {
                    // Skip previous character
                    if (skipI == -1 && i > 1 && CheckLineConditions( line , i - 1))
                        return true;
                    // Skip first character
                    if (skipI == -1 && CheckLineConditions( line , 0))
                        return true;
                    // Skip this character
                    skipped = true;
                    increasing = null;
                    continue;   
                }                
                else
                    return false;
            }

            if ((increasing.Value && ((n - lastChar) < 1 || (n - lastChar) > 3)) || (!increasing.Value && ((lastChar - n) < 1 || (lastChar - n) > 3)))
            {
                if (!skipped)
                {
                    // Skip previous character
                    if (skipI == -1 && i > 1 && CheckLineConditions( line , i - 1))
                        return true;
                    // Skip first character
                    if (skipI == -1 && CheckLineConditions( line , 0))
                        return true;
                    // Skip this character
                    skipped = true;
                    continue;
                }                   
                else
                    return false;
            }
            
            lastChar = n;
        }

        return true;
    }
}
