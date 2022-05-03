using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CoreModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace WebCrawler
{
    
    public abstract class CrawlableSource
    {
        //главный метод обхода сайта, этот обход должен заканчиваться в определенный момент
        //сделаю флаг, "Crawl", пока он true то обход продолжается
        public abstract IAsyncEnumerable<Event> CrawlAsync(int? maxCountEvents, HttpClient httpClient);

        //флаг, пока он истина у нас продолжается обход сайта,
        //его состояние должно меняться в определенный момент, от наследника к наследнику этот момент может быть разным
        //по логике должен изменяться когда мы натыкаемся на ссылку статьи которая у нас уже есть в базе
        //но если мы парсим этот источник впервый раз то должна быть другая причина чтобы остановиться, например количество уже распаршенных новостей или дата рассматриваемой новости
        //
        public bool isCrawl { get; protected set; }

        //метод который будет узменять состояние флага, при достижении определенных условий, определяемых в наследнике
        protected abstract bool StopCrawl();
    }

   
}