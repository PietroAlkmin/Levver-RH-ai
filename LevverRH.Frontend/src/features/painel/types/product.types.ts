export interface Product {
  id: string;
  produtoNome: string;
  descricao?: string;
  categoria: string;
  icone?: string;
  corPrimaria?: string;
  rotaBase?: string;
  ordemExibicao: number;
  lancado: boolean;
  ativo: boolean;
}

export interface TenantProduct {
  productId: string;
  produtoNome: string;
  descricao?: string;
  categoria: string;
  icone?: string;
  corPrimaria?: string;
  rotaBase?: string;
  ordemExibicao: number;
  lancado: boolean;
  acessoAtivo: boolean;
  dataAtivacao: string;
}
