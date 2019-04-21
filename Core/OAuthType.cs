using System;
using System.Collections.Generic;

namespace SS.Login.Core
{
    public class OAuthType : IEquatable<OAuthType>, IComparable<OAuthType>
    {
        public static readonly OAuthType Weibo = new OAuthType(nameof(Weibo));
        public static readonly OAuthType Weixin = new OAuthType(nameof(Weixin));
        public static readonly OAuthType Qq = new OAuthType(nameof(Qq));

        private OAuthType(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            Value = value;
        }

        public static OAuthType Parse(string type)
        {
            if (Utils.EqualsIgnoreCase(type, Weixin.Value))
            {
                return Weixin;
            }
            if (Utils.EqualsIgnoreCase(type, Qq.Value))
            {
                return Qq;
            }
            return Weibo;
        }

        public string Value { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as OAuthType);
        }

        public static bool operator ==(OAuthType a, OAuthType b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object)a == null || (object)b == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(OAuthType a, OAuthType b)
        {
            return !(a == b);
        }

        public bool Equals(OAuthType other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return
                Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public int CompareTo(OAuthType other)
        {

            if (other == null)
            {
                return 1;
            }

            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            return StringComparer.OrdinalIgnoreCase.Compare(Value, other.Value);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<string>.Default.GetHashCode(Value);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
