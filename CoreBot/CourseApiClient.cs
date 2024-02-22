using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using CoreBot.Models;

namespace CoreBot
{
    public class CourseApiClient
    {
        private readonly HttpClient _httpClient;

        public CourseApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Courses[]> GetCoursesAsync()
        {
            _httpClient.DefaultRequestHeaders.Add("X-CSRFToken", "3hm4Bz8ObA5qIRoVXgUBlfHdlINgwAE4xEFxtDXjD4AXjKYXIqMZakgEfarRZzx2");

            var request = new HttpRequestMessage(HttpMethod.Get, "https://courses.edx.org/api/courses/v1/courses/?page=1&page_size=5");
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var courses = JsonDocument.Parse(content);
            var coursesList = new List<Courses>();

            foreach (var course in courses.RootElement.GetProperty("results").EnumerateArray())
            {
                var name = course.GetProperty("name").GetString();
                var id = course.GetProperty("course_id").GetString();
                var description = course.GetProperty("short_description").GetString();
                var courses_model = new Courses { Name = name, Id=id , Description=description};
                coursesList.Add(courses_model);
            }

            return coursesList.ToArray();

        }
    }

}
