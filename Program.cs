using System.CommandLine;
using Spectre.Console;

Option<string> heroOption = new("--hero")
{
    Description = "Your hero's name",
    DefaultValueFactory = _ => "Hero"
};

Option<string> enemyOption = new("--enemy")
{
    Description = "The enemy to fight",
    DefaultValueFactory = _ => "goblin"
};
enemyOption.AcceptOnlyFromAmong("goblin", "orc", "dragon");

Option<int?> seedOption = new("--seed")
{
    Description = "Replay a fight with the same dice rolls"
};

RootCommand rootCommand = new("Fight a goblin, an orc or a dragon in your terminal.");
rootCommand.Options.Add(heroOption);
rootCommand.Options.Add(enemyOption);
rootCommand.Options.Add(seedOption);

ParseResult parseResult = rootCommand.Parse(args);
if (parseResult.Action is not null)
{
    return parseResult.Invoke();
}

string heroName = Markup.Escape(parseResult.GetRequiredValue(heroOption));
string enemyName = parseResult.GetRequiredValue(enemyOption);
int seed = parseResult.GetValue(seedOption) ?? Random.Shared.Next();

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
    default:
        enemyHp = 40;
        enemyArmor = 16;
        enemyDie = 10;
        intro = "A dragon lands in front of you. This might be a mistake.";
        break;
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

AnsiConsole.Write(new FigletText(enemyName).Color(Color.Red));
AnsiConsole.MarkupLine($"[bold]{intro}[/]");

while (heroHp > 0 && enemyHp > 0)
{
    round++;
    AnsiConsole.WriteLine();
    AnsiConsole.Write(new Rule($"Round {round}"));
    AnsiConsole.Write(new BarChart()
        .Width(50)
        .AddItem(heroName, heroHp, Color.Green)
        .AddItem(enemyName, enemyHp, Color.Red));

    string action = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("What do you do?")
            .AddChoices("attack", "defend", "flee"));
    bool defending = false;
    
    if (action == "attack")
    {
        int toHit = Roll(20);
        if (toHit >= enemyArmor)
        {
            int damage = Roll(heroDie);
            enemyHp -= damage;
            AnsiConsole.MarkupLine($"You roll [bold]{toHit}[/] and [green]hit the {enemyName} for {damage} damage[/]."); 
            log.Add($"Round {round}: You hit for {damage}");
        }
        else
        {
            AnsiConsole.MarkupLine($"You roll [bold]{toHit}[/] and [grey]miss[/].");
            log.Add($"Round {round}: You missed");
        }
    }
    else if (action == "defend")
    {
        defending = true;
        AnsiConsole.MarkupLine("You raise your [blue]shield[/].");
        log.Add($"Round {round}: You defended");
    }
    else if (action == "flee")
    {
        int escapeRoll = Roll(20);
        if (escapeRoll >= 10)
        {
            fled = true;
            AnsiConsole.MarkupLine($"You roll [bold]{escapeRoll}[/] and [yellow]escape![/]");
            log.Add($"Round {round}: You fled");
            break;
        }
        
        AnsiConsole.MarkupLine($"You roll [bold]{escapeRoll}[/]. The {enemyName} blocks your way.");
        log.Add($"Round {round}: You failed to flee");
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
        AnsiConsole.MarkupLine($"The {enemyName} rolls [bold]{enemyToHit}[/] and [red]hits you for {enemyDamage} damage[/].");
        log.Add($"Round {round}: {enemyName} hit for {enemyDamage}");
    }
    else
    {
        AnsiConsole.MarkupLine($"The {enemyName} rolls [bold]{enemyToHit}[/] and [grey]misses[/].");
        log.Add($"Round {round}: {enemyName} missed");
    }
}
        
AnsiConsole.WriteLine();
if (fled)
{
    AnsiConsole.MarkupLine($"[yellow]{heroName} lives to fight another day.[/]");
}
else if (heroHp <= 0)
{
    AnsiConsole.MarkupLine($"[red]{heroName} has been defeated.[/]");
}
else
{
    AnsiConsole.MarkupLine($"[green]The {enemyName} is defeated![/]");
}
        
AnsiConsole.Write(new Panel(new Text(string.Join(Environment.NewLine, log)))
    .Header("Combat log"));

AnsiConsole.MarkupLine($"[grey]Seed: {seed} (use --seed {seed} to replay this fight)[/]");
return 0;


