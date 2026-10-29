Random random = new Random();

int heroHp = 20;
int heroArmor = 12;
int heroDie = 6;

int goblinHp = 10;
int goblinArmor = 10;
int goblinDie = 4;

List<string> log = [];
int round = 0;
bool fled = false;

int Roll(int sides)
{
    return random.Next(1, sides + 1);
}

Console.WriteLine("A goblin jumps out of the bushes!");

while (heroHp > 0 && goblinHp > 0)
{
    round++;
    Console.WriteLine();
    Console.WriteLine($"Round {round} | Hero HP: {heroHp} | Goblin HP: {goblinHp}");
    Console.Write("What do you do? (attack/defend/flee): ");
    
    string? input = Console.ReadLine();
    string action = (input ?? "").Trim().ToLower();
    bool defending = false;
    
    if (action == "attack")
    {
        int toHit = Roll(20);
        if (toHit >= goblinArmor)
        {
            int damage = Roll(heroDie);
            goblinHp -= damage;
            Console.WriteLine($"You roll {toHit} and hit the goblin for {damage} damage."); 
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
        
        Console.WriteLine($"You roll {escapeRoll}. The goblin blocks your way.");
        log.Add($"Round {round}: You failed to flee");
    }
    else
    {
        Console.WriteLine("You hesitate. The goblin doesn't.");
        log.Add($"Round {round}: You hesitated");
    }

    if (goblinHp <= 0)
    {
        break;
    }

    int goblinToHit = Roll(20);
    if (goblinToHit >= heroArmor)
    {
        int goblinDamage = Roll(goblinDie);
        if (defending)
        {
            goblinDamage /= 2;
        }
        
        heroHp -= goblinDamage;
        Console.WriteLine($"The goblin rolls {goblinToHit} and hits you for {goblinDamage} damage.");
        log.Add($"Round {round}: goblin hit for {goblinDamage}");
    }
    else
    {
        Console.WriteLine($"The goblin rolls {goblinToHit} and misses.");
        log.Add($"Round {round}: goblin missed");
    }
}

if (fled)
{
    Console.WriteLine("You live to fight another day.");
}
else if (heroHp <= 0)
{
    Console.WriteLine("You have been defeated.");
}
else
{
    Console.WriteLine("The goblin is defeated!");
}

Console.WriteLine();
Console.WriteLine("Combat log:");
foreach (var entry in log)
{
    Console.WriteLine(entry);
}