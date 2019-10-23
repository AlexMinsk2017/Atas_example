namespace ATAS.Indicators.Technical
{
	using System;
	using System.ComponentModel;
	using System.Windows.Media;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;

	using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

	[DisplayName("ParabolicSAR")]
	[Description( "ParabolicSAR")]
	public class ParabolicSAR : Indicator
	{
		private decimal _accel;
		private decimal _accelMax;
		private decimal _accelStart;
		private decimal _accelStep;
		private decimal _current;

		private bool _isDown;
		private bool _isIncreased;
		private int _lastbar;
		private decimal _prev;
		private decimal _revers;

		[Category( "Common")]
		[DisplayName( "AccelStart")]
		[PropertyOrder(20)]
		[Parameter]
		public decimal AccelStart
		{
			get => _accelStart;
			set
			{
				if (value <= 0)
					return;

				_accelStart = value;
				RecalculateValues();
			}
		}

		[Category( "Common")]
		[DisplayName( "AccelStep")]
		[PropertyOrder(21)]
		[Parameter]
		public decimal AccelStep
		{
			get => _accelStep;
			set
			{
				if (value <= 0)
					return;

				_accelStep = value;
				RecalculateValues();
			}
		}

		[Category( "Common")]
		[DisplayName( "AccelMax")]
		[PropertyOrder(22)]
		[Parameter]
		public decimal AccelMax
		{
			get => _accelMax;
			set
			{
				if (value <= 0)
					return;

				_accelMax = value;
				RecalculateValues();
			}
		}

		public ParabolicSAR()
			: base(true)
		{
			((ValueDataSeries)DataSeries[0]).VisualType = VisualMode.Dots;
			((ValueDataSeries)DataSeries[0]).Color = Colors.Blue;
			((ValueDataSeries)DataSeries[0]).Width = 2;

			AccelStart = 0.02m;
			AccelStep = 0.02m;
			AccelMax = 0.2m;
		}

		protected override void OnCalculate(int bar, decimal value)
		{
			if (bar == _lastbar)
				return;
			_lastbar = bar;
			Process(bar - 1);
		}

		private void Process(int bar)
		{
			var candle = GetCandle(bar);

			if (bar == 0)
			{
				_isDown = true;
				_isIncreased = true;

				_accel = AccelStart;

				_prev = candle.Low;
				_current = candle.High;
				_revers = candle.Low;

				return;
			}

			var prevCandle = GetCandle(bar - 1);

			if (!_isDown)
			{
				if (candle.High > _prev)
				{
					_isDown = true;
					_isIncreased = true;

					_prev = _revers;
					_current = candle.High;

					_accel = AccelStart;

					this[bar] = _prev;
				}
				else
				{
					if (candle.Low < _revers)
					{
						_revers = candle.Low;

						if (!_isIncreased)
						{
							_accel += AccelStep;

							if (_accel > AccelMax)
								_accel = AccelMax;
						}
					}

					_isIncreased = false;
					_prev += (_revers - _prev) * _accel;

					var maxHigh = Math.Max(candle.High, prevCandle.High);

					if (_prev < maxHigh)
						_prev = maxHigh;

					this[bar] = _prev;
				}
			}
			else
			{
				if (candle.Low >= _prev)
				{
					if (candle.High > _current)
					{
						_current = candle.High;

						if (!_isIncreased)
						{
							_accel += AccelStart;

							if (_accel > AccelMax)
								_accel = AccelMax;
						}
					}

					_isIncreased = false;
					_prev += (_current - _prev) * _accel;

					var minLow = Math.Min(candle.Low, prevCandle.Low);

					if (_prev > minLow)
						_prev = minLow;

					this[bar] = _prev;
				}
				else
				{
					_isDown = false;
					_isIncreased = true;

					_prev = _current;
					_revers = candle.Low;

					_accel = AccelStep;

					this[bar] = _prev;
				}
			}
		}
	}
}