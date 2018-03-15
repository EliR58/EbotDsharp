using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace EbotDsharp2
{
    public class Question
    {
        [JsonProperty("response_code")]
        public int responseCode { get; set; }

        [JsonProperty("results")]
        public Results[] results { get; set; }
        
    }

    public class Results
    {
        [JsonProperty("category")]
        public string category { get; set; }
        [JsonProperty("type")]
        public string type { get; set; }
        [JsonProperty("difficulty")]
        public string difficulty { get; set; }
        [JsonProperty("question")]
        public string question { get; set; }
        [JsonProperty("correct_answer")]
        public string correct { get; set; }
        [JsonProperty("incorrect_answers")]
        public List<string> incorrect { get; set; }
    }

    public class Incorrect
    {
        public string incorrect_answers;
    }
}
