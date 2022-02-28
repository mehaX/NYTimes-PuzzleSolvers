using System.Linq;
using NUnit.Framework;

namespace ConsoleAppTest;

public class Tests
{
    private Solver mSolver;

    [SetUp]
    public void Setup()
    {
        mSolver = new Solver();
    }

    [Test]
    public void Test1()
    {
        var input = "these";
        var result = "ggbgg";

        mSolver.RegisterWord(input, result);
        
        Assert.IsFalse(mSolver.GetWords().Any(w => w == "these"));
    }
}