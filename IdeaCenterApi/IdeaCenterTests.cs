using RestSharp;
using RestSharp.Authenticators;
using System.ComponentModel.Design.Serialization;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using IdeaCenterApi;


namespace IdeaCenterApi
{
    public class ApiTests
    {
        private string _url = "http://softuni-qa-loadbalancer-2137572849.eu-north-1.elb.amazonaws.com:84/api";
        private string _token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJKd3RTZXJ2aWNlQWNjZXNzVG9rZW4iLCJqdGkiOiI4OWI1NGM4Yi05N2IyLTRkM2QtOGI2OC1jYTVlY2VhN2ExZjkiLCJpYXQiOiIwNC8wOS8yMDI0IDEyOjA3OjU1IiwiVXNlcklkIjoiOTJkZTA3NjQtMTc2NC00YmYxLTgzYzctMDhkYzRmZWM4Yzc4IiwiRW1haWwiOiJ0ZXN0dGVzdEB0ZXN0LmNvbSIsIlVzZXJOYW1lIjoiVGVzdFVzZXJJZGVhIiwiZXhwIjoxNzEyNjg2MDc1LCJpc3MiOiJJZGVhQ2VudGVyX0FwcF9Tb2Z0VW5pIiwiYXVkIjoiSWRlYUNlbnRlcl9XZWJBUElfU29mdFVuaSJ9.Ln7a1eeBCtr0JAZ39VZ_cWE6wnfb8JDRChxdLOBwAS0";
        private RestClient _client;

        public static string _lastCreatedIdeaId;

        [SetUp]
        public void Setup()
        {
            // Create options for authentication to the url
            var options = new RestClientOptions(_url)
            {
                // Create new authenticator using JWT token
                Authenticator = new JwtAuthenticator(_token)
            };
            
            // Create new a instance of the Rest Client using the options
            this._client = new RestClient(options);
        }

        [Order(1)]
        [Test]
        public async Task Post_PostRequest_CreateAnIdeaWithValidParameters_ReturnsOk()
        {
            // Arrange
            var request = new RestRequest("/Idea/Create", Method.Post);

            var ideaCreation = new Idea()
            {
                Title = "Test RestSharp",
                Description = "Random Description",
                Url = ""
            };
            request.AddJsonBody(ideaCreation);

            // Act
            var response = await this._client.ExecuteAsync(request);
            var responseJson = JsonSerializer.Deserialize<ApiResponse>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseJson.Msg, Is.EqualTo("Successfully created!"));
            Assert.That(responseJson.Idea.Title, Is.EqualTo(ideaCreation.Title));
        }

        [Order(2)]
        [Test]
        public async Task Get_GetAllIdeas_ShouldReturnAllAvailableIdeas()
        {
            // Arrange
            var request = new RestRequest("/Idea/All", Method.Get);

            // Act
            var response = await this._client.ExecuteAsync(request);

            var responseJson = JsonSerializer.Deserialize<List<Idea>>(response.Content);

            var lastCreatedIdea = responseJson.LastOrDefault();
            _lastCreatedIdeaId = lastCreatedIdea.IdeaId;

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseJson.Count, Is.Positive);           

        }

        [Order(3)]
        [Test]
        public async Task Put_EditIdea_ShouldUpdateWholeIdeaInfo()
        {
            // Arrange
            var request = new RestRequest($"/Idea/Edit?ideaId={_lastCreatedIdeaId}", Method.Put);

            var ideaToUpdate = new Idea()
            {
                Title = "Updated Title",
                Description = "Updated Description"
            };

            request.AddBody(ideaToUpdate);

            // Act
            var response = await this._client.ExecuteAsync(request);
            var responseJson = JsonSerializer.Deserialize<ApiResponse>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseJson.Msg, Is.EqualTo("Edited successfully"));
            Assert.That(responseJson.Idea.Title, Is.EqualTo(ideaToUpdate.Title));

        }

        [Order(4)]
        [Test]
        public async Task Delete_DeleteRequest_ShouldRemoveIdeaByGivenId()
        {
            // Arrange
            var request = new RestRequest($"Idea/Delete?ideaId={_lastCreatedIdeaId}", Method.Delete);

            // Act
            var response = await this._client.ExecuteAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Does.Contain("The idea is deleted!"));
        }

        [Order(5)]
        [Test]
        public async Task Post_CreateIdea_ShouldReturnBadRequest_WhenMissingRequiredFields()
        {
            // Arrange
            var request = new RestRequest("/Idea/Create", Method.Post);

            var ideaCreation = new Idea()
            {            
                Url = ""
            };
            request.AddJsonBody(ideaCreation);

            // Act
            var response = await this._client.ExecuteAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        }

        [Order(6)]
        [Test]
        public async Task Put_EditIdea_ShouldReturnBadRequest_WhenGivenInvalidId()
        {
            // Arrange
            string invalidId = "invalidId134";
            var request = new RestRequest($"/Idea/Edit?ideaId={invalidId}", Method.Put);

            var ideaToUpdate = new Idea()
            {
                Title = "Updated Title",
                Description = "Updated Description"
            };

            request.AddBody(ideaToUpdate);

            // Act
            var response = await this._client.ExecuteAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Does.Contain("There is no such idea!"));

        }

        [Order(7)]
        [Test]
        public async Task Delete_DeleteRequest_ShouldReturnBadRequest_WhenGivenInvalidIdeaId()
        {
            // Arrange
            string invalidId = "invalidIdeaId23984943";

            var request = new RestRequest($"/Idea/Delete?ideaId={invalidId}", Method.Delete);

            // Act
            var response = await this._client.ExecuteAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Does.Contain("There is no such idea!"));

        }
       
    }

}