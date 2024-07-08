using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

public class RestfulApi

{
	private readonly HttpClient _client;

	public RestfulApi()
	{
		_client = new HttpClient { BaseAddress = new Uri("https://api.restful-api.dev/") };
		
	}

	[Test]
	public async Task GetObjects()
	{
        var response = await _client.GetAsync("objects");
        response.EnsureSuccessStatusCode();

        var objects = await response.Content.ReadFromJsonAsync<List<dynamic>>();
        Assert.NotNull(objects);
        Assert.NotEmpty(objects);

        // Specific object to check
        var expectedObject = new
        {
            id = "1",
            name = "Google Pixel 6 Pro",
            data = new
            {
                color = "Cloudy White",
                capacity = "128 GB"
            }
        };

        // Assertion to check if the expected object is in the list
        var objectExists = objects.Any(obj =>
            (string)obj.id == expectedObject.id &&
            (string)obj.name == expectedObject.name &&
            (string)obj.data.color == expectedObject.data.color &&
            (string)obj.data.capacity == expectedObject.data.capacity
        );

        Assert.True(objectExists, "The expected object is not present in the list.");
    }

    [Test]
    public async Task AddObject_ReturnsCreatedObject()
    {
        var newObject = new
        {
            name = "Apple MacBook Pro 16",
            data = new
            {
                year = 2019,
                price = 1849.99,
                CPU_model = "Intel Core i9",
                Hard_disk_size = "1 TB"
            }
        };

        var response = await _client.PostAsJsonAsync("objects", newObject);
        response.EnsureSuccessStatusCode();

        var createdObject = await response.Content.ReadFromJsonAsync<dynamic>();
        Assert.NotNull(createdObject);
        Assert.Equal(newObject.name, (string)createdObject.name);
        Assert.Equal(newObject.data.year, (int)createdObject.data.year);
        Assert.Equal(newObject.data.price, (decimal)createdObject.data.price);
        Assert.Equal(newObject.data.CPU_model, (string)createdObject.data.CPU_model);
        Assert.Equal(newObject.data.Hard_disk_size, (string)createdObject.data.Hard_disk_size);
    }

    [Test]
    public async Task GetObjectById_ReturnsCorrectObject()
    {
        // Expected object details
        var expectedObject = new
        {
            id = "7",
            name = "Apple MacBook Pro 16",
            data = new
            {
                year = 2019,
                price = 1849.99,
                CPU_model = "Intel Core i9",
                Hard_disk_size = "1 TB"
            }
        };

        // Make a GET request to retrieve the object by ID
        var response = await _client.GetAsync("https://api.restful-api.dev/objects/7");
        response.EnsureSuccessStatusCode();

        var fetchedObject = await response.Content.ReadFromJsonAsync<dynamic>();
        Assert.NotNull(fetchedObject);

        // Assert that the fetched object's properties match the expected object's properties
        Assert.Equal(expectedObject.id, (string)fetchedObject.id);
        Assert.Equal(expectedObject.name, (string)fetchedObject.name);
    }

    [Test]
    public async Task UpdateObject_ReturnsUpdatedObject()
    {
        var updatedObject = new
        {
            name = "Apple MacBook Pro 16",
            data = new
            {
                year = 2019,
                price = 2049.99,
                CPU_model = "Intel Core i9",
                Hard_disk_size = "1 TB",
                color = "silver"
            }
        };

        // Update object with ID 7 using PUT request
        var response = await _client.PutAsJsonAsync("https://api.restful-api.dev/objects/7", updatedObject);
        response.EnsureSuccessStatusCode();

        var updatedResponse = await response.Content.ReadFromJsonAsync<dynamic>();
        Assert.NotNull(updatedResponse);

        // Assert updated object properties
        Assert.Equal(updatedObject.name, (string)updatedResponse.name);
        Assert.Equal(updatedObject.data.year, (int)updatedResponse.data.year);
        Assert.Equal(updatedObject.data.price, (decimal)updatedResponse.data.price);
        Assert.Equal(updatedObject.data.CPU_model, (string)updatedResponse.data.CPU_model);
        Assert.Equal(updatedObject.data.Hard_disk_size, (string)updatedResponse.data.Hard_disk_size);
        Assert.Equal(updatedObject.data.color, (string)updatedResponse.data.color);
    }

    [Test]
    public async Task DeleteObject_RemovesObject()
    {
        // Perform DELETE request to delete object with ID 6
        var response = await _client.DeleteAsync("https://api.restful-api.dev/objects/6");
        response.EnsureSuccessStatusCode();

        var deleteResponse = await response.Content.ReadFromJsonAsync<dynamic>();
        Assert.NotNull(deleteResponse);
        Assert.Equal("Object with id = 6, has been deleted.", (string)deleteResponse.message);
    }
}