using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IdeaCenterApi
{
    public class ApiResponse
    {
        [JsonPropertyName("msg")]
        public string Msg { get; set; }

        [JsonPropertyName("idea")]
        public Idea Idea { get; set; }

        public ApiResponse()
        {
            Idea = new Idea();
        }
    }
}
