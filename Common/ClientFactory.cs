using Amazon;
using Amazon.DynamoDBv2;
using System;

namespace Common
{
    public class ClientFactory
    {
        public static IAmazonDynamoDB GetAmazonDynamoDBClient()
        {
            var dynamoDbClient = new AmazonDynamoDBClient(RegionEndpoint.GetBySystemName(AwsRegion));

            return dynamoDbClient;
        }
        public static string AwsRegion
        {
            get
            {
                return "eu-west-1";
            }
        }

        private static string _moviesDynamoDbTable;
        public static string MoviesDynamoDbTable
        {
            get
            {
                if (string.IsNullOrEmpty(_moviesDynamoDbTable))
                {
                    _moviesDynamoDbTable = Environment.GetEnvironmentVariable(Constants.CINEMA_NOW_DYNAMO_DB_MOVIES);

                    if (_moviesDynamoDbTable == null)
                    {
                        _moviesDynamoDbTable = "cinema-now-movies";
                    }

                }
                return _moviesDynamoDbTable;
            }
        }

        private static string _showsDynamoDbTable;
        public static string ShowsDynamoDbTable
        {
            get
            {
                if (string.IsNullOrEmpty(_showsDynamoDbTable))
                {
                    _showsDynamoDbTable = Environment.GetEnvironmentVariable(Constants.CINEMA_NOW_DYNAMO_DB_SHOWS);
                 
                    if (_showsDynamoDbTable == null)
                    {
                        _moviesDynamoDbTable = "cinema-now-shows";
                    }
                }
                return _showsDynamoDbTable;
            }
        }
    }
}
