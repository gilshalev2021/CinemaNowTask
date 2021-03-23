namespace Common
{
    public class Constants
    {
        //Lambda - Environment Variables
        public const string CINEMA_NOW_DYNAMO_DB_MOVIES = "CINEMA_NOW_DYNAMO_DB_MOVIES";
        public const string CINEMA_NOW_DYNAMO_DB_SHOWS = "CINEMA_NOW_DYNAMO_DB_SHOWS";

        //Movies
        public const string COLUMN_MOVIE_ID = "Id";
        public const string COLUMN_MOVIE_TITLE = "Title";
        public const string COLUMN_MOVIE_OVERVIEW = "Overview";
        public const string COLUMN_MOVIE_POPULARITY = "Popularity";
        public const string COLUMN_MOVIE_RELEASE_DATE = "ReleaseDate";
        public const string COLUMN_MOVIE_POSTER_PATH = "PosterPath";
        public const string COLUMN_MOVIE_BACKDROP_PATH = "BackdropPath";

        //Shows
        public const string COLUMN_SHOW_ID = "Id";
        public const string COLUMN_SHOW_MOVIE = "Movie";
        public const string COLUMN_SHOW_HALL = "Hall";
        public const string COLUMN_SHOW_SHOW_DATE = "ShowDate";
        public const string COLUMN_SHOW_SHOW_TIME = "ShowTime";
    }
}
