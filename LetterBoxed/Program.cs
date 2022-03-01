Run(args);

void Run(params string[] args)
{
    var letters = args.Any() ? args[0] : Input("Enter letters", out _);
    var solver = new Solver(letters);
    var results = new List<List<string>>();
    var maxWords = 4;
    
    DFS(solver, maxWords, ref results);
}

void DFS(Solver solver, int maxWords, ref List<List<string>> results)
{
    var filteredWords = solver.Filter().ToList();
    if (!filteredWords.Any())
    {
        return;
    }
    
    foreach (var word in filteredWords)
    {
        solver.RegisterWord(word);
        if (solver.IsSolved())
        {
            Console.WriteLine(string.Join(", ", solver.Words));
            results.Add(solver.Words);
        }
        else if (solver.Words.Count < maxWords)
        {
            DFS(solver, maxWords, ref results);
        }
        solver.Undo();
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

internal class Solver
{
    private readonly List<string> mAllWords = new();
    private readonly char[] mLetters;
    private readonly List<string> mWords = new();

    public List<string> Words => mWords;
    
    private char? ToBeginWith => mWords.Any() ? mWords.Last().Last() : null;

    public Solver(string letters)
    {
        mAllWords = mAllWords.Concat(File.ReadAllLines("5-letter-words.txt")).ToList();
        mAllWords = mAllWords.Concat(File.ReadAllLines("6-letter-words.txt")).ToList();
        mAllWords = mAllWords.Concat(File.ReadAllLines("7-letter-words.txt")).ToList();
        mAllWords = mAllWords.Concat(File.ReadAllLines("8-letter-words.txt")).ToList();
        mAllWords = mAllWords.Shuffle().Select(w => w.ToUpper()).ToList();
        mLetters = letters.ToUpper().ToArray();
    }

    public void RegisterWord(string word)
    {
        word = word.ToUpper();
        ValidateWord(word);

        mWords.Add(word);
    }

    public void Undo()
    {
        if (mWords.Any())
        {
            mWords.RemoveAt(mWords.Count - 1);
        }
    }

    public bool IsSolved()
    {
        foreach (var letter in mLetters)
        {
            var contains = false;
            
            foreach (var word in mWords)
            {
                if (word.Contains(letter))
                {
                    contains = true;
                    break;
                }
            }

            if (!contains)
            {
                return false;
            }
        }

        return true;
    }

    public IEnumerable<string> Filter()
    {
        var letters = mLetters.ToList();
        foreach (var word in mAllWords)
        {
            if (ToBeginWith != null && !word.StartsWith(ToBeginWith.Value))
            {
                continue;
            }

            var shouldContinue = false;
            for (var i = 0; i < word.Length - 1; i++)
            {
                var index1 = letters.IndexOf(word[i]);
                var index2 = letters.IndexOf(word[i + 1]);

                if (index1 == -1 || index2 == -1)
                {
                    shouldContinue = true;
                    break;
                }

                if (index1 / 3 == index2 / 3)
                {
                    shouldContinue = true;
                    break;
                }
            }

            if (shouldContinue)
            {
                continue;
            }

            if (mWords.Contains(word))
            {
                continue;
            }

            yield return word;
        }
    }

    private void ValidateWord(string word)
    {
        if (mWords.Any() && ToBeginWith != word.First())
        {
            throw new Exception($"Expected to begin with letter: {ToBeginWith}, got {word.First()}");
        }

        if (!mAllWords.Contains(word))
        {
            throw new Exception("Word not found!");
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

