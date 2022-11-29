using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WebApiServer.Models;

namespace WebApiServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    { 
        [HttpGet]
        public async Task<string> Get()
        {
            return "Server is Launched";
        }

        [Route("getCourses")]
        public async Task<string> getCourses(string abbr, DateTime startDate, DateTime endDate)
        {
            try
            {
                string fileName = "data.json";
                if(!System.IO.File.Exists(fileName))
                {
                    WriteData();
                }
                string result = System.IO.File.ReadAllText(fileName);
                List<CourseJson> courses = new List<CourseJson>();
                foreach (CourseJson item in JsonSerializer.Deserialize<List<CourseJson>>(result))
                {
                    bool a = item.Currency == abbr;
                    bool b = startDate >= item.Date;
                    bool c = endDate <= item.Date;
                    if (item.Currency == abbr && item.Date >= startDate && item.Date <= endDate)
                    {

                        courses.Add(item);
                    }
                }
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(courses, options);
                return json;
            } catch (Exception ex)
            {
                return ex.Message;
            }

        }

        private async void WriteData()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync("https://www.nbrb.by/api/exrates/currencies");
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        List<string> currency = new List<string> { "USD", "EUR", "RUB" };
                        string message = await response.Content.ReadAsStringAsync();
                        List<CourseJson> courses = new List<CourseJson>();

                        foreach (var item in JsonSerializer.Deserialize<List<Course>>(message))
                        {
                            if (currency.Contains(item.Cur_Abbreviation) && item.Cur_DateEnd.Year > 2016)
                            {
                                var date = DateTime.Now.AddYears(-5);
                                while (date < DateTime.Now)
                                {
                                    if (date > item.Cur_DateStart && date < item.Cur_DateEnd)
                                    {
                                        var resp = await client.GetAsync($"https://www.nbrb.by/api/exrates/rates/dynamics/{item.Cur_ID}?startdate={date.Year}-{date.Month}-{date.Day}&enddate={date.AddYears(1).Year}-{date.AddYears(1).Month}-{date.AddYears(1).Day}");
                                        string msg = await resp.Content.ReadAsStringAsync();

                                        foreach (var it in JsonSerializer.Deserialize<List<Course>>(msg))
                                        {
                                            it.Cur_Abbreviation = item.Cur_Abbreviation;
                                            it.Cur_Scale = item.Cur_Scale;
                                            CourseJson courseJson = new CourseJson();
                                            courseJson.Date = it.Date;
                                            courseJson.Currency = it.Cur_Abbreviation;
                                            courseJson.Amount = it.Cur_Scale;
                                            courseJson.Value = it.Cur_OfficialRate;
                                            courses.Add(courseJson);
                                        }
                                    }
                                    date = date.AddYears(1);

                                }
                            }
                        }
                        string fileName = "data.json";
                        var options = new JsonSerializerOptions { WriteIndented = true };
                        string json = JsonSerializer.Serialize(courses, options);

                        System.IO.File.WriteAllText(fileName, json);


                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}

