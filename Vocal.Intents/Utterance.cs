using Newtonsoft.Json.Linq;

namespace Vocal.Intents {
    public class Utterance {

        private string? _query;
        public String Query {
            get {
                return _query;
            }
            set {
                _query = value.ToLower();
            }
        }
        public List<Intent> Intents { get; set; }
        public Intent TopScoringIntent {
            get {
                return GetTopScoringIntent();
            }
        }

        public Utterance() {
            Intents = new List<Intent>();
        }

        private Intent GetTopScoringIntent() {

            Intent topScoringIntent = new Intent();
            float maxValue = 0;

            foreach (Intent intent in Intents) {
                if (intent.Score > maxValue) {
                    topScoringIntent = intent;
                    maxValue = intent.Score;
                }
            }

            return topScoringIntent;
        }

        public JObject GetResponse() {

            JObject json =
                new JObject(
                    new JProperty("query", Query),
                    new JProperty("intents",
                        new JArray(
                            (from intent in Intents
                             orderby intent.Score descending
                             select new JObject(
                                 new JProperty("intent", intent.Name),
                                 new JProperty("score", intent.Score)
                             )).Take(2)
                        )
                    )
                );

            return json;
        }
    }
}
