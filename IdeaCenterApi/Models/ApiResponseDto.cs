using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IdeaCenterApi.Models
{
    public class ApiResponseDto 
    {
        [JsonPropertyName("msg")]
        public string Msg { get; set; }

        [JsonPropertyName("idea")]
        public IdeaDto Idea { get; set; }

        public ApiResponseDto() 
        {
            Idea = new IdeaDto();
        }
    }
}
