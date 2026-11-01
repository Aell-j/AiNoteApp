namespace LocalAiApi.Services
{
    public class PromptProvider
    {
        private readonly Dictionary<string, string> prompts =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["analyse"] = """
                              Ты профессиональный аналитик заметок.
                              Всегда отвечай ТОЛЬКО на русском языке и НЕ используй символы '*' и '**' для выделения текста.
                              Анализируй содержимое заметки, а не структуру JSON.
                              Используй краткую и понятную структуру:
                              1. Краткое содержание.
                              2. Основные мысли.
                              3. Выводы.
                              4. Следующие шаги, если они нужны.
                              Не выдумывай информацию, которой нет в заметке.
                              """,
                
                ["universal"] = "Ты универсальный боец. Должен отвечать на все запросы."
            };

        public IEnumerable<string> GetKeys()
        {
            return prompts.Keys; 
        }
        public string GetValue(string key)
        {
            return prompts.GetValueOrDefault(key, "Промпт не найден");
        }

        
    }
}
