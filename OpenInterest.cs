namespace ATAS.Indicators.Technical
{
	using System.ComponentModel;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;

	[DisplayName("Open Interest")]
	public class OpenInterest : Indicator
	{
		public enum OpenInterestMode
		{
			ByBar,
			Session,
			Cumulative
		}

		private readonly CandleDataSeries _oi = new CandleDataSeries("Open interest");

		private OpenInterestMode _mode = OpenInterestMode.ByBar;

		[DisplayName( "Mode")]
		public OpenInterestMode Mode
		{
			get => _mode;
			set
			{
				_mode = value;
				RecalculateValues();
			}
		}

		public OpenInterest()
			: base(true)
		{
			((ValueDataSeries)DataSeries[0]).VisualType = VisualMode.OnlyValueOnAxis; //Скрываем датасерию по умолчанию
			DataSeries[0].Name = "Value";
			DataSeries.Add(_oi); //Добавляем датасерию свечек
			Panel = IndicatorDataProvider.NewPanel; //По умолчанию свечки будем отображать в отдельном окне
		}

		protected override void OnCalculate(int bar, decimal value)
		{
			if (bar == 0)
				return;

			var currentCandle = GetCandle(bar);
			var prevCandle = GetCandle(bar - 1);
			var currentOpen = prevCandle.OI;
			var candle = _oi[bar];

			switch (_mode)
			{
				case OpenInterestMode.ByBar:
					candle.Open = 0;
					candle.Close = currentCandle.OI - currentOpen;
					candle.High = currentCandle.MaxOI - currentOpen;
					candle.Low = currentCandle.MinOI - currentOpen;
					break;

				case OpenInterestMode.Cumulative:
					candle.Open = currentOpen;
					candle.Close = currentCandle.OI;
					candle.High = currentCandle.MaxOI;
					candle.Low = currentCandle.MinOI;
					break;

				default:
					var prevvalue = _oi[bar - 1].Close;
					var dOi = currentOpen - prevvalue;

					if (IsNewSession(bar))
						dOi = currentOpen;

					candle.Open = currentOpen - dOi;
					candle.Close = currentCandle.OI - dOi;
					candle.High = currentCandle.MaxOI - dOi;
					candle.Low = currentCandle.MinOI - dOi;
					break;
			}

			this[bar] = candle.Close;
		}
	}
}