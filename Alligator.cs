namespace ATAS.Indicators.Technical
{
	using System;
	using System.ComponentModel;
	using System.Windows.Media;

	[DisplayName("Alligator")]
	[Description("Alligator by Bill Williams")]
	public class Alligator : Indicator
	{
		#region Properties
		private readonly SMMA _jaw = new SMMA();
		private readonly SMMA _teeth = new SMMA();
		private readonly SMMA _lips = new SMMA();

		[DisplayName("1. Jaw Period")]
		public int JawPeriod
		{
			get { return _jaw.Period; }
			set
			{
				_jaw.Period = Math.Max(1, value);
				RecalculateValues();
			}
		}

		[DisplayName("2. Teeth Period")]
		public int TeethPeriod
		{
			get { return _teeth.Period; }
			set
			{
				_teeth.Period = Math.Max(1, value);
				RecalculateValues();
			}
		}

		[DisplayName("3. Lips Period")]
		public int LipsPeriod
		{
			get { return _lips.Period; }
			set
			{
				_lips.Period = Math.Max(1, value);
				RecalculateValues();
			}
		}
		#endregion

		public Alligator()
		{
			((ValueDataSeries)DataSeries[0]).Name = "1 Jaw";
			((ValueDataSeries)DataSeries[0]).VisualType = VisualMode.Line;
			((ValueDataSeries)DataSeries[0]).Color = Colors.Blue;

			DataSeries.Add(new ValueDataSeries("2 Teeth")
			{
				VisualType = VisualMode.Line,
				Color = Colors.Red
			});

			DataSeries.Add(new ValueDataSeries("3 Lips")
			{
				VisualType = VisualMode.Line,
				Color = Colors.Green
			});

			JawPeriod = 13;
			TeethPeriod = 8;
			LipsPeriod = 5;

		}

		protected override void OnCalculate(int bar, decimal value)
		{
			decimal average = (GetCandle(bar).Low + GetCandle(bar).High) / 2;
			if (bar < 8)
			{
				this[bar] = average;
			}
			else
			{
				this[bar] = _jaw.Calculate(bar - 8, (GetCandle(bar - 8).Low + GetCandle(bar - 8).High) / 2);
			}

			if (bar < 5)
			{
				DataSeries[1][bar] = average;
			}
			else
			{
				DataSeries[1][bar] = _teeth.Calculate(bar - 5, (GetCandle(bar - 5).Low + GetCandle(bar - 5).High) / 2);
			}

			if (bar < 3)
			{
				DataSeries[2][bar] = average;
			}
			else
			{
				DataSeries[2][bar] = _lips.Calculate(bar - 3, (GetCandle(bar - 3).Low + GetCandle(bar - 3).High) / 2);
			}
		}
	}
}