

public  interface IMatchFinder
{
    public Drop Drop { get; set; }
    public bool FindMatch(bool markIsMatched);
}
