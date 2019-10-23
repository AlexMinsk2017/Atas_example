namespace ATAS.Indicators.Technical
{
	using System;
	using System.ComponentModel;
	using System.Windows.Media;

	[DisplayName("Daily HighLow")]
	public class DailyHighLow : Indicator
	{
		private readonly ValueDataSeries _highSeres;
		readonly ValueDataSeries _lowSeres = new ValueDataSeries("Low") { Color = Color.FromArgb(255, 135, 135, 135), VisualType = VisualMode.Square };
		readonly ValueDataSeries _medianSeres = new ValueDataSeries("Median") { Color = Colors.Lime, VisualType = VisualMode.Square };
		readonly ValueDataSeries _yesterdaymedianaSeres = new ValueDataSeries("Yesterday median") { Color = Colors.Blue, VisualType = VisualMode.Square };

		private decimal _high;
		private decimal _low;
		private decimal Mediana => _low + (_high - _low) / 2;

		private decimal _yesterdaymediana;
		private DateTime _lastSessionTime;

		public DailyHighLow()
		{
			_highSeres = (ValueDataSeries)DataSeries[0];
			_highSeres.Name = "High";
			_highSeres.Color = Color.FromArgb(255, 135, 135, 135);
			_highSeres.VisualType = VisualMode.Square;
			DataSeries.Add(_lowSeres);
			DataSeries.Add(_medianSeres);
			DataSeries.Add(_yesterdaymedianaSeres);

		}

		#region Overrides of Indicator

		protected override void OnCalculate(int bar, decimal value)
		{
			if (bar == 0)
			{
				_high = _low = _yesterdaymediana = 0;
				return;
			}
			var candle = GetCandle(bar);
			if (IsNewSession(bar))
			{
				if (_lastSessionTime != candle.Time)
				{
					_lastSessionTime = candle.Time;
					_yesterdaymediana = Mediana;
					_high = _low = 0;
				}
			}

			if (candle.High > _high || _high == 0) _high = candle.High;
			if (candle.Low < _low || _low == 0) _low = candle.Low;

			_highSeres[bar] = _high;
			_lowSeres[bar] = _low;
			_medianSeres[bar] = Mediana;
			_yesterdaymedianaSeres[bar] = _yesterdaymediana;
		}

		#endregion

		#region Overrides of Indicator

		public override string ToString()
		{
			return "Daily HighLow";
		}

		#endregion
	}
}