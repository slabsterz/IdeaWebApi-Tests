using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IdeaCenterApi.Models
{
    public class IdeaDto : IBaseIdeaModel
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("id")]
        public string IdeaId { get; set; }

        [JsonPropertyName("authorUsername")]
        public string AuthorUserName { get; set; }

        [JsonPropertyName("createdOn")]
        public string CreatedOn { get; set; }

        [JsonPropertyName("updatedOn")]
        public string UpdatedOn { get; set; }

    }
}
