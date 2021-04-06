using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace seo_bot_docker
{
    public class SearchHelper {
        private List<string> _searchPhrases;
        static Random _random;

        public SearchHelper() {
            JObject o1 = JObject.Parse(File.ReadAllText(@"searchPhrases.json"));
            JArray a = (JArray)o1["searchPhrases"];
            _searchPhrases = a.ToObject<List<string>>();
            _random = new Random();
        }

        public string GetRandomSearch() {
            int r = _random.Next(_searchPhrases.Count);
            return _searchPhrases[r];
        }
    }
}