namespace CinemaNowApi.Common.Model
{
    public class Show
    {
        public string Id { get; set; }
        public Movie movie { get; set; }
        public string hall { get; set; }
        public string date { get; set; }
        public string time { get; set; }
    }
}
