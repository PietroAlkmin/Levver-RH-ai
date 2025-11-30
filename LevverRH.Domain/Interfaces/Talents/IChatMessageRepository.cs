using LevverRH.Domain.Entities.Talents;

namespace LevverRH.Domain.Interfaces.Talents;

/// <summary>
/// Interface para repositório de mensagens de chat do Talents
/// Gerencia o histórico de conversas com IA
/// </summary>
public interface IChatMessageRepository : IRepository<ChatMessage>
{
    /// <summary>
    /// Busca todas as mensagens de uma conversa específica
    /// Retorna em ordem cronológica (da mais antiga para a mais recente)
    /// </summary>
    /// <param name="conversationId">ID da conversa</param>
    /// <returns>Lista de mensagens ordenadas por timestamp</returns>
    Task<List<ChatMessage>> GetByConversationIdAsync(Guid conversationId);

    /// <summary>
    /// Busca a última mensagem de uma conversa
    /// Útil para verificar o último estado da conversa
    /// </summary>
    /// <param name="conversationId">ID da conversa</param>
    /// <returns>Última mensagem ou null se não houver mensagens</returns>
    Task<ChatMessage?> GetLastMessageAsync(Guid conversationId);

    /// <summary>
    /// Conta o total de mensagens em uma conversa
    /// Útil para estatísticas e validações
    /// </summary>
    /// <param name="conversationId">ID da conversa</param>
    /// <returns>Número total de mensagens</returns>
    Task<int> CountMessagesAsync(Guid conversationId);

    /// <summary>
    /// Calcula o total de tokens gastos em uma conversa
    /// Usado para controle de custos da API OpenAI
    /// </summary>
    /// <param name="conversationId">ID da conversa</param>
    /// <returns>Soma total de tokens usados</returns>
    Task<int> GetTotalTokensUsedAsync(Guid conversationId);

    /// <summary>
    /// Deleta todas as mensagens de uma conversa
    /// Usado quando o usuário cancela/abandona a criação de vaga
    /// </summary>
    /// <param name="conversationId">ID da conversa</param>
    Task DeleteByConversationIdAsync(Guid conversationId);
}
