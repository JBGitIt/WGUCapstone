using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomForest
{
    public static class OutputProcessor
    {
        public static Dictionary<string, double> GetPredictiveRange(double v_DBLhigh, double v_DBLlow, double v_DBLprediction, int v_INTvaluesHigh, int v_INTvaluesLow)
        {
            Dictionary<string, double> l_COLLpredictiveRange = new Dictionary<string, double>();

            double l_DBLbaseMean = (v_DBLhigh + v_DBLlow + v_DBLprediction) / 3;
            double l_DBLhighDiff = v_DBLhigh - l_DBLbaseMean;
            double l_DBLpredictionDiff = v_DBLprediction - l_DBLbaseMean;
            double l_DBLlowDiff = v_DBLlow - l_DBLbaseMean;
            double l_DBLrange = Math.Abs(l_DBLhighDiff) + Math.Abs(l_DBLlowDiff);
            double l_DBLhighVariance = l_DBLhighDiff * l_DBLhighDiff;
            double l_DBLpredictionVariance = l_DBLpredictionDiff * l_DBLpredictionDiff;
            double l_DBLlowVariance = l_DBLlowDiff * l_DBLlowDiff;
            double l_DBLvariance = (l_DBLhighVariance + l_DBLpredictionVariance + l_DBLlowVariance) / 3;
            double l_DBLstandardDeviation = Math.Sqrt(l_DBLvariance);
            double l_DBLpercentAbove = v_INTvaluesHigh / Convert.ToDouble(v_INTvaluesHigh + v_INTvaluesLow);
            double l_DBLpercentBelow = v_INTvaluesLow / Convert.ToDouble(v_INTvaluesHigh + v_INTvaluesLow);
            double l_DBLpredRangeHigh = (l_DBLstandardDeviation * l_DBLpercentAbove) + v_DBLprediction;
            double l_DBLpredRangeLow = v_DBLprediction - (l_DBLstandardDeviation * l_DBLpercentBelow);

            l_COLLpredictiveRange.Add("Standard Deviation", l_DBLstandardDeviation);
            l_COLLpredictiveRange.Add("Predictive High", l_DBLpredRangeHigh);
            l_COLLpredictiveRange.Add("Predictive Low", l_DBLpredRangeLow);
            l_COLLpredictiveRange.Add("Prediction", v_DBLprediction);

            return l_COLLpredictiveRange;
        }
    }
}
