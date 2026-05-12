namespace RPGGame.Models;

public class Monstre : Personnage
{
    public int Recompense { get; }

    public override bool   EstHeros  => false;
    public override string TypeLabel => "Monstre";

    public Monstre(string nom, string icone,
        int hp, int force, int agi, int def, int resM, int recompense)
        : base(nom, icone, hp, 0, force, 6, agi, def, resM)
    {
        Recompense = recompense;
    }
}
