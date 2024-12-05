namespace Advent2024;

public static class D01P02
{
    public static int Process(string filename){
        using (var r = FileUtil.Open(filename)){
            string p1, p2;            
            string? line = r.ReadLine();
            List<int> one = new List<int>();
            Dictionary<int, int> two = new Dictionary<int, int>();
            while (line != null){            
                var parts = line.Split(new char[] { ' '}, StringSplitOptions.RemoveEmptyEntries);
                p1 = parts[0];
                p2 = parts[1];
                one.Add( Int32.Parse(p1));
                int i2 = Int32.Parse(p2);
                if (!two.TryGetValue(i2, out int v))
                    v = 0;
                two[i2] = v + 1;
                
                line = r.ReadLine();
            }

            int similarityScore = 0;
            for(int i = 0; i < one.Count; i++){
                if (!two.TryGetValue(one[i], out int v))
                    v = 0;
                similarityScore += one[i] * v;
            }
            
            return similarityScore;
        }
    }
}
