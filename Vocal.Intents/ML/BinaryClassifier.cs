using Microsoft.ML;
using Microsoft.ML.Data;
using static Microsoft.ML.DataOperationsCatalog;

namespace Vocal.Intents.ML {
    class BinaryClassifier {

        MLContext _mlContext;
        ITransformer _model;
        readonly string _dataName;
        IDataView _dataView;

        public BinaryClassifier(string dataName, string dataPath, bool hasHeader) {

            _mlContext = new MLContext();
            _dataName = dataName;


            if (dataPath == null)
                dataPath = Path.Combine("../Vocal.Intents/Data/Intents", _dataName + ".txt");
            else
                dataPath = Path.Combine(dataPath, _dataName + ".txt");

            TrainTestData splitDataView = LoadData(dataPath, hasHeader);                
            _model = BuildAndTrainModel(splitDataView.TrainSet);
            Evaluate(splitDataView.TestSet);
        }

        public BinaryClassifier(string dataName, string dataPath) : this(dataName, dataPath, false) {
            
        }

        public Intent Classify(string query) {

            ClassificationData statement = new ClassificationData {
                Content = query
            };
            return PredictSingleItem(statement);

        }

        TrainTestData LoadData(String dataPath, bool hasHeader) {

            _dataView = _mlContext.Data.LoadFromTextFile<ClassificationData>(dataPath, hasHeader: hasHeader);
            TrainTestData splitDataView = _mlContext.Data.TrainTestSplit(_dataView, testFraction: 0.00000001);
            return splitDataView;
        }

        ITransformer BuildAndTrainModel(IDataView splitTrainSet) {

            var estimator = _mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(ClassificationData.Content))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));

            return estimator.Fit(splitTrainSet);

        }

        void Evaluate(IDataView splitTestSet) {

            IDataView predictions = _model.Transform(splitTestSet);

            try {
                CalibratedBinaryClassificationMetrics metrics = _mlContext.BinaryClassification.Evaluate(predictions, "Label");
            }
            catch (ArgumentOutOfRangeException) {

            }
        }

        Intent PredictSingleItem(ClassificationData statement) {

            PredictionEngine<ClassificationData, ClassificationPrediction> predictionFunction =
                _mlContext.Model.CreatePredictionEngine<ClassificationData, ClassificationPrediction>(_model);
            

            var resultprediction = predictionFunction.Predict(statement);

            Intent intent = new Intent();
            intent.Name = _dataName;
            intent.Score = resultprediction.Probability;

            return intent;
        }
    }
}
