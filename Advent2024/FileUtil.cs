namespace Advent2024;

public static class FileUtil
{
    public const string BasePath = @"H:\Code\advent2024\Advent2024\Exclude\";

    public static StreamReader Open(string filename){
        string fileNameAndPath = BasePath + filename;
        return File.OpenText(fileNameAndPath);
    }
}


