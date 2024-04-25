using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IdeaCenterApi.Models
{
    public class AuthResponseDto
    {
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }
    }
}
