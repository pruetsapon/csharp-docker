namespace movie.data.objects
{
    public class Movie : EntityWithTime
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
