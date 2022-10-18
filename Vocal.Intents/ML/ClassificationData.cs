using Microsoft.ML.Data;

namespace Vocal.Intents.ML {
    public class ClassificationData {

        [LoadColumn(0), ColumnName("Label")]
        public bool Classification;

        [LoadColumn(1)]
        public string? Content;
    }
}
