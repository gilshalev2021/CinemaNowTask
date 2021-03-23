using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using CinemaNowApi.Common.Model;
using Newtonsoft.Json;

namespace Common
{
    public class UtilityFunctions
    {
        public static List<string> GetErrorListFromModelState
                                              (ModelStateDictionary modelState)
        {
            var query = from state in modelState.Values
                        from error in state.Errors
                        select error.ErrorMessage;

            var errorList = query.ToList();
            return errorList;
        }


        #region Movies
        public static ActionResult AddMovie(Movie movie)
        {
            IAmazonDynamoDB client = ClientFactory.GetAmazonDynamoDBClient();

            if (IsMovieExist(client, movie.Id))
            {
                return new OkObjectResult("This movie has already been saved to your local data base, and want be saved twice!");
            }

            var Id = movie.Id;

            //Values can not be empty!
            Dictionary<string, AttributeValue> dic = new Dictionary<string, AttributeValue>
            {
                { Constants.COLUMN_MOVIE_ID, new AttributeValue(Id) },
                { Constants.COLUMN_MOVIE_TITLE, new AttributeValue(movie.title) },
                { Constants.COLUMN_MOVIE_OVERVIEW, new AttributeValue(movie.overview) }
            };

            if (!string.IsNullOrEmpty(movie.posterPath))
            {
                dic.Add(Constants.COLUMN_MOVIE_POSTER_PATH, new AttributeValue(movie.posterPath));
            }

            if (!string.IsNullOrEmpty(movie.backdropPath))
            {
                dic.Add(Constants.COLUMN_MOVIE_BACKDROP_PATH, new AttributeValue(movie.backdropPath));
            }

            PutItemResponse response = client.PutItemAsync(ClientFactory.MoviesDynamoDbTable, dic).Result;

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                return new OkObjectResult(true);
            else
                return new StatusCodeResult((int)response.HttpStatusCode);
        }

        private static bool IsMovieExist(IAmazonDynamoDB client, string id)
        {
            ScanResponse result;

            var req = new ScanRequest
            {
                TableName = ClientFactory.MoviesDynamoDbTable
            };

            result = client.ScanAsync(req).Result;

            var movieFound = result.Items.FirstOrDefault(item => item[Constants.COLUMN_MOVIE_ID].S == id);

            return (movieFound != null) ? true : false;
        }

        public static List<Movie> GetMovies()
        {
            IAmazonDynamoDB client = ClientFactory.GetAmazonDynamoDBClient();

            ScanResponse result;

            var req = new ScanRequest
            {
                TableName = ClientFactory.MoviesDynamoDbTable
            };

            result = client.ScanAsync(req).Result;

            List<Movie> list = new List<Movie>();

            foreach (var item in result.Items)
            {
                Movie movie = new Movie
                {
                    Id = item[Constants.COLUMN_MOVIE_ID].S,
                    title = item[Constants.COLUMN_MOVIE_TITLE].S,
                    overview = item[Constants.COLUMN_MOVIE_OVERVIEW].S
                    //Popularity = item[Constants.COLUMN_MOVIE_POPULARITY].S,
                    //ReleaseDate = item[Constants.COLUMN_MOVIE_RELEASE_DATE].S
                };

                if (item.ContainsKey(Constants.COLUMN_MOVIE_POSTER_PATH))
                {
                    movie.posterPath = item[Constants.COLUMN_MOVIE_POSTER_PATH].S;
                }

                if (item.ContainsKey(Constants.COLUMN_MOVIE_BACKDROP_PATH))
                {
                    movie.backdropPath = item[Constants.COLUMN_MOVIE_BACKDROP_PATH].S;
                }

                list.Add(movie);
            }
            return list;
        }

