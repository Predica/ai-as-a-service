using System;

namespace AwesomeBot.Models
{
    [Flags]
    public enum Language : byte
    {
        NotSupported = 1,
        English = 2,
        Polish = 4
    }
}