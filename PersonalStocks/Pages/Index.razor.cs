using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using PersonalStocks;
using PersonalStocks.Shared;
using PersonalStocks.Data;
using System.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ChartJs.Blazor.LineChart;
using ChartJs.Blazor.Common;
using ChartJs.Blazor.Util;
using ChartJs.Blazor.Common.Enums;
using System.Drawing;

namespace PersonalStocks.Pages
{
    public partial class Index
    {
        [Inject]
        private PersonalStocksService service { get; set; }
        private LineConfig _config;

        protected override async Task OnInitializedAsync()
        {
            _config = new LineConfig
            {
                Options = new LineOptions
                {
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = "Personal Stocks"
                    },
                    Tooltips = new Tooltips
                    {
                        Mode = InteractionMode.Nearest,
                        Intersect = true
                    },
                    Hover = new Hover
                    {
                        Mode = InteractionMode.Nearest,
                        Intersect = true
                    }
                }
            };

            var stocks = await service.GetStocks();          

            foreach (var stock in stocks)
            {
                var stockMovements = await service.GetMovements(stock);

                StockDataset dataset = new StockDataset(stock.Name); 
                if(!_config.Data.XLabels.Contains("Start")) _config.Data.XLabels.Add("Start");

                dataset.Add(stock.StartingValue);
                foreach (var m in stockMovements)
                {
                    if (!_config.Data.XLabels.Contains(m.Date.ToShortDateString())) _config.Data.XLabels.Add(m.Date.ToShortDateString());
                    if(m.Unit == "%")
                        m.Stock.CurrentValue += m.Stock.CurrentValue * m.Value / 100;
                    else
                        m.Stock.CurrentValue += m.Value;
                    dataset.Add(m.Stock.CurrentValue);
                }
                _config.Data.Datasets.Add(dataset);
            }
        }
    }

    class StockDataset : LineDataset<double>
    {
        public StockDataset(string name)
        {
            Random random = new Random();
            Color randomColor = Color.FromArgb((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));

            Label = name;
            BackgroundColor = ColorUtil.FromDrawingColor(randomColor);
            BorderColor = ColorUtil.FromDrawingColor(randomColor);
            Fill = FillingMode.Disabled;

        }
    }
}