        public static ActionResult DeleteMovie(string movieId)
        {
            try
            {
                IAmazonDynamoDB client = ClientFactory.GetAmazonDynamoDBClient();

                Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>
                {
                    { Constants.COLUMN_MOVIE_ID, new AttributeValue(movieId) }
                };

                DeleteItemResponse response = client.DeleteItemAsync(new DeleteItemRequest(ClientFactory.MoviesDynamoDbTable, key)).Result;

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    return new OkObjectResult(true);
                else
                    return new StatusCodeResult((int)response.HttpStatusCode);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception when deleting from DB {ClientFactory.MoviesDynamoDbTable}", e);

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }


        #endregion Movies

        #region Shows
        private static bool IsShowTimeTaken(IAmazonDynamoDB client, Show show)
        {
            ScanResponse result;

            var req = new ScanRequest
            {
                TableName = ClientFactory.ShowsDynamoDbTable
            };

            result = client.ScanAsync(req).Result;

            var showFound = result.Items.FirstOrDefault(item => item[Constants.COLUMN_SHOW_HALL].S == show.hall && item[Constants.COLUMN_SHOW_SHOW_DATE].S == show.date && item[Constants.COLUMN_SHOW_SHOW_TIME].S == show.time);
            
            return (showFound != null) ? true : false;
        }

        public static ActionResult AddShow(Show show)
        {
            IAmazonDynamoDB client = ClientFactory.GetAmazonDynamoDBClient();

            if(IsShowTimeTaken(client, show))
            {
                return new OkObjectResult("This hall already presenting another show at this hour, please choose another date/time or delete the other show first!");
            }

            var Id = Guid.NewGuid().ToString();

            //Serializing the Movie property
            string serializedMovie = JsonConvert.SerializeObject(show.movie);

            //Values can not be empty!
            Dictionary<string, AttributeValue> dic = new Dictionary<string, AttributeValue>
            {
                { Constants.COLUMN_SHOW_ID, new AttributeValue(Id) },
                { Constants.COLUMN_SHOW_MOVIE, new AttributeValue(serializedMovie) },
                { Constants.COLUMN_SHOW_HALL, new AttributeValue(show.hall) },
                { Constants.COLUMN_SHOW_SHOW_DATE, new AttributeValue(show.date) },
                { Constants.COLUMN_SHOW_SHOW_TIME, new AttributeValue(show.time) },
            };

            PutItemResponse response = client.PutItemAsync(ClientFactory.ShowsDynamoDbTable, dic).Result;

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                return new OkObjectResult(true);
            else
                return new StatusCodeResult((int)response.HttpStatusCode);
        }

        public static List<Show> GetShows()
        {
            IAmazonDynamoDB client = ClientFactory.GetAmazonDynamoDBClient();

            ScanResponse result;

            var req = new ScanRequest
            {
                TableName = ClientFactory.ShowsDynamoDbTable
            };

            result = client.ScanAsync(req).Result;

            List<Show> list = new List<Show>();

            foreach (var item in result.Items)
            {
                Show show = new Show
                {
                    Id = item[Constants.COLUMN_SHOW_ID].S,
                    hall = item[Constants.COLUMN_SHOW_HALL].S,
                    date = item[Constants.COLUMN_SHOW_SHOW_DATE].S,
                    time = item[Constants.COLUMN_SHOW_SHOW_TIME].S,
                };

                //Deserializing the movie srting into a Movie Object
                var movieString = item[Constants.COLUMN_SHOW_MOVIE].S;
                var movieObject = JsonConvert.DeserializeObject<Movie>(movieString);
                show.movie = movieObject;

                list.Add(show);
            }
            return list;
        }

        public static List<Show> GetShowsByDate(string date)
        {
            Console.WriteLine("INSIDE Utility GetShowsByDate, GOT date = " + date);

            IAmazonDynamoDB client = ClientFactory.GetAmazonDynamoDBClient();

            ScanResponse result;

            var req = new ScanRequest
            {
                TableName = ClientFactory.ShowsDynamoDbTable
            };

            result = client.ScanAsync(req).Result;

            List<Show> list = new List<Show>();

            foreach (var item in result.Items)
            {
                Show show = new Show
                {
                    Id = item[Constants.COLUMN_SHOW_ID].S,
                    hall = item[Constants.COLUMN_SHOW_HALL].S,
                    date = item[Constants.COLUMN_SHOW_SHOW_DATE].S,
                    time = item[Constants.COLUMN_SHOW_SHOW_TIME].S,
                };

                //Deserializing the movie srting into a Movie Object
                var movieString = item[Constants.COLUMN_SHOW_MOVIE].S;
                var movieObject = JsonConvert.DeserializeObject<Movie>(movieString);
                show.movie = movieObject;

                if (show.date == date)
                {
                    list.Add(show);
                }
            }

            Console.WriteLine("INSIDE Utility list size = " + list.Count);

            return list;
        }

        public static ActionResult DeleteShow(string showId)
        {
            try
            {
                IAmazonDynamoDB client = ClientFactory.GetAmazonDynamoDBClient();

                Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>
                {
                    { Constants.COLUMN_SHOW_ID, new AttributeValue(showId) }
                };

                DeleteItemResponse response = client.DeleteItemAsync(new DeleteItemRequest(ClientFactory.ShowsDynamoDbTable, key)).Result;

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    return new OkObjectResult(true);
                else
                    return new StatusCodeResult((int)response.HttpStatusCode);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception when deleting from DB {ClientFactory.ShowsDynamoDbTable}", e);

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        public static ActionResult UpdateShow(Show show)
        {
            try
            {
                IAmazonDynamoDB client = ClientFactory.GetAmazonDynamoDBClient();

                if (IsShowTimeTaken(client, show))
                {
                    return new OkObjectResult("This hall already presenting another show at this hour, please choose another date/time or delete your current show first!");
                }

                Dictionary<string, AttributeValueUpdate> attributeUpdates = new Dictionary<string, AttributeValueUpdate>
                {
                    { Constants.COLUMN_SHOW_HALL,      new AttributeValueUpdate(new AttributeValue(show.hall), AttributeAction.PUT) },
                    { Constants.COLUMN_SHOW_SHOW_DATE, new AttributeValueUpdate(new AttributeValue(show.date), AttributeAction.PUT) },
                    { Constants.COLUMN_SHOW_SHOW_TIME, new AttributeValueUpdate(new AttributeValue(show.time), AttributeAction.PUT) },
                };

                Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>
                {
                  { Constants.COLUMN_SHOW_ID, new AttributeValue(show.Id) }
                };

                UpdateItemResponse response = client.UpdateItemAsync(ClientFactory.ShowsDynamoDbTable, key, attributeUpdates).Result;

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    return new OkObjectResult(true);
                else
                    return new StatusCodeResult((int)response.HttpStatusCode);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception when scanning DB {ClientFactory.ShowsDynamoDbTable}", e);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion Shows
    }
}