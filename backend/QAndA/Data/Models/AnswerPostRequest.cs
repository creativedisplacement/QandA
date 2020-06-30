using System.ComponentModel.DataAnnotations;

namespace QAndA.Data.Models
{
    public class AnswerPostRequest
    {
        [Required]
        public int? QuestionId { get; set; }
        
        [Required]
        public string Content { get; set; }
        public string UserName { get; set; }
    }
}