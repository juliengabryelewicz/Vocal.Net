using Newtonsoft.Json.Linq;
using Vocal.Intents.ML;

namespace Vocal.Intents {
    public class IntentClassifier {

        static readonly string dataPath = Path.Combine("../Vocal.Intents/Data/Intents");
        static List<string> _intentList;
        List<BinaryClassifier> _binaryClassificators;

        public IntentClassifier(string datapath = null) {

            _binaryClassificators = new List<BinaryClassifier>();

            foreach (var intentName in GetIntentList())
                _binaryClassificators.Add(new BinaryClassifier(intentName, datapath, false));
        }

        public JObject FindIntents(string query) {

            Utterance utterance = new Utterance();
            utterance.Query = query;

            for (int i = 0; i < _intentList.Count; i++)
                utterance.Intents.Add(_binaryClassificators[i].Classify(query));

            if (utterance.TopScoringIntent.Score < 0.6) {
                utterance.Intents.Add(new Intent("none", (float) 0.6));
            }

            return utterance.GetResponse();

        }

        static List<string> GetIntentList() {

            DirectoryInfo d = new DirectoryInfo(dataPath);
            FileInfo[] files = d.GetFiles("*.txt");
            _intentList = new List<string>();
            foreach (FileInfo file in files) {
                _intentList.Add(file.Name.Replace(".txt", string.Empty));
            }
            return _intentList;
        }
    }
}
