using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CoreBot
{
    public class CourseApiClient
    {
        private readonly HttpClient _httpClient;

        public CourseApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CourseDetails[]> GetCoursesAsync()
        {
            // X-CSRFToken başlığını eklemek için HttpClient'in DefaultRequestHeaders özelliğini kullanabilirsiniz.
            _httpClient.DefaultRequestHeaders.Add("X-CSRFToken", "3hm4Bz8ObA5qIRoVXgUBlfHdlINgwAE4xEFxtDXjD4AXjKYXIqMZakgEfarRZzx2");

            var request = new HttpRequestMessage(HttpMethod.Get, "https://courses.edx.org/api/courses/v1/courses/?page=1&page_size=5");
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var courses = JsonDocument.Parse(content);
            var courseDetailsList = new List<CourseDetails>();

            foreach (var course in courses.RootElement.GetProperty("results").EnumerateArray())
            {
                var name = course.GetProperty("name").GetString();
                var courseDetails = new CourseDetails { Name = name };
                courseDetailsList.Add(courseDetails);
            }

            return courseDetailsList.ToArray();

        }
    }
    public class Media
    {
        public string image { get; set; }
    }

    public class Result
    {
        public string name { get; set; }
        public Media media { get; set; }
    }

    public class Pagination
    {
        public string next { get; set; }
        public object previous { get; set; }
        public int count { get; set; }
        public int num_pages { get; set; }
    }
    public class RootObject
    {
        public List<Result> results { get; set; }
        public Pagination pagination { get; set; }
    }

}
