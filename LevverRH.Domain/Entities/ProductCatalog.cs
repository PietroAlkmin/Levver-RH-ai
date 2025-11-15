using LevverRH.Domain.Enums;
using LevverRH.Domain.Exceptions;

namespace LevverRH.Domain.Entities;

public class ProductCatalog
{
    public Guid Id { get; private set; }
    public string ProdutoNome { get; private set; } = null!;
    public string? Descricao { get; private set; }
    public string Categoria { get; private set; } = null!;
    public string? Icone { get; private set; }
    public string? CorPrimaria { get; private set; }
    public string? RotaBase { get; private set; }
    public int OrdemExibicao { get; private set; }
    public bool Lancado { get; private set; }
    public ModeloCobranca ModeloCobranca { get; private set; }
    public decimal ValorBasePadrao { get; private set; }
    public string? ConfigJsonPadrao { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime DataAtualizacao { get; private set; }

    // EF Constructor
    private ProductCatalog() { }

    public ProductCatalog(
        string produtoNome,
        string categoria,
        ModeloCobranca modeloCobranca,
        decimal valorBasePadrao,
        string? descricao = null,
        string? icone = null,
        string? corPrimaria = null,
        string? rotaBase = null,
        int ordemExibicao = 0,
        bool lancado = false)
    {
        if (string.IsNullOrWhiteSpace(produtoNome))
            throw new DomainException("Nome do produto é obrigatório.");

        if (valorBasePadrao < 0)
            throw new DomainException("Preço deve ser maior ou igual a zero.");

        Id = Guid.NewGuid();
        ProdutoNome = produtoNome;
        Categoria = categoria;
        ModeloCobranca = modeloCobranca;
        ValorBasePadrao = valorBasePadrao;
        Descricao = descricao;
        Icone = icone;
        CorPrimaria = corPrimaria;
        RotaBase = rotaBase;
        OrdemExibicao = ordemExibicao;
        Lancado = lancado;
        Ativo = true;
        DataCriacao = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Ativar()
    {
        Ativo = true;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Desativar()
    {
        Ativo = false;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AtualizarPreco(decimal novoValor)
    {
        if (novoValor < 0)
            throw new DomainException("Preço deve ser maior ou igual a zero.");

        ValorBasePadrao = novoValor;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AtualizarDescricao(string? descricao)
    {
        Descricao = descricao;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AtualizarConfiguracao(string? configJson)
    {
        ConfigJsonPadrao = configJson;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AtualizarVisualizacao(string? icone, string? corPrimaria, int? ordemExibicao)
    {
        if (icone != null) Icone = icone;
        if (corPrimaria != null) CorPrimaria = corPrimaria;
        if (ordemExibicao.HasValue) OrdemExibicao = ordemExibicao.Value;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void MarcarComoLancado()
    {
        Lancado = true;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void MarcarComoEmBreve()
    {
        Lancado = false;
        DataAtualizacao = DateTime.UtcNow;
    }
}