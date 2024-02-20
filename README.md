# EduBot

EduBot, edX platformundan kurs bilgilerini çekmek için kullanılan basit bir tutorial projesidir.

## Kullanım

1. `CourseApiClient.cs` dosyasındaki `_httpClient` alanınızı yapılandırın.
2. `GetCoursesAsync` metodunu kullanarak kurs bilgilerini alabilirsiniz.
3. Dönen `CourseDetails[]` dizisi, kurs adlarını içerir.

Örnek Kullanım:

```csharp
var apiClient = new CourseApiClient();
var courses = await apiClient.GetCoursesAsync();
foreach (var course in courses)
{
    Console.WriteLine(course.Name);
}

[edX api](https://courses.edx.org/api-docs/#/)
