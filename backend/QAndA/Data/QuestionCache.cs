using Microsoft.Extensions.Caching.Memory;
using QAndA.Data.Models;

namespace QAndA.Data
{
    public class QuestionCache : IQuestionCache
    {
        public QuestionCache()
        {
            Cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 100
            });
        }

        private MemoryCache Cache { get; }

        public QuestionGetSingleResponse Get(int questionId)
        {
            Cache.TryGetValue(GetCacheKey(questionId), out QuestionGetSingleResponse question);
            return question;
        }

        public void Remove(int questionId)
        {
            Cache.Remove(GetCacheKey(questionId));
        }

        public void Set(QuestionGetSingleResponse question)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1);
            Cache.Set(GetCacheKey(question.QuestionId), question, cacheEntryOptions);
        }

        private string GetCacheKey(int questionId)
        {
            return $"Question-{questionId}";
        }
    }
}