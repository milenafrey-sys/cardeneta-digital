using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShopApi.Data;
using PetShopApi.Models;

namespace PetShopApi.Controllers;

[ApiController]
[Route("api/caderneta")]
public class CadernetaController : ControllerBase
{
    private readonly PetShopContext _context;

    public CadernetaController(PetShopContext context)
    {
        _context = context;
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "online", api = "PetShopApi" });
    }

    [HttpGet("tutores")]
    public async Task<ActionResult<IEnumerable<TutorResponse>>> GetTutores()
    {
        var tutores = await _context.Tutores
            .Include(t => t.Pets)
            .Select(t => new TutorResponse(
                t.Id,
                t.Nome,
                t.Telefone,
                t.Email,
                t.Pets.Count))
            .ToListAsync();

        return Ok(tutores);
    }

    [HttpPost("tutores")]
    public async Task<ActionResult<TutorResponse>> PostTutor(TutorRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            return ValidationProblem("Nome do tutor é obrigatório.");

        var tutor = new Tutor
        {
            Nome = request.Nome,
            Telefone = request.Telefone,
            Email = request.Email
        };

        _context.Tutores.Add(tutor);
        await _context.SaveChangesAsync();

        var response = new TutorResponse(tutor.Id, tutor.Nome, tutor.Telefone, tutor.Email, 0);
        return CreatedAtAction(nameof(GetTutores), new { id = tutor.Id }, response);
    }

    [HttpGet("pets")]
    public async Task<ActionResult<IEnumerable<PetResponse>>> GetPets()
    {
        var pets = await _context.Pets
            .Include(p => p.Tutor)
            .Select(p => new PetResponse(
                p.Id,
                p.Nome,
                p.Especie,
                p.Raca,
                p.DataNascimento,
                p.TutorId,
                p.Tutor != null ? p.Tutor.Nome : string.Empty))
            .ToListAsync();

        return Ok(pets);
    }

    [HttpPost("pets")]
    public async Task<ActionResult<PetResponse>> PostPet(PetRequest request)
    {
        var tutorExiste = await _context.Tutores.AnyAsync(t => t.Id == request.TutorId);
        if (!tutorExiste)
            return BadRequest("Tutor não encontrado.");

        var pet = new Pet
        {
            Nome = request.Nome,
            Especie = request.Especie,
            Raca = request.Raca,
            DataNascimento = request.DataNascimento,
            TutorId = request.TutorId
        };

        _context.Pets.Add(pet);
        await _context.SaveChangesAsync();

        var response = new PetResponse(
            pet.Id,
            pet.Nome,
            pet.Especie,
            pet.Raca,
            pet.DataNascimento,
            pet.TutorId,
            (await _context.Tutores.FindAsync(pet.TutorId))?.Nome ?? string.Empty);

        return CreatedAtAction(nameof(GetPets), new { id = pet.Id }, response);
    }

    [HttpGet("funcionarios")]
    public async Task<ActionResult<IEnumerable<FuncionarioResponse>>> GetFuncionarios()
    {
        var funcionarios = await _context.Funcionarios
            .Select(f => new FuncionarioResponse(f.Id, f.Nome, f.Cargo))
            .ToListAsync();

        return Ok(funcionarios);
    }

    [HttpPost("funcionarios")]
    public async Task<ActionResult<FuncionarioResponse>> PostFuncionario(FuncionarioRequest request)
    {
        var funcionario = new Funcionario
        {
            Nome = request.Nome,
            Cargo = request.Cargo
        };

        _context.Funcionarios.Add(funcionario);
        await _context.SaveChangesAsync();

        var response = new FuncionarioResponse(funcionario.Id, funcionario.Nome, funcionario.Cargo);
        return CreatedAtAction(nameof(GetFuncionarios), new { id = funcionario.Id }, response);
    }

    [HttpGet("atendimentos")]
    public async Task<ActionResult<IEnumerable<AtendimentoResponse>>> GetAtendimentos()
    {
        var atendimentos = await _context.Atendimentos
            .Include(a => a.Pet)
            .Include(a => a.Funcionario)
            .Select(a => new AtendimentoResponse(
                a.Id,
                a.PetId,
                a.Pet != null ? a.Pet.Nome : string.Empty,
                a.FuncionarioId,
                a.Funcionario != null ? a.Funcionario.Nome : string.Empty,
                a.Tipo,
                a.DataHora,
                a.Observacoes))
            .ToListAsync();

        return Ok(atendimentos);
    }

    [HttpPost("atendimentos")]
    public async Task<ActionResult<AtendimentoResponse>> PostAtendimento(AtendimentoRequest request)
    {
        var petExiste = await _context.Pets.AnyAsync(p => p.Id == request.PetId);
        if (!petExiste)
            return BadRequest("Pet não encontrado.");

        var funcionarioExiste = await _context.Funcionarios.AnyAsync(f => f.Id == request.FuncionarioId);
        if (!funcionarioExiste)
            return BadRequest("Funcionário não encontrado.");

        var atendimento = new Atendimento
        {
            PetId = request.PetId,
            FuncionarioId = request.FuncionarioId,
            Tipo = request.Tipo,
            DataHora = request.DataHora,
            Observacoes = request.Observacoes
        };

        _context.Atendimentos.Add(atendimento);
        await _context.SaveChangesAsync();

        var pet = await _context.Pets.FindAsync(request.PetId);
        var funcionario = await _context.Funcionarios.FindAsync(request.FuncionarioId);

        var response = new AtendimentoResponse(
            atendimento.Id,
            atendimento.PetId,
            pet?.Nome ?? string.Empty,
            atendimento.FuncionarioId,
            funcionario?.Nome ?? string.Empty,
            atendimento.Tipo,
            atendimento.DataHora,
            atendimento.Observacoes);

        return CreatedAtAction(nameof(GetAtendimentos), new { id = atendimento.Id }, response);
    }
}
