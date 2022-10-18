using Microsoft.ML.Data;

namespace Vocal.Intents.ML {
    class ClassificationPrediction : ClassificationData {

        [ColumnName("PredictedLabel")]

        public bool Prediction { get; set; }
        public float Probability { get; set; }
        public float Score { get; set; }
    }
}
