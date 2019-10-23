namespace ATAS.Indicators.Technical
{
	using System;
	using System.ComponentModel;
	using System.Windows.Media;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;

	public class VWAP : Indicator
	{
		#region Properties

		[DisplayName( "Period")]
		public VWAPPeriodType Type
		{
			get => periodType;
			set { periodType = value; RecalculateValues(); }
		}

		[DisplayName( "FirstDev")]
		public double StDev
		{
			get => _stdev;
			set { _stdev = Math.Max(value, 0); RecalculateValues(); }
		}

		[DisplayName( "SecondDev")]
		public double StDev1
		{
			get => _stdev1;
			set { _stdev1 = Math.Max(value, 0); RecalculateValues(); }
		}

		[DisplayName( "Period")]
		public int Period
		{
			get => period;
			set
			{
				period = Math.Max(value, 1);
				RecalculateValues();
			}
		}

		#endregion

		#region private variables

		private int zeroBar = 0;
		private int _lastbar = -1;
		private decimal sum = 0;
		private int n = 0;
		private double _stdev = 1;
		private double _stdev1 = 2;
		public int period = 300;
		private VWAPPeriodType periodType = VWAPPeriodType.Daily;

		#endregion

		#region Dataserieces

		ValueDataSeries _sqrt = new ValueDataSeries("sqrt");
		ValueDataSeries _upper = new ValueDataSeries("Upper std1") { Color = Colors.DodgerBlue };
		ValueDataSeries _lower = new ValueDataSeries("Lower std1") { Color = Colors.DodgerBlue };
		ValueDataSeries _upper1 = new ValueDataSeries("Upper std2") { Color = Colors.DodgerBlue };
		ValueDataSeries _lower1 = new ValueDataSeries("Lower std2") { Color = Colors.DodgerBlue };
		ValueDataSeries _totalVolume = new ValueDataSeries("totalVolume");
		ValueDataSeries _totalVolToClose = new ValueDataSeries("volToClose");

		#endregion

		public VWAP()
		{
			var series = (ValueDataSeries)DataSeries[0];
			series.Color = Colors.Firebrick;
			DataSeries.Add(_lower1);
			DataSeries.Add(_upper1);
			DataSeries.Add(_lower);
			DataSeries.Add(_upper);
		}

		protected override void OnCalculate(int bar, decimal value)
		{
			bool needReset = false;
			var candle = GetCandle(bar);
			if(candle.Volume==0) return;
			if (bar == 0)
			{
				zeroBar = bar;
				n = 0;
				sum = 0;
				this[bar] = _upper[bar] = _lower[bar] = _upper1[bar] = _lower1[bar] = candle.Close;
				return;
			}

			if (Type == VWAPPeriodType.Daily && IsNewSession(bar))
			{
				needReset = true;
			}
			else if (Type == VWAPPeriodType.Weekly && IsNewWeek(bar))
			{
				needReset = true;
			}
			else if (Type == VWAPPeriodType.Monthly && IsNewMonth(bar))
			{
				needReset = true;
			}

			bool setStartOfLine = needReset;
			if (setStartOfLine && Type == VWAPPeriodType.Daily && this.TimeFrame == "Daily")
				setStartOfLine = false;

			if (bar == 0 || needReset)
			{
				zeroBar = bar;
				n = 0;
				sum = 0;
				_totalVolume[bar] = candle.Volume;
				_totalVolToClose[bar] = candle.Close * 1.0m * candle.Volume * 1.0m;
				if (setStartOfLine)
				{
					((ValueDataSeries)DataSeries[0]).SetPointOfEndLine(bar - 1);
					_upper.SetPointOfEndLine(bar - 1);
					_lower.SetPointOfEndLine(bar - 1);
					_upper1.SetPointOfEndLine(bar - 1);
					_lower1.SetPointOfEndLine(bar - 1);
				}
				

			}
			else
			{
				_totalVolume[bar] = _totalVolume[bar - 1] + candle.Volume;
				_totalVolToClose[bar] = _totalVolToClose[bar-1]+ candle.Close * 1.0m * candle.Volume * 1.0m;

			}
			this[bar] = _totalVolToClose[bar] / _totalVolume[bar];
			_sqrt[bar]= (decimal)Math.Pow((double)((candle.Close - this[bar])/TickSize), 2);

			int k = bar;
			if (_lastbar != bar)
			{
				n = 0;
				sum = 0;
				for (int j = 0; j < Period; j++)
				{
					n++;
					k--;
					if (k < zeroBar) break;
					sum += _sqrt[k];
				}
			}
			
			var summ = sum + _sqrt[bar];
			var stdDev = (decimal)Math.Sqrt((double)summ / (n+1));

			_upper[bar] = this[bar] + stdDev * (decimal)_stdev*TickSize;
			_lower[bar] = this[bar] - stdDev * (decimal)_stdev * TickSize;
			_upper1[bar] = this[bar] + stdDev * (decimal)_stdev1 * TickSize;
			_lower1[bar] = this[bar] - stdDev * (decimal)_stdev1 * TickSize;
		}

		public enum VWAPPeriodType
		{
			Daily, Weekly, Monthly, All
		}
	}
}