namespace ProjectReview.Models
{
    internal class LocationWithComments
    {
        public Location Location { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
    }
}