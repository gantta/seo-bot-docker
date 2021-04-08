using System;
using Newtonsoft.Json;

namespace seo_bot_docker {

    public class SearchStats {
        [JsonProperty("id")]
        public Guid _Id { get; }
        [JsonProperty("searchDate")]
        public DateTime _searchDate { get; }
        [JsonProperty("searchProvider")]
        public string _searchProvider { get; set; }
        [JsonProperty("queryString")]
        public string _queryString { get; set; }
        [JsonProperty("resultPage")]
        public int _resultPage { get; set; }
        [JsonProperty("rank")]
        public int _rank { get; set; }

        public SearchStats() {
            _Id = Guid.NewGuid();
            _searchDate = DateTime.UtcNow;
        }

    }
}