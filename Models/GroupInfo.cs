namespace SS.Home.Model
{
	public class GroupInfo
	{
        public int Id { get; set; }

	    public string GroupName { get; set; }

        public bool IsWriting { get; set; }

        public string WritingAdmin { get; set; }

        public int LastWritingSiteId { get; set; }

        public int LastWritingChannelId { get; set; }
    }
}
