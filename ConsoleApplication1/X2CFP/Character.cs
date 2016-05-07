using System.Linq;

namespace X2CFP
{
    internal class Character : PropertySet
    {

        public override string ToString()
        {
            return string.Format("{0} {1} {2}",GetProperty("strFirstName"), GetProperty("strNickName"),
            GetProperty("strLastName"));
        }

        public string Details()
        {
            string result = "";
            if (FieldNames != null)
                result = FieldNames.Values.Aggregate(result, (current, prop) => current + prop.ToString() + '\n');

            return result;
        }
    }
}
