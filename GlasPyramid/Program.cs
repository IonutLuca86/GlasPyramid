
/* Föreställer en pyramid av identiska glas, där den översta raden består av ett glas, raden under av två glas och raden därunder av tre glas osv. Vatten rinner ner i det översta 
 * glaset i jämn hastighet. Det tar exakt 10 sekunder att fylla glaset. När glaset är fullt och svämmar över kommer det tillrinnande vattnet att rinna ner i de två glasen på rad två, 
 * lika snabbt i varje. Efter ytterligare 20 sekunder kommer således de två glasen på rad två att vara fyllda. Glasen på rad tre kommer dock inte fyllas lika snabbt, då vatten rinner
 * ner i det mittersta glaset dubbelt så snabbt som i de andra två glasen på samma rad. Skriv ett program som bestämmer efter hur många sekunder ett givet glas i pyramiden börjar rinna
 * över. Programmet ska fråga efter en rad r (2 ≤ r ≤ 50) och vilket glas g på raden räknat från vänster (1 ≤ g ≤ r ). Svaret ska vara korrekt med minst 3 decimalers noggrannhet, utom 
 * för de allra högsta tiderna där hänsyn tas till begränsad numerisk precision (men se till att använda double) */

// Ionut Luca

Console.WriteLine("Ange rad (r) : ");
int rad = GetInput();
Console.WriteLine("Ange glas (g) : ");
int glas = GetInput();

Console.WriteLine($"Glas {glas} från rad {rad} börjar översvämma efter : {ChampagneTower(rad, glas)} sekunder");


int GetInput(){
    int returnInput = 0;
    string input = Console.ReadLine();
    bool result = int.TryParse(input, out returnInput);
    if (!result && returnInput >= 1 && returnInput < 50 )   
    {
        Console.WriteLine("Fel input! Ange ett nummer mellan 2 och 50 for rad och mellan 1 och 50 for glas!");
        return GetInput();
    }
    return returnInput;   
}

//den här funktionen går över glas pyramiden och fördelar överflödeshastighet till varje glas under. Dessa värde tillsammans med information om föreldrar sparas i en variabel
//för att senare i foreach loopen användas för att räkna totala tiden för varje glas
// timeToFill räknas som tiden för att fylla första glas men också som hur många måttenheter behövs för att fylla en glas
double ChampagneTower(int rad, int glas)
{
    double timeToFill = 10.0;
    int limit = 6;
    double[][] glasPyramid = new double[limit][];
    Dictionary<(int, int), CupData> overflowValues = new Dictionary<(int, int), CupData>();
    Dictionary<(int, int), double> fillTimes = new Dictionary<(int, int), double>();

    overflowValues[(0, 0)] = new CupData { OverflowValue = 1, Parents = new HashSet<(int, int)>() };    
    fillTimes[(0, 0)] = timeToFill;

    for (int i = 0; i < limit; i++)
    {
        glasPyramid[i] = new double[i + 1];
    }
    glasPyramid[0][0] = 10.0;

    for (int row = 0; row < limit; row++)
    {
        for (int cup = 0; cup <= row; cup++)
        {
            
                var leftPosition = (row + 1, cup);
                var rightPosition = (row + 1, cup + 1);
                double overflow = overflowValues[(row, cup)].OverflowValue / 2.0;
                if (!overflowValues.ContainsKey(leftPosition))
                {
                    overflowValues[leftPosition] = new CupData { OverflowValue = overflow, Parents = new HashSet<(int, int)>() { (row, cup) } };
                  
                }
                else
                {
                    if (!overflowValues[leftPosition].Parents.Contains((row, cup)))
                    {
                        overflowValues[leftPosition].OverflowValue += overflow;
                        overflowValues[leftPosition].Parents.Add((row, cup));                      
                    }
                }
                if (!overflowValues.ContainsKey(rightPosition))
                {
                    overflowValues[rightPosition] = new CupData { OverflowValue = overflow, Parents = new HashSet<(int, int)>() { (row, cup) } };               
                }
                else
                {
                    if (!overflowValues[rightPosition].Parents.Contains((row, cup)))
                    {
                        overflowValues[rightPosition].OverflowValue += overflow;
                        overflowValues[rightPosition].Parents.Add((row, cup));                      
                    }

                }            
        }
    }

    foreach(var kvp in overflowValues)
    {
        var position = kvp.Key;
        HashSet<(int, int)> parents = kvp.Value.Parents;    
        double totalTimeToFill = CalculateTime(parents, fillTimes, overflowValues, kvp.Value.OverflowValue,timeToFill);       
        fillTimes[position] = totalTimeToFill;        
    }
    return fillTimes[(rad - 1, glas - 1)];
}

//den här funktionen räknar tiden för varje glas. Glas 1 har en förbästemd tid. För alla glas som har en enda "förälder" används förälder tiden + tiden som behövs att översvämninghastigheten
//ska fylla enheten full
//För dessa glas som fylls av 2 andra glas, om tidskillnaden mellan föräldrar tiderna är mindre än tiden som skulle behövas att glasen som har en större overflow kan fylla samma glas,
//då räknas bara denna färälder tiden och tiden det tar för att den ska fylla glasen
//annars, räknas skillnad tiden mellan föröldrar + extraherars hur mycket kan förälder med större overflow kan fylla i tidskillnaden från måttenheterna som behövs för att fylla full glasen
//och delar allt till sammanlagd overflow (för att räkna hur länge det tar att båda föräldrar ska fylla glasen) + minsta förälder tid
static double CalculateTime(HashSet<(int, int)> parents, Dictionary<(int, int), double> fillTimes, Dictionary<(int, int), CupData> overflowValues,double overflow, double timeToFill)
{
    double maxTime = 0;
    if (parents.Count == 0)
        maxTime = timeToFill;
    else if(parents.Count == 1)
    {
        var parent = parents.FirstOrDefault();
        maxTime = fillTimes[parent] + timeToFill/overflow;
    }
    else
    {
        var parentsArray = parents.ToArray();
        double firstParentTime = fillTimes[parentsArray[0]];
        double secondParentTime = fillTimes[parentsArray[1]];
        double overflowFirstParent = overflowValues[parentsArray[0]].OverflowValue;
        double overflowSecondParent = overflowValues[parentsArray[1]].OverflowValue;

        double minParentTime = Math.Min(firstParentTime, secondParentTime);
        double maxParentTime = Math.Max(firstParentTime, secondParentTime);
        double minOverflow = Math.Min(overflowFirstParent, overflowFirstParent);
        double maxOverflow = Math.Max(overflowFirstParent, overflowSecondParent);

        double timeDifference = maxParentTime-minParentTime;
        double timeToFillByMaxOverflow = timeToFill / (maxOverflow / 2);

        if(timeToFillByMaxOverflow <= timeDifference)
        {
            maxTime = minParentTime + timeToFillByMaxOverflow;
        }
        else
        {
            maxTime = timeDifference + ((timeToFill - (maxOverflow / 2 * timeDifference)) / overflow) + minParentTime;
        }
            
    }
    

    return maxTime;
}

public class CupData
{
    public double OverflowValue { get; set; }
    public HashSet<(int, int)> Parents { get; set; }

}