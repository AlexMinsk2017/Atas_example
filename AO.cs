using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATAS.Indicators.Technical
{
	using System.ComponentModel;

	[DisplayName("Awesome Oscillator")]
	public class AwesomeOscillator : Indicator
	{
		private int p1 = 34;
		private int p2 = 5;
		private decimal lastAw = 0;
		readonly CandleDataSeries _reversalCandles = new CandleDataSeries("Candles");
		public int P1
		{
			get { return p1; }
			set
			{
				p1 = value;
				RecalculateValues();
			}
		}
		public int P2
		{
			get { return p2; }
			set
			{
				p2 = value;
				RecalculateValues();
			}
		}
		public AwesomeOscillator()
		{
			((ValueDataSeries)DataSeries[0]).VisualType = VisualMode.Hide;//Скрываем датасерию по умолчанию
			DataSeries.Add(_reversalCandles);//Добавляем датасерию свечек
			Panel = IndicatorDataProvider.NewPanel;//По умолчанию свечки будем отображать в отдельном окне
		}
		protected override void OnCalculate(int bar, decimal value)
		{
			if (bar >= (p1 - 1))
			{
				var f = bar;
				decimal sma1 = 0;
				decimal sma2 = 0;
				for (int ct = 1; ct <= p1; ct += 1)
				{
					var candleCt = GetCandle(f);
					sma1 = sma1 + (candleCt.High + candleCt.Low) / 2;
					if (ct <= p2)
					{
						sma2 = sma2 + (candleCt.High + candleCt.Low) / 2;
					}
					f -= 1;
				}
				decimal Aw = sma2 / p2 - sma1 / p1; //АО = SMA5 (Median) -SMA34 (Median) = fx-wiki.ru/wiki/Awesome_Oscillator
				var candleCt2 = GetCandle(bar);

				if (Aw >= lastAw)
				{
					if (Aw > 0)
					{
						_reversalCandles[bar].Open = 0;
						_reversalCandles[bar].Close = Aw;
						_reversalCandles[bar].High = Aw;
						_reversalCandles[bar].Low = 0;
					}
					else
					{
						_reversalCandles[bar].Open = Aw;
						_reversalCandles[bar].Close = 0;
						_reversalCandles[bar].High = 0;
						_reversalCandles[bar].Low = Aw;
					}
				}
				else
				{
					if (Aw > 0)
					{
						_reversalCandles[bar].Open = Aw;
						_reversalCandles[bar].Close = 0;
						_reversalCandles[bar].High = Aw;
						_reversalCandles[bar].Low = 0;
					}
					else
					{
						_reversalCandles[bar].Open = 0;
						_reversalCandles[bar].Close = Aw;
						_reversalCandles[bar].High = 0;
						_reversalCandles[bar].Low = Aw;
					}
				}
				lastAw = Aw;
				this[bar] = Aw;
			}
		}

		public override string ToString()
		{
			return "Awesome Oscillator";
		}
	}
}
