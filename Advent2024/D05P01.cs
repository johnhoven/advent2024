using System.Diagnostics;
using System.Numerics;

namespace Advent2024;

public static class D05P01
{   
    public static int Process(string filename){        
        Dictionary<int, List<int>> rules = new Dictionary<int, List<int>>();
        List<string> updateLines = new List<string>();
        using (var r = FileUtil.Open(filename)){         
            string? line = r.ReadLine();         
            bool isRules = false;
            while (line != null){            

                if (line == string.Empty)
                {
                    isRules = true;
                }
                else if (!isRules){
                    var split = line.Split(new char[] { '|'}, StringSplitOptions.RemoveEmptyEntries);
                    var n = int.Parse(split[0]);
                    if (!rules.TryGetValue(n, out List<int>? list))
                    {
                        list = new List<int>();
                        rules[n] = list;
                    }
                    list.Add(int.Parse(split[1]));
                }
                else if (isRules){
                    updateLines.Add(line);
                }                
              
                line = r.ReadLine();
            }             
        }
       return ProcessUpdates(rules, updateLines);
    }

    private static int ProcessUpdates(Dictionary<int, List<int>> rules, List<string> updates)
    {
        int result = 0;
        foreach(var update in updates){
            result += ProcessUpdate(rules, update);
        }
        return result;
    }

    private static int ProcessUpdate(Dictionary<int, List<int>> rules, string update)
    {
        var pages = Array.ConvertAll<string, int>( update.Split(','), new Converter<string, int>( (a) => int.Parse(a) ));

        // Start at last of page
            // Get my list of pages that appear after me
            // Check each prior page for presence
            
        for (int i = pages.Length - 1; i >= 0; i--)
        {
            int n = pages[i];
            if (!rules.TryGetValue(n, out List<int>? dependencies))
                continue;
            
            for (int j = 0; j < i; j++){
                if (dependencies.Contains(pages[j]))
                    return 0;
            }
        }
        var middlePage = pages [ pages.Length / 2 ];
        Console.WriteLine($"Line {update} - middle: {middlePage}");
        return middlePage;
    }

}