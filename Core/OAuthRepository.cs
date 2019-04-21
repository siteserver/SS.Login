using System.Collections.Generic;
using Datory;
using SiteServer.Plugin;

namespace SS.Login.Core
{
    public class OAuthRepository
    {
        private readonly Repository<OAuthInfo> _repository;

        public OAuthRepository()
        {
            _repository = new Repository<OAuthInfo>(Context.Environment.DatabaseType, Context.Environment.ConnectionString);
        }

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string UserName = nameof(OAuthInfo.UserName);
            public const string Source = nameof(OAuthInfo.Source);
            public const string UniqueId = nameof(OAuthInfo.UniqueId);
        }

        public int Insert(OAuthInfo loginInfo)
        {
            return _repository.Insert(loginInfo);
        }

        public void Delete(string userName, string source)
        {
            _repository.Delete(Q
                .Where(Attr.UserName, userName)
                .Where(Attr.Source, source)
            );
        }

        public string GetUserName(string source, string uniqueId)
        {
            return _repository.Get<string>(Q
                .Select(Attr.UserName)
                .Where(Attr.Source, source)
                .Where(Attr.UniqueId, uniqueId)
            );
        }
    }
}
