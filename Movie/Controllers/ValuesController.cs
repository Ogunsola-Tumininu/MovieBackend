using LiteDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Movie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private string APIKey = "da979bab";
        IList<MovieDetail> LastFiveMovies = new List<MovieDetail>();
        private string BaseURL = "http://www.omdbapi.com/";

       
        [HttpGet]
        [Route("Search")]
        public async Task<IActionResult> Search(string term)
        {
            if (term == null)
            {
                return StatusCode(404, new { error = "Oops !!!. Kindly enter a title." });
            }


            HttpClient client = new HttpClient();
            var BaseURL = "http://www.omdbapi.com/";

            try
            {
                client.BaseAddress = new Uri(BaseURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync(
                    $"?s={term}&apikey={APIKey}");

                if (response.IsSuccessStatusCode)
                {
                    Movies result = await response.Content.ReadFromJsonAsync<Movies>();

                    response.EnsureSuccessStatusCode();

                    return Ok(result.Search);

                }
                else
                {
                    return StatusCode(Convert.ToInt32(response.StatusCode), new { response.StatusCode, response.ReasonPhrase });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errMsg = ex.Message });

            }


        }

        [HttpGet]
        [Route("Detail")]
        public async Task<IActionResult> Detail(string title)
        {
            if (title == null)
            {
                return StatusCode(404, new { error = "Oops !!!. Kindly enter a search term." });
            }

            HttpClient client = new HttpClient();

            try
            {
                client.BaseAddress = new Uri(BaseURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync(
                    $"?t={title}&apikey={APIKey}");

                if (response.IsSuccessStatusCode)
                {
                    MovieDetail result = await response.Content.ReadFromJsonAsync<MovieDetail>();

                    response.EnsureSuccessStatusCode();

                    // Open database (or create if doesn't exist)
                    using (var db = new LiteDatabase(@"C:\Temp\MyData.db"))
                    {
                        // Get a collection (or create, if doesn't exist)
                        var col = db.GetCollection<MovieDetail>("MovieDetail");

                        col.EnsureIndex(x => x.Title);

                        // Insert new movie detail document (Id will be auto-incremented)
                        col.Insert(result);

                    }

                    return Ok(result);

                }
                else
                {
                    return StatusCode(Convert.ToInt32(response.StatusCode), new { response.StatusCode, response.ReasonPhrase });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errMsg = ex.Message });

            }


        }

        [HttpGet]
        [Route("PreviousSearch")]
        public IActionResult PreviousSearch()
        {
            try
            {
                using (var db = new LiteDatabase(@"C:\Temp\MyData.db"))
                {

                    // Get a collection (or create, if doesn't exist)
                    var col = db.GetCollection<MovieDetail>("MovieDetail");
                    // Use LINQ to query documents (filter, sort, transform)
                    var results = col.Query()
                        .OrderByDescending(e => e.Id)
                        .ToList().GroupBy(e =>e.Title)
                        .Select(grp => grp.First());

                    return Ok(results.Take(5));

                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
