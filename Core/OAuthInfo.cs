using Datory;

namespace SS.Login.Core
{
    [Table("ss_oauth")]
	public class OAuthInfo : Entity
	{
        [TableColumn]
        public string UserName { get; set; }

        [TableColumn]
        public string Source { get; set; }

        [TableColumn]
        public string UniqueId { get; set; }
    }
}
