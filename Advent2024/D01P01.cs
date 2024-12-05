namespace Advent2024;

public static class D01P01
{
    public static int Process(string filename){
        using (var r = FileUtil.Open(filename)){
            string p1, p2;            
            string? line = r.ReadLine();
            List<int> one = new List<int>();
            List<int> two = new List<int>();
            while (line != null){            
                var parts = line.Split(new char[] { ' '}, StringSplitOptions.RemoveEmptyEntries);
                p1 = parts[0];
                p2 = parts[1];
                one.Add( Int32.Parse(p1));
                two.Add( Int32.Parse(p2));
                line = r.ReadLine();
            }

            one.Sort();
            two.Sort();

            int runningDiff = 0;
            for(int i = 0; i < one.Count; i++){
                runningDiff += Math.Abs( one[i] - two[i] );
            }
            
            return runningDiff;
        }
    }
}
