namespace RPGGame.Models;

/// <summary>Immuable par conception (init-only).</summary>
public sealed class Competence
{
    public string         Nom         { get; init; }
    public string         Icone       { get; init; }
    public TypeCompetence Type        { get; init; }
    public TypeCible      Cible       { get; init; }
    public int            Puissance   { get; init; }
    public int            CoutMP      { get; init; }
    public int            NombreCoups { get; init; } = 1;
    public float          ChanceCrit  { get; init; } = 0f;
    public EffetStatut    Effet       { get; init; } = EffetStatut.Aucun;

    public Competence(string nom, string icone, TypeCompetence type, TypeCible cible,
        int puissance, int coutMP = 0, int coups = 1,
        float critique = 0f, EffetStatut effet = EffetStatut.Aucun)
    {
        Nom = nom; Icone = icone; Type = type; Cible = cible;
        Puissance = puissance; CoutMP = coutMP; NombreCoups = coups;
        ChanceCrit = critique; Effet = effet;
    }
}

public sealed class Objet
{
    public string   Nom       { get; init; }
    public string   Icone     { get; init; }
    public TypeObjet Type     { get; init; }
    public int      Puissance { get; init; }
    public int      Quantite  { get; set; }

    public bool Disponible => Quantite > 0;

    public Objet(string nom, string icone, TypeObjet type, int puissance, int quantite)
    {
        Nom = nom; Icone = icone; Type = type;
        Puissance = puissance; Quantite = quantite;
    }
}
