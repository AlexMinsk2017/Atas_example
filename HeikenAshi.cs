namespace ATAS.Indicators.Technical
{
	using System.ComponentModel;
	using System.Windows.Media;

	[DisplayName("Heiken Ashi")]
	public class HeikenAshi:Indicator
	{
		readonly CandleDataSeries _candles=new CandleDataSeries("Heiken Ashi");
		readonly PaintbarsDataSeries _bars= new PaintbarsDataSeries("Bars");
		public HeikenAshi()
		{
			DataSeries[0]= _bars;
			DataSeries.Add(_candles);
		}

		protected override void OnCalculate(int bar, decimal value)
		{
			var candle = GetCandle(bar);
			_bars[bar] = Colors.Transparent;
			if (bar == 0)
			{
				_candles[bar] = new Candle()
				{
					Close = candle.Close,
					High = candle.High,
					Low = candle.Low,
					Open = candle.Open
				};
			}
			else
			{
				var prevCandle = _candles[bar - 1];
				_candles[bar] = new Candle()
				{
					Close = (candle.Open+candle.Close+candle.High+candle.Low)*0.25m,
					High = candle.High,
					Low = candle.Low,
					Open = (prevCandle.Open+ prevCandle.Close)*0.5m,
				};
			}
			
		}
	}
}