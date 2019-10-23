using System.Linq;
using ATAS.Indicators;

namespace CustomIndicatorSet
{
    public class SampleClusterSearch : Indicator
    {
        private int _filter = 10;
        readonly PriceSelectionDataSeries _priceSelectionSeries = new PriceSelectionDataSeries("Clusters Selection");
        public int Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                RecalculateValues();
            }
        }
        public SampleClusterSearch()
        {
            DataSeries.Add(_priceSelectionSeries);
        }
        protected override void OnCalculate(int bar, decimal value)
        {
            var candle = GetCandle(bar);
            for (decimal price = candle.High; price >= candle.Low; price -= TickSize)
            {
                var volumeinfo = candle.GetPriceVolumeInfo(price);
                if (volumeinfo == null) continue;
                if (volumeinfo.Volume > _filter)
                {
                    var values = _priceSelectionSeries[bar];
                    var priceSelection = values.FirstOrDefault(t => t.MinimumPrice == volumeinfo.Price);
                    if (priceSelection == null) values.Add(new PriceSelectionValue(volumeinfo.Price) { VisualObject = ObjectType.Rectangle, Size = 10 });
                }
            }
        }
    }
}