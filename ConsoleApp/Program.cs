
Run();

void Run()
{
    var solver = new Solver();

    while (true)
    {
        Console.Clear();
        solver.Print();

        var word = Input("Add word", out var isExit).ToUpper();
        if (isExit)
        {
            break;
        }
        
        var result = Input("Result", out isExit);
        if (isExit)
        {
            break;
        }
        
        solver.RegisterWord(word, result);
    }
}

string Input(string promptMessage, out bool isExit)
{
    string? word = null;
    
    while (string.IsNullOrEmpty(word))
    {
        Console.Write($"{promptMessage}: ");
        word = Console.ReadLine();
    }

    isExit = word == "exit";

    return word;
}

public class Solver
{
    private readonly List<string> mWords = File.ReadAllLines("words.txt").Shuffle().Select(w => w.ToUpper()).ToList();
    private readonly List<(int index, char c)> mGreens = new();
    private readonly List<(int index, char c)> mYellows = new();
    private readonly List<char> mBlacks = new();
    
    public void RegisterWord(string word, string result)
    {
        if (word.Length != 5 || result.Length != 5)
        {
            throw new Exception("Invalid input");
        }
        
        for (var i = 0; i < 5; i++)
        {
            var letter = word[i];
            switch (result[i])
            {
                case 'b':
                case 'B':
                    AddBlack(letter);
                    break;
                
                case 'y':
                case 'Y':
                    AddYellow(i, letter);
                    break;
                
                case 'g':
                case 'G':
                    AddGreen(i, letter);
                    break;
                
                default:
                    throw new Exception("Invalid result");
            }
        }

    }

    public IEnumerable<string> GetWords()
    {
        return mWords.Take(10);
    }

    private void AddYellow(int index, char c)
    {
        if (!mYellows.Contains((index, c)))
        {
            mYellows.Add((index, c));
            mBlacks.Remove(c);
        }
    }

    private void AddGreen(int index, char c)
    {
        if (!mGreens.Contains((index, c)))
        {
            mGreens.Add((index, c));
            mBlacks.Remove(c);
        }
    }

    private void AddBlack(char c)
    {
        if (!mBlacks.Contains(c) && mGreens.All(g => g.c != c) && mYellows.All(y => y.c != c))
        {
            mBlacks.Add(c);
        }
    }

    public void Print()
    {
        var words = Filter().Take(10);
        Console.WriteLine("Word list:");
        foreach (var word in words)
        {
            Console.WriteLine(word);
        }

        Console.WriteLine();
    }

    private IEnumerable<string> Filter()
    {
        foreach (var word in mWords)
        {
            if (mBlacks.Any(b => word.Contains(b)))
            {
                continue;
            }

            if (mYellows.Any(y => !word.Contains(y.c) || word[y.index] == y.c))
            {
                continue;
            }
            
            if (mGreens.Any(g => word[g.index] != g.c))
            {
                continue;
            }
            
            yield return word;
        }
    }
}


static class Extensions
{
    private static readonly Random Random = new();
    
    public static IList<T> Shuffle<T>(this IList<T> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = Random.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }

        return list;
    }
}
