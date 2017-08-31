using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace Nido.Common
{
    /// <summary>
    /// Convert a enum object to a key value pair list.
    /// </summary>
    public class EnumValueDto
    {
        /// <summary>
        /// Key or ID/ integer representation of the corresponding enum
        /// </summary>
        public int Key { get; set; }
        /// <summary>
        /// Value or name/ display text representation of the corresponding enum
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Convert any enum to a List of type EnumValueDto
        /// </summary>
        /// <typeparam name="T">Generic type of the enum</typeparam>
        /// <returns>List of object representing the enum</returns>
        public ICollection<EnumValueDto> ConvertEnumToList<T>() where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new Exception("Type given T must be an Enum");
            }

            var result = Enum.GetValues(typeof(T))
                             .Cast<T>()
                             .Select(x => new EnumValueDto
                             {
                                 Key = Convert.ToInt32(x),
                                 Value = x.ToString(new CultureInfo("en"))
                             })
                             .ToList()
                             .AsReadOnly();

            return result;
        }

    }
}