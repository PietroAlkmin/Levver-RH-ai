import React, { useState, useRef, useEffect } from 'react';
import { Send } from 'lucide-react';
import './ChatInterface.css';

export interface Message {
  role: 'user' | 'assistant';
  content: string;
  timestamp: Date;
}

export interface ChatInterfaceProps {
  messages: Message[];
  onSendMessage: (message: string) => void;
  completionPercentage: number;
  isLoading?: boolean;
}

/**
 * Componente de Chat minimalista
 * Lado esquerdo da tela de criação de vaga
 */
export const ChatInterface: React.FC<ChatInterfaceProps> = ({
  messages,
  onSendMessage,
  completionPercentage,
  isLoading = false
}) => {
  const [inputValue, setInputValue] = useState('');
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const textareaRef = useRef<HTMLTextAreaElement>(null);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  useEffect(() => {
    // Define altura inicial ao montar o componente
    if (textareaRef.current) {
      textareaRef.current.style.height = '44px';
    }
  }, []);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (inputValue.trim() && !isLoading) {
      onSendMessage(inputValue.trim());
      setInputValue('');
      
      // Reset textarea height
      const textarea = document.querySelector('.chat-input') as HTMLTextAreaElement;
      if (textarea) {
        textarea.style.height = '44px';
      }
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
    // Enviar com Enter (sem Shift)
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      handleSubmit(e);
    }
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
    const textarea = e.target;
    setInputValue(textarea.value);
    
    // Se o campo estiver vazio, volta para altura base
    if (!textarea.value) {
      textarea.style.height = '44px';
      return;
    }
    
    // Remove altura inline para recalcular baseado no conteúdo
    textarea.style.height = 'auto';
    
    // Calcula nova altura: mínimo 44px, máximo 150px
    const newHeight = Math.min(Math.max(textarea.scrollHeight, 44), 150);
    textarea.style.height = `${newHeight}px`;
  };

  return (
    <div className="chat-container">
      {/* Barra de Progresso */}
      <div className="chat-progress">
        <div className="chat-progress-header">
          <span>Completude</span>
          <span className="chat-progress-value">{Math.round(completionPercentage)}%</span>
        </div>
        <div className="chat-progress-bar">
          <div 
            className="chat-progress-fill" 
            style={{ width: `${completionPercentage}%` }}
          />
        </div>
      </div>

      {/* Mensagens */}
      <div className="chat-messages">
        {messages.map((message, index) => (
          <div
            key={index}
            className={`chat-message-wrapper ${message.role === 'user' ? 'chat-message-user' : 'chat-message-assistant'}`}
          >
            <div className={`chat-message ${message.role === 'user' ? 'user' : 'assistant'}`}>
              {message.content}
            </div>
          </div>
        ))}
        {isLoading && (
          <div className="chat-message-wrapper chat-message-assistant">
            <div className="chat-message assistant chat-loading">
              Digitando...
            </div>
          </div>
        )}
        <div ref={messagesEndRef} />
      </div>

      {/* Input */}
      <form onSubmit={handleSubmit} className="chat-input-container">
        <textarea
          ref={textareaRef}
          value={inputValue}
          onChange={handleInputChange}
          onKeyDown={handleKeyDown}
          placeholder="Digite sua mensagem..."
          disabled={isLoading}
          className="chat-input"
          rows={1}
        />
        <button
          type="submit"
          disabled={isLoading}
          className="chat-button"
        >
          <Send size={18} />
        </button>
      </form>
    </div>
  );
};
