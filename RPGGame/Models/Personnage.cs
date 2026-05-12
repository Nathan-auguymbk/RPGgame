namespace RPGGame.Models;

/// <summary>
/// Contient UNIQUEMENT l'état d'un personnage.
/// SRP strict : aucune formule de combat ici.
/// Les calculs de dégâts appartiennent aux Strategies.
/// </summary>
public abstract class Personnage
{
    public string Nom  { get; }
    public string Icone { get; }

    public int HP    { get; private set; }
    public int MaxHP { get; }
    public int MP    { get; private set; }
    public int MaxMP { get; }

    public int Force             { get; private set; }
    public int Intelligence      { get; }
    public int Agilite           { get; }
    public int Defense           { get; }
    public int ResistanceMagique { get; }

    public bool EnDefense { get; set; } = false;

    private readonly List<EffetStatut> _statuts = new();
    public IReadOnlyList<EffetStatut> Statuts => _statuts;

    public bool EstEnVie => HP > 0;

    // Statistiques de session (SRP : tracking minimal, pas de logique)
    public int TotalDegats { get; set; } = 0;
    public int TotalSoins  { get; set; } = 0;

    protected Personnage(string nom, string icone,
        int hp, int mp, int force, int intel,
        int agi, int def, int resM)
    {
        Nom = nom; Icone = icone;
        HP = MaxHP = hp;
        MP = MaxMP = mp;
        Force = force; Intelligence = intel;
        Agilite = agi; Defense = def; ResistanceMagique = resM;
    }

    // ── Mutations d'état pures (pas de logique métier) ───────────────────────

    /// <summary>Retire exactement <paramref name="montant"/> PV. Aucune réduction.</summary>
    public void SubirDegats(int montant)       => HP = Math.Max(0, HP - montant);

    /// <summary>Ajoute exactement <paramref name="montant"/> PV.</summary>
    public void RecevoirSoins(int montant)     => HP = Math.Min(MaxHP, HP + montant);

    /// <summary>Restaure <paramref name="montant"/> PM. Retourne la quantité réelle.</summary>
    public int RestaurerMP(int montant)
    {
        int avant = MP;
        MP = Math.Min(MaxMP, MP + montant);
        return MP - avant;
    }

    /// <summary>Retire <paramref name="cout"/> PM. Retourne false si insuffisant.</summary>
    public bool ConsommerMP(int cout)
    {
        if (MP < cout) return false;
        MP -= cout;
        return true;
    }

    public void AjouterStatut(EffetStatut s)  { if (!_statuts.Contains(s)) _statuts.Add(s); }
    public void RetirerStatut(EffetStatut s)  => _statuts.Remove(s);
    public bool AStatut(EffetStatut s)        => _statuts.Contains(s);
    public void AugmenterForce(int bonus)     => Force += bonus;

    public abstract bool   EstHeros   { get; }
    public abstract string TypeLabel  { get; }
}
