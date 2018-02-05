namespace SS.Home.Model
{
	public class MenuInfo
	{
        public int Id { get; set; }

        public int ParentId { get; set; }

        public int Taxis { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public bool IsOpenWindow { get; set; }
    }
}
