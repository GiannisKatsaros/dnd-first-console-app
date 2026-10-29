int heroHp = 20;
int goblinHp = 10;
int heroDamage = 4;
int goblinDamage = 3;

Console.WriteLine("A goblin jumps out of the bushes!");
Console.WriteLine($"Hero HP: {heroHp} | Goblin HP: {goblinHp}");
Console.Write("What do you do? (attack/defend): ");

string? input = Console.ReadLine();
string action = (input ?? "").Trim().ToLower();

if (action == "attack")
{
    goblinHp -= heroDamage;
    Console.WriteLine($"You hit the goblin for {heroDamage} damage.");
}
else if (action == "defend")
{
    goblinDamage /= 2;
    Console.WriteLine("You raise your shield.");
}
else
{
    Console.WriteLine("You hesitate. The goblin doesn't.");
}

heroHp -= goblinDamage;
Console.WriteLine($"The goblin hits you for {goblinDamage} damage.");
Console.WriteLine($"Hero HP: {heroHp} | Goblin HP: {goblinHp}");