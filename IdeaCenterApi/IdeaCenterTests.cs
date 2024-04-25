using RestSharp;
using RestSharp.Authenticators;
using System.ComponentModel.Design.Serialization;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using IdeaCenterApi.Models;


namespace IdeaCenterApi
{
    public class ApiTests
    {
        private string _url = "http://softuni-qa-loadbalancer-2137572849.eu-north-1.elb.amazonaws.com:84/api";
        private RestClient _client;

        private string email = "testtest@test.com";
        private string password = "testuseridea";

        public static string _lastCreatedIdeaId;

        [OneTimeSetUp]
        public void Setup()
        {
            var token = GetToken(email, password);

            // Create options for authentication to the url
            var options = new RestClientOptions(_url)
            {
                // Create new authenticator using JWT token
                Authenticator = new JwtAuthenticator(token)
            };
            
            // Create new a instance of the Rest Client using the options
            this._client = new RestClient(options);
        }

        private string GetToken(string email, string password)
        {
            var authClient = new RestClient(_url);
            var authRequest = new RestRequest("/User/Authentication", Method.Post);

            authRequest.AddJsonBody(new
            {
                email = email,
                password = password
            });

            var authResponse = authClient.Execute(authRequest);

            if(authResponse.StatusCode == HttpStatusCode.OK)
            {
                var authJson = JsonSerializer.Deserialize<AuthResponseDto>(authResponse.Content);
                var token = authJson.AccessToken;

                if(string.IsNullOrWhiteSpace(token))
                {
                    throw new InvalidOperationException("Token is empty.");
                }

                return token;
            }
            else
            {
                throw new InvalidOperationException($"Response status {authResponse.StatusCode}");
            }

        }

        [Order(1)]
        [Test]
        public async Task Post_PostRequest_CreateAnIdeaWithValidParameters_ReturnsOk()
        {
            // Arrange
            var request = new RestRequest("/Idea/Create", Method.Post);

            var ideaCreation = new IdeaDto()
            {
                Title = "Test RestSharp",
                Description = "Random Description",
                Url = ""
            };
            request.AddJsonBody(ideaCreation);

            // Act
            var response = await this._client.ExecuteAsync(request);
            var responseJson = JsonSerializer.Deserialize<ApiResponseDto>(response.Content);

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

            var responseJson = JsonSerializer.Deserialize<List<IdeaDto>>(response.Content);

            var lastCreatedIdea = responseJson.LastOrDefault();
            _lastCreatedIdeaId = lastCreatedIdea.IdeaId;

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseJson.Count, Is.AtLeast(1));           

        }

        [Order(3)]
        [Test]
        public async Task Put_EditIdea_ShouldUpdateWholeIdeaInfo()
        {
            // Arrange
            var request = new RestRequest($"/Idea/Edit?ideaId={_lastCreatedIdeaId}", Method.Put);

            var ideaToUpdate = new IdeaDto()
            {
                Title = "Updated Title",
                Description = "Updated Description"
            };

            request.AddBody(ideaToUpdate);

            // Act
            var response = await this._client.ExecuteAsync(request);
            var responseJson = JsonSerializer.Deserialize<ApiResponseDto>(response.Content);

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

            var ideaCreation = new IdeaDto()
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

            var ideaToUpdate = new IdeaDto()
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