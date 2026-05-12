namespace RPGGame.Models;

public class Heros : Personnage
{
    public TypeClasse       Classe      { get; }
    public List<Competence> Competences { get; } = new();

    public override bool   EstHeros  => true;
    public override string TypeLabel => Classe.ToString();

    public Heros(string nom, string icone, TypeClasse classe,
        int hp, int mp, int force, int intel, int agi, int def, int resM)
        : base(nom, icone, hp, mp, force, intel, agi, def, resM)
    {
        Classe = classe;
    }

    public void ApprendreCompetence(Competence c) => Competences.Add(c);
    public bool PeutUtiliser(Competence c)        => MP >= c.CoutMP;
}
