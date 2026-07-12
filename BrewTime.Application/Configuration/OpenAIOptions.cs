using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.Configuration
{
    public class OpenAIOptions
    {
        public string ApiKey { get; set; } = string.Empty;

        public string Model { get; set; } = "gpt-4.1-mini";
    }
}

