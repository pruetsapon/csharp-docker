namespace movie.data.objects
{
    public class ResultSet<TEntity> where TEntity : class
    {
        public int Page { get; set; }
        public int TotalPage { get; set; }
        public int Total { get; set; }
        public int ItemPerPage { get; set; }
        public int ItemStart { get; set; }
        public int ItemEnd { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }
        public IEnumerable<TEntity> Items { get; set; }

        public ResultSet(int page, int rows, int totalItem, List<TEntity> items)
        {
            var totalPage = 1;
            if (totalItem > rows)
            {
                if (totalItem%rows == 0)
                {
                    totalPage = (int)Math.Ceiling((decimal)(totalItem / rows));
                }
                else
                {
                    totalPage = (int)Math.Ceiling((decimal)(totalItem / rows)) + 1;
                }
            }
            this.Page = page;
            this.TotalPage = totalPage;
            this.Total = totalItem;
            this.ItemPerPage = rows;
            this.Items = items;
            this.ItemStart = (rows * (page - 1)) + 1;
            this.ItemEnd = (rows * page) > totalItem ? totalItem : (rows * page);
            this.StartPage = (page - 3) < 1 ? 1 : (page - 3);
            this.EndPage = (page + 3) > this.TotalPage ? this.TotalPage : (page + 3);
        }
    }
}
