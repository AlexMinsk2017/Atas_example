namespace ATAS.Indicators.Technical
{
	using System;
	using System.ComponentModel;
	using System.Windows.Media;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;
	using Utils.Common.Logging;

	[DisplayName("Cumulative Delta")]
	[Category("Bid x Ask,Delta,Volume")]
	public class CumulativeDelta : Indicator
	{
		[Serializable]
		public enum SessionDeltaVisualMode
		{
			Candles = 0,
			Bars = 1,
			Line = 2,
		}

		private SessionDeltaVisualMode _mode = SessionDeltaVisualMode.Candles;
		[DisplayName( "VisualMode")]
		public SessionDeltaVisualMode Mode
		{
			get => _mode;
			set
			{

				_mode = value;
				if (_mode == SessionDeltaVisualMode.Candles)
				{
					((CandleDataSeries)DataSeries[1]).Visible = true;
					((ValueDataSeries)DataSeries[0]).VisualType = VisualMode.OnlyValueOnAxis;
					((ValueDataSeries)DataSeries[2]).VisualType = VisualMode.Hide;
					((ValueDataSeries)DataSeries[3]).VisualType = VisualMode.Hide;
				}
				else
				{
					((CandleDataSeries)DataSeries[1]).Visible = false;
					if (_mode == SessionDeltaVisualMode.Line)
					{
						((ValueDataSeries)DataSeries[0]).VisualType = VisualMode.Line;
						((ValueDataSeries)DataSeries[2]).VisualType = VisualMode.Hide;
						((ValueDataSeries)DataSeries[3]).VisualType = VisualMode.Hide;
					}
					else
					{
						((ValueDataSeries)DataSeries[0]).VisualType = VisualMode.Hide;
						((ValueDataSeries)DataSeries[2]).VisualType = VisualMode.Histogram;
						((ValueDataSeries)DataSeries[3]).VisualType = VisualMode.Histogram;
					}
				}

				RecalculateValues();
			}
		}


		private bool _sessionDeltaMode = true;

		[DisplayName( "SessionDeltaMode")]
		public bool SessionDeltaMode
		{
			get => _sessionDeltaMode;
			set { _sessionDeltaMode = value; RaisePropertyChanged("SessionDeltaMode"); RecalculateValues(); }
		}


		#region Overrides of Indicator

		public CumulativeDelta()
		{
			LineSeries.Add(new LineSeries("Zero"){Color = Colors.Gray,Width = 1});
			var series = (ValueDataSeries)DataSeries[0];
			series.Width = 2;
			series.Color = Colors.Green;
			DataSeries.Add(new CandleDataSeries("Candles"));
			//DataSeries.Add(new ValueDataSeries("Zero"){Color = Colors.Gray});
			DataSeries.Add(new ValueDataSeries("PositiveHistogram") { Color = Colors.Green });
			DataSeries.Add(new ValueDataSeries("NegativeHistogram") { Color = Colors.Red });
			Panel = IndicatorDataProvider.NewPanel;
			Mode = Mode;
			
		}
		private decimal _cumDelta;
		private decimal _open = 0;
		private decimal _low;
		private decimal _high;
		protected override void OnCalculate(int i, decimal value)
		{
			if (i == 0)
			{
				_cumDelta = 0;
			}

			try
			{

				bool NewSession = false;
				if (i > CurrentBar) return;

				if (SessionDeltaMode && i > 0 && IsNewSession(i))
				{
					_open = _cumDelta = _high = _low = 0;
					NewSession = true;
				}
				if(NewSession) ((ValueDataSeries)DataSeries[0]).SetPointOfEndLine(i-1);
				var currencCandle = GetCandle(i);

				if (i == 0|| NewSession) _cumDelta += currencCandle.Ask - currencCandle.Bid;
				else
				{
					if (SessionDeltaMode && i > 0 && IsNewSession(i))
					{
						_open = 0;
						_low = currencCandle.MinDelta;
						_high = currencCandle.MaxDelta;
						_cumDelta = currencCandle.Delta;
					}
					else
					{
						var prev =(decimal)DataSeries[0][i-1];
						_open = prev;
						_cumDelta = prev + currencCandle.Delta;
						var dh = currencCandle.MaxDelta - currencCandle.Delta;
						var dl = currencCandle.Delta - currencCandle.MinDelta;
						_low = _cumDelta - dl;
						_high = _cumDelta + dh;
					}
				}
				DataSeries[0][i] = _cumDelta;

				if (_cumDelta >= 0)
				{
					DataSeries[2][i] = _cumDelta;
					DataSeries[3][i] = 0m;
				}
				else
				{
					DataSeries[2][i] = 0m;
					DataSeries[3][i] = _cumDelta;
				}
				DataSeries[1][i] = new Candle
				{
					Close = _cumDelta,
					High = _high,
					Low = _low,
					Open = _open,
				};



				//if (set.Mode == SessionDeltaIndicatorSettings.SessionDeltaVisualMode.Bars)
				//	val.Color = val.value > 0 ? set.BuyColor : set.SellColor;
				//if (set.Mode == SessionDeltaIndicatorSettings.SessionDeltaVisualMode.Line)
				//	val.Color = set.BuyColor;
				//if (set.Mode == SessionDeltaIndicatorSettings.SessionDeltaVisualMode.Candles)
				//{
				//	val.Color = Candles[i].Delta > 0 ? set.BuyColor : set.SellColor;
				//}

				//DataSerieses[1].Values[i] = val;
				//if (set.Mode != SessionDeltaIndicatorSettings.SessionDeltaVisualMode.Candles)
				//	DataSerieses[0].Values[i] = new Value { Color = Color.Gray, value = 0 };
				//else DataSerieses[0].Values[i] = new Value { Color = lightGray, value = 0 };


				//CalculateMaxMin(i);
			}
			catch (Exception exc) {  }


		}

		#endregion
	}
}