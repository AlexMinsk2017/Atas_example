namespace ATAS.Indicators.Technical
{
	using System;
	using System.ComponentModel;
	using System.Windows.Media;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;

	[Category("Bid x Ask,Delta,Volume")]
	public class Delta : Indicator
	{
		[Serializable]
		public enum DeltaVisualMode
		{
			Candles = 0,

			HighLow = 1,

			Bars = 2
		}

		[Serializable]
		public enum BarDirection
		{

			Any = 0,


			Bullish = 1,

			Bearlish = 2
		}

		[Serializable]
		public enum DeltaType
		{

			Any = 0,


			Positive = 1,

			Negative = 2
		}


		private readonly ValueDataSeries _positiveDelta;
		private readonly ValueDataSeries _negativeDelta = new ValueDataSeries("Negative delta") { Color = System.Windows.Media.Colors.Red, VisualType = VisualMode.Histogram, ShowZeroValue = false };
		private readonly ValueDataSeries _diapasonhigh = new ValueDataSeries("Delta range high") { Color = System.Windows.Media.Color.FromArgb(128, 128, 128, 128), ShowZeroValue = false, ShowCurrentValue = false };
		private readonly ValueDataSeries _diapasonlow = new ValueDataSeries("Delta range low") { Color = System.Windows.Media.Color.FromArgb(128, 128, 128, 128), ShowZeroValue = false, ShowCurrentValue = false };
		private readonly CandleDataSeries _candles = new CandleDataSeries("Delta candles") { DownCandleColor = System.Windows.Media.Colors.Red, UpCandleColor = System.Windows.Media.Colors.Green, };
		private readonly ValueDataSeries _values = new ValueDataSeries("Values") { VisualType = VisualMode.OnlyValueOnAxis};

		[Category( "Alerts")]
		[DisplayName( "UseAlerts")]
		public bool UseAlerts
		{
			get;
			set;
		}

		[Category( "Alerts")]
		[DisplayName( "AlertFile")]
		public string AlertFile { get; set; } = "alert1";

		[Category( "Alerts")]
		[DisplayName( "FontColor")]
		public System.Windows.Media.Color AlertForeColor { get; set; } = Color.FromArgb(255, 247, 249, 249);

		[Category( "Alerts")]
		[DisplayName( "BackGround")]
		public System.Windows.Media.Color AlertBGColor { get; set; } = Color.FromArgb(255, 75, 72, 72);

		[Category( "Alerts")]
		[DisplayName( "Filter")]
		public decimal AlertFilter { get; set; }



		private DeltaVisualMode _mode = DeltaVisualMode.Candles;

		[DisplayName( "VisualMode")]
		public DeltaVisualMode Mode
		{
			get => _mode;
			set
			{
				_mode = value;

				if (_mode == DeltaVisualMode.Bars)
				{
					_positiveDelta.VisualType = _negativeDelta.VisualType = VisualMode.Histogram;
					_diapasonhigh.VisualType = VisualMode.Hide;
					_diapasonlow.VisualType = VisualMode.Hide;
					_candles.Visible = false;
				}
				else if (_mode == DeltaVisualMode.HighLow)
				{
					_positiveDelta.VisualType = _negativeDelta.VisualType = VisualMode.Histogram;
					_diapasonhigh.VisualType = VisualMode.Histogram;
					_diapasonlow.VisualType = VisualMode.Histogram;
					_candles.Visible = false;
				}
				else
				{
					_positiveDelta.VisualType = _negativeDelta.VisualType = VisualMode.Hide;
					_diapasonhigh.VisualType = VisualMode.Hide;
					_diapasonlow.VisualType = VisualMode.Hide;
					_candles.Visible = true;
				}

				RaisePropertyChanged("Mode");
				RecalculateValues();
			}
		}

		private bool _minimizedMode;

		[DisplayName( "Minimizedmode")]
		public bool MinimizedMode
		{
			get => _minimizedMode;
			set
			{
				_minimizedMode = value; RaisePropertyChanged("MinimizedMode");
				RecalculateValues();
			}
		}

		private decimal _filter;

		private BarDirection _barDirection;

		[Category( "Filters")]
		[DisplayName( "BarsDirection")]
		public BarDirection BarsDirection
		{
			get { return _barDirection; }
			set
			{
				_barDirection = value;
				RecalculateValues();
			}
		}


		private DeltaType _deltaType;

		[Category( "Filters")]
		[DisplayName( "DeltaType")]
		public DeltaType DeltaTypes
		{
			get { return _deltaType; }
			set
			{
				_deltaType = value;
				RecalculateValues();
			}
		}


		[DisplayName( "Filter")]
		[Category( "Filters")]
		public decimal Filter
		{
			get { return _filter; }
			set { _filter = value;RecalculateValues(); }
		}


		public Delta()
		{
			Panel = IndicatorDataProvider.NewPanel;
			_positiveDelta = (ValueDataSeries)DataSeries[0];//2
			_positiveDelta.Name = "Positive delta";
			_positiveDelta.Color = System.Windows.Media.Colors.Green;
			_positiveDelta.VisualType = VisualMode.Histogram;
			_positiveDelta.ShowZeroValue = false;
			DataSeries.Add(_negativeDelta);//3
			DataSeries.Insert(0, _diapasonhigh);//0
			DataSeries.Insert(1, _diapasonlow);//1
			DataSeries.Add(_candles);//4
			DataSeries.Add(_values);
			Mode = Mode;
		}

		#region Overrides of Indicator

		protected override void OnCalculate(int bar, decimal value)
		{
			var candle = GetCandle(bar);
			var deltavalue = candle.Delta;
			var absdelta = Math.Abs(deltavalue);
			var maxDelta = candle.MaxDelta;
			var minDelta = candle.MinDelta;

			bool isUnderFilter = absdelta < _filter;

			if (_barDirection == BarDirection.Bullish)
			{
				if (candle.Close < candle.Open)
					isUnderFilter = true;
			}
			else if (_barDirection == BarDirection.Bearlish)
			{
				if (candle.Close > candle.Open)
					isUnderFilter = true;
			}

			if (_deltaType == DeltaType.Negative && deltavalue > 0)
				isUnderFilter = true;

			if (_deltaType == DeltaType.Positive && deltavalue < 0)
				isUnderFilter = true;

			if (isUnderFilter)
			{
				deltavalue = 0;
				absdelta = 0;
				minDelta = maxDelta = 0;
			}
			if (deltavalue > 0)
			{
				_positiveDelta[bar] = deltavalue;
				_negativeDelta[bar] = 0;
			}
			else
			{
				_positiveDelta[bar] = 0;
				_negativeDelta[bar] = MinimizedMode ? absdelta : deltavalue;
			}

			
			if (MinimizedMode)
			{
				var high = Math.Abs(maxDelta);
				var low = Math.Abs(minDelta);
				_diapasonlow[bar] = Math.Min(Math.Min(high, low), absdelta);
				_diapasonhigh[bar] = Math.Max(high, low);

				_candles[bar].Open = deltavalue > 0 ? 0 : absdelta;
				_candles[bar].Close = deltavalue > 0 ? absdelta : 0;
				_candles[bar].High = _diapasonhigh[bar];
				_candles[bar].Low = _diapasonlow[bar];

				_values[bar] = _candles[bar].Close;
			}
			else
			{
				_diapasonlow[bar] = minDelta;
				_diapasonhigh[bar] = maxDelta;



				_candles[bar].Open = 0;
				_candles[bar].Close = deltavalue;
				_candles[bar].High = maxDelta;
				_candles[bar].Low = minDelta;

				if (UseAlerts&&CurrentBar - 1 == bar&& lastBarAlert!=bar)
				{
					if (AlertFilter > 0)
					{
						if (deltavalue > AlertFilter && _values[bar] < AlertFilter)
						{
							lastBarAlert = bar;
							AddAlert(AlertFile, Instrument, $"Delta is reached {AlertFilter} filter", AlertBGColor, AlertForeColor);
						}
					}
					if (AlertFilter < 0)
					{
						if (deltavalue < AlertFilter && _values[bar] > AlertFilter)
						{
							lastBarAlert = bar;
							AddAlert(AlertFile, Instrument, $"Delta is reached {AlertFilter} filter", AlertBGColor, AlertForeColor);
						}
					}

				}
				_values[bar] = deltavalue;
			}
		}

		private int lastBarAlert = 0;

		#endregion
	}
}