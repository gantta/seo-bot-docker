using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace seo_bot_docker
{
    public class SearchHelper {
        private List<string> _searchPhrases;
        private string _targetDomain;
        static Random _random;

        public SearchHelper() {
            JObject o1 = JObject.Parse(File.ReadAllText(@"appConfig.json"));
            JArray a = (JArray)o1["searchPhrases"];
            _targetDomain = o1["targetDomain"].ToString();
            _searchPhrases = a.ToObject<List<string>>();
            _random = new Random();
        }

        public string GetRandomSearch() {
            int r = _random.Next(_searchPhrases.Count);
            return _searchPhrases[r];
        }

        public string GetTargetDomain(){
            return _targetDomain;
        }
    }
}