using System;

namespace AwesomeBot.Models
{
    [Flags]
    public enum BotContext : byte
    {
        QnA = 1,
        Echo = 2
    }
}