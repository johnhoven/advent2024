namespace Advent2024;

public static class D03P02
{
    public static Int64 Process(string filename){
        using (var r = FileUtil.Open(filename)){
         
            string? line = r.ReadLine();
           
            bool domul = true;
            Int64 result = 0;
            while (line != null){            

                int state = -2;
                int i = 0;
                int digit1 = 0;
                int digit2 = 0;
                while ( i < line.Length){
                    char c = line[i];

                    if (c == 'm'){
                        state = -1;                    
                        digit1 = 0;
                        digit2 = 0;
                    }
                    else if (c == 'd'){
                        state = 100;
                    }
                    else if (state == 100 && c == 'o'){
                        state = 101;
                    }
                    else if (state == 101 && c == '('){
                        state = 102;
                    }
                    else if (state == 101 && c == 'n'){
                        state = 200;
                    }
                    else if (state == 102 && c == ')'){
                        state = -2;
                        domul = true;
                        Console.Write("do()");
                    }
                    else if (state == 200 && c == '\''){
                        state = 201;
                    }
                    else if (state == 201 && c == 't'){
                        state = 202;
                    }
                    else if (state == 202 && c == '('){
                        state = 203;
                    }
                    else if (state == 203 && c == ')'){
                        state = -2;
                        domul = false;
                        Console.Write("don't()");
                    }
                    else if (state == -1 && c == 'u')
                        state = 0;                    
                    else if (state == 0 && c == 'l')
                        state = 1;
                    else if (state == 1 && c == '(')
                        state = 2;
                    else if (state == 2 && char.IsAsciiDigit(c) )
                    {
                        state = 3;
                        digit1 = (int)Char.GetNumericValue(c);
                    }
                    else if (state == 3 && char.IsAsciiDigit(c))                                            
                        digit1 = digit1 * 10 + (int)Char.GetNumericValue(c);
                    else if (state == 3 && c == ',')                                            
                        state = 4;
                    else if (state == 4 && char.IsAsciiDigit(c) )
                    {
                        state = 5;
                        digit2  = (int)Char.GetNumericValue(c);
                    }
                    else if (state == 5 && char.IsAsciiDigit(c))                                            
                        digit2 = digit2 * 10 + (int)Char.GetNumericValue(c);
                    else if (state == 5 && c == ')')                                            
                    {
                        state = -2;
                        if (domul){
                            Console.Write($"mul({digit1},{digit2})");
                            result += ( digit1 * digit2 );
                        }
                    }
                    else 
                        state = -2;
                    

                    i++;
                }


                line = r.ReadLine();
            }

             return result;
        }
       
    }

}