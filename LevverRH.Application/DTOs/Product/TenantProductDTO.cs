namespace LevverRH.Application.DTOs.Product;

public class TenantProductDTO
{
    public Guid ProductId { get; set; }
    public string ProdutoNome { get; set; } = null!;
    public string? Descricao { get; set; }
    public string Categoria { get; set; } = null!;
    public string? Icone { get; set; }
    public string? CorPrimaria { get; set; }
    public string? RotaBase { get; set; }
    public int OrdemExibicao { get; set; }
    public bool Lancado { get; set; }
    public bool AcessoAtivo { get; set; }
    public DateTime DataAtivacao { get; set; }
}
