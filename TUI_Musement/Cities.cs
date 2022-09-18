using System;
using System.Globalization;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace TUI_Musement
{
    public class Cities
    {

        [JsonPropertyName("name")]
        public string Name { get; set; }

    }
}


