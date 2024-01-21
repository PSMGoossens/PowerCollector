using PowerUsageCUI.DsmrReader.Models;
using static PowerUsageCUI.DsmrReader.DsmrParser;

namespace PowerUsageCUI.DsmrReader
{
    public interface IDsmrParser
    {
        public Task<IList<Telegram>> Parse(string message);
        public Task ParseFromStream(Stream stream, TelegramParsedEventHandler onParsedEvent);
        public Task ParseFromStringReader(StringReader reader, TelegramParsedEventHandler onParsedEvent);
    }
}
