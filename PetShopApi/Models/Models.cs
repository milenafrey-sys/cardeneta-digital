namespace PetShopApi.Models;

public enum TipoAtendimento
{
    Consulta,
    Vacina,
    Cirurgia,
    BanhoETosa,
    Exame,
    Outros
}

public class Tutor
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public ICollection<Pet> Pets { get; set; } = new List<Pet>();
}

public class Pet
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Especie { get; set; } = string.Empty;
    public string Raca { get; set; } = string.Empty;
    public DateOnly DataNascimento { get; set; }

    public int TutorId { get; set; }
    public Tutor? Tutor { get; set; }

    public ICollection<Atendimento> Atendimentos { get; set; } = new List<Atendimento>();
}

public class Funcionario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;

    public ICollection<Atendimento> Atendimentos { get; set; } = new List<Atendimento>();
}

public class Atendimento
{
    public int Id { get; set; }
    public int PetId { get; set; }
    public Pet? Pet { get; set; }

    public int FuncionarioId { get; set; }
    public Funcionario? Funcionario { get; set; }

    public TipoAtendimento Tipo { get; set; }
    public DateTime DataHora { get; set; }
    public string? Observacoes { get; set; }
}

public record TutorRequest(
    string Nome,
    string Telefone,
    string Email);

public record TutorResponse(
    int Id,
    string Nome,
    string Telefone,
    string Email,
    int QuantidadePets);

public record PetRequest(
    string Nome,
    string Especie,
    string Raca,
    DateOnly DataNascimento,
    int TutorId);

public record PetResponse(
    int Id,
    string Nome,
    string Especie,
    string Raca,
    DateOnly DataNascimento,
    int TutorId,
    string TutorNome);

public record FuncionarioRequest(
    string Nome,
    string Cargo);

public record FuncionarioResponse(
    int Id,
    string Nome,
    string Cargo);

public record AtendimentoRequest(
    int PetId,
    int FuncionarioId,
    TipoAtendimento Tipo,
    DateTime DataHora,
    string? Observacoes);

public record AtendimentoResponse(
    int Id,
    int PetId,
    string PetNome,
    int FuncionarioId,
    string FuncionarioNome,
    TipoAtendimento Tipo,
    DateTime DataHora,
    string? Observacoes);
