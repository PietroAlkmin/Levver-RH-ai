using LevverRH.Domain.Entities.Talents;
using LevverRH.Domain.Interfaces.Talents;
using LevverRH.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace LevverRH.Infra.Data.Repositories.Talents;

/// <summary>
/// Implementação do repositório de mensagens de chat
/// Usa Entity Framework Core para acessar a tabela TALENTS.chat_messages
/// </summary>
public class ChatMessageRepository : Repository<ChatMessage>, IChatMessageRepository
{
    public ChatMessageRepository(LevverDbContext context) : base(context)
    {
    }

    public async Task<List<ChatMessage>> GetByConversationIdAsync(Guid conversationId)
    {
        return await _context.Set<ChatMessage>()
            .Where(cm => cm.ConversationId == conversationId)
            .OrderBy(cm => cm.Timestamp)
            .ToListAsync();
    }

    public async Task<ChatMessage?> GetLastMessageAsync(Guid conversationId)
    {
        return await _context.Set<ChatMessage>()
            .Where(cm => cm.ConversationId == conversationId)
            .OrderByDescending(cm => cm.Timestamp)
            .FirstOrDefaultAsync();
    }

    public async Task<int> CountMessagesAsync(Guid conversationId)
    {
        return await _context.Set<ChatMessage>()
            .CountAsync(cm => cm.ConversationId == conversationId);
    }

    public async Task<int> GetTotalTokensUsedAsync(Guid conversationId)
    {
        return await _context.Set<ChatMessage>()
            .Where(cm => cm.ConversationId == conversationId)
            .SumAsync(cm => cm.TokensUsados ?? 0);
    }

    public async Task DeleteByConversationIdAsync(Guid conversationId)
    {
        var messages = await _context.Set<ChatMessage>()
            .Where(cm => cm.ConversationId == conversationId)
            .ToListAsync();

        if (messages.Any())
        {
            _context.Set<ChatMessage>().RemoveRange(messages);
            await _context.SaveChangesAsync();
        }
    }
}
