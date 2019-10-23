namespace ATAS.Indicators.Technical
{
	using System;
	using System.ComponentModel;
	using System.Windows.Media;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;

	public class Pivots : Indicator
	{
		public enum Period
		{
			M5 = 3,
			M10 = 4,
			M15 = 5,
			M30 = 6,
			Hourly = -1,
			H4 = 7,
			Daily = 0,
			Weekly = 1,
			Monthly = 2,
			
		}

		private readonly ValueDataSeries _ppSeries;
		private readonly ValueDataSeries _r1Series;
		private readonly ValueDataSeries _r2Series;
		private readonly ValueDataSeries _r3Series;
		private readonly ValueDataSeries _s1Series;
		private readonly ValueDataSeries _s2Series;
		private readonly ValueDataSeries _s3Series;

		private decimal _currentDayClose;

		private decimal _currentDayHigh;
		private decimal _currentDayLow;
		private bool _newSessionWasStarted;
		private Period _pivotRange;

		private decimal _pp;
		private decimal _r1;
		private decimal _r2;
		private decimal _r3;
		private decimal _s1;
		private decimal _s2;
		private decimal _s3;

		[DisplayName( "PivotRange")]
		public Period PivotRange
		{
			get => _pivotRange;
			set
			{
				_pivotRange = value;
				RecalculateValues();
			}
		}

		public Pivots()
			: base(true)
		{
			_ppSeries = (ValueDataSeries)DataSeries[0];
			_ppSeries.VisualType = VisualMode.Hash;
			_ppSeries.Color = Colors.Goldenrod;
			_ppSeries.Name = "PP";

			_s1Series = new ValueDataSeries("S1")
			{
				Color = Colors.Crimson,
				VisualType = VisualMode.Hash
			};
			DataSeries.Add(_s1Series);
			_s2Series = new ValueDataSeries("S2")
			{
				Color = Colors.Crimson,
				VisualType = VisualMode.Hash
			};
			DataSeries.Add(_s2Series);
			_s3Series = new ValueDataSeries("S3")
			{
				Color = Colors.Crimson,
				VisualType = VisualMode.Hash
			};
			DataSeries.Add(_s3Series);

			_r1Series = new ValueDataSeries("R1")
			{
				Color = Colors.DodgerBlue,
				VisualType = VisualMode.Hash
			};
			DataSeries.Add(_r1Series);
			_r2Series = new ValueDataSeries("R2")
			{
				Color = Colors.DodgerBlue,
				VisualType = VisualMode.Hash
			};
			DataSeries.Add(_r2Series);
			_r3Series = new ValueDataSeries("R3")
			{
				Color = Colors.DodgerBlue,
				VisualType = VisualMode.Hash
			};
			DataSeries.Add(_r3Series);
		}

		protected override void OnCalculate(int bar, decimal value)
		{
			if (bar == 0)
			{
				_newSessionWasStarted = false;
				return;
			}
			_ppSeries[bar] = 0;
			_s1Series[bar] = 0;
			_s2Series[bar] = 0;
			_s3Series[bar] = 0;
			_r1Series[bar] = 0;
			_r2Series[bar] = 0;
			_r3Series[bar] = 0;

			var candle = GetCandle(bar);
			var isNewSession = IsNeSession(bar);
			if (isNewSession)
			{
				_newSessionWasStarted = true;
				_pp = (_currentDayHigh + _currentDayLow + _currentDayClose) / 3;
				_s1 = 2 * _pp - _currentDayHigh;
				_r1 = 2 * _pp - _currentDayLow;
				_s2 = _pp - (_currentDayHigh - _currentDayLow);
				_r2 = _pp + (_currentDayHigh - _currentDayLow);
				_s3 = _pp - 2 * (_currentDayHigh - _currentDayLow);
				_r3 = _pp + 2 * (_currentDayHigh - _currentDayLow);

				_currentDayHigh = _currentDayLow = _currentDayClose = 0;
			}
			//else
			//{
				if (candle.High > _currentDayHigh)
					_currentDayHigh = candle.High;
				if (candle.Low < _currentDayLow || _currentDayLow == 0)
					_currentDayLow = candle.Low;
				_currentDayClose = candle.Close;
		//	}
			if (_newSessionWasStarted)
			{
				_ppSeries[bar] = _pp;
				_s1Series[bar] = _s1;
				_s2Series[bar] = _s2;
				_s3Series[bar] = _s3;

				_r1Series[bar] = _r1;
				_r2Series[bar] = _r2;
				_r3Series[bar] = _r3;
			}
		}

		bool IsNeSession(int bar)
		{
			if (bar == 0)
				return true;
			switch (PivotRange)
			{
				case Period.M5:
					return isnewsession(5,bar);
				case Period.M10:
					return isnewsession(10, bar);
				case Period.M15:
					return isnewsession(15, bar);
				case Period.M30:
					return isnewsession(30, bar);
				case Period.Hourly:
					return GetCandle(bar).Time.Hour != GetCandle(bar - 1).Time.Hour;
				case Period.H4:
					return isnewsession(240, bar);
				case Period.Daily:
					return IsNewSession(bar);
				case Period.Weekly:
					return IsNewWeek(bar);
				case Period.Monthly:
					return IsNewMonth(bar);
			}
			return false;

		}

		bool isnewsession(int tf,int bar)
		{
			return (GetBeginTime(GetCandle(bar).Time, tf) - GetBeginTime(GetCandle(bar - 1).Time, tf)).TotalMinutes >= tf;
		}
		DateTime GetBeginTime(DateTime tim, int period)
		{
			var tim2 = tim;
			tim2 = tim2.AddMilliseconds(-tim2.Millisecond);
			tim2 = tim2.AddSeconds(-tim2.Second);
			var begin = Convert.ToInt32((tim2 - new DateTime()).TotalMinutes % period);
			var res = tim2.AddMinutes(-begin).AddMilliseconds(-tim2.Millisecond).AddSeconds(-tim2.Second);
			return res;
		}
	}
}