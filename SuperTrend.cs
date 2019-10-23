namespace ATAS.Indicators.Technical
{
	using System;
	using System.ComponentModel;
	using System.Windows.Media;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;

	using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

	[DisplayName("Super Trend")]
	public class SuperTrend : Indicator
	{
		private readonly ATR _atr = new ATR();

		[Category( "Common")]
		[DisplayName( "Period")]
		[PropertyOrder(20)]
		[Parameter]
		public int Period
		{
			get => _atr.Period;
			set
			{
				if (value <= 0)
					return;
				_atr.Period = value;
				RecalculateValues();
			}
		}

		private decimal _multiplier=1.7m;
		[DisplayName( "Multiplier")]
		[Category( "Common")]
		public decimal Multiplier
		{
			get { return _multiplier; }
			set { _multiplier = value; RecalculateValues(); }
		}

		ValueDataSeries _trend=new ValueDataSeries("trend");
		private ValueDataSeries _upTrend;
		private ValueDataSeries _dnTrend = new ValueDataSeries("Down Trend"){VisualType = VisualMode.Square,Color = Colors.Maroon,Width = 2};

		public SuperTrend():base(true)
		{
			Period = 14;
			_upTrend = (ValueDataSeries)DataSeries[0];
			_upTrend.Name = "Up Trend";
			_upTrend.Width = 2;
			_upTrend.VisualType=VisualMode.Square;
			_upTrend.Color = Colors.Blue;
			DataSeries.Add(_dnTrend);

			Add(_atr);
		}

		protected override void OnCalculate(int bar, decimal value)
		{
			if(bar==0) return;

			_upTrend[bar] = _dnTrend[bar] = 0;
			var candle = GetCandle(bar);
			var prevcandle = GetCandle(bar - 1);
			var median = (candle.Low + candle.High) / 2;
			var atr = _atr[bar];
			var dUpperLevel = median + atr * Multiplier;
			var dLowerLevel = median - atr * Multiplier;
			

			// Set supertrend levels
			if (candle.Close > _trend[bar-1] && prevcandle.Close <= _trend[bar-1])
			{
				_trend[bar] = dLowerLevel;
			}
			else if (candle.Close < _trend[bar-1] && prevcandle.Close >= _trend[bar- 1])
			{
				_trend[bar] = dUpperLevel;
			}
			else if (_trend[bar-1] < dLowerLevel)
				_trend[bar] = dLowerLevel;
			else if (_trend[bar-1] > dUpperLevel)
				_trend[bar] = dUpperLevel;
			else
				_trend[bar] = _trend[bar-1];

			if (candle.Close > _trend[bar] || (candle.Close == _trend[bar] &&prevcandle.Close  > _trend[bar- 1]))
				_upTrend[bar] = _trend[bar];
			else if (candle.Close < _trend[bar] || (candle.Close == _trend[bar] && prevcandle.Close < _trend[bar- 1]))
				_dnTrend[bar] = _trend[bar];

		}
	}



}