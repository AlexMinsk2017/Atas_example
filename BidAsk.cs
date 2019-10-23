namespace ATAS.Indicators.Technical
{
	using System.ComponentModel;
	using System.Windows.Media;

	[DisplayName("Bid Ask")]
	[Category("Bid x Ask,Delta,Volume")]
	public class BidAsk : Indicator
	{
		private readonly ValueDataSeries _bids;
		private readonly ValueDataSeries _asks;
		public BidAsk():base(true)
		{
			Panel = IndicatorDataProvider.NewPanel;
			_bids = (ValueDataSeries)DataSeries[0];
			_bids.Color = Colors.Red;
			_bids.VisualType=VisualMode.Histogram;
			_bids.Name = "Bid";

			_asks = new ValueDataSeries("Ask")
			{
				VisualType = VisualMode.Histogram,
				Color = Colors.Green,
			};
			DataSeries.Add(_asks);
		}
		
		protected override void OnCalculate(int bar, decimal value)
		{
			var candle = GetCandle(bar);
			_bids[bar] = -candle.Bid;
			_asks[bar] = candle.Ask;
		}

		public override string ToString()
		{
			return "Bid Ask";
		}
	}
}