string heroName = "Hero";
string enemyName = "goblin";
int seed = Random.Shared.Next();

for (int i = 0; i < args.Length; i++)
{
    string arg = args[i];
    bool hasValue = i + 1 < args.Length;

    if (arg == "-h" || arg == "--help")
    {
        Console.WriteLine("""
            Usage: dotnet run -- [options]
            
            Options:
                --hero <name>       Your hero's name (default: Hero)
                --enemy <name>     goblin, orc or dragon (default: goblin)
                --seed <number>     Replay a fight with the same dice rolls
                -h, --help          Show this help                  
            """);
        return 0;
    }
    else if (arg == "--hero" && hasValue)
    {
        heroName = args[i + 1];
        i++;
    }
    else if (arg == "--enemy" && hasValue)
    {
        enemyName =  args[i + 1].ToLower();
        i++;
    }
    else if (arg == "--seed" && hasValue)
    {
        if (!int.TryParse(args[i + 1], out seed))
        {
            Console.WriteLine($"--seed needs a whole number, not '{args[i + 1]}'.");
            return 1;
        }
        i++;
    }
    else
    {
        Console.WriteLine($"Unknown or incomplete option: {arg}");
        Console.WriteLine("Run with --help to see the options.");
        return 1;
    }
}

int enemyHp;
int enemyArmor;
int enemyDie;
string intro;

switch (enemyName)
{
    case "goblin":
        enemyHp = 10;
        enemyArmor = 10;
        enemyDie = 4;
        intro = "A goblin jumps out of the bushes!";
        break;
    case "orc":
        enemyHp = 16;
        enemyArmor = 12;
        enemyDie = 6;
        intro = "An orc blocks the road!";
        break;
    case "dragon":
        enemyHp = 40;
        enemyArmor = 16;
        enemyDie = 10;
        intro = "A dragon lands in front of you. This might be a mistake.";
        break;
    default:
        Console.WriteLine($"Unknown enemy: {enemyName}. Choose goblin, orc or dragon.");
        return 1; 
}

Random random = new Random(seed);

int heroHp = 20;
int heroArmor = 12;
int heroDie = 6;

List<string> log = [];
int round = 0;
bool fled = false;

int Roll(int sides)
{
    return random.Next(1, sides + 1);
}

Console.WriteLine(intro);

while (heroHp > 0 && enemyHp > 0)
{
    round++;
    Console.WriteLine();
    Console.WriteLine($"Round {round} | {heroName} HP: {heroHp} | {enemyName} HP: {enemyHp}");
    Console.Write("What do you do? (attack/defend/flee): ");

    string? input = Console.ReadLine();
    string action = (input ?? "").Trim().ToLower();
    bool defending = false;
    
    if (action == "attack")
    {
        int toHit = Roll(20);
        if (toHit >= enemyArmor)
        {
            int damage = Roll(heroDie);
            enemyHp -= damage;
            Console.WriteLine($"You roll {toHit} and hit the {enemyName} for {damage} damage."); 
            log.Add($"Round {round}: You hit for {damage}");
        }
        else
        {
            Console.WriteLine($"You roll {toHit} and miss.");
            log.Add($"Round {round}: You missed");
        }
    }
    else if (action == "defend")
    {
        defending = true;
        Console.WriteLine("You raise your shield.");
        log.Add($"Round {round}: You defended");
    }
    else if (action == "flee")
    {
        int escapeRoll = Roll(20);
        if (escapeRoll >= 10)
        {
            fled = true;
            Console.WriteLine($"You roll {escapeRoll} and escape!");
            log.Add($"Round {round}: You fled");
            break;
        }
        
        Console.WriteLine($"You roll {escapeRoll}. The {enemyName} blocks your way.");
        log.Add($"Round {round}: You failed to flee");
    }
    else
    {
        Console.WriteLine($"You hesitate. The {enemyName} doesn't.");
        log.Add($"Round {round}: You hesitated");
    }
    
    if (enemyHp <= 0)
    {
        break;
    }
        
    int enemyToHit = Roll(20);
    if (enemyToHit >= heroArmor)
    {
        int enemyDamage = Roll(enemyDie);
        if (defending)
        {
            enemyDamage /= 2;
        }
        
        heroHp -= enemyDamage;
        Console.WriteLine($"The {enemyName} rolls {enemyToHit} and hits you for {enemyDamage} damage.");
        log.Add($"Round {round}: {enemyName} hit for {enemyDamage}");
    }
    else
    {
        Console.WriteLine($"The {enemyName} rolls {enemyToHit} and misses.");
        log.Add($"Round {round}: {enemyName} missed");
    }
}
        
if (fled)
{
    Console.WriteLine($"{heroName} lives to fight another day.");
}
else if (heroHp <= 0)
{
    Console.WriteLine($"{heroName} has been defeated.");
}
else
{
    Console.WriteLine($"The {enemyName} is defeated!");
}
        
Console.WriteLine();
Console.WriteLine("Combat log:");
foreach (var entry in log)
{
    Console.WriteLine(entry);
}

Console.WriteLine();
Console.WriteLine($"Seed: {seed} (use --seed {seed} to replay this fight)");
return 0;


